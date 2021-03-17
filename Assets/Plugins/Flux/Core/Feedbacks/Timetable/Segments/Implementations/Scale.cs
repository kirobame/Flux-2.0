using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Transform")]
    public class Scale : Segment
    {
        public Transform target;
        public Vector3 start;
        public Vector3 end;

        protected override void Execute(float ratio)
        {
            target.localScale = Vector3.Lerp(start, end, ratio);
        }
    }
}