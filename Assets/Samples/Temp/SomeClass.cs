using System;
using UnityEngine;

[Serializable]
public abstract class SomeClass
{
    [SerializeField] private string name;
}

[Serializable]
public abstract class Intermediary : SomeClass { }

[Serializable]
public class Implementation : Intermediary
{
    [SerializeField] private float value;
}

[Serializable]
public class OtherImplementation : Intermediary
{
    [SerializeField] private Vector3 value;
}