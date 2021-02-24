using UnityEngine;

namespace Flux
{
    public class AudioProvider : Provider<AudioSource, PoolableAudio>
    {
        public AudioProvider() { }
        public AudioProvider(PoolableAudio prefab) => this.prefab = prefab;
    }
}