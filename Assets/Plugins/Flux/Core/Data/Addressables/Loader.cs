using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Flux.Data
{
    /// <summary>Utility class used to merge <c>AsyncOperationHandle</c> callbacks into one.</summary>
    public class Loader
    {
        public Loader()
        {
            values = new List<LoadedValue>();
            count = 0;
        }
        
        //---[Events]---------------------------------------------------------------------------------------------------/
        
        /// <summary>Called when all registered <c>AsyncOperationHandle</c> are completed.
        /// Returns a list of all the results of each registered handle sorted by their associated <c>byte</c> index.</summary>
        public event Action<object[]> onDone;

        private List<LoadedValue> values; // Handles are registered into a list for the innate sorting capacities of the container
        private int count;
        
        //---[Methods]--------------------------------------------------------------------------------------------------/
        
        /// <summary>Adds a dependency onto the given <c>AsyncOperationHandle</c>'s completed callback.</summary>
        /// <param name="index">Sorting value.</param>
        public void Register(byte index, AsyncOperationHandle handle)
        {
            count++;

            handle.Completed += OnHandleCompletion;
            values.Add(new LoadedValue(index, handle));
        }

        void OnHandleCompletion(AsyncOperationHandle handle)
        {
            count--;
            if (count <= 0) // If all of the handles are completed, begin value listing & merged callback
            {
                values.Sort();
                
                var output = new object[values.Count];
                for (var i = 0; i < values.Count; i++) output[i] = values[i].GetRaw();
                
                onDone?.Invoke(output);
            }
        }
    }
}