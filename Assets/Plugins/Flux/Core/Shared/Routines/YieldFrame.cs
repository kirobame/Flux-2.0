using System.Collections;
using UnityEngine;

namespace Flux
{
    public struct YieldFrame : IYieldInstruction
    {
        public YieldFrame(ushort count)
        {
            hasCount = true;
            this.count = count;
        }

        private bool hasCount;
        private ushort count;

        public object Wait() => Routine();
        public float Increment() => Time.deltaTime;

        private IEnumerator Routine()
        {
            if (hasCount) for (var i = 0; i < count; i++) yield return new WaitForEndOfFrame();
            else yield return new WaitForEndOfFrame();
        }
    }
}