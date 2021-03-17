using UnityEngine;

namespace Flux
{
    public class KeyValuePair<TKey, TValue>
    {
        public TKey Key => key;
        [SerializeField] private TKey key;
        
        public TValue Value => value;
        [SerializeField] private TValue value;
    }
}