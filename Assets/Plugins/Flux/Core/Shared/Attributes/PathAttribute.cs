using System;

namespace Flux
{
    public class PathAttribute : Attribute
    {
        public PathAttribute(string value) => Value = value;
        
        public string Value { get; private set; }
    }
}