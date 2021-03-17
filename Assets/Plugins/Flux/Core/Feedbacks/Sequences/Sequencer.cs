using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flux.Feedbacks
{
    public class Sequencer : MonoBehaviour
    {
        [HideInInspector, SerializeField] private Empty root = new Empty();
        [HideInInspector, SerializeReference] private List<Effect> effects = new List<Effect>();
        
        private EventArgs args;
        private bool isPlaying;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/
        
        void Awake()
        {
            Initialize(root);
            foreach (var effect in effects) Initialize(effect);
        }
        private void Initialize(Effect effect)
        {
            var cast = (IEffect)effect;

            var links = new Effect[cast.LinkIndices.Count()];
            var count = 0;
                
            foreach (var linkIndex in cast.LinkIndices)
            {
                links[count] = effects[linkIndex];
                count++;
            }
                
            cast.Inject(links);
            effect.Bootup(root, effects);
        }
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        void Update()
        {
            if (isPlaying) isPlaying = !root.Update(args);
        }

        public void Play(EventArgs args)
        {
            this.args = args;
            
            root.Ready();
            foreach (var effect in effects) effect.Ready();

            isPlaying = true;
        }
        public void Stop() => isPlaying = false;
    }
}