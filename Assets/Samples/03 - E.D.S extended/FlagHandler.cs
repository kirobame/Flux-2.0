using Flux;
using Flux.EDS;
using UnityEngine;

namespace Example03
{
    // Simple utility class to toggle between addition & removal of a flag
    public class FlagHandler : MonoBehaviour
    {
        [SerializeField] private Entity entity;

        private bool state;
        
        //---[Core]-----------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.P)) return;

            if (!state)
            {
                entity.AddFlag(Health.Good);
                state = true;
            }
            else
            {
                entity.RemoveFlag(Health.Good);
                state = false;
            }
        }
    }
}