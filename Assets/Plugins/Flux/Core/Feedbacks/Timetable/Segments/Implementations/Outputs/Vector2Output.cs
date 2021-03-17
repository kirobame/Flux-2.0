using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Output"), Name("Vector2")]
    public class Vector2Output : Segment
    {
        [SerializeField] private Id pin;
        public Vector2 start;
        public Vector2 end;
        public LerpOp operation;

        private Pin<Vector2> output;

        public override void Open(Timetable table)
        {
            base.Open(table);
            
            output = new Pin<Vector2>();
            table.AddPin(pin, output);
        }

        protected override void Execute(float ratio)
        {
            switch (operation)
            {
                case LerpOp.Lerp:
                    output.Update(Vector2.Lerp(start, end, ratio));
                    break;
                
                case LerpOp.LerpUnclamped:
                    output.Update(Vector2.LerpUnclamped(start, end, ratio));
                    break;
            }
        }
    }
}