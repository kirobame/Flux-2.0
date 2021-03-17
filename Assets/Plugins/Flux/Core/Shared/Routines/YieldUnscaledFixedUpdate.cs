using UnityEngine;

namespace Flux
{
    public struct YieldUnscaledFixedUpdate: IYieldInstruction
    {
        public object Wait() => new WaitForFixedUpdate();
        public float Increment() => Time.fixedUnscaledDeltaTime;
    }
}