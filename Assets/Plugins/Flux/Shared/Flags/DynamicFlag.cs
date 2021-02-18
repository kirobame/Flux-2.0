using System;
using UnityEngine;

namespace Flux
{
    public struct DynamicFlag : IFlag, IBootable
    {
        public Enum Value { get; private set; }

        [SerializeField] private string type;
        [SerializeField] private byte value;
        
        public void Bootup()
        {
            var cast = Type.GetType(type);
            Value = (Enum)Enum.ToObject(cast, value);
        }
    }
}