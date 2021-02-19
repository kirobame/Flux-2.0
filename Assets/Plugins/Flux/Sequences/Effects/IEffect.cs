using System.Collections.Generic;

namespace Flux
{
    public interface IEffect
    {
        IEnumerable<int> LinkIndices { get; }
        void Inject(Effect[] links);
    }
}