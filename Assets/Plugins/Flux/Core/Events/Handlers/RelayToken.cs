using System;

namespace Flux.Event
{
    public class RelayToken<TRelay> : EventToken where TRelay : EventRelay
    {
        public RelayToken(Enum address, object method) : base(address) => this.method = method;

        public override object Method => method;
        private object method;

        public override void Dispose()
        {
            Events.Suppress<TRelay>(Address, method);
            method = null;
        }
    }
}