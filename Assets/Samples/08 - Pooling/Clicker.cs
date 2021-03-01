using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Example08
{
    public class Clicker : MonoBehaviour
    {
        [SerializeField] private AssetReference leftVfxPrefab;
        [SerializeField] private AssetReference rightVfxPrefab;

        private bool hasLoaded;
        private PoolableVfx leftVfxKey;
        private PoolableVfx rightVfxKey;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/

        void Awake()
        {
            var loader = new Loader();
            loader.onDone += OnLoadingDone;
            
            loader.Register(0, leftVfxPrefab.LoadAssetAsync<GameObject>());
            loader.Register(1, rightVfxPrefab.LoadAssetAsync<GameObject>());
        }

        void OnLoadingDone(object[] values)
        {
            leftVfxKey = ((GameObject)values[0]).GetComponent<PoolableVfx>();
            rightVfxKey = ((GameObject)values[1]).GetComponent<PoolableVfx>();

            hasLoaded = true; // Only allow the execution of logic once all references have been loaded
        }
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        void Update()
        {
            if (!hasLoaded) return;
            
            if (Input.GetMouseButtonDown(0)) SpawnVfx(rightVfxKey);
            if (Input.GetMouseButtonDown(1)) SpawnVfx(leftVfxKey);
        }

        private void SpawnVfx(PoolableVfx key)
        {
            // Fetches the pool & return if it is not ready
            // A pool might be not operational because it references its Provider with the Addressables system
            var pool = Repository.Get<VfxPool>(References.VfxPool); 
            if (!pool.IsOperational) return;
            
            // Fetch the camera & convert the mouse position into worldspace
            var camera = Repository.Get<Camera>(References.Camera);
            var point = camera.ScreenToWorldPoint(Input.mousePosition);
            point.z = transform.position.z;
            
            // Request a specific Vfx by giving a prefab key
            // If the pool has already in store disabled instances of this key, it can give it right away
            // If there are no disabled instance, an instantiation is made added to the pool & returned
            // Otherwise the key does not exist for the moment & its added to the pool for later use
            var vfx = pool.RequestSingle(key);
            vfx.transform.position = point;
            vfx.Play(); // Lifetime of the vfx is encapsulated in the PoolableVfx class and does not has to be done here
        }
    }
}