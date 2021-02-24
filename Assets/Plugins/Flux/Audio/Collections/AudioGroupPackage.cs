using System;
using UnityEngine;

namespace Flux
{
    public class AudioGroupPackage : AudioCollectionPackage
    {
        public override void AssignTo(AudioSource source, EventArgs args) { foreach (var value in values) value.AssignTo(source, args); }
    }
}