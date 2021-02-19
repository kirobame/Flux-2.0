using System;

namespace Flux
{
    [Serializable]
    public abstract class InstantEffect : Effect
    {
        protected override void OnUpdate(EventArgs args) => IsDone = true;
    }
}