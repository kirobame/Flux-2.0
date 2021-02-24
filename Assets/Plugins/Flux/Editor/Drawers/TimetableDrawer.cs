using UnityEditor;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(Timetable))]
    public class TimetableDrawer : PropertyDrawer
    {
        private const int minWidth = 850;
        private const int minHeight = 500;
        private const int padding = 25;

        private TimetableEditor window;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.LabelField(rect, label);
            
            var contentRect = rect.GetValueRect().Indent(2.0f);
            var spacing = 4.0f;

            var copy = property.Copy();
            copy.Next(true);
            copy.Next(false);
            var durationRect = new Rect(contentRect.position, new Vector2(contentRect.width * 0.675f - spacing, rect.height));
            EditorGUI.PropertyField(durationRect, copy, GUIContent.none);
            
            var buttonRect = new Rect(new Vector2(durationRect.xMax + spacing, rect.yMin), new Vector2(contentRect.width - durationRect.width - spacing, rect.height));
            if (GUI.Button(buttonRect, "Edit"))
            {
                window = ScriptableObject.CreateInstance(typeof(TimetableEditor)) as TimetableEditor;
                window.titleContent = EditorGUIUtility.TrTextContentWithIcon("Timetable", "SceneViewFx");
                
                var position = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
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
            
            EditorGUI.EndProperty();
        }
    }
}