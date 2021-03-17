using System;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary>Direct mapping of the corresponding <c>AudioSource</c>'s value.</summary>
    public struct StereoPan : IAudioPackage
    {
        public StereoPan(float value) => this.value =  Mathf.Clamp(value, -1, 1);

        public float Value
        {
            get => value;
            set => Mathf.Clamp(value, -1, 1); // Respect of Unity's data range
        }
        private float value;
        
        public void AssignTo(AudioSource source, EventArgs args) => source.panStereo = value;
    }
}