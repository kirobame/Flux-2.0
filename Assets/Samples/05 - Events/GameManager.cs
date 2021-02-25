using Flux;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Example05
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AssetReference playerPrefab;
        [SerializeField] private AssetReference printerPrefab;

        [Space, SerializeField] private Transform spawnAnchor;
        
        void Awake()
        {
            StartCoroutine(Routines.DoAfter(() =>
            {
                var handle = playerPrefab.InstantiateAsync(spawnAnchor.position, spawnAnchor.rotation);
                handle.Completed += completedHandle => printerPrefab.InstantiateAsync();
                
            }, new YieldTime(0.75f)));
        }
    }
}