using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Flux
{
    [CreateAssetMenu(fileName = "NewAudioClipPackage", menuName = "Audio/Clip")]
    public class AudioClipPackage : AudioPackage, IInjectable<AudioClip>
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField, Range(0, 1)] private float volume = 1;
        [SerializeField, Range(-3, 3)] private float pitch = 1;

        public override void AssignTo(AudioSource source, EventArgs args)
        {
            source.clip = clip;
            source.outputAudioMixerGroup = mixerGroup;
            source.volume = volume;
            source.pitch = pitch;
        }

        void IInjectable<AudioClip>.Inject(AudioClip value) => clip = value;
    }
}