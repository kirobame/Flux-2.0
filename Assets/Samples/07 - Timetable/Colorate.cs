using Flux;
using UnityEngine;

namespace Example07
{
    [Path("Samples/07")]
    public class Colorate : Segment
    {
        [SerializeField] private MeshRenderer renderer;
        [SerializeField] private Color start;
        [SerializeField] private Color end;
        
        protected override void Execute(float ratio)
        {
            renderer.material.SetColor("_Color", Color.Lerp(start, end, ratio));
        }
    }
}