using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Flux
{
    public static class Routines
    {
        private static Hook hook;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootup()
        {
            var hookObject = new GameObject("RoutineHook");
            Object.DontDestroyOnLoad(hookObject);
            hook = hookObject.AddComponent<Hook>();
        }
        
        public static void Clear() => hook.StopAllCoroutines();

        public static Coroutine Start(IEnumerator routine) => hook.StartCoroutine(routine);
        public static bool Stop(Coroutine routine)
        {
            hook.StopCoroutine(routine);
            return true;
        }

        public static IEnumerator Chain(this IEnumerator source, IEnumerator routine)
        {
            yield return source;
            yield return routine;
        }
        
        public static IEnumerator DoAfter(Action method, float time) => DoAfter(method, new YieldTime(time));
        public static IEnumerator DoAfter(Action method, IYieldInstruction yieldInstruction)
        {
            yield return yieldInstruction.Wait();
            method();
        }

        public static IEnumerator Do(Action method)
        {
            method();
            yield break;
        }
        
        public static IEnumerator RepeatFor(float time, Action<float> method) => RepeatFor(time, method, new YieldFrame());
        public static IEnumerator RepeatFor(float time, Action<float> method, IYieldInstruction yieldInstruction)
        {
            var current = 0.0f;
            while (current < time)
            {
                yield return yieldInstruction.Wait();
                current += yieldInstruction.Increment();

                method(Mathf.Clamp01(current / time));
            }
            method(1.0f);
        }
    }
}