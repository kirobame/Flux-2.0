using UnityEngine;

namespace Flux
{
    public struct YieldTime : IYieldInstruction
    {
        public YieldTime(float step) => this.step = step;
        
        private float step;
        
        public object Wait() => new WaitForSeconds(step);
        public float Increment() => step;
    }
}