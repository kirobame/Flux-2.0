using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    public static class EventUtilities
    {
        public static bool OnMouseDown(this UnityEngine.Event source, Rect rect, int index)
        {
            var id = GUIUtility.GetControlID(FocusType.Passive);
            var type = source.GetTypeForControl(id);
                
            if (type == EventType.MouseDown && source.button == index && rect.Contains(source.mousePosition))
            {
                GUIUtility.hotControl = id;
                source.Use();
                
                return true;
            }
            else return false;
        }
        public static bool OnMouseDown(this UnityEngine.Event source, int index)
        {
            var id = GUIUtility.GetControlID(FocusType.Passive);
            var type = source.GetTypeForControl(id);
                
            if (type == EventType.MouseDown && source.button == index)
            {
                GUIUtility.hotControl = id;
                source.Use();
                
                return true;
            }
            else return false;
        }

        public static bool OnMouseUp(this UnityEngine.Event source, Rect rect, int index)
        {
            var id = GUIUtility.GetControlID(FocusType.Passive);
            var type = source.GetTypeForControl(id);
            
            if (type == EventType.MouseUp && source.button == index && rect.Contains(source.mousePosition))
            {
                GUIUtility.hotControl = 0;
                source.Use();
                
                return true;
            }
            else return false;
        }
        public static bool OnMouseUp(this UnityEngine.Event source, int index)
        {
            var id = GUIUtility.GetControlID(FocusType.Passive);
            var type = source.GetTypeForControl(id);
            
            if (type == EventType.MouseUp && source.button == index)
            {
                GUIUtility.hotControl = 0;
                source.Use();
                
                return true;
            }
            else return false;
        }

        public static bool OnMouseMoveDrag(this UnityEngine.Event source)
        {
            var id = GUIUtility.GetControlID(FocusType.Passive);
            var type = source.GetTypeForControl(id);
            
            if (type == EventType.MouseDrag)
            {
                GUIUtility.hotControl = id;
                source.Use();
                
                return true;
            }
            else return false;
        }
    }
    
    public static class DrawingUtilities
    {
        public static float Margin => EditorGUIUtility.standardVerticalSpacing;
    
        public static Texture2D Placeholder
        {
            get
            {
                if (!hasPlaceholder)
                {
                    placeholder = new Texture2D(1,1);
                    placeholder.SetPixel(0,0, new Color(0,0,0,0));
                    placeholder.Apply();
                }

                return placeholder;
            }   
        }
        private static Texture2D placeholder;
        private static bool hasPlaceholder = false;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public static void DrawValues(this SerializedProperty property, Vector2 start, float totalWidth, bool indent = true)
        {
            var copy = property.Copy();
        
            var rootPath = copy.propertyPath.Split('.');
            var root = rootPath[rootPath.Length - 1];

            var rect = new Rect(start, new Vector2(totalWidth, 0.0f));
            var search = true;
        
            if (indent) EditorGUI.indentLevel++;
            else EditorGUIUtility.labelWidth -= 14.0f; 
        
            while (copy.Next(search))
            {
                search = false;

                var path = copy.propertyPath.Split('.');
                if (path.Length <= 1 || root != path[path.Length - 2]) break;
            
                DrawCurrent();
            }
        
            if (indent) EditorGUI.indentLevel--;
            else EditorGUIUtility.labelWidth += 14.0f; 

            void DrawCurrent()
            {
                rect.height = EditorGUI.GetPropertyHeight(copy);
                EditorGUI.PropertyField(rect, copy, new GUIContent(copy.displayName));
                rect.y += rect.height + Margin;
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public static (Rect label, Rect value) GetLayout(this Rect source)
        {
            var labelRect = new Rect(source.position, new Vector2(EditorGUIUtility.labelWidth , source.height));
            var valueRect = new Rect(new Vector2(labelRect.xMax, labelRect.y) + Vector2.right * Margin, new Vector2(source.width - labelRect.width - Margin, source.height));
        
            return (labelRect, valueRect);
        }
        public static (Rect left, Rect right) Split(this Rect rect, float spacing = 0.0f)
        {
            (Rect left, Rect right) output;
            
            var halfWidth = (rect.width - spacing) / 2.0f;
            output.left = new Rect(rect.position, new Vector2(halfWidth, rect.height));
            output.right = new Rect(rect.position + Vector2.right * (halfWidth + spacing), output.left.size);

            return output;
        }
        
        public static Rect Indent(this Rect rect) => Indent(rect, EditorGUI.indentLevel * 19);
        public static Rect Indent(this Rect rect, float value)
        {
            rect.x += value;
            rect.width -= value;

            return rect;
        }
        
        public static Rect Expand(this Rect rect, float amount) => rect.Stretch(amount, amount, amount, amount);
        public static Rect Stretch(this Rect rect, float left, float right, float up, float down)
        {
            rect.x -= left;
            rect.y -= up;

            rect.width += right * 2.0f;
            rect.height += down * 2.0f;

            return rect;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
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
        
        //--------------------------------------------------------------------------------------------------------------/

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