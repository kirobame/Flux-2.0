using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Data
{
    public static class Repository
    {
        private static Dictionary<int, object> values;
        private static FlagTranslator flagTranslator;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Bootup()
        {
            values = new Dictionary<int, object>();
            flagTranslator = new FlagTranslator();
        }

        public static void Clear()
        {
            values.Clear();
            flagTranslator.Reset();
        }
        
        public static void Register(Enum flag, object value)
        {
            var key = flagTranslator.Translate(flag);

            if (values.ContainsKey(key)) Debug.LogWarning($"The address [{flag}] is already occupied by {values[key]}.");
            else values.Add(key, value);
        }
        public static void Unregister(Enum flag)
        {
            var key = flagTranslator.Translate(flag);
            values.Remove(key);
        }

        public static bool Exists(Enum flag)
        {
            var key = flagTranslator.Translate(flag);
            return values.ContainsKey(key);
        }

        public static void Set(Enum flag, object value)
        {
            var key = flagTranslator.Translate(flag);
            values[key] = value;
        }
        public static T Get<T>(Enum flag) => (T)GetRaw(flag);
        public static object GetRaw(Enum flag)
        {
            var key = flagTranslator.Translate(flag);
            return values[key];
        }

        public static bool TrySet(Enum flag, object value)
        {
            var key = flagTranslator.Translate(flag);
            if (values.ContainsKey(key))
            {
                values[key] = value;
                return true;
            }

            return false;
        }
        public static bool TryGet<T>(Enum flag, out T value)
        {
            if (TryGetRaw(flag, out var rawValue) && rawValue is T castedValue)
            {
                value = castedValue;
                return true;
            }

            value = default;
            return false;
        }
        public static bool TryGetRaw(Enum flag, out object value)
        {
            var key = flagTranslator.Translate(flag);
            return values.TryGetValue(key, out value);
        }

        public static T[] GetAll<T>(Enum flag)
        {
            if (TryGetList(flag, out var list))
            {
                var cast = new T[list.Count];
                for (var i = 0; i < list.Count; i++) cast[i] = (T) list[i];

                return cast;
            }

            return null;
        }
        public static bool TryGetAll<T>(Enum flag, out T[] values)
        {
            values = null;
            
            if (TryGetList(flag, out var list))
            {
                var cast = new T[list.Count];
                for (var i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is T castedValue)) return false;
                    cast[i] = castedValue;
                }

                values = cast;
                return true;
            }

            return false;
        }

        public static void SetAt(Enum flag, int index, object value)
        {
            if (TryGetList(flag, out var list)) list[index] = value;
        }
        public static T GetAt<T>(Enum flag, int index) => (T)GetAtRaw(flag, index);
        public static object GetAtRaw(Enum flag, int index)
        {
            if (TryGetList(flag, out var list)) return list[index];
            else return null;
        }
        
        public static void AddTo(Enum flag, object value)
        {
            if (TryGetList(flag, out var list)) list.Add(value);
        }
        public static void RemoveFrom(Enum flag, object value)
        {
            if (TryGetList(flag, out var list))list.Remove(value);
        }
        private static bool TryGetList(Enum flag, out IList list)
        {
            var key = flagTranslator.Translate(flag);
            if (values[key] is IList cast)
            {
                list = cast;
                return true;
            }

            list = null;
            return false;
        }
    }
}