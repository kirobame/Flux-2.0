using UnityEngine;

namespace Flux
{
    public struct YieldFixedUpdate: IYieldInstruction
    {
        public object Wait() => new WaitForFixedUpdate();
        public float Increment() => Time.fixedDeltaTime;
    }
}