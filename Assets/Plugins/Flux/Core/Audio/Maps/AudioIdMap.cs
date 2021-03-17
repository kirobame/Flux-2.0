using System;
using UnityEngine;

namespace Flux.Audio
{
    /// <summary>Basic implementation of <c>AudioMap</c>.</summary>
    [CreateAssetMenu(fileName = "NewAudioClipPackage", menuName = "Audio Packages/Maps/By id", order = 215)]
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