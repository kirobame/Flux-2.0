using System;
using UnityEngine;

namespace Flux
{
    [Serializable]
    public struct Id : IComparable<Id>
    {
        public Id(string value)
        {
            if (value.Length > 3) value = value.Substring(0, 3);
            this.value = value;
        }
        
        [SerializeField] private string value;

        public int CompareTo(Id other) => string.Compare(value, other.value, StringComparison.Ordinal);

        public override int GetHashCode() => value[0] * value[1] / 2 * value[2] / 3;
        public override bool Equals(object obj)
        {
            if (!(obj is Id id)) return false;
            return value.Equals(id.value);
        }
    }
}