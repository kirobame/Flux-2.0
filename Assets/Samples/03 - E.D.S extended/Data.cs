using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Example03
{
    public struct Advance : IBridge<Transform>
    {
        public float value;
        public int step;
        public bool isLocked;

        public void ReceiveDataFrom(Transform component) => value = component.position.x;
        public void SendDataTo(Transform component)
        {
            var position = component.position;
            position.x = value;

            component.position = position;
        }
    }
}