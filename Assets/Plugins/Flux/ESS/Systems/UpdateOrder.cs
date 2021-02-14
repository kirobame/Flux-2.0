using System;

namespace Flux
{
    public class UpdateOrder : Attribute
    {
        public UpdateOrder(string parent, string order)
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