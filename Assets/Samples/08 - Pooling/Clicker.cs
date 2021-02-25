using Flux;
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

            hasLoaded = true;
        }

        void Update()
        {
            if (!hasLoaded) return;
            
            if (Input.GetMouseButtonDown(0)) SpawnVfx(rightVfxKey);
            if (Input.GetMouseButtonDown(1)) SpawnVfx(leftVfxKey);
        }

        private void SpawnVfx(PoolableVfx key)
        {
            var pool = Repository.Get<VfxPool>(References.VfxPool);
            if (!pool.IsOperational) return;
            
            var camera = Repository.Get<Camera>(References.Camera);
            var point = camera.ScreenToWorldPoint(Input.mousePosition);
            point.z = transform.position.z;
            
            var vfx = pool.RequestSingle(key);
            vfx.transform.position = point;
            vfx.Play();
        }
    }
}