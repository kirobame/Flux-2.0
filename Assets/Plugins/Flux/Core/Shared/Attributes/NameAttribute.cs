using System;

namespace Flux
{
    public class NameAttribute : Attribute
    {
        public NameAttribute(string value) => Value = value;
        
        public string Value { get; private set; }
    }
}