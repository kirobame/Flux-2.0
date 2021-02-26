using Flux;
using UnityEngine;

namespace Example08
{
    public class Flake : MonoBehaviour
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private Vector2 speedRange;

        private float speed;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/
        
        void OnEnable()
        {
            // Random state assignement
            speed = Random.Range(speedRange.x, speedRange.y);
            var size = Random.Range(0.5f, 1.25f);
            transform.localScale = Vector3.one * size;
            
            // A poolable will automatically return to its corresponding pool once its disabled
            // It is preferable to deactivate its GameObject entirely to avoid any issue
            StartCoroutine(Routines.DoAfter(() => gameObject.SetActive(false), lifeTime));
        }
        
        //---[Behaviour]------------------------------------------------------------------------------------------------/

        // Go down
        void Update() => transform.Translate(Vector3.down * (Time.deltaTime * speed));
    }
}