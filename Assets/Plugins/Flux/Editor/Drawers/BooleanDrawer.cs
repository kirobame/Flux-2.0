using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    public class BooleanDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            Debug.Log("CUSTOM DRAW");
            EditorGUI.BeginChangeCheck();
            property.boolValue = EditorGUI.Toggle(rect, label, property.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("CHANGE");
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}