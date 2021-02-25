using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Example07
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Timetable timetable;
        [SerializeField] private float speed;

        void Awake()
        {
            timetable.Initialize();
            timetable.GoTo(0.0f);
            
            timetable.SubscribeTo<float>(new Id('H', 'G', 'T'), value =>
            {
                var scale = transform.localScale;
                scale.y = value;
                transform.localScale = scale;
            });
        }
        
        void Update()
        {
            var  input = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            if (input == Vector3.zero) return;
            
            input = Quaternion.AngleAxis(45, Vector3.up) * input;
            var delta = input.normalized * (Time.deltaTime * speed);
            var endPosition = transform.position + delta;
            
            var ray = new Ray(endPosition + Vector3.up, Vector3.down);
            if (!Physics.Raycast(ray, float.PositiveInfinity, LayerMask.GetMask("Environment"))) return;

            transform.position = endPosition;
            timetable.MoveBy(Time.deltaTime);
        }
    }
}