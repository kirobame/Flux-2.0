using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Hub
{
    
}

public struct Position : IBridge<Transform>
{
    public Vector3 value;
    
    public void ReceiveDataFrom(Transform source)
    {
        value = source.position;
    }

    public void SendDataTo(Transform destination)
    {
        destination.position = value;
    }
}

public class Entity : MonoBehaviour
{
    [SerializeReference] private List<IComponent> components;
    
    private Dictionary<int, int> lookupTable;
    
    private List<int> availableBridgeKeys;
    private Dictionary<int, int> linkLookupTable;
    
    private Dictionary<int, Link> links;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var someObject = gameObject.GetComponent<Test>();

            var key = typeof(Position).GetHashCode();
            var position = (Position)this[key];
            
            someObject.Modify(ref position);
            SetComponent(position);
            
            if (linkLookupTable.TryGetValue(key, out var id)) links[id].ReceiveEntityData(this);
        }
    }

    public IComponent this[int key] => components[lookupTable[key]];
    
    public new T GetComponent<T>() where T : IComponent => (T)components[lookupTable[typeof(T).GetHashCode()]];
    public void SetComponent(IComponent component) => components[lookupTable[component.GetType().GetHashCode()]] = component;
    
    void Awake()
    {
        lookupTable = new Dictionary<int, int>();
        availableBridgeKeys = new List<int>();
        
        for (var i = 0; i < components.Count; i++)
        {
            var component = components[i];
            var key = component.GetType().GetHashCode();

            if (components[i] is IBridge) availableBridgeKeys.Add(key);
            
            if (lookupTable.ContainsKey(key))
            {
                components.RemoveAt(i);
                i--;
                
                continue;
            }

            lookupTable.Add(key, i);
        }

        links = new Dictionary<int, Link>();
        linkLookupTable = new Dictionary<int, int>();
        
        var matches = new List<int>();
        foreach (var component in GetComponents<Component>())
        {
            var type = component.GetType();
            var key = type.GetHashCode();

            if (links.ContainsKey(key)) continue;
            
            var bridgeType = typeof(IBridge<>).MakeGenericType(type);
            for (var i = 0; i < availableBridgeKeys.Count; i++)
            {
                if (!bridgeType.IsInstanceOfType(components[lookupTable[availableBridgeKeys[i]]])) continue;

                matches.Add(availableBridgeKeys[i]);
                linkLookupTable.Add(availableBridgeKeys[i], key);
                
                availableBridgeKeys.RemoveAt(i);
                i--;
            }
            
            if (matches.Any())
            {
                var linkType = typeof(Link<>).MakeGenericType(type);
                var link = (Link) Activator.CreateInstance(linkType, component, matches);
                link.SendUnityData(this);
                
                links.Add(key, link);
                matches.Clear();
            }
        }
    }
}