using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Flux.Data
{
    public class GenericPool : Pool<Object,GenericPoolable>
    {
        #region Nested Types

        [Serializable]
        private class GenericProvider : Provider<Object, GenericPoolable> { }

        #endregion

        protected override IList<Provider<Object,GenericPoolable>> Providers => providers;
        [SerializeField] private GenericProvider[] providers;

        public T CastSingle<T>() where T : Object => (T)RequestSinglePoolable().Value;
        public T CastSingle<T>(GenericPoolable key) where T : Object => (T)RequestSinglePoolable(key).Value;
        
        public T[] Cast<T>(int count) where T : Object => RequestPoolable(count).Select(poolable => poolable.Value).Cast<T>().ToArray();
        public T[] Cast<T>(GenericPoolable key, int count) where T : Object => RequestPoolable(key, count).Select(poolable => poolable.Value).Cast<T>().ToArray();
    }
}