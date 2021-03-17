using System;
using System.Collections.Generic;
using Flux.Event;
using UnityEngine;

public class EventHandler : MonoBehaviour, IEventHandler
{
    private List<EventToken> tokens = new List<EventToken>();

    public void AddDependency(EventToken token) => tokens.Add(token);
    public void RemoveDependency(Enum address, object method)
    {
        var hashCode = method.GetHashCode();
        for (var i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].Method.GetHashCode() != hashCode) continue;
            
            tokens[i].Dispose();
            tokens.RemoveAt(i);
        }
    }
    
    void OnDestroy() { foreach(var token in tokens) token.Dispose(); }
}