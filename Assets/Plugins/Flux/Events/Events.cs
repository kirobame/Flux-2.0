using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Event
{
    public static class Events
    {
        //---[Data]-----------------------------------------------------------------------------------------------------/

        private static Dictionary<int, EventWrapper> wrappers;
        private static Dictionary<int, HashSet<Action<EventArgs>>> toWrap;

        private static Dictionary<int, List<EventRelay>> relays;

        private static FlagTranslator flagTranslator;

        //---[Initialization]-------------------------------------------------------------------------------------------/

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Bootup()
        {
            wrappers = new Dictionary<int, EventWrapper>();
            toWrap = new Dictionary<int, HashSet<Action<EventArgs>>>();

            relays = new Dictionary<int, List<EventRelay>>();

            flagTranslator = new FlagTranslator();
        }

        //---[Utilities]------------------------------------------------------------------------------------------------/

        public static void Clear()
        {
            wrappers.Clear();
            toWrap.Clear();

            relays.Clear();
        }

        public static void ResetFlagTranslation()
        {
            flagTranslator.Reset();
        }

        //---[Open]-----------------------------------------------------------------------------------------------------/

        public static void Open(Enum address)
        {
            var key = flagTranslator.Translate(address);
            if (wrappers.ContainsKey(key)) return;

            var wrapper = new EventWrapper();
            if (toWrap.TryGetValue(key, out var hashSet))
            {
                foreach (var method in hashSet) wrapper.callback += method;
                toWrap.Remove(key);
            }

            wrappers.Add(key, wrapper);
        }

        //---[Call]-----------------------------------------------------------------------------------------------------/

        public static void EmptyCall(Enum address)
        {
            Call(address, EventArgs.Empty);
        }

        public static void Call(Enum address, EventArgs args)
        {
            var key = flagTranslator.Translate(address);
            wrappers[key].Call(args);
        }

        public static void ZipCall<T1>(Enum address, T1 argOne)
        {
            Call(address, new WrapperArgs<T1>(argOne));
        }

        public static void ZipCall<T1, T2>(Enum address, T1 argOne, T2 argTwo)
        {
            Call(address, new WrapperArgs<T1, T2>(argOne, argTwo));
        }

        public static void ZipCall<T1, T2, T3>(Enum address, T1 argOne, T2 argTwo, T3 argThree)
        {
            Call(address, new WrapperArgs<T1, T2, T3>(argOne, argTwo, argThree));
        }

        public static void ZipCall<T1, T2, T3, T4>(Enum address, T1 argOne, T2 argTwo, T3 argThree, T4 argFour)
        {
            Call(address, new WrapperArgs<T1, T2, T3, T4>(argOne, argTwo, argThree, argFour));
        }

        //---[Register]-------------------------------------------------------------------------------------------------/

        public static void Register(Enum address, Action<EventArgs> method)
        {
            var key = flagTranslator.Translate(address);
            if (wrappers.ContainsKey(key))
            {
                wrappers[key].callback += method;
            }
            else
            {
                if (!toWrap.TryGetValue(key, out var hashSet))
                {
                    hashSet = new HashSet<Action<EventArgs>>();
                    toWrap.Add(key, hashSet);
                }

                hashSet.Add(method);
            }
        }

        public static void RelayByVoid(Enum address, Action method)
        {
            Relay<VoidRelay>(address, method);
        }

        public static void RelayByCast<T>(Enum address, Action<T> method) where T : EventArgs
        {
            Relay<CastRelay<T>>(address, method);
        }

        public static void RelayByValue<T1>(Enum address, Action<T1> method)
        {
            Relay<GenericRelay<T1>>(address, method);
        }

        public static void RelayByValue<T1, T2>(Enum address, Action<T1, T2> method)
        {
            Relay<GenericRelay<T1, T2>>(address, method);
        }

        public static void RelayByValue<T1, T2, T3>(Enum address, Action<T1, T2, T3> method)
        {
            Relay<GenericRelay<T1, T2, T3>>(address, method);
        }

        public static void RelayByValue<T1, T2, T3, T4>(Enum address, Action<T1, T2, T3, T4> method)
        {
            Relay<GenericRelay<T1, T2, T3, T4>>(address, method);
        }

        private static void Relay<TRelay>(Enum address, object method) where TRelay : EventRelay, new()
        {
            var key = flagTranslator.Translate(address);
            if (relays.TryGetValue(key, out var possibilities))
            {
                foreach (var relay in possibilities)
                {
                    if (!(relay is TRelay castedRelay)) continue;

                    castedRelay.Subscribe(method);
                    return;
                }
            }
            else
            {
                possibilities = new List<EventRelay>();
                relays.Add(key, possibilities);
            }

            var newRelay = new TRelay();
            Register(address, newRelay.Call);
            newRelay.Subscribe(method);

            possibilities.Add(newRelay);
        }

        //---[Unregister]-----------------------------------------------------------------------------------------------/

        public static void Unregister(Enum address, Action<EventArgs> method)
        {
            var key = flagTranslator.Translate(address);
            if (wrappers.ContainsKey(key)) wrappers[key].callback -= method;
            else if (toWrap.ContainsKey(key)) toWrap[key].Remove(method);
        }

        public static void BreakVoidRelay(Enum address, Action method)
        {
            BreakRelay<VoidRelay>(address, method);
        }

        public static void BreakCastRelay<T>(Enum address, Action<T> method) where T : EventArgs
        {
            BreakRelay<CastRelay<T>>(address, method);
        }

        public static void BreakValueRelay<T1>(Enum address, Action<T1> method)
        {
            BreakRelay<GenericRelay<T1>>(address, method);
        }

        public static void BreakValueRelay<T1, T2>(Enum address, Action<T1, T2> method)
        {
            BreakRelay<GenericRelay<T1, T2>>(address, method);
        }

        public static void BreakValueRelay<T1, T2, T3>(Enum address, Action<T1, T2, T3> method)
        {
            BreakRelay<GenericRelay<T1, T2, T3>>(address, method);
        }

        public static void BreakValueRelay<T1, T2, T3, T4>(Enum address, Action<T1, T2, T3, T4> method)
        {
            BreakRelay<GenericRelay<T1, T2, T3, T4>>(address, method);
        }

        private static void BreakRelay<TRelay>(Enum address, object method) where TRelay : EventRelay
        {
            var key = flagTranslator.Translate(address);
            if (relays.TryGetValue(key, out var possibilities))
                for (var i = 0; i < possibilities.Count; i++)
                {
                    var relay = possibilities[i];
                    if (!(relay is TRelay castedRelay)) continue;

                    castedRelay.Unsubscribe(method);
                    if (!relay.IsOperational)
                    {
                        Unregister(address, castedRelay.Call);
                        possibilities.RemoveAt(i);
                    }

                    return;
                }
        }

        #region Nested Types

        private class EventWrapper
        {
            public event Action<EventArgs> callback;

            public void Call(EventArgs args)
            {
                callback?.Invoke(args);
            }
        }

        private abstract class EventRelay
        {
            private short subscriptions;
            public bool IsOperational => subscriptions > 0;

            public abstract void Call(EventArgs args);

            public void Subscribe(object method)
            {
                subscriptions++;
                OnSubscription(method);
            }

            protected abstract void OnSubscription(object method);

            public void Unsubscribe(object method)
            {
                subscriptions--;
                OnSubscription(method);
            }

            protected abstract void OnUnsubscription(object method);
        }

        private class VoidRelay : EventRelay
        {
            private event Action callback;

            public override void Call(EventArgs args)
            {
                callback();
            }

            protected override void OnSubscription(object method)
            {
                callback += (Action) method;
            }

            protected override void OnUnsubscription(object method)
            {
                callback -= (Action) method;
            }
        }

        private class GenericRelay<T1> : EventRelay
        {
            private event Action<T1> callback;

            public override void Call(EventArgs args)
            {
                if (args is WrapperArgs<T1> wrapperArgs) callback(wrapperArgs.ArgOne);
            }

            protected override void OnSubscription(object method)
            {
                callback += (Action<T1>) method;
            }

            protected override void OnUnsubscription(object method)
            {
                callback -= (Action<T1>) method;
            }
        }

        private class GenericRelay<T1, T2> : EventRelay
        {
            private event Action<T1, T2> callback;

            public override void Call(EventArgs args)
            {
                if (args is WrapperArgs<T1, T2> wrapperArgs) callback(wrapperArgs.ArgOne, wrapperArgs.ArgTwo);
            }

            protected override void OnSubscription(object method)
            {
                callback += (Action<T1, T2>) method;
            }

            protected override void OnUnsubscription(object method)
            {
                callback -= (Action<T1, T2>) method;
            }
        }

        private class GenericRelay<T1, T2, T3> : EventRelay
        {
            private event Action<T1, T2, T3> callback;

            public override void Call(EventArgs args)
            {
                if (args is WrapperArgs<T1, T2, T3> wrapperArgs)
                    callback(wrapperArgs.ArgOne, wrapperArgs.ArgTwo, wrapperArgs.ArgThree);
            }

            protected override void OnSubscription(object method)
            {
                callback += (Action<T1, T2, T3>) method;
            }

            protected override void OnUnsubscription(object method)
            {
                callback -= (Action<T1, T2, T3>) method;
            }
        }

        private class GenericRelay<T1, T2, T3, T4> : EventRelay
        {
            private event Action<T1, T2, T3, T4> callback;

            public override void Call(EventArgs args)
            {
                if (args is WrapperArgs<T1, T2, T3, T4> wrapperArgs)
                    callback(wrapperArgs.ArgOne, wrapperArgs.ArgTwo, wrapperArgs.ArgThree, wrapperArgs.ArgFour);
            }

            protected override void OnSubscription(object method)
            {
                callback += (Action<T1, T2, T3, T4>) method;
            }

            protected override void OnUnsubscription(object method)
            {
                callback -= (Action<T1, T2, T3, T4>) method;
            }
        }

        private class CastRelay<T> : EventRelay where T : EventArgs
        {
            private event Action<T> callback;

            public override void Call(EventArgs args)
            {
                if (args is T castedArgs) callback(castedArgs);
            }

            protected override void OnSubscription(object method)
            {
                callback += (Action<T>) method;
            }

            protected override void OnUnsubscription(object method)
            {
                callback -= (Action<T>) method;
            }
        }

        #endregion
    }
}