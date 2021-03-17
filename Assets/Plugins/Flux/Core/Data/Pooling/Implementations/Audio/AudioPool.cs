using System;
using System.Collections.Generic;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Flux.Data
{
    public class AudioPool : Pool<AudioSource, PoolableAudio>
    {
        protected override IList<Provider<AudioSource, PoolableAudio>> Providers => providers;
        [SerializeField] private AudioProvider[] providers = new AudioProvider[0];

        public override void AddProvider(Provider<AudioSource, PoolableAudio> provider)
        {
            Array.Resize(ref providers, providers.Length + 1);
            providers[providers.Length - 1] = (AudioProvider)provider;
        }
    }
}