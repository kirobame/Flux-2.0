using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Example03
{
    public struct Advance : IBridge<Transform>
    {
        public float value; // == Transform.position.x
        public int step; // The number of times the Character will run for one single call
        
        public bool isLocked; // Supplementary data to showcase System Update group & order

        public void ReceiveDataFrom(Transform component) => value = component.position.x;
        public void SendDataTo(Transform component)
        {
            var position = component.position;
            position.x = value;

            component.position = position;
        }
    }
}