using System.Linq;
using UnityEditor;

namespace Flux.Editor
{
    public static class SerializationUtilities
    {
        public static bool IsRoot(this SerializedProperty property) => property.propertyPath.Count(letter => letter == '/') == 0;
        public static SerializedProperty GetParent(this SerializedProperty property)
        {
            var path = property.propertyPath;
            var index = path.LastIndexOf('/');
            path = path.Remove(index, path.Length - index);

            return property.serializedObject.FindProperty(path);
        }
    }
}