using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Utilities")]
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