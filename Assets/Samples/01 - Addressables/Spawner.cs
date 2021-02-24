using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Example01
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private AssetReference prefabReference;
        [SerializeField] private AssetReference spriteReference;

        [Space, SerializeField] private Transform spawnAnchor;

        private SpriteRenderer spriteRenderer;
        private Sprite happySprite;
        
        void Awake()
        {
            var loader = new Loader();
            loader.onDone += OnLoadDone;
            
            loader.Register(0, prefabReference.InstantiateAsync(spawnAnchor.position, spawnAnchor.rotation));
            loader.Register(1, spriteReference.LoadAssetAsync<Sprite>());
            loader.Register(2, Addressables.LoadAssetAsync<Sprite>("01-Happy"));
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && happySprite != null) spriteRenderer.sprite = happySprite;
        }

        void OnLoadDone(object[] values)
        {
            spriteRenderer = ((GameObject)values[0]).GetComponent<SpriteRenderer>();
            var sprite = (Sprite)values[1];

            spriteRenderer.sprite = sprite;

            happySprite = (Sprite)values[2];
        }
    }
}