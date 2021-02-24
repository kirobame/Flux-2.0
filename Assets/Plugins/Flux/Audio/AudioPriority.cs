using System;
using UnityEngine;

namespace Flux
{
    public struct AudioPriority : IAudioPackage
    {
        public AudioPriority(byte value) => this.value = value;
        
        public byte value;

        public void AssignTo(AudioSource source, EventArgs args) => source.priority = value;
    }
}