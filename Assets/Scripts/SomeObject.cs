using System;
using System.Diagnostics;
using System.Linq;
using Flux;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Timeline;

public class SomeObject : MonoBehaviour
{
    [SerializeField] private Timetable timetable;
    [SerializeField] private Id id;
    [SerializeField] private bool state;

    void Awake()
    {
        timetable.Initialize();
        timetable.SubscribeTo<float>(new Id("FLT"), value =>
        {

        });
    }

    void Update()
    {
        var time = (Mathf.Sin(Time.time) + 1.0f) / 2.0f * timetable.Duration;
        timetable.GoTo(time);
    }
}