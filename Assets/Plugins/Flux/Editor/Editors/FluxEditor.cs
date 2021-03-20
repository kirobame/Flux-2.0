using System.Collections.Generic;
using Malee.List;
using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    public abstract class FluxEditor : UnityEditor.Editor
    {
        private bool hasBeenInitialized;
        private Dictionary<string, ReorderableList> lists;

        public override void OnInspectorGUI()
        {
            if (!hasBeenInitialized)
            {
                Initialize();
                hasBeenInitialized = true;
            }
        
            var property = serializedObject.GetIterator();
            property.NextVisible(true);

            while (property.NextVisible(false))
            {
                if (property.isArray)
                {
                    if (!lists.TryGetValue(property.propertyPath, out var list)) continue;
                
                    EditorGUI.indentLevel--;
                    list.DoLayoutList();
                    EditorGUI.indentLevel++;
                }
                else EditorGUILayout.PropertyField(property);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void Initialize()
        {
            lists = new Dictionary<string, ReorderableList>();
        
            var property = serializedObject.GetIterator();
            property.NextVisible(true);
        
            while (property.NextVisible(false))
            {
                if (!property.isArray) continue;
            
                var list = new ReorderableList(property.Copy(), true, true, true);
                list.footerHeight -= 1.0f;
                list.paginate = true;
                list.pageSize = 10;

                list.onAddCallback += AddElement;
                list.onRemoveCallback += RemoveElements;

                lists.Add(property.propertyPath, list);
            }
        }

        private void AddElement(ReorderableList list)
        {
            list.List.NewElementAtEnd();
            list.SetPage(Mathf.FloorToInt((float)list.List.arraySize / list.pageSize));
        
            serializedObject.ApplyModifiedProperties();
        }
        private void RemoveElements(ReorderableList list)
        {
            foreach (var selection in list.Selected) list.List.DeleteArrayElementAtIndex(selection);
            serializedObject.ApplyModifiedProperties();
        }
    }
}