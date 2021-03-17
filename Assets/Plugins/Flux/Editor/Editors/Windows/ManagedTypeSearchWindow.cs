using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Flux.Editor
{
    public class ManagedTypeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        public event Action<SerializedProperty, Type> onEntrySelected;

        public SerializedProperty Ticket
        {
            get => ticket;
            set => ticket = value.Copy();
        }
        private SerializedProperty ticket;
    
        private Type typeToSearch;
        private IEnumerable<Type> catalogue;
    
        public void Initialize(Type typeToSearch)
        {
            this.typeToSearch = typeToSearch;
        
            var matches = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeToSearch.IsAssignableFrom(type) || type.IsAbstract) continue;
                    matches.Add(type);
                }
            }

            catalogue = matches;
        }
    
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("Choose a type"), 0) };
            foreach (var type in catalogue)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(type.Name, DrawingUtilities.Placeholder))
                {
                    userData = type,
                    level = 1
                });
            }

            return tree;
        }
        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var type = (Type)entry.userData;
            onEntrySelected?.Invoke(Ticket, type);

            ticket.managedReferenceValue = Activator.CreateInstance(type);
            ticket.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        
            return true;
        }
    }
}