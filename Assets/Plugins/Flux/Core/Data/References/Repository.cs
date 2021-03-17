using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Data
{
    public static class Repository
    {
        private static Dictionary<int, object> registry;
        private static FlagTranslator translator;
        
        //--------------------------------------------------------------------------------------------------------------/

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Bootup()
        {
            registry = new Dictionary<int, object>();
            translator = new FlagTranslator();
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public static void Clear()
        {
            registry.Clear();
            translator.Reset();
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public static void Register(Enum flag, object value)
        {
            var key = translator.Translate(flag);

            if (registry.ContainsKey(key)) Debug.LogWarning($"The address [{flag}] is already occupied by {registry[key]}.");
            else registry.Add(key, value);
        }
        public static void Unregister(Enum flag)
        {
            var key = translator.Translate(flag);
            registry.Remove(key);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public static bool Exists(Enum flag)
        {
            var key = translator.Translate(flag);
            return registry.ContainsKey(key);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public static void Set(Enum flag, object value)
        {
            var key = translator.Translate(flag);
            registry[key] = value;
        }
        public static bool TrySet(Enum flag, object value)
        {
            var key = translator.Translate(flag);
            if (registry.ContainsKey(key))
            {
                registry[key] = value;
                return true;
            }

            return false;
        }
        
        public static T Get<T>(Enum flag) => (T)GetRaw(flag);
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
        
        public static object GetRaw(Enum flag)
        {
            var key = translator.Translate(flag);
            return registry[key];
        }
        public static bool TryGetRaw(Enum flag, out object value)
        {
            var key = translator.Translate(flag);
            return registry.TryGetValue(key, out value);
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
            if (TryGetList(flag, out var list)) list.Remove(value);
        }
        
        private static bool TryGetList(Enum flag, out IList list)
        {
            var key = translator.Translate(flag);
            if (registry[key] is IList cast)
            {
                list = cast;
                return true;
            }

            list = null;
            return false;
        }
    }
}