using UnityEngine;

namespace Flux
{
    public struct YieldUnscaledTime : IYieldInstruction
    {
        public YieldUnscaledTime(float step) => this.step = step;
        
        private float step;
        
        public object Wait() => new WaitForSecondsRealtime(step);
        public float Increment() => step;
    }
}