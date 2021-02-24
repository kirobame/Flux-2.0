using System;
using UnityEngine;

namespace Flux
{
    public interface IAudioPackage
    {
        void AssignTo(AudioSource source, EventArgs args);
    }
}