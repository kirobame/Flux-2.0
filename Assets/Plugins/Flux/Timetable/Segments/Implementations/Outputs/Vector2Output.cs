using System;
using UnityEngine;

namespace Flux
{
    [Serializable, Path("Output")]
    public class Vector2Output : Segment
    {
        [SerializeField] private Id pin;
        public Vector2 start;
        public Vector2 end;

        private Pin<Vector2> output;

        public override void Open(Timetable table)
        {
            base.Open(table);
            
            output = new Pin<Vector2>();
            table.AddPin(pin, output);
        }

        protected override void Execute(float ratio)
        {
            output.Update(Vector2.Lerp(start, end, ratio));
        }
    }
}