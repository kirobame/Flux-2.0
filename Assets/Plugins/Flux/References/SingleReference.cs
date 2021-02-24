using UnityEngine;

namespace Flux
{
    public class SingleReference : Reference
    {
        public override object Value => value;

        [SerializeField] private Object value;
    }
}