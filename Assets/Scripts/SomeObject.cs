using System;
using System.Diagnostics;
using System.Linq;
using Flux;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SomeObject : MonoBehaviour
{
    [SerializeField] private Sequencer sequencer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) sequencer.Play(EventArgs.Empty);
    }
}