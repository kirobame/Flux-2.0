using Flux;
using UnityEngine;

namespace Example08
{
    public class PoolableVfx : Poolable<ParticleSystem>
    {
        void Update()
        {
            if (!Value.isPlaying) gameObject.SetActive(false);
        }
    }
}