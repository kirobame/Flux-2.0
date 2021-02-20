using System;

namespace Flux
{
    [Serializable, Path("Utilities")]
    public class Empty : Effect
    {
        protected override void OnUpdate(EventArgs args)
        {
            IsDone = true;
            return;
        }
    }
}