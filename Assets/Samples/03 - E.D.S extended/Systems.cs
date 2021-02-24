using Example02;
using Flux;
using UnityEngine;

namespace Example03
{
    [Group("Init", "Any/Any"), Order("Init/Reset", "Any/Any")]
    public class ResetSystem : Flux.System
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref Advance advance) => { advance.isLocked = false; });
        }
    }
    
    [Group("Update", "Any/Any"), Order("Update/Lock", "Race/Any")]
    public class LockSystem : Flux.System
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref Advance advance) => { advance.isLocked = true; });
        }
    }
    
    [Order("Update/Race", "Any/Any")]
    public class RaceSystem : BindedSystem
    {
        private float distance;
        private void SetDistance(float value) => distance = value;

        public override void Initialize()
        {
            base.Initialize();
            BindTo<Settings, float>("03-Settings", SetDistance);
        }

        public override void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            Entities.ForEach((Entity entity, ref Advance advance) =>
            {
                if (advance.isLocked) return;
                
                advance.value += distance * advance.step;
                Entities.MarkDirty<Transform>(entity, advance);

            }, Health.Good);
        }
    }
}