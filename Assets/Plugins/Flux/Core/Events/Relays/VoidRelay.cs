using System;

namespace Flux.Event
{
    public class VoidRelay : EventRelay
    {
        private event Action callback;

        public override void TryCall(EventArgs args) => callback();

        protected override void OnSubscription(object method) => callback += (Action)method;
        protected override void OnUnsubscription(object method) => callback -= (Action)method;
    }
}