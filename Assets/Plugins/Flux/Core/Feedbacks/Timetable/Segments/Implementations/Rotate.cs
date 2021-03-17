using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Transform")]
    public class Rotate : Segment
    {
        public Transform target;
        public Vector3 start;
        public Vector3 end;
        public QuatLerpOp operation;
        public bool local;

        private Quaternion from;
        private Quaternion to;
        
        public override void Open(Timetable table)
        {
            base.Open(table);
            
            from = Quaternion.Euler(start);
            to = Quaternion.Euler(end);
        }

        protected override void Execute(float ratio)
        {
            if (local) target.localRotation = Do(ratio);
            else target.rotation = Do(ratio);
        }

        private Quaternion Do(float ratio)
        {
            switch (operation)
            {
                case QuatLerpOp.Lerp:
                    return Quaternion.Lerp(from, to, ratio);
                
                case QuatLerpOp.SLerp:
                    return Quaternion.Slerp(from, to, ratio);

                case QuatLerpOp.LerpUnclamped:
                    return Quaternion.LerpUnclamped(from, to, ratio);
                
                case QuatLerpOp.SLerpUnclamped:
                    return Quaternion.SlerpUnclamped(from, to, ratio);
            }

            return default;
        }
    }
}