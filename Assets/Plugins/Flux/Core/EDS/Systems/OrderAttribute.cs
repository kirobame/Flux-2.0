using System;

namespace Flux.EDS
{
    public abstract class SystemUpdateAttribute : Attribute
    {
        public SystemUpdateAttribute(string path, string order)
        {
            Path = path;

            var split = order.Split('/');
            if (split.Length == 2)
            {
                After = split[0];
                Before = split[1];
            }
            else
            {
                After = "Any";
                Before = "Any";
            }
        }

        public string Path { get; private set; }
        public string After { get; private set; }
        public string Before { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GroupAttribute : SystemUpdateAttribute
    {
        public GroupAttribute(string path, string order) : base(path, order) { }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class OrderAttribute : SystemUpdateAttribute
    {
        public OrderAttribute(string path, string order) : base(path, order) { }
    }
}