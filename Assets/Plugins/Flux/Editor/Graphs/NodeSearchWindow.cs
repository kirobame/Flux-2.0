using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flux.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private static bool hasBeenInitialized;
        private static Category<Type> searchCatalogue;

        private EditorWindow window;
        private SequenceGraphView graphView;

        private Texture2D indent;
        
        public void Initialize(EditorWindow window, SequenceGraphView graphView)
        {
            this.window = window;
            this.graphView = graphView;
            
            indent = new Texture2D(1,1);
            indent.SetPixel(0,0, new Color(0,0,0,0));
            indent.Apply();
            
            if (hasBeenInitialized) return;

            searchCatalogue = new Category<Type>("Root");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(Effect).IsAssignableFrom(type) || type.IsAbstract) continue;

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
            
            hasBeenInitialized = true;
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("Create node"), 0) };
            foreach (var nodeType in searchCatalogue.Values)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(nodeType.Name, indent))
                {
                    userData = nodeType,
                    level = 1
                });
            }
            
            searchCatalogue.Relay((category, depth) =>
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent(category.Name), depth));
                
                foreach (var nodeType in category.Values)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent(nodeType.Name, indent))
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
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
            
            graphView.AddNode((Type)searchTreeEntry.userData, graphMousePosition);
            return true;
        }
    }
}