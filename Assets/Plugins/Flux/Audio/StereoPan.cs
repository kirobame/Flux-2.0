using System;
using UnityEngine;

namespace Flux
{
    public struct StereoPan : IAudioPackage
    {
        public StereoPan(float value) => this.value =  Mathf.Clamp(value, -1, 1);

        public float Value
        {
            get => value;
            set => Mathf.Clamp(value, -1, 1);
        }
        private float value;
        
        public void AssignTo(AudioSource source, EventArgs args) => source.panStereo = value;
    }
}