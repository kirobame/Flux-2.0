using System.Collections;
using System.Collections.Generic;
using Flux;
using Flux.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Example01
{
    public class Spawner : MonoBehaviour
    {
        // Asset references are small classes containing the necessary data to automatize the process of fetching
        // data from the Addressable system.
        // assetReference.InstantiateAsync() == Addressables.InstantiateAsync(assetReference.RuntimeKey);
        // assetReference.LoadAssetAsync() == Addressables.LoadAssetsAsync(spriteReference.RuntimeKey);
        [SerializeField] private AssetReference prefabReference;
        [SerializeField] private AssetReference spriteReference;

        [Space, SerializeField] private Transform spawnAnchor;

        // Caching of the explicit underlying values of Asset references are necessary to avoid casting
        private SpriteRenderer spriteRenderer;
        private Sprite happySprite;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/

        void Awake()
        {
            // A Loader is a utility class meant to merge AsyncOperationHandle callbacks
            // With this, you can have only one single callback telling you when all Asset references have finished loading
            var loader = new Loader();
            loader.onDone += OnLoadDone;
            
            // Registering an AsyncOperationHandle to a Loader, tells his to wait for the callback of said handle
            // The passed byte index is used for sorting & correctly retrieving data from the Loader.onDone : Action<object[]> event
            loader.Register(0, prefabReference.InstantiateAsync(spawnAnchor.position, spawnAnchor.rotation));
            loader.Register(1, spriteReference.LoadAssetAsync<Sprite>());
            loader.Register(2, Addressables.LoadAssetAsync<Sprite>("01-Happy"));
        }
        
        // Like Asset references, types cannot be explicit & casting is necessary
        void OnLoadDone(object[] values)
        {
            spriteRenderer = ((GameObject)values[0]).GetComponent<SpriteRenderer>();
            var sprite = (Sprite)values[1];

            spriteRenderer.sprite = sprite;

            happySprite = (Sprite)values[2];
        }
        
        //---[Behaviour]------------------------------------------------------------------------------------------------/

        void Update()
        {
            // Always check if an asset has finished loading. Otherwise it will throw null
            if (Input.GetKeyDown(KeyCode.Space) && spriteReference.IsDone) 
            {
                spriteRenderer.sprite = happySprite; // == spriteRenderer.sprite = (Sprite)spriteReference.Asset;
            }
        }
    }
}