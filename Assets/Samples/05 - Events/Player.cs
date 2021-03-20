using System.Collections;
using Flux;
using Flux.Event;
using UnityEngine;

namespace Example05
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        void Awake() => Debug.Log("Player has spawned"); // Attestation of the Player spawns, after the CameraTracker
        
        void Update()
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input != Vector2.zero)
            {
                input.Normalize();
                var delta = input * (Time.deltaTime * speed);
                
                transform.Translate(delta);
                
                // Calling the event once the operation has been done
                Events.ZipCall(GameEvent.OnPlayerMove, delta); // == Events.Call(GameEvent.OnPlayerMove, new WrapperArgs<Vector2>(delta));
            }
        }
    }
}