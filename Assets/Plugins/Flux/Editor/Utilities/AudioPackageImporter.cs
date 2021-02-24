using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Flux.Editor
{
    /*[ScriptedImporter(1, "mp3")]
    public class AudioPackageImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (!(ctx.mainObject is AudioClip)) return;
            
            var package = ScriptableObject.CreateInstance<AudioClipPackage>();
            ctx.AddObjectToAsset("Package", package);
        }
    }*/
}