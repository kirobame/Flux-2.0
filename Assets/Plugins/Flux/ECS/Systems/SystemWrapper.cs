using System;
using System.Collections.Generic;

namespace Flux
{
    public class SystemWrapper : IComparable<SystemWrapper>
    {
        public System Value { get; private set; }

        private byte group;
        private ushort order;
        
        public SystemWrapper(Type systemType)
        {
            Value = (System)Activator.CreateInstance(systemType);

            var attribute = systemType.GetCustomAttribute<GroupAttribute>();
            if (attribute != null)
            {
                group = attribute.Group;
                order = attribute.Order;
            }
            else
            {
                group = byte.MaxValue;
                order = ushort.MaxValue;
            }
        }

        public int CompareTo(SystemWrapper other)
        {
            if (group == other.group) return order.CompareTo(other.order);
            return group.CompareTo(other.group);
        }
    }
}