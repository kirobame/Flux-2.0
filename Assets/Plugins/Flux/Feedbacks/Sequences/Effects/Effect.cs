using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable]
    public abstract class Effect : IEffect
    {
        public Effect() { }
        
        //---[Data]-----------------------------------------------------------------------------------------------------/
        
        public bool IsDone { get; protected set; }
        
        #if UNITY_EDITOR
        [HideInInspector, SerializeField] private Rect rect = new Rect(100,100,250,150);
        #endif

        IEnumerable<int> IEffect.LinkIndices => linkIndices;
        [HideInInspector, SerializeField] private List<int> linkIndices = new List<int>();

        void IInjectable<Effect[]>.Inject(Effect[] links) => this.links = links;
        private Effect[] links;
        
        //---[Initializations]------------------------------------------------------------------------------------------/

        public virtual void Bootup(Effect root, IReadOnlyList<Effect> effects) { }
        public virtual void Ready() => IsDone = false;
        
        public void Reset()
        {
            OnReset();
            foreach (var link in links) link.Reset();
        }
        protected virtual void OnReset() => Ready();
        
        //---[Flow control]---------------------------------------------------------------------------------------------/
        
        public bool Update(EventArgs args)
        {
            if (!IsDone)
            {
                OnUpdate(args);
                
                if (!IsDone) return false;
                else return Relay();
            }
            return Relay();

            bool Relay()
            {
                args = OnTraversed(args);
                var hasFinished = true;

                foreach (var link in links)
                {
                    if (hasFinished) hasFinished = link.Update(args);
                    else link.Update(args);
                }

                return hasFinished;
            }
        }

        protected abstract void OnUpdate(EventArgs args);
        protected virtual EventArgs OnTraversed(EventArgs args) => args;
    }
}