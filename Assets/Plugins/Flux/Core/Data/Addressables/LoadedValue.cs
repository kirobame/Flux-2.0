using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Flux.Data
{
    /// <summary>Wrapper holding a loading <c>AsyncOperationHandle</c>.</summary>
    internal struct LoadedValue : IComparable<LoadedValue>
    {
        public LoadedValue(byte index, AsyncOperationHandle handle)
        {
            Index = index;
            this.handle = handle;
        }
        
        //---[Data]-----------------------------------------------------------------------------------------------------/
        
        /// <summary>Sorting value.</summary>
        public byte Index { get; private set; } 

        private AsyncOperationHandle handle; 
        
        //---[Getters]--------------------------------------------------------------------------------------------------/
        
        public T Get<T>() => (T)handle.Result;
        public object GetRaw() => handle.Result;

        //---[Sorting]--------------------------------------------------------------------------------------------------/
        
        int IComparable<LoadedValue>.CompareTo(LoadedValue other) => Index.CompareTo(other.Index);
    }
}