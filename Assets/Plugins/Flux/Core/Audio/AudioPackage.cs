using System;
using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary><c>ScriptableObject</c> implementation of <c>IAudioPackage</c>.</summary>
    public abstract class AudioPackage : ScriptableObject, IAudioPackage
    {
        public abstract void AssignTo(AudioSource source, EventArgs args);
    }
}