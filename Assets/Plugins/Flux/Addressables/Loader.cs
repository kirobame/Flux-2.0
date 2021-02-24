using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Flux
{
    public class Loader
    {
        public Loader()
        {
            values = new List<LoadedValue>();
            count = 0;
        }

        public event Action<object[]> onDone;

        private List<LoadedValue> values;
        private int count;
        
        public void Register(byte index, AsyncOperationHandle handle)
        {
            count++;

            handle.Completed += OnHandleCompletion;
            values.Add(new LoadedValue(index, handle));
        }

        void OnHandleCompletion(AsyncOperationHandle handle)
        {
            count--;
            if (count <= 0)
            {
                values.Sort();
                
                var output = new object[values.Count];
                for (var i = 0; i < values.Count; i++) output[i] = values[i].GetRaw();
                
                onDone?.Invoke(output);
            }
        }
    }
}