using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Temporary : MonoBehaviour
{
    [DrawWithUnity, Explicit, SerializeReference] private SomeClass someClass = new Implementation();
    [DrawWithUnity, SerializeField] private Container container = new Container();
    [DrawWithUnity, SerializeField] private Safe safe = new Safe();
    
    [DrawWithUnity, Explicit, SerializeReference] private List<SomeClass> classes = new List<SomeClass>()
    {
        new Implementation(), 
        new OtherImplementation(), 
    };
    
    [DrawWithUnity, Explicit, SerializeReference] private SomeClass otherClass;
}