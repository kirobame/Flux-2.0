using System;
using Flux;
using UnityEngine;

namespace Example06
{
    public class Sender : MonoBehaviour
    {
        [SerializeField] private float channel;
        
        void Awake()
        {
            Events.Open(Feedback.Unique);
            
            // Create the EventArgs which will be sent to the sequence & subscribe to its callback to resume logic after the feedback is done
            var args = new CustomArgs(channel);
            args.onDone += OnDone;
            
            // Call the event after 0.75seconds
            StartCoroutine(Routines.DoAfter(() => Events.Call(Feedback.Unique, args), 0.75f));
        }

        void OnDone(EventArgs args) => Debug.Log("Logic can be resumed");
    }
}