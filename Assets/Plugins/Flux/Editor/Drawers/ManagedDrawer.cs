using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(ExplicitAttribute))]
    public class ManagedDrawer : PropertyDrawer
    {
        private static Dictionary<Type, ManagedTypeSearchWindow> windows = new Dictionary<Type, ManagedTypeSearchWindow>();
    
        private bool hasBeenInitialized;
        private Type fieldType;

        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            TryInitialize(property);
            EditorGUI.BeginProperty(rect, label, property);

            var header = new Rect(rect.position, new Vector2(rect.width, EditorGUIUtility.singleLineHeight));
            Rect buttonRect;

            var isArrayElement = property.name == "data";
            if (!isArrayElement)
            {
                var layout = header.GetLayout();
                property.isExpanded = EditorGUI.Foldout(layout.label, property.isExpanded, label);

                buttonRect = layout.value;
            }
            else
            {
                var type = property.type;
                var firstIndex = type.IndexOf('<');
                var secondIndex = type.IndexOf('>');
            
                if (secondIndex - firstIndex <= 1) buttonRect = header;
                else
                {
                    buttonRect = EditorGUI.IndentedRect(header);
                    var foldoutRect = new Rect(buttonRect.position, Vector2.one * buttonRect.height);
                    property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);

                    buttonRect = buttonRect.Indent(DrawingUtilities.Margin);
                }
            }
        
            var style = EditorStyles.textField;
            var name = property.type.Split('<')[1];
            name = name.Remove(name.Length - 1);
        
            if (GUI.Button(buttonRect, $"     {name}", style))
            {
                var x = buttonRect.x + buttonRect.width * 0.5f;
                var y = buttonRect.yMax + buttonRect.height * 0.75f;
                var anchor = EditorGUIUtility.GUIToScreenPoint(new Vector2(x, y));

                var window = windows[fieldType];
                window.Ticket = property;

                SearchWindow.Open(new SearchWindowContext(anchor, buttonRect.width), windows[fieldType]);
            }

            var iconRect = new Rect(buttonRect.position, Vector2.one * buttonRect.height).Expand(-1.0f);
            GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("Collab@2x").image, ScaleMode.StretchToFill);

            if (!property.isExpanded)
            {
                End(property);
                return;
            }

            if (property.TryGetTypedDrawer(out var drawer))
            {
                var rest = new Rect(new Vector2(rect.x, header.yMax + DrawingUtilities.Margin), new Vector2(rect.width, rect.height - header.height));
                drawer.OnGUI(rest, property, label);
            }
            else
            {
                var start = header.position + Vector2.up * (header.height + DrawingUtilities.Margin);
                property.DrawValues(start, rect.width, !isArrayElement);
            }

            End(property);
        }
        private void End(SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            TryInitialize(property);
        
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            else
            {
                if (property.TryGetTypedDrawer(out var drawer))
                {
                    var height = drawer.GetPropertyHeight(property, label);
                    return height + EditorGUIUtility.singleLineHeight + DrawingUtilities.Margin;
                }
                else return EditorGUI.GetPropertyHeight(property);
            }
        }

        private void TryInitialize(SerializedProperty property)
        {
            if (hasBeenInitialized) return;
        
            Initialize(property);
            hasBeenInitialized = true;
        }
        private void Initialize(SerializedProperty property)
        {
            fieldType = property.FindType();
            if (windows.ContainsKey(fieldType)) return;
        
            var searchWindow = ScriptableObject.CreateInstance<ManagedTypeSearchWindow>();
            searchWindow.Initialize(fieldType);

            windows.Add(fieldType, searchWindow);
        }
    }
}