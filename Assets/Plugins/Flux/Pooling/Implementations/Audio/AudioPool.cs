using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    public class AudioPool : Pool<AudioSource, PoolableAudio>
    {
        protected override IList<Provider<AudioSource, PoolableAudio>> Providers => providers;
        [SerializeField] private AudioProvider[] providers;
    }
}