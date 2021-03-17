using UnityEngine;

namespace Flux
{
    public struct YieldUnscaledFrame : IYieldInstruction
    {
        public object Wait() => new WaitForEndOfFrame();
        public float Increment() => Time.unscaledDeltaTime;
    }
}