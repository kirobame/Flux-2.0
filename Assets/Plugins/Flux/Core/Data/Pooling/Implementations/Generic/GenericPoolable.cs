using System;
using Object = UnityEngine.Object;

namespace Flux.Data
{
    public class GenericPoolable : Poolable<Object>
    {
        public event Action<GenericPoolable> onPrepare;
        public event Action<GenericPoolable> onReboot;

        public override void Prepare() => onPrepare?.Invoke(this);
        public override void Reboot() => onReboot?.Invoke(this);
    }
}