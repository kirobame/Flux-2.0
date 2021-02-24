using System;
using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Flux
{
    public abstract class AudioPackage : ScriptableObject, IAudioPackage
    {
        public abstract void AssignTo(AudioSource source, EventArgs args);
    }
}