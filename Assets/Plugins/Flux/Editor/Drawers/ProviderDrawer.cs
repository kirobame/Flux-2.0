using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(Provider), true)]
    public class ProviderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect.height = EditorGUIUtility.singleLineHeight;

            property.NextVisible(true);
            EditorGUI.PropertyField(rect, property, label);
            
            var prefab = property.objectReferenceValue;
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing * 2f;
            
            property.NextVisible(false);
            
            var copy = property.Copy();
            copy.NextVisible(false);
            
            var count = 0;
            for (var i = 0; i < copy.arraySize; i++)
            {
                if (copy.GetArrayElementAtIndex(i).objectReferenceValue != null) count++;
            }
            var countLabel = new GUIContent($"Count : {count} / {property.intValue}");
            
            EditorGUI.IntSlider(rect, property, 0, 100, countLabel);
            
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing * 2f;
            rect.x += EditorGUI.indentLevel * 14f;
            rect.width -= EditorGUI.indentLevel * 14f;

            GUI.enabled = prefab != null;
            if (GUI.Button(rect, new GUIContent("Fill")))
            {
                if (property.intValue == 0) copy.arraySize = 0;
                else
                {
                    var component = (Component)property.serializedObject.targetObject;
                
                    for (var i = 0; i < copy.arraySize; i++)
                    {
                        var instance = (Component)copy.GetArrayElementAtIndex(i).objectReferenceValue;
                        if (instance != null) Object.DestroyImmediate(instance.gameObject);
                    }
                
                    copy.arraySize = property.intValue;
                    for (var i = 0; i < copy.arraySize; i++)
                    {
                        var instance = (Component)PrefabUtility.InstantiatePrefab(prefab, component.transform);
                        instance.gameObject.SetActive(false);

                        var elementProperty = copy.GetArrayElementAtIndex(i);
                        elementProperty.objectReferenceValue = instance;
                    }
                }
            }
            GUI.enabled = true;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 4f;
    }
}