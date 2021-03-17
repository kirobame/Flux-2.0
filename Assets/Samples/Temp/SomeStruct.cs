using System;
using UnityEngine;

[Serializable]
public struct SomeStruct
{
    [SerializeField] private NestedStruct value;
}

[Serializable]
public struct NestedStruct
{
    [SerializeField] private string name;
    [SerializeField] private float value;
}