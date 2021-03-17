using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Container
{
    [DrawWithUnity, Explicit, SerializeReference] private SomeClass collectionClass = new Implementation();
}

[Serializable]
public class Safe
{
    [DrawWithUnity, Explicit, SerializeReference] private Intermediary deepClass = new Implementation();
}