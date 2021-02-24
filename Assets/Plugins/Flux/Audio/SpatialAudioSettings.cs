using System;
using UnityEngine;

namespace Flux
{
    [CreateAssetMenu(fileName = "NewAudioClipPackage", menuName = "Audio/3d settings")]
    public class SpatialAudioSettings : AudioPackage
    {
        [SerializeField, Range(0,1)] private float blend;
        [SerializeField, Range(0,5)] private float dopplerLevel;
        [SerializeField, Range(0,360)] private float spread;
        [SerializeField, Range(0,500)] private float maxDistance;
        
        public override void AssignTo(AudioSource source, EventArgs args)
        {
            source.spatialBlend = blend;
            source.dopplerLevel = dopplerLevel;
            source.spread = spread;
            source.maxDistance = maxDistance;
        }
    }
}