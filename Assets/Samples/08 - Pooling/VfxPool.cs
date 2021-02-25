using System;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Example08
{
    public class VfxPool : Pool<ParticleSystem, PoolableVfx>
    {
        #region Nested Types

        [Serializable]
        private class VfxProvider : Provider<ParticleSystem,PoolableVfx> { }

        #endregion

        protected override IList<Provider<ParticleSystem, PoolableVfx>> Providers => providers;
        [SerializeField] private VfxProvider[] providers;
    }
}