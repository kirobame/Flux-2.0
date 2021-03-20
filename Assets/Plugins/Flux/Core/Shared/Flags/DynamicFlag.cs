using System;
using UnityEngine;

namespace Flux
{
    [Serializable]
    public struct DynamicFlag : IFlag, IBootable
    {
        public Enum Value { get; private set; }

        [SerializeField] private string type;
        [SerializeField] private int value;
        
        public void Bootup()
        {
            var cast = Type.GetType(type);
            Value = (Enum)Enum.ToObject(cast, value);
        }
    }
}