using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(DynamicFlag))]
    public class DynamicFlagDrawer : PropertyDrawer
    {
        private static bool hasBeenGloballyInitialized;
        private static Type[] types;
        private static GUIContent[] options;

        private bool hasBeenLocallyInitialized;
        private bool hasFlag;
        private int typeIndex;
        private Enum flag;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            property.NextVisible(true);
            
            if (!hasBeenGloballyInitialized) InitializeGlobally();
            if (!hasBeenLocallyInitialized) InitializeLocally(property.Copy());

            if (types.Length == 0) EditorGUI.LabelField(rect, "There are no enum addresses!");
            else
            {
                Rect leftRect;
                Rect rightRect;
                
                if (property.depth <= 1)
                {
                    var labelRect = new Rect(rect.position, new Vector2(EditorGUIUtility.labelWidth, rect.height));
                    EditorGUI.LabelField(labelRect, label);
                    
                    var contentRect = new Rect(new Vector2(labelRect.xMax, rect.yMin), new Vector2(rect.width - labelRect.width, rect.height));
                    contentRect = contentRect.Indent(2.0f);
                    var tuple = contentRect.Split(4.0f);

                    leftRect = tuple.left;
                    leftRect = leftRect.Stretch(0.0f, 1.0f, 0.0f, 0.0f);
                    rightRect = tuple.right;
                }
                else
                {
                    var copy = rect.Indent(3.0f);
                    var tuple = copy.Split(4.0f);
                    
                    leftRect = tuple.left;
                    leftRect = leftRect.Stretch(0.0f, 5.5f, 0.0f, 0.0f);
                    
                    rightRect = tuple.right;
                    rightRect = rightRect.Stretch(5.5f, 2.75f, 0.0f, 0.0f);
                }
                
                EditorGUI.BeginChangeCheck();
                
                typeIndex = EditorGUI.Popup(leftRect, GUIContent.none, typeIndex, options);
                property.stringValue = types[typeIndex].AssemblyQualifiedName;

                property.NextVisible(false);
                if (EditorGUI.EndChangeCheck())
                {
                    SetupFlag();
                    
                    flag = (Enum)Enum.ToObject(types[typeIndex], 0);
                    property.intValue = 0;
                }
                
                if (hasFlag) flag = EditorGUI.EnumFlagsField(rightRect, GUIContent.none, flag);
                else flag = EditorGUI.EnumPopup(rightRect, GUIContent.none, flag);
                
                property.intValue = Convert.ToByte(flag);
            }
            
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
        }

        private void SetupFlag()
        {
            var attribute = types[typeIndex].GetCustomAttribute<FlagsAttribute>();
            hasFlag = attribute != null;
        }
    }
}