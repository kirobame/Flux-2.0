using System;
using UnityEngine;

namespace Flux.Event
{
    public class Listener : MonoBehaviour
    {
        [SerializeField] private DynamicFlag flag;
        [SerializeField] private GenericEvent genericEvent;

        private bool hasBeenInitialized;

        void Awake()
        {
            flag.Bootup();
            Events.Subscribe(flag.Value, Callback);
        }

        void OnEnable()
        {
            if (!hasBeenInitialized)
            {
                hasBeenInitialized = true;
                return;
            }
            
            Events.Subscribe(flag.Value, Callback);
        }
        void OnDisable() => Events.Unsubscribe(flag.Value, Callback);

        void Callback(EventArgs args) => genericEvent.Invoke(args);
    }
}