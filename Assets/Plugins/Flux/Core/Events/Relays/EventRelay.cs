using System;
using System.Collections.Generic;

namespace Flux.Event
{
    public abstract class EventRelay
    {
        public bool IsOperational => subscriptions > 0;
            
        private short subscriptions;
        private HashSet<int> ids = new HashSet<int>();

        public abstract void TryCall(EventArgs args);

        public void Subscribe(object method)
        {
            if (!ids.Add(method.GetHashCode())) return;
                
            subscriptions++;
            OnSubscription(method);
        }
        protected abstract void OnSubscription(object method);

        public void Unsubscribe(object method)
        {
            if (!ids.Remove(method.GetHashCode())) return;
                
            subscriptions--;
            OnUnsubscription(method);
        }
        protected abstract void OnUnsubscription(object method);
    }
}