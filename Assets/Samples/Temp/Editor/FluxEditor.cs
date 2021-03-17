using System.Collections.Generic;
using Flux.Editor;
using Malee.List;
using UnityEditor;

public abstract class FluxEditor : Editor
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
    }

    private void Initialize()
    {
        lists = new Dictionary<string, ReorderableList>();
        
        var property = serializedObject.GetIterator();
        property.NextVisible(true);
        
        while (property.NextVisible(false))
        {
            if (!property.isArray) continue;
            
            var target = serializedObject.FindProperty("classes");
            var list = new ReorderableList(target, true, true, true);
            list.footerHeight -= 1.0f;

            list.onAddCallback += AddElement;
            list.onRemoveCallback += RemoveElements;

            lists.Add(property.propertyPath, list);
        }
    }

    private void AddElement(ReorderableList list)
    {
        var item = list.List.NewElementAtEnd();
        item.managedReferenceValue = null;

        serializedObject.ApplyModifiedProperties();
    }
    private void RemoveElements(ReorderableList list)
    {
        foreach (var selection in list.Selected) list.List.DeleteArrayElementAtIndex(selection);
        serializedObject.ApplyModifiedProperties();
    }
}