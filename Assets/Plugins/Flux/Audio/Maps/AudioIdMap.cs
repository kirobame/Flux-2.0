using System;
using UnityEngine;

namespace Flux
{
    [CreateAssetMenu(fileName = "NewAudioClipPackage", menuName = "Audio/Maps/By id")]
    public class AudioIdMap : AudioMap<Id>
    {
        #region Nested Types

        [Serializable]
        private class IdAudioPackagePair : KeyValuePair<Id,AudioPackage> { }

        #endregion

        protected override KeyValuePair<Id, AudioPackage>[] keyValuePairs => pairs;
        [SerializeField] private IdAudioPackagePair[] pairs;
    }
}