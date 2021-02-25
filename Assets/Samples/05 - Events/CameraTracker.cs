using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flux;
using UnityEngine;

namespace Example05
{
    public class CameraTracker : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        
        private Queue<string> queue;
        
        void Awake()
        {
            queue = new Queue<string>();
            Debug.Log("Camera tracker is active");
            
            Events.RelayByVoid(GameEvent.OnPlayerMove, OnPlayerMoveArgless);
            Events.Register(GameEvent.OnPlayerMove, OnPlayerMove);
            Events.RelayByCast<WrapperArgs<Vector2>>(GameEvent.OnPlayerMove, OnPlayerMoveCasted);
            Events.RelayByValue<Vector2>(GameEvent.OnPlayerMove, OnPlayerMoveExplicit);
        }

        void Update()
        {
            if (!queue.Any()) return;

            var builder = new StringBuilder();
            while (queue.Count > 0)
            {
                var message = queue.Dequeue();
                builder.AppendLine(message);
            }
            
            Debug.Log(builder);
        }

        void OnPlayerMoveArgless()
        {
            queue.Enqueue("VOID : Player has moved");
        }
        void OnPlayerMove(EventArgs args)
        {
            if (!(args is WrapperArgs<Vector2> castedArgs)) return;
            
            queue.Enqueue($"IMPLICIT : Player has moved");
        }
        void OnPlayerMoveCasted(WrapperArgs<Vector2> args)
        {
            queue.Enqueue($"CAST : Player has moved");
        }
        void OnPlayerMoveExplicit(Vector2 delta)
        {
            queue.Enqueue($"EXPLICIT : Player has moved");
            camera.transform.Translate(delta);
        }
    }
}