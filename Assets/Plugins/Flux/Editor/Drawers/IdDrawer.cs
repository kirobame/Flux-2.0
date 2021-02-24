using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(Id))]
    public class IdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            var labelWidth = EditorGUIUtility.labelWidth;

            var spacing = 4.0f;
            var labelRect = new Rect(rect.position, new Vector2(EditorGUIUtility.labelWidth, rect.height));
            rect.width -= labelRect.width;

            EditorGUI.LabelField(labelRect, label);

            var terseWidth = rect.width / 3.0f - spacing;
            EditorGUIUtility.labelWidth = terseWidth * 0.25f;
            
            property.NextVisible(true);
            if (property.stringValue.Length != 3)
            {
                property.stringValue = "NNN";
                property.serializedObject.ApplyModifiedProperties();
            }

            var firstRect = new Rect(new Vector2(labelRect.xMax + spacing - 2.0f, rect.yMin), new Vector2(terseWidth, rect.height));
            var first = EditorGUI.TextField(firstRect, GUIContent.none, property.stringValue[0].ToString())[0].ToString();
            
            var secondRect = new Rect(new Vector2(firstRect.xMax + spacing, rect.yMin), new Vector2(terseWidth, rect.height));
            var second = EditorGUI.TextField(secondRect, GUIContent.none, property.stringValue[1].ToString())[0].ToString();

            var thirdRect = new Rect(new Vector2(secondRect.xMax + spacing, rect.yMin), new Vector2(terseWidth + 2.0f, rect.height));
            var third = EditorGUI.TextField(thirdRect, GUIContent.none, property.stringValue[2].ToString())[0].ToString();

            property.stringValue = first + second + third;
            property.serializedObject.ApplyModifiedProperties();
            
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}