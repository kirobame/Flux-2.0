using System;
using System.Collections;
using UnityEngine;

namespace Flux.Data
{
    public class PoolableAudio : Poolable<AudioSource>
    {
        public event Action onDone;
        
        private Coroutine deactivationRoutine;

        void Update()
        {
            if (!hasBeenBootedUp || Value.loop || deactivationRoutine != null) return;

            if (Value.clip.length - Value.time <= 0.1f)
            {
                onDone?.Invoke();
                deactivationRoutine = StartCoroutine(DeactivationRoutine());
            }
        }

        private IEnumerator DeactivationRoutine()
        {
            yield return new WaitForSeconds(0.25f);
            gameObject.SetActive(false);

            deactivationRoutine = null;
        }
    }
}