using UnityEngine;

namespace Flux.EDS
{
    public interface ILink : IData { }

    public interface IReader<in T> : ILink
    {
        void ReceiveDataFrom(T component);
    }

    public interface IWriter<in T> : ILink
    {
        void SendDataTo(T component);
    }

    public interface IBridge<in T> : ILink where T : Component
    {
        void ReceiveDataFrom(T component);
        void SendDataTo(T component);
    }
}