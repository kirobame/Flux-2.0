using UnityEngine;

namespace Flux
{
    public class CollectionReference : Reference
    {
        public override object Value => values;

        [SerializeField] private Object[] values;
    }
}