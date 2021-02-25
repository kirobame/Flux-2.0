using System.Collections;
using Flux;
using UnityEngine;

namespace Example08
{
    public class Background : MonoBehaviour
    {
        [SerializeField, Range(0,100)] private int count;
        [SerializeField] private Vector2Int spawnRange;
        [SerializeField] private Vector2 delayRange;

        private Coroutine routine;
        private Rect rect;

        void Start()
        {
            var camera = Repository.Get<Camera>(References.Camera);
            var min = camera.ViewportToWorldPoint(Vector2.zero);
            var max = camera.ViewportToWorldPoint(Vector2.one);
            var size = max - min;
            
            rect = new Rect(min, size);
        }

        void Update()
        {
            if (routine == null && Input.GetKeyDown(KeyCode.Space))
            {
                var pool = Repository.Get<GenericPool>(References.FlakePool);
                if (!pool.IsOperational) return;
                
                routine = StartCoroutine(Routine(pool));
            }
        }

        private IEnumerator Routine(GenericPool pool)
        {
            var remainder = count;
            var state = true;
 
            while (state)
            {
                var batch = Random.Range(spawnRange.x, spawnRange.y);
                remainder -= batch;

                if (remainder < 0)
                {
                    remainder = 0;
                    batch -= remainder;

                    state = false;
                }

                for (var i = 0; i < batch; i++)
                {
                    var flake = pool.CastSingle<Flake>();
                    flake.transform.position = new Vector2(Random.Range(rect.xMin, rect.xMax), rect.yMax);
                }
                
                yield return new WaitForSeconds(Random.Range(delayRange.x, delayRange.y));
            }

            routine = null;
        }
    }
}