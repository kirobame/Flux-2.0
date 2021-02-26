using Flux;
using UnityEngine;

namespace Example04
{
    public class Remote : MonoBehaviour // Utility class printing the message inside the data class every step in seconds
    {
        [SerializeField] private float step;
        
        private SomeData data;
        private float tick; // Countdown until the next print
        
        //---[Initialization]-------------------------------------------------------------------------------------------/

        void Awake()
        {
            data = new SomeData() {message = "Original "}; // Setting a base value
            tick = step;
            
            // Any data can be referenced. Do mind that it is only linked if it is a reference type
            Repository.Register(Name.Link, data); 
        }

        // Because the referencing of SomeData is not handled by a Referencer, unregistering must be done by the owner
        // of the reference
        void OnDisable() => Repository.Unregister(Name.Link);
        
        //---[Core]-----------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            tick -= Time.deltaTime;
            if (tick > 0) return;

            tick = step - tick;
            Debug.Log($"-[{Time.time}]---|{data.message}");
        }
    }
}