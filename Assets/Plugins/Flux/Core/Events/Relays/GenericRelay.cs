using System;

namespace Flux.Event
{
    public class GenericRelay<T1> : EventRelay
    {
        private event Action<T1> callback;

        public override void TryCall(EventArgs args)
        {
            if (!(args is WrapperArgs<T1> wrapperArgs)) return;
            callback(wrapperArgs.ArgOne);
        }

        protected override void OnSubscription(object method) => callback += (Action<T1>)method;
        protected override void OnUnsubscription(object method) => callback -= (Action<T1>)method;
    }
    
    public class GenericRelay<T1,T2> : EventRelay
    {
        private event Action<T1,T2> callback;

        public override void TryCall(EventArgs args)
        {
            if (!(args is WrapperArgs<T1,T2> wrapperArgs)) return;
            callback(wrapperArgs.ArgOne, wrapperArgs.ArgTwo);
        }

        protected override void OnSubscription(object method) => callback += (Action<T1,T2>)method;
        protected override void OnUnsubscription(object method) => callback -= (Action<T1,T2>)method;
    }
    
    public class GenericRelay<T1,T2,T3> : EventRelay
    {
        private event Action<T1,T2,T3> callback;

        public override void TryCall(EventArgs args)
        {
            if (!(args is WrapperArgs<T1, T2, T3> wrapperArgs)) return;
            callback(wrapperArgs.ArgOne, wrapperArgs.ArgTwo, wrapperArgs.ArgThree);
        }

        protected override void OnSubscription(object method) => callback += (Action<T1,T2,T3>)method;
        protected override void OnUnsubscription(object method) => callback -= (Action<T1,T2,T3>)method;
    }

    public class GenericRelay<T1, T2, T3, T4> : EventRelay
    {
        private event Action<T1, T2, T3, T4> callback;

        public override void TryCall(EventArgs args)
        {
            if (!(args is WrapperArgs<T1, T2, T3, T4> wrapperArgs)) return;
            callback(wrapperArgs.ArgOne, wrapperArgs.ArgTwo, wrapperArgs.ArgThree, wrapperArgs.ArgFour);
        }

        protected override void OnSubscription(object method) => callback += (Action<T1,T2,T3,T4>)method;
        protected override void OnUnsubscription(object method) => callback -= (Action<T1,T2,T3,T4>) method;
    }
}