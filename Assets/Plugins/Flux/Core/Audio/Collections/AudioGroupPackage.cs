using System;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary>Simple grouping of <c>IAudioPackage</c>. All childs will be called on the Assignation.</summary>
    public class AudioGroupPackage : AudioCollectionPackage
    {
        public override void AssignTo(AudioSource source, EventArgs args) { foreach (var value in values) value.AssignTo(source, args); }
    }
}