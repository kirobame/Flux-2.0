using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Event
{
    public static class Events
    {
        #region Nested Types

        private class EventWrapper
        {
            public event Action<EventArgs> callback;

            public bool IsLocked { get; set; } = false;            
            
            private List<EventRelay> relays = new List<EventRelay>();

            public void Call(EventArgs args)
            {
                if (IsLocked) return;   
                callback?.Invoke(args);
            }
            public void CallUnsafe(EventArgs args)
            {
                if (IsLocked) return;   
                callback(args);
            }

            public bool Relay<TRelay>(object method, out EventRelay match) where TRelay : EventRelay, new()
            {
                foreach (var existingRelay in relays)
                {
                    if (!(existingRelay is TRelay castedRelay)) continue;

                    castedRelay.Subscribe(method);
                    match = castedRelay;
                    
                    return false;
                }

                var relay = new TRelay();
                relay.Subscribe(method);
                relays.Add(relay);

                match = relay;
                return true;
            }
            public void Suppress<TRelay>(object method) where TRelay : EventRelay
            {
                foreach (var existingRelay in relays)
                {
                    if (!(existingRelay is TRelay castedRelay)) continue;

                    castedRelay.Unsubscribe(method);
                    break;
                }
            }
        }

        #endregion

        //---[Data]-----------------------------------------------------------------------------------------------------/
        
        public static bool IsFullyLocked { get; private set; }
        
        private static Dictionary<int, EventWrapper> registry;
        private static Dictionary<int, HashSet<Action<EventArgs>>> queue;
        
        private static FlagTranslator translator;

        //---[Initialization]-------------------------------------------------------------------------------------------/

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Bootup()
        {
            registry = new Dictionary<int, EventWrapper>();
            queue = new Dictionary<int, HashSet<Action<EventArgs>>>();

            translator = new FlagTranslator();
        }

        //---[Utilities]------------------------------------------------------------------------------------------------/

        public static void Clear()
        {
            registry.Clear();
            queue.Clear();
        }

        public static void ResetFlagTranslation() => translator.Reset();

        //---[General]-----------------------------------------------------------------------------------------------------/

        private static void Open(int key, EventWrapper wrapper)
        {
            if (queue.TryGetValue(key, out var hashSet))
            {
                foreach (var method in hashSet) wrapper.callback += method;
                queue.Remove(key);
            }

            registry.Add(key, wrapper);
        }
        private static EventWrapper GetWrapper(int key)
        {
            if (!registry.TryGetValue(key, out var output))
            {
                var wrapper = new EventWrapper();

                Open(key, wrapper);
                return wrapper;
            }

            return output;
        }

        public static bool Exists(Enum address)
        {
            var key = translator.Translate(address);
            return registry.ContainsKey(key);
        }
        
        //---[Lock]-----------------------------------------------------------------------------------------------------/

        public static void LockAll()
        {
            IsFullyLocked = true;
            foreach (var wrapper in registry.Values) wrapper.IsLocked = true;
        }
        public static void UnlockAll()
        {
            IsFullyLocked = false;
            foreach (var wrapper in registry.Values) wrapper.IsLocked = false;
        }
        
        public static void Lock(Enum address)
        {
            if (IsFullyLocked) return;
            
            var key = translator.Translate(address);
            if (registry.TryGetValue(key, out var wrapper)) wrapper.IsLocked = true;
        }
        public static void Unlock(Enum address)
        {
            if (IsFullyLocked) return;
            
            var key = translator.Translate(address);
            if (registry.TryGetValue(key, out var wrapper)) wrapper.IsLocked = false;
        }

        public static bool IsLocked(Enum address)
        {
            if (IsFullyLocked) return true;
            
            var key = translator.Translate(address);
            if (registry.TryGetValue(key, out var wrapper)) return wrapper.IsLocked;
            else return false;
        }
        
        //---[Call]-----------------------------------------------------------------------------------------------------/
        
        public static void Call(Enum address, EventArgs args)
        {
            var key = translator.Translate(address);
            GetWrapper(key).Call(args);
        }
        public static void CallUnsafe(Enum address, EventArgs args)
        {
            var key = translator.Translate(address);
            GetWrapper(key).CallUnsafe(args);
        }

        public static void Call(Enum address) => Call(address, EventArgs.Empty);
        public static void ZipCall<T1>(Enum address, in T1 argOne) => Call(address, new WrapperArgs<T1>(argOne));
        public static void ZipCall<T1,T2>(Enum address, in T1 argOne, in T2 argTwo) => Call(address, new WrapperArgs<T1,T2>(argOne, argTwo));
        public static void ZipCall<T1,T2,T3>(Enum address, in T1 argOne, in T2 argTwo, in T3 argThree) => Call(address, new WrapperArgs<T1,T2,T3>(argOne, argTwo, argThree));
        public static void ZipCall<T1,T2,T3,T4>(Enum address, in T1 argOne, in T2 argTwo, in T3 argThree, in T4 argFour) => Call(address, new WrapperArgs<T1,T2,T3,T4>(argOne, argTwo, argThree, argFour));

        //---[Subscribe]-------------------------------------------------------------------------------------------------/

        public static EventToken Subscribe(Enum address, Action<EventArgs> method)
        {
            var key = translator.Translate(address);
            if (registry.ContainsKey(key))
            {
                registry[key].callback += method;
            }
            else
            {
                if (!queue.TryGetValue(key, out var hashSet))
                {
                    hashSet = new HashSet<Action<EventArgs>>();
                    queue.Add(key, hashSet);
                }

                hashSet.Add(method);
            }
            
            return new Token(address, method);
        }

        public static EventToken Subscribe(Enum address, Action method) => Relay<VoidRelay>(address, method);
        public static EventToken Subscribe<T1>(Enum address, Action<T1> method) => Relay<GenericRelay<T1>>(address, method);
        public static EventToken Subscribe<T1,T2>(Enum address, Action<T1,T2> method) => Relay<GenericRelay<T1,T2>>(address, method);
        public static EventToken Subscribe<T1,T2,T3>(Enum address, Action<T1,T2,T3> method) => Relay<GenericRelay<T1,T2,T3>>(address, method);
        public static EventToken Subscribe<T1,T2,T3,T4>(Enum address, Action<T1,T2,T3,T4> method) => Relay<GenericRelay<T1,T2,T3,T4>>(address, method);

        public static EventToken Relay<TRelay>(Enum address, object method) where TRelay : EventRelay, new()
        {
            var key = translator.Translate(address);
            if (!registry.TryGetValue(key, out var wrapper))
            {
                wrapper = new EventWrapper();
                Open(key, wrapper);
            }

            if (wrapper.Relay<TRelay>(method, out var relay)) registry[key].callback += relay.TryCall;
            return new RelayToken<TRelay>(address, method);
        }

        //---[Unsubscribe]-----------------------------------------------------------------------------------------------/

        public static void Unsubscribe(Enum address, Action<EventArgs> method)
        {
            var key = translator.Translate(address);
            
            if (registry.ContainsKey(key)) registry[key].callback -= method;
            else if (queue.ContainsKey(key)) queue[key].Remove(method);
        }

        public static void Unsubscribe(Enum address, Action method) => Suppress<VoidRelay>(address, method);
        public static void Unsubscribe<T1>(Enum address, Action<T1> method) => Suppress<GenericRelay<T1>>(address, method);
        public static void Unsubscribe<T1,T2>(Enum address, Action<T1,T2> method) => Suppress<GenericRelay<T1,T2>>(address, method);
        public static void Unsubscribe<T1,T2,T3>(Enum address, Action<T1,T2,T3> method) => Suppress<GenericRelay<T1,T2,T3>>(address, method);
        public static void Unsubscribe<T1,T2,T3,T4>(Enum address, Action<T1,T2,T3,T4> method) => Suppress<GenericRelay<T1,T2,T3,T4>>(address, method);

        public static void Suppress<TRelay>(Enum address, object method) where TRelay : EventRelay
        {
            var key = translator.Translate(address);
            if (!registry.TryGetValue(key, out var wrapper)) return;

            wrapper.Suppress<TRelay>(method);
        }
    }
}