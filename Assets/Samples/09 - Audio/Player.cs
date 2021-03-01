using System.Collections;
using System.Collections.Generic;
using Flux;
using Flux.Audio;
using Flux.Event;
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
        
        //---[Initialization]-------------------------------------------------------------------------------------------/

        protected override void OnAwake()
        {
            loader.Register(0, leftSound.LoadAssetAsync<AudioPackage>());
            loader.Register(0, rightSound.LoadAssetAsync<AudioPackage>());
        }
        
        protected override void OnLoadingDone(object[] values)
        {
            leftAudio = (AudioPackage)values[0];
            rightAudio = (IAudioPackage)values[1];
        }
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        void Update() // The LoadingWrapper ensures that Update is only called once all references have been loaded
        {
            // Usage if the IAudioPackage allows the combination of audio effects
            // E.g : .Chain(...) will give leftAudio the defined Player.stereoPan to the AudioSource responsible for playing the IAudioPackage
            if (Input.GetMouseButtonDown(0)) AudioHandler.Play(leftAudio.Chain(new StereoPan(stereoPan)));
            
            // Usage of EventArgs allow to put some selective logic in IAudioPackage
            // In this case rightAudio is a AudioMapById which means the audi played corresponds the given Id
            if (Input.GetMouseButtonDown(1)) AudioHandler.Play(rightAudio, new WrapperArgs<Id>(id));
        }
    }
}