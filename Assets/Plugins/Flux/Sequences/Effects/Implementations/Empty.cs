using System;

namespace Flux
{
    [Serializable]
    public class Empty : Effect
    {
        protected override void OnUpdate(EventArgs args) => IsDone = true;
    }
}