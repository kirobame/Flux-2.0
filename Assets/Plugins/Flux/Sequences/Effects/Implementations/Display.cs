using System;
using UnityEngine;

namespace Flux
{
    [Path("Utilities")]
    public class Display : Effect
    {
        [SerializeField] private string message;

        protected override void OnUpdate(EventArgs args)
        {
            Debug.Log(message);
            IsDone = true;

            return;
        }
    }
}