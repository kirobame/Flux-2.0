using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Output"), Name("Color")]
    public class ColorOutput : Segment
    {
        [SerializeField] private Id pin;
        public Color start;
        public Color end;
        public LerpOp operation;

        private Pin<Color> output;

        public override void Open(Timetable table)
        {
            base.Open(table);
            
            output = new Pin<Color>();
            table.AddPin(pin, output);
        }

        protected override void Execute(float ratio)
        {
            switch (operation)
            {
                case LerpOp.Lerp:
                    output.Update(Color.Lerp(start, end, ratio));
                    break;
                
                case LerpOp.LerpUnclamped:
                    output.Update(Color.LerpUnclamped(start, end, ratio));
                    break;
            }
        }
    }
}