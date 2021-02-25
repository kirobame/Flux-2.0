using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Flux
{
    public abstract class Provider { }
    
    [Serializable]
    public abstract class Provider<T,TPoolable> : Provider, IBootable where TPoolable : Poolable<T>
    {
        public event Action<Provider<T, TPoolable>> onLoaded;
        
        public TPoolable Prefab => actualPrefab;
        public int Count => count;

        public IReadOnlyList<TPoolable> Instances => instances;
            
        [SerializeField] protected AssetReference prefab;
        [SerializeField] protected int count = 1;
        [SerializeField] protected TPoolable[] instances;

        protected TPoolable actualPrefab;

        public void Bootup()
        {
            var handle = prefab.LoadAssetAsync<GameObject>();
            handle.Completed += OnAssetLoaded;
        }

        void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            actualPrefab = handle.Result.GetComponent<TPoolable>();
            onLoaded?.Invoke(this);
        }
    }
}