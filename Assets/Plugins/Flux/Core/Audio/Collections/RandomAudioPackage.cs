using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Flux.Audio
{
    /// <summary>Picks a random <c>IAudioPackage</c> for execution.</summary>
    public class RandomAudioPackage : AudioCollectionPackage
    {
        public override void AssignTo(AudioSource source, EventArgs args)
        {
            if (values.Count == 0) return;
            
            var index = Random.Range(0, values.Count);
            values[index].AssignTo(source, args);
        }
    }
}