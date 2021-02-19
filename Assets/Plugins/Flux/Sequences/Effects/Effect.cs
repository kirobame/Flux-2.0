using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    [Serializable]
    public abstract class Effect : IEffect
    {
        public Effect() { }
        
        public bool IsDone { get; protected set; }
        
        #if UNITY_EDITOR
        [HideInInspector, SerializeField] private Rect rect = new Rect(100,100,250,150);
        #endif

        IEnumerable<int> IEffect.LinkIndices => linkIndices;
        [HideInInspector, SerializeField] private List<int> linkIndices = new List<int>();

        void IEffect.Inject(Effect[] links) => this.links = links;
        private Effect[] links;

        public virtual void Reset() => IsDone = false;
        public bool Update(EventArgs args)
        {
            if (!IsDone)
            {
                OnUpdate(args);
                return false;
            }
            else
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