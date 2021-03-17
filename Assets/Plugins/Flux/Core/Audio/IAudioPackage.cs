using System;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary>Representation of an affect onto an <c>AudioSource</c>.</summary>
    public interface IAudioPackage
    {
        /// <summary>Affects the <c>AudioSource</c> by the implementation's specification.</summary>
        /// <param name="args">Args are sent to trickle down potential chained implementation.
        /// <see cref="AudioHandler.Chain(this IAudioPackage source, IAudioPackage value)"/></param>
        void AssignTo(AudioSource source, EventArgs args);
    }
}