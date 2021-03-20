using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable]
    public class Sequence : IBootable
    {
        public EventArgs Args => args;
        public bool IsPlaying => isPlaying;
        
        [HideInInspector, SerializeField] private Empty root = new Empty();
        [HideInInspector, SerializeReference] private List<Effect> effects = new List<Effect>();

        private bool hasBeenBootedUp;
        
        private EventArgs args;
        private bool isPlaying;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/
        
        public void Bootup()
        {
            Initialize(root);
            foreach (var effect in effects) Initialize(effect);

            hasBeenBootedUp = true;
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

        public void Play(EventArgs args)
        {
            if (isPlaying) return;
            
            Prepare(args);
            SequenceHandler.Add(this);
        }
        public void Stop(bool interruption = true)
        {
            if (!isPlaying) return;
            
            if (interruption) SequenceHandler.Remove(this);
            isPlaying = false;
        }

        public void Prepare(EventArgs args)
        {
            if (!hasBeenBootedUp) Bootup();
            
            this.args = args;
            
            root.Ready();
            foreach (var effect in effects) effect.Ready();
            
            isPlaying = true;
        }
        public bool Update()
        {
            if (isPlaying) isPlaying = !root.Update(args);
            return isPlaying;
        }
    }
}