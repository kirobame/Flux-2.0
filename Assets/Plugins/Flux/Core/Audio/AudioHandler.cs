using System;
using System.Reflection;
using Flux.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Flux.Audio
{
    /// <summary>Utility class allowing to play simple <c>IAudioPackage</c>.
    /// Note that it might be better to implement your own solution if you want control over
    /// the pooling process.</summary>
    public static class AudioHandler
    {
        /// <summary>Implicitly created pool with only one default <c>AudioProvider</c> key.</summary>
        public static AudioPool Pool => pool;
        
        private static PoolableAudio prefab;
        private static AudioPool pool;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootup() // Creation of a default Poolable & Pool to allow automatization of the pooling process
        {
            // Poolable creation
            var prefabObject = new GameObject("PoolableAudio") { hideFlags = HideFlags.HideInHierarchy };
            Object.DontDestroyOnLoad(prefabObject);
            
            var audioSource = prefabObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            
            prefab = prefabObject.AddComponent<PoolableAudio>();
            ((IInjectable<AudioSource>)prefab).Inject(audioSource);
            
            // Pool creation
            var poolObject = new GameObject("AudioPool") {hideFlags = HideFlags.HideInHierarchy};
            Object.DontDestroyOnLoad(poolObject);
            pool = poolObject.AddComponent<AudioPool>();

            // Provider creation
            var provider = new AudioProvider(prefab);
            pool.AddProvider(provider);
        }
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        /// <summary>Plays an <c>IAudioPackage</c> by passing it an <c>EventArgs.Empty</c>.</summary>
        /// <returns>The instantiated <c>PoolableAudio</c> which will be used to play the given <c>IAudioPackage</c>.</returns>
        public static PoolableAudio Play(IAudioPackage package) => Play(package, EventArgs.Empty);
        public static PoolableAudio Play(IAudioPackage package, EventArgs args)
        {
            var poolable = pool.RequestSinglePoolable();
            Reset(poolable.Value);
            
            package.AssignTo(poolable.Value, args);
            poolable.Value.Play();

            return poolable;
        }

        /// <summary>Combines two <c>IAudioPackage</c> into one.</summary>
        /// <param name="value">The value to combine with the given source.</param>
        public static IAudioPackage Chain(this IAudioPackage source, IAudioPackage value)
        {
            // If one of the given values is already a group
            // It is used for the merge in order to avoid a pollution of the heap with imbricated data containers
            if (source is AudioGroupPackage lGroup) 
            {
                lGroup.Add(value);
                return lGroup;
            }
            else if (value is AudioGroupPackage rGroup)
            {
                rGroup.Add(source);
                return rGroup;
            }
            else // If no group was ground, create a new one
            {
                var group = new AudioGroupPackage();
                group.Add(source);
                group.Add(value);

                return group;
            }
        }
        
        //---[Utilities]------------------------------------------------------------------------------------------------/

        private static void Reset(AudioSource source) // Sets an AudioSource to its default values.
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