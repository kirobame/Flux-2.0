using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

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
        
        public static Type GetManagedType(this SerializedProperty property)
        {
            var split = property.managedReferenceFullTypename.Split(' ');
            return Type.GetType($"{split[1]}, {split[0]}");
        }
        public static bool TryGetManagedType(this SerializedProperty property, out Type type)
        {
            type = null;
            if (property.managedReferenceFullTypename == string.Empty) return false;
            
            var split = property.managedReferenceFullTypename.Split(' ');
            type = Type.GetType($"{split[1]}, {split[0]}");

            return true;
        }

        public static void CopyTo(this SerializedProperty source, SerializedProperty destination)
        {
            var root = source.GetName();
            
            var copy = source.Copy();
            var destCopy = destination.Copy();
            
            if (!copy.Next(true)) return;
            destCopy.Next(true);
            
            if (root != copy.GetParentName()) return;
            destCopy.MatchDataWith(copy);

            while (copy.Next(false))
            {
                if (root != copy.GetParentName()) return;

                destCopy.Next(false);
                destCopy.MatchDataWith(copy);
            }
        }

        public static void MatchDataWith(this SerializedProperty left, SerializedProperty right)
        {
            switch (left.propertyType)
            {
                case SerializedPropertyType.Generic:
                    break;
                
                case SerializedPropertyType.Integer:
                    left.intValue = right.intValue;
                    break;
                
                case SerializedPropertyType.Boolean:
                    left.boolValue = right.boolValue;
                    break;
                
                case SerializedPropertyType.Float:
                    left.floatValue = right.floatValue;
                    break;
                
                case SerializedPropertyType.String:
                    left.stringValue = right.stringValue;
                    break;
                
                case SerializedPropertyType.Color:
                    left.colorValue = right.colorValue;
                    break;
                
                case SerializedPropertyType.ObjectReference:
                    left.objectReferenceValue = right.objectReferenceValue;
                    break;

                case SerializedPropertyType.LayerMask:
                    left.intValue = right.intValue;
                    break;
                
                case SerializedPropertyType.Enum:
                    left.enumValueIndex = right.enumValueIndex;
                    break;
                
                case SerializedPropertyType.Vector2:
                    left.vector2Value = right.vector2Value;
                    break;
                
                case SerializedPropertyType.Vector3:
                    left.vector3Value = right.vector3Value;
                    break;
                
                case SerializedPropertyType.Vector4:
                    left.vector4Value = right.vector4Value;
                    break;
                
                case SerializedPropertyType.Rect:
                    left.rectValue = right.rectValue;
                    break;
                
                case SerializedPropertyType.ArraySize:
                    left.arraySize = right.arraySize;
                    break;
                
                case SerializedPropertyType.Character:
                    left.stringValue = right.stringValue;
                    break;
                
                case SerializedPropertyType.AnimationCurve:
                    left.animationCurveValue = right.animationCurveValue;
                    break;
                
                case SerializedPropertyType.Bounds:
                    left.boundsValue = right.boundsValue;
                    break;
                
                case SerializedPropertyType.Gradient:
                    break;
                
                case SerializedPropertyType.Quaternion:
                    left.quaternionValue = right.quaternionValue;
                    break;
                
                case SerializedPropertyType.ExposedReference:
                    left.exposedReferenceValue = right.exposedReferenceValue;
                    break;
                
                case SerializedPropertyType.FixedBufferSize:
                    break;
                
                case SerializedPropertyType.Vector2Int:
                    left.vector2IntValue = right.vector2IntValue;
                    break;
                
                case SerializedPropertyType.Vector3Int:
                    left.vector3IntValue = right.vector3IntValue;
                    break;
                
                case SerializedPropertyType.RectInt:
                    left.rectIntValue = right.rectIntValue;
                    break;
                
                case SerializedPropertyType.BoundsInt:
                    left.boundsIntValue = right.boundsIntValue;
                    break;
                
                case SerializedPropertyType.ManagedReference:
                    break;
            }
        }
    }
}