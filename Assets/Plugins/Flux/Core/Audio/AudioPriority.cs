using System;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary>Direct mapping of the corresponding <c>AudioSource</c>'s value.</summary>
    public struct AudioPriority : IAudioPackage
    {
        public AudioPriority(byte value) => this.value = value;
        
        public byte value; // The bye data type corresponds to the 0-256 range given for the AudioPriority

        public void AssignTo(AudioSource source, EventArgs args) => source.priority = value;
    }
}