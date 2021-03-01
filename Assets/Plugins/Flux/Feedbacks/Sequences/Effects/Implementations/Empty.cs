using System;

namespace Flux.Feedbacks
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