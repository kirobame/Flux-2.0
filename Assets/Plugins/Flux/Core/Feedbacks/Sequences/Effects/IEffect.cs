using System.Collections.Generic;

namespace Flux.Feedbacks
{
    public interface IEffect : IInjectable<Effect[]>
    {
        IEnumerable<int> LinkIndices { get; }
    }
}