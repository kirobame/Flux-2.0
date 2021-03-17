using UnityEngine;

namespace Flux.Data
{
    public class AudioProvider : Provider<AudioSource, PoolableAudio>
    {
        public AudioProvider() { }
        public AudioProvider(PoolableAudio prefab) => actualPrefab = prefab;
    }
}