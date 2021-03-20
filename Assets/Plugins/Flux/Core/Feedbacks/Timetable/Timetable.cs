using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flux.Feedbacks
{
    [Serializable]
    public class Timetable : IBootable
    {
        public event Action onEndReached;
        public event Action onStartReached;
        
        public float Duration
        {
            get => duration;
            set
            {
                duration = value;
                GoTo(time);
            }
        }

        [SerializeReference] private List<Segment> segments;
        [SerializeField] private float duration;
        public bool isLooping;
        
        private Dictionary<Id, Pin> pins = new Dictionary<Id, Pin>();

        private bool hasBeenBootedUp;
        private float time;
        
        //---[Accessors]------------------------------------------------------------------------------------------------/

        public Segment this[int index] => segments[index];
        public Pin this[Id key] => pins[key];
        
        public T GetSegment<T>() where T : Segment 
        {
            foreach (var segment in segments)
            {
                if (!(segment is T cast)) continue;
                return cast;
            }

            return default;
        }
        
        //---[Initialization]-------------------------------------------------------------------------------------------/

        public void Bootup()
        {
            foreach (var segment in segments) segment.Open(this);
            hasBeenBootedUp = true;
        } 
        
        //---[Segments handling]----------------------------------------------------------------------------------------/
        
        public void Add(Segment segment)
        {
            segment.Open(this);
            segments.Add(segment);
        }
        public void Remove(Segment segment)
        {
            if (!segments.Remove(segment)) return;
            segment.Close();
        }
        
        //---[Pin handling]---------------------------------------------------------------------------------------------/
        
        public void AddPin(Id key, Pin pin) => pins.Add(key, pin);
        public void RemovePin(Id key) => pins.Remove(key);

        public void SubscribeTo<T>(Id key, Action<T> method)
        {
            if (!hasBeenBootedUp) Bootup();
            ((Pin<T>)pins[key]).callback += method;
        }
        public void UnsubscribeFrom<T>(Id key, Action<T> method)
        {
            if (!hasBeenBootedUp) Bootup();
            ((Pin<T>)pins[key]).callback -= method;
        }

        //---[Core]-----------------------------------------------------------------------------------------------------/
        
        public void MoveBy(float delta)
        {
            var time = this.time + delta;
            
            if (time > duration)
            {
                onEndReached?.Invoke();
                
                if (isLooping) this.time = time - duration;
                else this.time = duration;
            }
            else if (time < 0.0f)
            {
                onStartReached?.Invoke();
                
                if (isLooping) this.time = duration + time;
                else this.time = 0.0f;
            }
            else this.time = time;
            
            Update();
        }
        public void GoTo(float time)
        {
            this.time = Mathf.Clamp(time, 0.0f, duration);
            Update();
        }

        private void Update()
        {
            if (!hasBeenBootedUp) Bootup();
            foreach (var segment in segments) segment.Update(time / duration);
        }
    }
}