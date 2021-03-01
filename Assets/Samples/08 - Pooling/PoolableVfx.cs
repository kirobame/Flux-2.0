using Flux;
using Flux.Data;
using UnityEngine;

namespace Example08
{
    public class PoolableVfx : Poolable<ParticleSystem>
    {
        void Update() // Lifetime handling
        {
            if (!Value.isPlaying) gameObject.SetActive(false);
        }
    }
}