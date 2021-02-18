using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    public static class DrawingUtilities
    {
        public static Rect GetValueRect(this Rect rect)
        {
            var width = rect.width - EditorGUIUtility.labelWidth;
            var position = rect.position + Vector2.right * EditorGUIUtility.labelWidth;
            
            return new Rect(position, new Vector2(width, rect.height));
        }
        
        public static (Rect left, Rect right) Split(this Rect rect, float spacing = 0.0f)
        {
            (Rect left, Rect right) output;
            
            var halfWidth = (rect.width - spacing) / 2.0f;
            output.left = new Rect(rect.position, new Vector2(halfWidth, rect.height));
            output.right = new Rect(rect.position + Vector2.right * (halfWidth + spacing), output.left.size);

            return output;
        }

        public static Rect Indent(this Rect rect)
        {
            var x = EditorGUI.indentLevel * 19;
            rect.x += x;
            rect.width -= x;

            return rect;
        }
        public static Rect Stretch(this Rect rect, float left, float right, float up, float down)
        {
            rect.x -= left;
            rect.y -= up;

            rect.width += right;
            rect.height += down;

            return rect;
        }
    }
}