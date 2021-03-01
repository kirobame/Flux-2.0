using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Utilities")]
    public class Wait : Effect
    {
        [SerializeField] private float time;
        private float counter;

        public override void Ready()
        {
            base.Ready();
            counter = time;
        }

        protected override void OnUpdate(EventArgs args)
        {
            counter -= Time.deltaTime;
            if (counter <= 0) IsDone = true;
        }
    }
}