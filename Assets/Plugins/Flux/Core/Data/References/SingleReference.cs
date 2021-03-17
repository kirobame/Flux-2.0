using UnityEngine;

namespace Flux.Data
{
    public class SingleReference : Reference
    {
        public override object Value => value;

        [SerializeField] private Object value;
    }
}