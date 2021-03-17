using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Output"), Name("Float")]
    public class FloatOutput : Segment
    {
        [SerializeField] private Id pin;
        public float start;
        public float end;

        private Pin<float> output;

        public override void Open(Timetable table)
        {
            base.Open(table);
            
            output = new Pin<float>();
            table.AddPin(pin, output);
        }

        protected override void Execute(float ratio)
        {
            output.Update(Mathf.Lerp(start, end, ratio));
        }
    }
}