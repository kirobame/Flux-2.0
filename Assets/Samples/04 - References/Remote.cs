using Flux;
using UnityEngine;

namespace Example04
{
    public class Remote : MonoBehaviour
    {
        [SerializeField] private float step;
        
        private SomeData data;
        private float tick;

        void Awake()
        {
            data = new SomeData() {message = "Original "};
            tick = step;
            
            Repository.Register(Name.Link, data);
        }

        void Update()
        {
            tick -= Time.deltaTime;
            if (tick > 0) return;

            tick = step - tick;
            Debug.Log($"-[{Time.time}]---|{data.message}");
        }
    }
}