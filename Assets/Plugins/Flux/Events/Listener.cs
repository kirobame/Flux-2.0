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
            Events.Register(flag.Value, Callback);
        }

        void OnEnable()
        {
            if (!hasBeenInitialized)
            {
                hasBeenInitialized = true;
                return;
            }
            
            Events.Register(flag.Value, Callback);
        }
        void OnDisable() => Events.Unregister(flag.Value, Callback);

        void Callback(EventArgs args) => genericEvent.Invoke(args);
    }
}