using System;

namespace Flux
{
    public struct Flag : IFlag
    {
        public Flag(Enum value) => Value = value;
        
        public Enum Value { get; private set; }
    }
}