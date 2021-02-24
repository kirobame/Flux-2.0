using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    public static class DrawingHandler
    {
        private static Dictionary<SerializedPropertyType, PropertyDrawer> drawers = new Dictionary<SerializedPropertyType, PropertyDrawer>()
        {
            {SerializedPropertyType.Vector2, new Vector2Drawer()},
            {SerializedPropertyType.Vector3, new Vector3Drawer()},

        };

        public static bool TryGetDrawer(this SerializedProperty property, out PropertyDrawer drawer) => drawers.TryGetValue(property.propertyType, out drawer);
        public static bool TryCustomDraw(this SerializedProperty property, Rect rect, GUIContent label)
        {
            if (drawers.TryGetValue(property.propertyType, out var drawer))
            {
                drawer.OnGUI(rect, property.Copy(), label);
                return true;
            }

            return false;
        }

        public static float GetHeight(this SerializedProperty property, GUIContent label) => drawers[property.propertyType].GetPropertyHeight(property.Copy(), label);
        public static void Draw(this SerializedProperty property, Rect rect, GUIContent label) => drawers[property.propertyType].OnGUI(rect, property.Copy(), label);
    }
}