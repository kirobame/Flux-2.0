using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    public abstract class AudioCollectionPackage : IAudioPackage
    {
        public AudioCollectionPackage() => values = new List<IAudioPackage>();
        
        protected List<IAudioPackage> values;

        public void Add(IAudioPackage value) => values.Add(value);
        public void Remove(IAudioPackage value) => values.Remove(value);

        public abstract void AssignTo(AudioSource source, EventArgs args);
    }
}