using System;
using Flux;
using UnityEngine;

[Address]
public enum Animal : byte
{
    Turtle,
    Boar,
    Frog,
    Deer
}

[Address, Flags]
public enum Country : byte
{
    None = 0,
    France = 1,
    USA = 2,
    Britain = 4,
    India = 8
}

public struct SomeFlag : IFlag
{
    public Enum Value { get; }
}