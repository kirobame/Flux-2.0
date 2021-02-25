using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Flux
{
    public static class Audio
    {
        public static AudioPool Pool => pool;
        
        private static PoolableAudio prefab;
        private static AudioPool pool;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootup()
        {
            var prefabObject = new GameObject("PoolableAudio") { hideFlags = HideFlags.HideInHierarchy };
            Object.DontDestroyOnLoad(prefabObject);
            
            var audioSource = prefabObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            
            prefab = prefabObject.AddComponent<PoolableAudio>();
            ((IInjectable<AudioSource>)prefab).Inject(audioSource);
            
            var poolObject = new GameObject("AudioPool") {hideFlags = HideFlags.HideInHierarchy};
            Object.DontDestroyOnLoad(poolObject);
            pool = poolObject.AddComponent<AudioPool>();

            var provider = new AudioProvider(prefab);
            pool.AddProvider(provider);
        }

        public static PoolableAudio Play(IAudioPackage package) => Play(package, EventArgs.Empty);
        public static PoolableAudio Play(IAudioPackage package, EventArgs args)
        {
            var poolable = pool.RequestSinglePoolable();
            Reset(poolable.Value);
            
            package.AssignTo(poolable.Value, args);
            poolable.Value.Play();

            return poolable;
        }

        public static IAudioPackage Chain(this IAudioPackage source, IAudioPackage value)
        {
            if (source is AudioGroupPackage group)
            {
                group.Add(value);
                return group;
            }
            else
            {
                group = new AudioGroupPackage();
                group.Add(source);
                group.Add(value);

                return group;
            }
        }

        private static void Reset(AudioSource source)
        {
            source.loop = false;
            source.priority = 128;
            source.panStereo = 0;
            source.spatialBlend = 0;
            source.reverbZoneMix = 1;
            source.dopplerLevel = 1;
            source.spread = 0;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 1;
            source.maxDistance = 500;
        }
    }
}