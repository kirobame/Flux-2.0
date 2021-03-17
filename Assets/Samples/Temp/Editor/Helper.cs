using System;
using System.Collections.Generic;
using System.Reflection;
using Flux.Editor;
using UnityEditor;
using UnityEngine;

public static class Helper
{
    public static float Margin => EditorGUIUtility.standardVerticalSpacing;
    
    public static Texture2D Indent
    {
        get
        {
            if (!hasIndent)
            {
                indent = new Texture2D(1,1);
                indent.SetPixel(0,0, new Color(0,0,0,0));
                indent.Apply();
            }

            return indent;
        }   
    }
    private static Texture2D indent;
    private static bool hasIndent = false;

    public static Type FindType(this SerializedProperty property)
    {
        var slices = property.propertyPath.Split('.');
        var type = property.serializedObject.targetObject.GetType();

        for (var i = 0; i < slices.Length; i++)
        {
            if (slices[i] == "Array")
            {
                if (type.IsArray) type = type.GetElementType();
                else
                {
                    Type match = null;
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        if (!interfaceType.IsGenericType || typeof(IEnumerable<>) != interfaceType.GetGenericTypeDefinition()) continue;
                        
                        match = interfaceType;
                        break;
                    }

                    type = match.GetGenericArguments()[0];
                }
                
                i++;
            }     
            else type = type.GetField(slices[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).FieldType;  
        }

        return type;
    }
    
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
    
    public static (Rect label, Rect value) GetLayout(this Rect source)
    {
        var labelRect = new Rect(source.position, new Vector2(EditorGUIUtility.labelWidth , source.height));
        var valueRect = new Rect(new Vector2(labelRect.xMax, labelRect.y) + Vector2.right * Margin, new Vector2(source.width - labelRect.width - Margin, source.height));
        
        return (labelRect, valueRect);
    }

    public static Rect Expand(this Rect rect, float amount) => rect.Stretch(amount, amount, amount, amount);
}