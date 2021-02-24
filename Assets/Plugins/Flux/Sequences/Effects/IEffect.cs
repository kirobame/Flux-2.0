using System.Collections.Generic;

namespace Flux
{
    public interface IEffect : IInjectable<Effect[]>
    {
        IEnumerable<int> LinkIndices { get; }
    }
}