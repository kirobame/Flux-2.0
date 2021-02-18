using System;

namespace Flux
{
    public struct SimpleFlag : IFlag
    {
        public SimpleFlag(Enum value) => Value = value;
        
        public Enum Value { get; private set; }
    }
}