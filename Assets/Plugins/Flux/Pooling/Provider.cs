using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    public abstract class Provider { }
    
    [Serializable]
    public abstract class Provider<T,TPoolable> : Provider where TPoolable : Poolable<T>
    {
        public TPoolable Prefab => prefab;
        public int Count => count;

        public IReadOnlyList<TPoolable> Instances => instances;
            
        [SerializeField] protected TPoolable prefab;
        [SerializeField] protected int count = 1;
            
        [SerializeField] protected TPoolable[] instances;
    }
}