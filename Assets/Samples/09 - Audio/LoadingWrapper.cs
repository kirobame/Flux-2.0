using Flux;
using Flux.Data;
using UnityEngine;

namespace Example09
{
    // The loading process of AssetReferences can be partially automated for Q.O.L
    public abstract class LoadingWrapper : MonoBehaviour
    {
        protected Loader loader { get; private set; }

        protected void Awake()
        {
            loader = new Loader();
            loader.onDone += OnAllHandlesCompleted;
            
            OnAwake();

            enabled = false;
        }
        protected virtual void OnAwake() { }

        protected void OnAllHandlesCompleted(object[] values)
        {
            enabled = true;
            OnLoadingDone(values);
        }
        protected abstract void OnLoadingDone(object[] values);
    }
}