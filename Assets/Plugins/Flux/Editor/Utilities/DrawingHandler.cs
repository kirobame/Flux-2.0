using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    public static class DrawingHandler
    {
        private static Dictionary<Type, PropertyDrawer> typedDrawers;
        private static Dictionary<SerializedPropertyType, PropertyDrawer> categorizedDrawers = new Dictionary<SerializedPropertyType, PropertyDrawer>()
        {
            {SerializedPropertyType.Vector2, new Vector2Drawer()},
            {SerializedPropertyType.Vector3, new Vector3Drawer()},
        };
        
        //--------------------------------------------------------------------------------------------------------------/
        
        [InitializeOnLoadMethod]
        static void Bootup()
        {
            typedDrawers = new Dictionary<Type, PropertyDrawer>();
            var fieldInfo = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.Instance | BindingFlags.NonPublic);
            
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()))
            {
                if (!typeof(PropertyDrawer).IsAssignableFrom(type) || type.IsAbstract) continue;
                
                var attribute = type.GetCustomAttribute<CustomPropertyDrawer>();
                if (attribute == null) continue;

                var key = (Type)fieldInfo.GetValue(attribute);
                if (key.IsGenericType || typedDrawers.ContainsKey(key)) continue;

                var drawer = (PropertyDrawer)Activator.CreateInstance(type);
                typedDrawers.Add(key, drawer);
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public static bool TryGetCategorizedDrawer(this SerializedProperty property, out PropertyDrawer drawer) => categorizedDrawers.TryGetValue(property.propertyType, out drawer);
        public static bool TryGetTypedDrawer(this SerializedProperty property, out PropertyDrawer drawer)
        {
            drawer = null;
            if (!property.TryGetManagedType(out var type)) return false;
            
            if (typedDrawers.TryGetValue(type, out drawer)) return true;
            while (type.BaseType != typeof(object))
            {
                type = type.BaseType;
                if (typedDrawers.TryGetValue(type, out drawer)) return true;
            }

            return false;
        }
        
        public static bool TryCustomCategorizedDraw(this SerializedProperty property, Rect rect, GUIContent label)
        {
            if (categorizedDrawers.TryGetValue(property.propertyType, out var drawer))
            {
                drawer.OnGUI(rect, property.Copy(), label);
                return true;
            }

            return false;
        }
        public static bool TryCustomTypedDraw(this SerializedProperty property, Rect rect, GUIContent label)
        {
            if (TryGetCategorizedDrawer(property, out var drawer))
            {
                drawer.OnGUI(rect, property, label);
                return true;
            }

            return false;
        }

        public static float GetCategorizedHeight(this SerializedProperty property, GUIContent label) => categorizedDrawers[property.propertyType].GetPropertyHeight(property.Copy(), label);
        public static float GetTypedHeight(this SerializedProperty property, GUIContent label)
        {
            TryGetCategorizedDrawer(property, out var drawer);
            return drawer.GetPropertyHeight(property, label);
        }
        
        public static void DrawByCategory(this SerializedProperty property, Rect rect, GUIContent label) => categorizedDrawers[property.propertyType].OnGUI(rect, property.Copy(), label);
        public static void DrawByType(this SerializedProperty property, Rect rect, GUIContent label)
        {
            TryGetCategorizedDrawer(property, out var drawer);
            drawer.OnGUI(rect, property, label);
        }
    }
}