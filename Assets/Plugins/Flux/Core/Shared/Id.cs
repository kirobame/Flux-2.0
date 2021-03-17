using System;
using System.IO;
using UnityEngine;

namespace Flux
{
    [Serializable]
    public struct Id : IComparable<Id>
    {
        public Id(params char[] chars)
        {
            if (chars.Length != 3) throw new InvalidDataException("An id can only contain 3 char!");
            value = new string(chars);
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