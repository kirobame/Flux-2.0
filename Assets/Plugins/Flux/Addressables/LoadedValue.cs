using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Flux
{
    public struct LoadedValue : IComparable<LoadedValue>
    {
        public LoadedValue(byte index, AsyncOperationHandle handle)
        {
            Index = index;
            this.handle = handle;
        }
        
        public byte Index { get; private set; }

        private AsyncOperationHandle handle;
        
        public T Get<T>() => (T)handle.Result;
        public object GetRaw() => handle.Result;

        int IComparable<LoadedValue>.CompareTo(LoadedValue other) => Index.CompareTo(other.Index);
    }
}