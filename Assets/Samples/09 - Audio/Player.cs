using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Example09
{
    public class Player : LoadingWrapper
    {
        [SerializeField] private AssetReference leftSound;
        [SerializeField] private float stereoPan;

        [Space, SerializeField] private AssetReference rightSound;
        [SerializeField] private Id id;

        private IAudioPackage leftAudio;
        private IAudioPackage rightAudio;

        protected override void OnAwake()
        {
            loader.Register(0, leftSound.LoadAssetAsync<AudioPackage>());
            loader.Register(0, rightSound.LoadAssetAsync<AudioPackage>());
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) Audio.Play(leftAudio.Chain(new StereoPan(stereoPan)));
            if (Input.GetMouseButtonDown(1)) Audio.Play(rightAudio, new WrapperArgs<Id>(id));
        }

        protected override void OnLoadingDone(object[] values)
        {
            leftAudio = (AudioPackage)values[0];
            rightAudio = (IAudioPackage)values[1];
        }
    }
}