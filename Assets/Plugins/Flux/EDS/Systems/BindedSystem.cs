using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Flux.EDS
{
    public abstract class BindedSystem : System
    {
        #region Nested Types

        private abstract class Relay
        {
            public abstract void Resolve(object source);
        }
        private class RelayGroup : Relay
        {
            public RelayGroup(Relay[] relays) => this.relays = relays;
            
            private Relay[] relays;

            public override void Resolve(object source) { foreach (var relay in relays) relay.Resolve(source); }
        }
        
        private class Binding<T> : Relay
        {
            public Binding(Action<T> setter) => this.setter = setter;
            
            private Action<T> setter;

            public override void Resolve(object source) => setter(((IWrapper<T>)source).Value);
        }
        private class Package<T> : Relay
        {
            public Package(Action<T> setter) => this.setter = setter;
            private Action<T> setter;

            public override void Resolve(object source) => setter((T)source);
        }
        #endregion

        //---[Initialization]-------------------------------------------------------------------------------------------/
        
        private Dictionary<int, Relay> unresolvedRelays;
        
        public override void Initialize()
        {
            unresolvedRelays = new Dictionary<int, Relay>();
            IsActive = false;
        }
        
        //---[Data binding]---------------------------------------------------------------------------------------------/

        protected void BindWhole<T>(string address, Action<T> setter)
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            unresolvedRelays.Add(handle.GetHashCode(), new Package<T>(setter));

            handle.Completed += ResolveRelay;
        }

        protected void BindTo<TSource,T1>(string address, Action<T1> relayOne) where TSource : IWrapper
        {
            BindTo<TSource>(address, new Binding<T1>(relayOne));
        }
        protected void BindTo<TSource,T1,T2>(string address, Action<T1> relayOne, Action<T2> relayTwo) where TSource : IWrapper
        {
            BindTo<TSource>(address, new Binding<T1>(relayOne), new Binding<T2>(relayTwo));
        }
        protected void BindTo<TSource,T1,T2,T3>(string address, Action<T1> relayOne, Action<T2> relayTwo, Action<T3> relayThree) where TSource : IWrapper
        {
            BindTo<TSource>(address, new Binding<T1>(relayOne), new Binding<T2>(relayTwo), new Binding<T3>(relayThree));
        }
        protected void BindTo<TSource,T1,T2,T3,T4>(string address, Action<T1> relayOne, Action<T2> relayTwo, Action<T3> relayThree, Action<T4> relayFour) where TSource : IWrapper
        {
            BindTo<TSource>(address, new Binding<T1>(relayOne), new Binding<T2>(relayTwo), new Binding<T3>(relayThree), new Binding<T4>(relayFour));
        }
        private void BindTo<T>(string address, params Relay[] relays)
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            unresolvedRelays.Add(handle.GetHashCode(), new RelayGroup(relays));

            handle.Completed += ResolveRelay;
        }

        private void ResolveRelay<T>(AsyncOperationHandle<T> handle)
        {
            var key = handle.GetHashCode();

            unresolvedRelays[key].Resolve(handle.Result);
            TryEndResolution(key);
        }
        private void TryEndResolution(int key)
        {
            unresolvedRelays.Remove(key);
            if (unresolvedRelays.Count != 0) return;
            
            IsActive = true;
            unresolvedRelays = null;
        }
    }
}