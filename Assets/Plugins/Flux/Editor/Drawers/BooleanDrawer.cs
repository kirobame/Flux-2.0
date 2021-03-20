using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    public class BooleanDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            
            EditorGUI.BeginChangeCheck();
            property.boolValue = EditorGUI.Toggle(rect, label, property.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("CHANGE");
            }
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}