using System;
using UnityEngine;

namespace Flux.Feedbacks
{
    public class SequenceWrapper : MonoBehaviour
    {
        [SerializeField] private Sequence value;

        void Awake() => value.Bootup();

        void Update()
        {
            if (!value.IsPlaying) return;
            value.Update();
        }
        
        public void Play(EventArgs args) => value.Prepare(args);
    }
}