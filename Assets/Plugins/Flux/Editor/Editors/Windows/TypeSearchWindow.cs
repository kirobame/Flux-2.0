using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flux.Editor
{
    public class TypeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private static Dictionary<Type, Category<Type>> searchCatalogues = new Dictionary<Type, Category<Type>>();

        public event Action<object, Vector2> onSelectEntry;

        private Type typeToSearch;
        private string message;

        private EditorWindow window;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/
        
        public void Initialize(string message, Type typeToSearch, EditorWindow window)
        {
            this.typeToSearch = typeToSearch;
            this.message = message;
            this.window = window;

            if (searchCatalogues.ContainsKey(typeToSearch)) return;

            var searchCatalogue = new Category<Type>("Root");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeToSearch.IsAssignableFrom(type) || type.IsAbstract) continue;

                    var attribute = type.GetCustomAttribute<PathAttribute>();
                    if (attribute == null)
                    {
                        searchCatalogue.Add(type);
                        continue;
                    }
                    
                    if (!searchCatalogue.TryGet(attribute.Value, out var category)) continue;

                    category.Add(type);
                }
            }
            
            searchCatalogues.Add(typeToSearch, searchCatalogue);
        }
        
        //---[Implementation]-------------------------------------------------------------------------------------------/
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent(message), 0) };
            foreach (var nodeType in searchCatalogues[typeToSearch].Values)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(nodeType.Name, DrawingUtilities.Placeholder))
                {
                    userData = nodeType,
                    level = 1
                });
            }
            
            searchCatalogues[typeToSearch].Relay((category, depth) =>
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent(category.Name), depth));
                
                foreach (var nodeType in category.Values)
                {
                    var name = nodeType.Name;
                    
                    var attribute = nodeType.GetCustomAttribute<NameAttribute>();
                    if (attribute != null) name = attribute.Value;
                    
                    tree.Add(new SearchTreeEntry(new GUIContent(name, DrawingUtilities.Placeholder))
                    {
                        userData = nodeType,
                        level = depth + 1
                    });
                }
            }, 0, false);

            return tree;
        }
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var mousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
            onSelectEntry?.Invoke(searchTreeEntry.userData, mousePosition);
            
            return true;
        }
    }
}