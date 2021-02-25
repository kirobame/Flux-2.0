using System;
using Flux;
using UnityEngine;

namespace Example06
{
    public class CustomArgs : EventArgs, IWrapper<float>, ISendback
    {
        public event Action<EventArgs> onDone;

        public CustomArgs(float value) => Value = value;
        
        public float Value { get; private set; }

        public void End(EventArgs args) => onDone?.Invoke(args);
    }
    
    public class Sender : MonoBehaviour
    {
        [SerializeField] private float channel;
        
        void Awake()
        {
            Events.Open(Feedback.Unique);
            
            var args = new CustomArgs(channel);
            args.onDone += OnDone;
            
            StartCoroutine(Routines.DoAfter(() => Events.Call(Feedback.Unique, args), 0.75f));
        }

        void OnDone(EventArgs args) => Debug.Log("Logic can be resumed");
    }
}