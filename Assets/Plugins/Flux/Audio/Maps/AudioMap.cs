using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    public abstract class AudioMap<TKey> : AudioPackage
    {
        public AudioPackage this[TKey key] => packages[key];
        
        protected abstract KeyValuePair<TKey, AudioPackage>[] keyValuePairs { get; }
        private Dictionary<TKey, AudioPackage> packages;

        private bool hasBeenBootedUp;

        void OnDisable() => hasBeenBootedUp = false;

        private void BootUp()
        {
            packages = new Dictionary<TKey, AudioPackage>();
            foreach (var keyValuePair in keyValuePairs)
            {
                if (packages.ContainsKey(keyValuePair.Key)) continue;
                
                packages.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override void AssignTo(AudioSource source, EventArgs inArgs)
        {
            if (!(inArgs is WrapperArgs<TKey> keyArgs)) return;
            
            if (!hasBeenBootedUp)
            {
                BootUp();
                hasBeenBootedUp = true;
            }
            
            packages[keyArgs.ArgOne].AssignTo(source, inArgs);
        }
        
        public bool TryGet(TKey key, out AudioPackage value)
        {
            if (!hasBeenBootedUp)
            {
                BootUp();
                hasBeenBootedUp = true;
            }

            if (packages.TryGetValue(key, out value)) return true;
            else return false;
        }
    }
}