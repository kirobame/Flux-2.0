using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Output"), Name("Vector3")]
    public class Vector3Output : Segment
    {
        [SerializeField] private Id pin;
        public Vector3 start;
        public Vector3 end;
        public LerpOp operation;

        private Pin<Vector3> output;

        public override void Open(Timetable table)
        {
            base.Open(table);
            
            output = new Pin<Vector3>();
            table.AddPin(pin, output);
        }

        protected override void Execute(float ratio)
        {
            switch (operation)
            {
                case LerpOp.Lerp:
                    output.Update(Vector3.Lerp(start, end, ratio));
                    break;
                
                case LerpOp.LerpUnclamped:
                    output.Update(Vector3.LerpUnclamped(start, end, ratio));
                    break;
            }
        }
    }
}