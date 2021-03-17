using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Utilities")]
    public class Loop : Jump
    {
        [SerializeField] private int count;
        private int counter;
        
        public override void Ready()
        {
            base.Ready();
            counter = count;
        }
        protected override void OnReset() { }

        protected override bool CanJump(EventArgs args)
        {
            counter--;
            return counter > 0;
        }
    }
}