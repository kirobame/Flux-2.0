using System;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Flux.Feedbacks
{
    [Serializable]
    public abstract class Segment
    {
        public Segment() { }
        
        public float Start => span.x;
        public float End => span.y;
        
        [SerializeField] private Vector2 span = new Vector2(0.25f, 0.75f);
        [SerializeField] private AnimationCurve curve;
        
        protected Timetable table { get; private set; }

        public virtual void Open(Timetable table) => this.table = table;
        
        internal void Update(float globalRatio)
        {
            var localRatio = Mathf.InverseLerp(span.x, span.y, globalRatio);
            Execute(curve.Evaluate(localRatio));
        }
        protected abstract void Execute(float ratio);
        
        public virtual void Close() { }
    }
}