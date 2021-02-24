using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Example02
{
    public struct Speed : IData
    {
        public float value;
    }
    
    public struct Rotation : IWriter<Transform>
    {
        public Vector3 axis;
        public float angle;
        
        void IWriter<Transform>.SendDataTo(Transform component) => component.rotation = Quaternion.AngleAxis(angle, axis);
    }
    
    public struct Height : IBridge<Transform>, IWriter<MeshRenderer>
    {
        public float value;
        public float offset;
        public Vector2 range;
        public Color minColor;
        public Color maxColor;

        void IBridge<Transform>.ReceiveDataFrom(Transform component) => offset = component.position.y;
        void IBridge<Transform>.SendDataTo(Transform component)
        {
            var position = component.position;
            position.y = value;
            
            component.position = position;
        }
        
        void IWriter<MeshRenderer>.SendDataTo(MeshRenderer component)
        {
            var ratio = Mathf.InverseLerp(range.x, range.y, value);
            var color = Color.Lerp(minColor, maxColor, ratio);
                
            component.material.SetColor("_Color", color);
        }
    }
}