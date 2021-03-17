using System;

namespace Flux.Feedbacks
{
    [Serializable, Path("Utilities")]
    public class End : Effect
    {
        protected override void OnUpdate(EventArgs args)
        {
            ((ISendback)args).End(EventArgs.Empty);
            IsDone = true;
        }
    }
}