using UnityEngine;

namespace Flux
{
    public struct YieldFrame : IYieldInstruction
    {
        public object Wait() => new WaitForEndOfFrame();
        public float Increment() => Time.deltaTime;
    }
}