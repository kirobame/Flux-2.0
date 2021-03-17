using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Transform")]
    public class Move : Segment
    {
        public Transform target;
        public bool a;
        public Vector3 start;
        public Vector3 end;
        public bool somethingElse;

        protected override void Execute(float ratio)
        {
            if (somethingElse) target.localPosition = Vector3.Lerp(start, end, ratio);
            else target.position = Vector3.Lerp(start, end, ratio);
        }
    }
}