using Flux;
using UnityEngine;

namespace Example02
{
    public class RotateSystem : Flux.System
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref Rotation rotation, in Speed speed) =>
            {
                rotation.angle += Time.deltaTime * speed.value;
                Entities.MarkDirty<Transform>(entity, rotation);
            });
        }
    }
    
    public class OscillateSystem : Flux.System
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref Height height, in Speed speed) =>
            {
                height.value = Mathf.Sin(height.offset + Time.time * speed.value * 0.1f);
                Entities.MarkDirty<Transform, MeshRenderer>(entity, height);
            });
        }
    }
}