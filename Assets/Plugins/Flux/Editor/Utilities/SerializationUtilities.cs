using System.Linq;
using UnityEditor;

namespace Flux.Editor
{
    public static class SerializationUtilities
    {
        public static bool IsRoot(this SerializedProperty property) => property.propertyPath.Count(letter => letter == '.') == 0;

        public static string GetName(this SerializedProperty property)
        {
            var split = property.propertyPath.Split('.');
            
            if (split.Length <= 1) return property.propertyPath;
            else return split[split.Length - 1];
        }
        public static string GetParentName(this SerializedProperty property)
        {
            var split = property.propertyPath.Split('.');
            
            if (split.Length <= 1) return string.Empty;
            else return split[split.Length - 2];
        }

        public static SerializedProperty NewElementAtEnd(this SerializedProperty arrayProperty)
        {
            if (arrayProperty.arraySize == 0) arrayProperty.InsertArrayElementAtIndex(0);
            else arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize - 1);
                
            return arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
        }
    }
}