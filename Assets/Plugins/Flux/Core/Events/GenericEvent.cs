using System;
using UnityEngine.Events;

namespace Flux.Event
{
    [Serializable]
    public class GenericEvent : UnityEvent<EventArgs> { }
}