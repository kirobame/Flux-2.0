using System;

namespace Flux
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