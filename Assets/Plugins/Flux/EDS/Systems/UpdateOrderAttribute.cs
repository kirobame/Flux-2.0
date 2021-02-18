using System;

namespace Flux
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UpdateOrderAttribute : Attribute
    {
        public UpdateOrderAttribute(string parent, string order)
        {
            Parent = parent;

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

        public string Parent { get; private set; }
        public string After { get; private set; }
        public string Before { get; private set; }
    }
}