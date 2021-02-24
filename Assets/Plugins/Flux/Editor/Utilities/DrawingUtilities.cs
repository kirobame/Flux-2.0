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
        public static Rect Indent(this Rect rect, float value)
        {
            rect.x += value;
            rect.width -= value;

            return rect;
        }
        
        public static Rect Stretch(this Rect rect, float left, float right, float up, float down)
        {
            rect.x -= left;
            rect.y -= up;

            rect.width += right * 2.0f;
            rect.height += down * 2.0f;

            return rect;
        }

        public static Color ToColor(this string htmlCode)
        {
            ColorUtility.TryParseHtmlString(htmlCode, out var color);
            return color;
        }
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static void DrawBorders(this Rect rect, float width, Color color)
        {
            DrawTopBorder(rect, width, color);
            DrawRightBorder(rect, width, color);
            DrawLeftBorder(rect, width, color);
            DrawBottomBorder(rect, width, color);
        }
        public static void DrawTopBorder(this Rect rect, float width, Color color)
        {
            var line = new Rect(rect.position, new Vector2(rect.width, width));
            EditorGUI.DrawRect(line, color);
        }
        public static void DrawRightBorder(this Rect rect, float width, Color color)
        {
            var line = new Rect(new Vector2(rect.xMax - width, rect.yMin), new Vector2(width, rect.height));
            EditorGUI.DrawRect(line, color);
        }
        public static void DrawLeftBorder(this Rect rect, float width, Color color)
        {
            var line = new Rect(rect.position, new Vector2(width, rect.height));
            EditorGUI.DrawRect(line, color);
        }
        public static void DrawBottomBorder(this Rect rect, float width, Color color)
        {
            var line = new Rect(new Vector2(rect.xMin, rect.yMax - width), new Vector2(rect.width, width));
            EditorGUI.DrawRect(line, color);
        }
    }
}