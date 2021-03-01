using Flux;
using Flux.EDS;
using UnityEngine;

namespace Example02
{
    public class RotateSystem : Flux.EDS.System
    {
        public override void Update() // Systems are the last to be updated in the Player loop
        {
            // A behaviour can be executed onto a specific query
            // In this case, this code will execute for all entities which have both Rotation & Speed data
            // ---[01] nothing = Passes a copy
            // ---[02] ref = Read & Write
            // ---[03] in = Read only
            // Keywords must be declared in this order (E.g : ...in Speed speed, ref Rotation rotation)... would not compile)
            Entities.ForEach((Entity entity, ref Rotation rotation, in Speed speed) =>
            {
                // Calls are single threaded, it is safe to access the main thread like with Time.deltaTime
                rotation.angle += Time.deltaTime * speed.value; 
                
                // For some linked data (IWrite<T>, IBridge<T>) to have its value updated on the associated Unity component
                // MarkDirty must called by indicating the Unity Component type to dirty, the associated Data making the 
                // link to this component & finally the entity on which to find the data
                Entities.MarkDirty<Transform>(entity, rotation);
            });
        }
    }
    
    public class OscillateSystem : Flux.EDS.System
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref Height height, in Speed speed) =>
            {
                height.value = Mathf.Sin(height.offset + Time.time * speed.value * 0.1f);
                
                // You can mark more than one Component type as long as the given data has links to all of them
                Entities.MarkDirty<Transform, MeshRenderer>(entity, height);
            });
        }
    }
}