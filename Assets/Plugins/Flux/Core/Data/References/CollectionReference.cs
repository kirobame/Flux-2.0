using UnityEngine;

namespace Flux.Data
{
    public class CollectionReference : Reference
    {
        public override object Value => values;

        [SerializeField] private Object[] values;
    }
}