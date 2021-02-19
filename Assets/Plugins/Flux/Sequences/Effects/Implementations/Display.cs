using System;
using UnityEngine;

namespace Flux
{
    [Serializable, Path("Three/SubThree")]
    public class Display : InstantEffect
    {
        [SerializeField] private string message;

        protected override EventArgs OnTraversed(EventArgs args)
        {
            Debug.Log(message);
            return base.OnTraversed(args);
        }
    }
}