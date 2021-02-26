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
            StartCoroutine(Routines.DoAfter(() => // Spawn the Player after 0.75seconds & spawn the Printer after that
            {
                var handle = playerPrefab.InstantiateAsync(spawnAnchor.position, spawnAnchor.rotation);
                handle.Completed += completedHandle => printerPrefab.InstantiateAsync();
                
            }, new YieldTime(0.75f))); // A yield instruction encapsulates a yield return statement
        }
    }
}