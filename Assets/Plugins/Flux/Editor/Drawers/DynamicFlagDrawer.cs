using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(DynamicFlag))]
    public class DynamicFlagDrawer : PropertyDrawer
    {
        private static bool hasBeenGloballyInitialized;
        private static Type[] types;
        private static GUIContent[] options;

        private Object lastTarget;
        private bool hasBeenLocallyInitialized;
        
        private bool hasFlag;
        private int typeIndex;
        private Enum flag;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            property.NextVisible(true);
            
            if (!hasBeenGloballyInitialized) InitializeGlobally();

            var target = property.serializedObject.targetObject;
            if (!hasBeenLocallyInitialized || target != lastTarget) InitializeLocally(property.Copy());

            if (types.Length == 0) EditorGUI.LabelField(rect, "There are no enum addresses!");
            else
            {
                var header = rect.GetLayout();
                EditorGUI.BeginChangeCheck();

                typeIndex = EditorGUI.Popup(header.label, GUIContent.none, typeIndex, options);
                property.stringValue = types[typeIndex].AssemblyQualifiedName;
                
                property.NextVisible(false);
                if (EditorGUI.EndChangeCheck())
                {
                    SetupFlag();
                    
                    flag = (Enum)Enum.ToObject(types[typeIndex], 0);
                    property.intValue = 0;
                }
                
                if (hasFlag) flag = EditorGUI.EnumFlagsField(header.value, GUIContent.none, flag);
                else flag = EditorGUI.EnumPopup(header.value, GUIContent.none, flag);
                
                property.intValue = Convert.ToByte(flag);
            }
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
        
        private void InitializeGlobally()
        {
            hasBeenGloballyInitialized = true;
            
            var output = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsEnum) continue;
                    
                    var attribute = type.GetCustomAttribute<AddressAttribute>();
                    if (attribute == null) continue;
                    
                    output.Add(type);
                }
            }

            types = output.ToArray();
            options = types.Select(type => new GUIContent(type.Name)).ToArray();
        }
        private void InitializeLocally(SerializedProperty property)
        {
            hasBeenLocallyInitialized = true;
            if (types.Length == 0) return;
            
            typeIndex = Array.FindIndex(types, type => type.AssemblyQualifiedName == property.stringValue);
            if (typeIndex == -1) typeIndex = 0;

            SetupFlag();
            
            property.NextVisible(false);
            flag = (Enum)Enum.ToObject(types[typeIndex], (byte)property.intValue);

            lastTarget = property.serializedObject.targetObject;
        }

        private void SetupFlag()
        {
            var attribute = types[typeIndex].GetCustomAttribute<FlagsAttribute>();
            hasFlag = attribute != null;
        }
    }
}