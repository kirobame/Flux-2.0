using System;
using System.Collections;
using Flux.Event;

public interface IEventHandler
{
    void AddDependency(EventToken token);
    void RemoveDependency(Enum address, object method);
}