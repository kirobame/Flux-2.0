using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    public class FlagTranslator
    {
        public FlagTranslator() => lookups = new Dictionary<Type, ushort>();
        
        private Dictionary<Type, ushort> lookups;
        
        public int Translate(Enum flag)
        {
            byte value;
            
            try { value = Convert.ToByte(flag); }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                return 0;
            }
            
            var type = flag.GetType();
            if (!lookups.TryGetValue(type, out var offset))
            {
                offset = (ushort)lookups.Count;
                lookups.Add(type, offset);
            }

            return int.MinValue + offset * 255 + value;
        }
        
        public void Reset() => lookups.Clear();
    }
}