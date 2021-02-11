using System;

namespace Flux
{
    public class GroupAttribute : Attribute
    {
        public GroupAttribute(byte group, ushort order)
        {
            Group = group;
            Order = order;
        }
        
        public byte Group { get; private set; }
        public ushort Order { get; private set; }
    }
}