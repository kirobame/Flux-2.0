using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable, Path("Utilities")]
    public class Label : Effect
    {
        public string Name => name;
        [SerializeField] private string name;

        protected override void OnUpdate(EventArgs args)
        {
            IsDone = true;
            return;
        }
    }
}