using System;

namespace Flux.Event
{
    public abstract class EventToken : IDisposable
    {
        public EventToken(Enum address) => this.address = address;
        
        public abstract object Method { get; }
        
        public Enum Address => address;
        private Enum address;

        public abstract void Dispose();
    }
}