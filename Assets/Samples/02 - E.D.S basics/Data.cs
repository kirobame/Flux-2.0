using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Example02
{
    // For a struct (or class) to be registered by the System, it must be marked with IData interface
    // which enforces no contract. IReader<T>, IWriter<T> & IBridge<T> are alternatives but force a link to specified
    // T Component type to be made
    public struct Speed : IData 
    {
        public float value; // The speed at which processes are made based on Time.delta time
    }
    
    public struct Rotation : IWriter<Transform>
    {
        public Vector3 axis;
        public float angle;
        
        //---[Links]----------------------------------------------------------------------------------------------------/
        
        // Explicit implementation of the IReader<T>, IWriter<T> & IBridge<T> interfaces are preferred to hide the logic
        // Write data to the linked Transform
        void IWriter<Transform>.SendDataTo(Transform component) => component.rotation = Quaternion.AngleAxis(angle, axis);
    }
    
    public struct Height : IBridge<Transform>, IWriter<MeshRenderer>
    {
        public float value; // == Transform.position.y
        public float offset; // The base base offset for sin function
        
        public Vector2 range; // The range between which the Height.value will be taken into account to Lerp between min & max Color
        public Color minColor;
        public Color maxColor;
        
        //---[Links]----------------------------------------------------------------------------------------------------/

        // Read & Write data to the linked Transform
        void IBridge<Transform>.ReceiveDataFrom(Transform component) => offset = component.position.y;
        void IBridge<Transform>.SendDataTo(Transform component)
        {
            var position = component.position;
            position.y = value;
            
            component.position = position;
        }
        
        // Write data to the linked MeshRenderer
        void IWriter<MeshRenderer>.SendDataTo(MeshRenderer component)
        {
            var ratio = Mathf.InverseLerp(range.x, range.y, value);
            var color = Color.Lerp(minColor, maxColor, ratio);
                
            component.material.SetColor("_Color", color);
        }
    }
}