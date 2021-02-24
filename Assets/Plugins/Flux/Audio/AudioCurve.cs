using System;
using UnityEngine;

namespace Flux
{
    [CreateAssetMenu(fileName = "NewAudioClipPackage", menuName = "Audio/Curve")]
    public class AudioCurve : AudioPackage
    {
        [SerializeField] private AudioSourceCurveType type;
        [SerializeField] private AnimationCurve value;
        
        public override void AssignTo(AudioSource source, EventArgs args) => source.SetCustomCurve(type, value);
    }
}