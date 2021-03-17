using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Flux.Data
{
    public abstract class Provider { }

    [Serializable]
    public abstract class Provider<T,TPoolable> : Provider, IBootable where TPoolable : Poolable<T>
    {
        public event Action<Provider<T, TPoolable>> onLoaded;
        
        public TPoolable Prefab => mode ? directRef : actualPrefab;
        public int Count => count;

        public IReadOnlyList<TPoolable> Instances => instances;
        
        [SerializeField] private bool mode;
        [SerializeField] protected AssetReference Address;
        [SerializeField] protected TPoolable directRef;
        [SerializeField] protected int count = 1;
        [SerializeField] protected TPoolable[] instances;

        protected TPoolable actualPrefab;

        public void Bootup()
        {
            if (mode) onLoaded?.Invoke(this);
            else
            {
                var handle = Address.LoadAssetAsync<GameObject>();
                handle.Completed += OnAssetLoaded;
            }
        }

        void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            actualPrefab = handle.Result.GetComponent<TPoolable>();
            onLoaded?.Invoke(this);
        }
    }
}