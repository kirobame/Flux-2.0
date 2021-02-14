using Flux;
using UnityEngine;

public struct SomeFlag : IFlag
{
    private enum Tag : byte
    {
        One = 1,
        Two = 2,
        Three = 3
    }

    public SomeFlag(byte value) => tag = (Tag)value;

    [SerializeField] private Tag tag;

    public int GetCode() => (int)tag;
}