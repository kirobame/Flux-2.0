using System;
using Flux;
using Flux.Feedbacks;
using UnityEngine;

namespace Example07
{
    [Serializable, Path("Samples/07")]
    public class Colorate : Segment
    {
        [SerializeField] private MeshRenderer renderer;
        [SerializeField] private Color start;
        [SerializeField] private Color end;
        
        //---[Core]-----------------------------------------------------------------------------------------------------/
        
        protected override void Execute(float ratio)
        {
            // The received ratio is between 0-1 & corresponds to time-value mapping of the curve associated to this segment
            renderer.material.SetColor("_Color", Color.Lerp(start, end, ratio));
        }
    }
}