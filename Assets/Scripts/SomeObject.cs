using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SomeObject : MonoBehaviour
{
    private Type[] types;

    void Awake()
    {
        types = new Type[1] {typeof(int)};
        Debug.Log(typeof(int) == types[0]);
    }
}