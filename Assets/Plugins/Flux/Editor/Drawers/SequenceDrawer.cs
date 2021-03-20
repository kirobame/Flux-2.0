using Flux.Feedbacks;
using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(Sequence))]
    public class SequenceDrawer : PropertyDrawer
    {
        private const int minWidth = 850;
        private const int minHeight = 500;
        private const int padding = 25;
        
        private SequenceGraph window;
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var header = rect.GetLayout();
            EditorGUI.LabelField(header.label, label);
            
            if (GUI.Button(header.value, new GUIContent("Edit")))
            {
                window = ScriptableObject.CreateInstance(typeof(SequenceGraph)) as SequenceGraph;
                window.titleContent = EditorGUIUtility.TrTextContentWithIcon("Sequence", "SceneViewFx");
                
                var position = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
                var fullRect = EditorGUIUtility.GetMainWindowPosition();
                
                var width = fullRect.width - position.x - padding;
                if (width < minWidth)
                {
                    width = minWidth;
                    position.x = fullRect.width - width - padding;
                }

                var height = fullRect.height - position.y - padding;
                if (height < minHeight)
                {
                    height = minHeight ;
                    position.y = fullRect.height - height - padding;
                }
                
                window.ShowUtility();
                window.position = new Rect(position, new Vector2(width, height));
                
                window.Initialize(property);
            }
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}