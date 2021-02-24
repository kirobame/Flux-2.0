using Flux;
using UnityEngine;

namespace Example03
{
    public class FlagHandler : MonoBehaviour
    {
        [SerializeField] private Entity entity;

        private bool state;
        
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