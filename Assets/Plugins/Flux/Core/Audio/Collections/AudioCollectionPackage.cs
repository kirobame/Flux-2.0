using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary>Representation of an embedded collection of <c>IAudioPackage</c>.</summary>
    public abstract class AudioCollectionPackage : IAudioPackage
    {
        public AudioCollectionPackage() => values = new List<IAudioPackage>();
        
        protected List<IAudioPackage> values;
        
        //---[Data container relays]------------------------------------------------------------------------------------/

        public void Add(IAudioPackage value) => values.Add(value);
        public void Remove(IAudioPackage value) => values.Remove(value);
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        // Delegation of the interface contract unto the abstract implementations
        public abstract void AssignTo(AudioSource source, EventArgs args);
    }
}