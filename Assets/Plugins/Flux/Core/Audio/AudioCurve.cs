using System;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary>Allows the assignation of a specific <c>AudioSourceCurveType</c> onto an <c>AudioSource</c>.</summary>
    [CreateAssetMenu(fileName = "NewAudioClipPackage", menuName = "Audio Packages/Curve", order = 215)]
    public class AudioCurve : AudioPackage
    {
        [SerializeField] private AudioSourceCurveType type;
        [SerializeField] private AnimationCurve value;
        
        public override void AssignTo(AudioSource source, EventArgs args) => source.SetCustomCurve(type, value);
    }
}