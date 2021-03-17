using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable]
    public abstract class Jump : Effect
    {
        [SerializeField] private string target;

        private bool hasTarget;
        private Effect jumpTarget;
        
        public override void Bootup(Effect root, IReadOnlyList<Effect> effects)
        {
            foreach (var effect in effects)
            {
                if (!(effect is Label label) || label.Name != target) continue;

                hasTarget = true;
                jumpTarget = effect;

                break;
            }
        }

        protected override void OnUpdate(EventArgs args)
        {
            if (!hasTarget)
            {
                IsDone = true;
                return;
            }

            if (CanJump(args)) jumpTarget.Reset();
            else IsDone = true;
        }

        protected abstract bool CanJump(EventArgs args);
    }
}