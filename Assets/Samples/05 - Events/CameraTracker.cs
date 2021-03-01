using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flux;
using Flux.Event;
using UnityEngine;

namespace Example05
{
    public class CameraTracker : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        
        private Queue<string> queue;
        
        //---[Lifetime handling]----------------------------------------------------------------------------------------/
        
        void Awake()
        {
            queue = new Queue<string>();
            Debug.Log("Camera tracker is active"); // Attestation of the CameraTracker initialization, before the Player
            
            // Subscription to the Player event before its instantiation is not a problem
            // Many overloads exists to ease standard processes like simply passing data or specific EventArgs
            Events.RelayByVoid(GameEvent.OnPlayerMove, OnPlayerMoveArgless); // Passes nothing
            Events.Register(GameEvent.OnPlayerMove, OnPlayerMove); // Standard subscription
            Events.RelayByCast<WrapperArgs<Vector2>>(GameEvent.OnPlayerMove, OnPlayerMoveCasted); // Casts implicitly the received EventArgs
            Events.RelayByValue<Vector2>(GameEvent.OnPlayerMove, OnPlayerMoveExplicit); // Directly passes a value if possible
        }
        
        void OnDestroy() // Like regular delegates, unsubscription must be based on logic AND lifetime!
        {
            Events.BreakVoidRelay(GameEvent.OnPlayerMove, OnPlayerMoveArgless);
            Events.Unregister(GameEvent.OnPlayerMove, OnPlayerMove);
            Events.BreakCastRelay<WrapperArgs<Vector2>>(GameEvent.OnPlayerMove, OnPlayerMoveCasted);
            Events.BreakValueRelay<Vector2>(GameEvent.OnPlayerMove, OnPlayerMoveExplicit);
            
            // Alternatively, you can wipe the current event addresses with Events.Clear();
        }
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        void Update() // Prints all received callback declared in Awake
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
        
        //---[Callbacks]------------------------------------------------------------------------------------------------/

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