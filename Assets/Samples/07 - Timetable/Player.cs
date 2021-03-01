using System.Collections;
using System.Collections.Generic;
using Flux;
using Flux.Feedbacks;
using UnityEngine;

namespace Example07
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Timetable timetable;
        [SerializeField] private float speed;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/

        void Awake()
        {
            timetable.Initialize(); // Every timetable needs to be initialized to give a chance for every segment to boot itself up
            
            // Subscribes to the FloatOutput segment by giving its outgoing Id
            timetable.SubscribeTo<float>(new Id('H', 'G', 'T'), value =>
            {
                var scale = transform.localScale;
                scale.y = value;
                
                transform.localScale = scale;
            });
            
            timetable.GoTo(0.0f); // Assures the execution of the base state
        }
        
        //---[Core]-----------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            var  input = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            if (input == Vector3.zero) return;
            
            input = Quaternion.AngleAxis(45, Vector3.up) * input; // Convert the input for the 45° yaw bias of the Camera
            var delta = input.normalized * (Time.deltaTime * speed);
            var endPosition = transform.position + delta; // Compute where the player will be after this input
            
            var ray = new Ray(endPosition + Vector3.up, Vector3.down); // If the position is valid, move him
            if (!Physics.Raycast(ray, float.PositiveInfinity, LayerMask.GetMask("Environment"))) return;

            transform.position = endPosition; // Application of the computed position
            
            // The timetable handles automatically its looping mechanism
            // It only has to be updated at the same time of the logic
            timetable.MoveBy(Time.deltaTime); 
        }
    }
}