using Flux;
using UnityEngine;

namespace Example08
{
    public class Flake : MonoBehaviour
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private Vector2 speedRange;

        private float speed;
        
        void OnEnable()
        {
            speed = Random.Range(speedRange.x, speedRange.y);
            
            var size = Random.Range(0.5f, 1.25f);
            transform.localScale = Vector3.one * size;
            
            StartCoroutine(Routines.DoAfter(() => gameObject.SetActive(false), lifeTime));
        }

        void Update() => transform.Translate(Vector3.down * (Time.deltaTime * speed));
    }
}