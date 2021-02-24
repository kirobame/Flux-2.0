using System;

namespace Flux
{
    [Serializable]
    public struct Flag : IFlag
    {
        public Flag(Enum value) => Value = value;
        
        public Enum Value { get; private set; }
    }
}