using UnityEditor;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Flux.Editor
{
    [CustomPropertyDrawer(typeof(Timetable))]
    public class TimetableDrawer : PropertyDrawer
    {
        private const int minWidth = 500;
        private const int minHeight = 550;
        private const int padding = 25;

        private TimetableEditor window;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            
            EditorGUI.LabelField(rect, label);

            var valueRect = rect.GetValueRect().Indent(2);
            if (GUI.Button(valueRect, "Edit"))
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
            }
            
            EditorGUI.EndProperty();
        }
    }
}