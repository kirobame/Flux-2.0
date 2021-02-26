using Example02;
using Flux;
using UnityEngine;

namespace Example03
{
    // Systems can declare in which Update group they and which is the order for their update
    // Any UpdateAttribute must first declare the path at which the System will be placed
    // The last .../Value will be the name of the group or system
    // Then order must always respect the format After/Before and allows sorting of groups or systems in their local hierarchy
    // A group only has to be declared once
    [Group("Init", "Any/Update"), Order("Init/Reset", "Any/Any")]
    public class ResetSystem : Flux.System // Resets the isLocked value before any the other systems are Updated because Init goes before Update
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref Advance advance) => { advance.isLocked = false; });
        }
    }
    
    //---[Systems in the Update group]----------------------------------------------------------------------------------/
    
    // Try to switch the order for the OrderAttribute to notice if the actions can still be made or not
    [Group("Update", "Init/Any"), Order("Update/Lock", "Race/Any")]
    public class LockSystem : Flux.System // Locks the ability to modify the Advance data by the Race system only if it is updated before it
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref Advance advance) => { advance.isLocked = true; });
        }
    }
    
    // The update group has already been declared, this system only has specify that it is a child of it with its path
    [Order("Update/Race", "Any/Any")]
    // A BindedSystem provides Utility methods to bind data to Addressable values
    // Update will not be called until all bindings are resolved
    public class RaceSystem : BindedSystem 
    {
        private float distance;
        private void SetDistance(float value) => distance = value;
        
        // OR
        // private Settings settings;

        public override void Initialize()
        {
            base.Initialize();
            BindTo<Settings, float>("03-Settings", SetDistance); // This can only be done because Settings is IWrapper<float>
            
            // OR
            // BindWhole<Settings>("03-Settings", value => settings = value);
        }
        
        //---[Behaviour]------------------------------------------------------------------------------------------------/

        public override void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            Entities.ForEach((Entity entity, ref Advance advance) =>
            {
                if (advance.isLocked) return;
                
                advance.value += distance * advance.step;
                Entities.MarkDirty<Transform>(entity, advance);

            }, Health.Good); 
            // Precising flags allows to fetch the entities with corresponding Data & Flags
            // Character<C> can never run because he is flagged with Health.Poor
            // Character<B> can run if the Health.Good flag is added to him by the FlagHandler
        }
    }
}