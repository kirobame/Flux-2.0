using System;

namespace Flux.Event
{
    public class Token : EventToken
    {
        public Token(Enum address, Action<EventArgs> method) : base(address) => this.method = method;

        public override object Method => method;
        private Action<EventArgs> method;

        public override void Dispose()
        {
            Events.Unsubscribe(Address, method);
            method = null;
        }
    }
}