using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary><c>ScriptableObject</c> wrapper class to map a key Type onto <c>IAudioPackage</c> values</summary>
    public abstract class AudioMap<TKey> : AudioPackage
    {
        public AudioPackage this[TKey key] => packages[key];
        
        protected abstract KeyValuePair<TKey, AudioPackage>[] keyValuePairs { get; }
        private Dictionary<TKey, AudioPackage> packages;

        private bool hasBeenBootedUp;

        //---[Lifetime handling]----------------------------------------------------------------------------------------/
        
        void OnDisable() => hasBeenBootedUp = false;

        private void BootUp() // Translation of the KeyValuePair array into a dictionary
        {
            packages = new Dictionary<TKey, AudioPackage>();
            foreach (var keyValuePair in keyValuePairs)
            {
                if (packages.ContainsKey(keyValuePair.Key)) continue;
                
                packages.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        public override void AssignTo(AudioSource source, EventArgs inArgs)
        {
            if (!(inArgs is IWrapper<TKey> keyArgs)) return; // If no key was passed, do nothing
            
            if (!hasBeenBootedUp) // If the dictionary hasn't been initialized, do so only once
            {
                BootUp();
                hasBeenBootedUp = true;
            }
            
            packages[keyArgs.Value].AssignTo(source, inArgs);
        }
        
        public bool TryGet(TKey key, out AudioPackage value)
        {
            if (!hasBeenBootedUp) // No specific initialization entry points exists, all entry points must be covered
            {
                BootUp();
                hasBeenBootedUp = true;
            }

            if (packages.TryGetValue(key, out value)) return true;
            else return false;
        }
    }
}