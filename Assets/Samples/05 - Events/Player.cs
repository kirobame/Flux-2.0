using System.Collections;
using Flux;
using UnityEngine;

namespace Example05
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        void Awake()
        {
            Debug.Log("Player has spawned");
            Events.Open(GameEvent.OnPlayerMove);
        }
        
        void Update()
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input != Vector2.zero)
            {
                input.Normalize();
                var delta = input * (Time.deltaTime * speed);
                
                transform.Translate(delta);
                
                Events.ZipCall(GameEvent.OnPlayerMove, delta);
            }
        }
    }
}