using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flux
{
    public class Entity : MonoBehaviour
    {
        public IEnumerable<int> Keys => lookups.Keys;
        public IEnumerable<IComponent> Components => components;
        
        [SerializeReference] private List<IComponent> components = new List<IComponent>();
    
        private Dictionary<int, int> lookups;
        private Dictionary<int, Link> links;

        /*void Update()
        {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var someObject = gameObject.GetComponent<Test>();

            var key = typeof(Position).GetHashCode();
            if (!lookups.ContainsKey(key)) return;
            
            var position = (Position)this[key];
            someObject.Modify(ref position);
            SetComponent(position);

            if (Hub.LinkLookups.TryGetValue(key, out var linkLookups))
            {
                foreach (var linkLookup in linkLookups)
                {
                    if (links.TryGetValue(linkLookup, out var link)) link.ReceiveEntityData(position);
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.T)) RemoveComponent<Position>();
        if (Input.GetKeyDown(KeyCode.Y)) AddComponent(new Rotation());

        if (Input.GetKeyDown(KeyCode.R))
        {
            var someObject = gameObject.GetComponent<Test>();

            var key = typeof(Rotation).GetHashCode();
            if (!lookups.ContainsKey(key)) return;
            
            var rotation = (Rotation)this[key];
            someObject.Rotate(ref rotation);
            SetComponent(rotation);

            if (Hub.LinkLookups.TryGetValue(key, out var linkLookups))
            {
                foreach (var linkLookup in linkLookups)
                {
                    if (links.TryGetValue(linkLookup, out var link)) link.ReceiveEntityData(rotation);
                }
            }
        }
    }*/

        public IComponent this[int key] => components[lookups[key]];
     
        public new T GetComponent<T>() where T : IComponent => (T)components[lookups[typeof(T).GetHashCode()]];
        public void SetComponent(IComponent component) => components[lookups[component.GetType().GetHashCode()]] = component;
    
        void Awake()
        {
            lookups = new Dictionary<int, int>();
            links = new Dictionary<int, Link>();

            for (var i = 0; i < components.Count; i++)
            {
                var key = components[i].GetType().GetHashCode();
                if (lookups.ContainsKey(key))
                {
                    components.RemoveAt(i);
                    i--;
                
                    continue;
                }
            
                lookups.Add(key, i);
                ActualizeLinksFor(key);
            }
        }

        void OnEnable() => Hub.Register(this);
        void OnDisable() => Hub.Unregister(this);

        public void Receive<T>(WriteTo<T> method) where T : IComponent
        {
            var key = typeof(T).GetHashCode();
            var component = (T)this[key];

            method(ref component);
            SetComponent(component);
            
            if (Hub.LinkLookups.TryGetValue(key, out var linkLookups))
            {
                foreach (var linkLookup in linkLookups)
                {
                    if (links.TryGetValue(linkLookup, out var link)) link.ReceiveEntityData(component);
                }
            }
        }
        public void Receive<T1,T2>(WW<T1,T2> method) where T1 : IComponent where T2 : IComponent
        {
            var compOne = GetComponent<T1>();
            var compTwo = GetComponent<T2>();
            
            method(ref compOne, ref compTwo);
            
            var components = new IComponent[] {compOne, compTwo};
            foreach (var component in components)
            {
                SetComponent(component);
                if (Hub.LinkLookups.TryGetValue(component.GetTypeKey(), out var linkLookups))
                {
                    foreach (var linkLookup in linkLookups)
                    {
                        if (links.TryGetValue(linkLookup, out var link)) link.ReceiveEntityData(component);
                    }
                }
            }
        }
        
        public void AddComponent(IComponent component)
        {
            var key = component.GetType().GetHashCode();
            if (lookups.ContainsKey(key)) return;
        
            lookups.Add(key, components.Count);
            components.Add(component);
        
            ActualizeLinksFor(key);
        }
        public void RemoveComponent<T>() where T : IComponent
        {
            var key = typeof(T).GetHashCode();
            if (!lookups.ContainsKey(key)) return;

            components.RemoveAt(lookups[key]);
            lookups.Remove(key);
        
            var existingLookups = new HashSet<int>();
            for (var i = 0; i < components.Count; i++)
            {
                key = components[i].GetType().GetHashCode();
                lookups[key] = i;

                if (Hub.TryGetLinkLookupsFor(key, out var output))
                {
                    foreach (var linkLookup in output) existingLookups.Add(linkLookup);
                }
            }

            var linkLookups = links.Keys.ToArray();
            foreach (var linkLookup in linkLookups)
            {
                if (existingLookups.Contains(linkLookup)) continue;
                links.Remove(linkLookup);
            }
        }

        private void ActualizeLinksFor(int key)
        {
            if (Hub.TryGetLinkLookupsFor(key, out var linkLookups))
            {
                foreach (var linkLookup in linkLookups)
                {
                    if (links.ContainsKey(linkLookup) || !Hub.LinkFactories[linkLookup].TryCreateFor(this, out var link)) continue;
                    
                    links.Add(linkLookup, link);
                    link.SendUnityData(this[key]);
                }
            }
        }
    }
}