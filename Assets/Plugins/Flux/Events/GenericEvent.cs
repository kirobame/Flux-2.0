using System;
using UnityEngine.Events;

namespace Flux
{
    [Serializable]
    public class GenericEvent : UnityEvent<EventArgs> { }
}