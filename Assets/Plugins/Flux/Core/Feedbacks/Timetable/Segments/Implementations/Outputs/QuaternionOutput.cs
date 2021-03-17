using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Output"), Name("Quaternion")]
    public class QuaternionOutput : Segment
    {
        [SerializeField] private Id pin;
        public Vector3 start;
        public Vector3 end;
        public QuatLerpOp operation;

        private Quaternion from;
        private Quaternion to;

        private Pin<Quaternion> output;

        public override void Open(Timetable table)
        {
            base.Open(table);

            from = Quaternion.Euler(start);
            to = Quaternion.Euler(end);
            
            output = new Pin<Quaternion>();
            table.AddPin(pin, output);
        }

        protected override void Execute(float ratio)
        {
            switch (operation)
            {
                case QuatLerpOp.Lerp:
                    output.Update(Quaternion.Lerp(from, to, ratio));
                    break;
                
                case QuatLerpOp.SLerp:
                    output.Update( Quaternion.Slerp(from, to, ratio));
                    break;

                case QuatLerpOp.LerpUnclamped:
                    output.Update( Quaternion.LerpUnclamped(from, to, ratio));
                    break;
                
                case QuatLerpOp.SLerpUnclamped:
                    output.Update( Quaternion.SlerpUnclamped(from, to, ratio));
                    break;
            }
        }
    }
}