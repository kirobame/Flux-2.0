using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(Provider), true)]
    public class ProviderDrawer : PropertyDrawer
    {
        private static Dictionary<string, Type> poolableTypes;
        private static bool hasBeenInitialized;

        private void Initialize()
        {
            poolableTypes = new Dictionary<string, Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(Provider).IsAssignableFrom(type) || type.IsAbstract) continue;
                    poolableTypes.Add(type.Name, type.BaseType.GetGenericArguments()[1]);
                }
            }

            hasBeenInitialized = true;
        }
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (!hasBeenInitialized) Initialize();

            EditorGUI.BeginProperty(rect, new GUIContent("Prefab"), property);
            var type = property.type;
            
            rect.height = EditorGUIUtility.singleLineHeight;

            property.NextVisible(true);
            EditorGUI.PropertyField(rect, property, new GUIContent("Prefab"));

            var subCopy = property.Copy();
            subCopy.Next(true);
            var guid = subCopy.stringValue;
            
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            
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
            
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.x += EditorGUI.indentLevel * 14f;
            rect.width -= EditorGUI.indentLevel * 14f;

            GUI.enabled = guid != string.Empty;
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
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var prefab = AssetDatabase.LoadAssetAtPath(path, poolableTypes[type]);
                                                    
                        var instance = (Component)PrefabUtility.InstantiatePrefab(prefab, component.transform);
                        instance.gameObject.SetActive(false);

                        var elementProperty = copy.GetArrayElementAtIndex(i);
                        elementProperty.objectReferenceValue = instance;
                    }
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            GUI.enabled = true;
            
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 3f;
    }
}