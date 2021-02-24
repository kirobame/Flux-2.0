using System.Linq;
using UnityEditor;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Flux.Editor
{
    public static class MenuItems
    {
        private static int collectionCount;

        [MenuItem("Assets/Package", true)]
        private static bool ValidatePackaging(MenuCommand menuCommand) => Selection.objects.Any(item => item is AudioClip);

        [MenuItem("Assets/Package")]
        private static void Package(MenuCommand menuCommand)
        {
            var clips = Selection.objects.Where(item => item is AudioClip).Cast<AudioClip>();

            var folderParent = AssetDatabase.GetAssetPath(clips.First());
            folderParent = folderParent.Remove(folderParent.LastIndexOf('/'));

            var folder = folderParent + "/Source";
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder(folderParent,"Source");

            foreach (var clip in clips)
            {
                var package = ScriptableObject.CreateInstance<AudioClipPackage>();
                ((IInjectable<AudioClip>)package).Inject(clip);
                
                var clipPath = AssetDatabase.GetAssetPath(clip);
                var path = clipPath.Remove(clipPath.LastIndexOf('/') + 1);
                path += $"Packaged-{clip.name}.asset";
                
                AssetDatabase.CreateAsset(package, path);
                AssetDatabase.MoveAsset(clipPath, folder + "/" + clipPath.Split('/').Last());
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("CONTEXT/Component/Reference")]
        private static void Reference(MenuCommand menuCommand)
        {
            var component = (Component)menuCommand.context;
            CreateSingleReferenceOn(component.gameObject, component);
        }
        
        [MenuItem("GameObject/Reference", false, -10)]
        private static void ReferenceWhole(MenuCommand menuCommand)
        {
            if (Selection.gameObjects.Length == 0) return;
            else if (Selection.gameObjects.Length == 1)
            {
                var gameObject = Selection.gameObjects[0];
                CreateSingleReferenceOn(gameObject, gameObject);
            }
            else
            {
                if (collectionCount == 0) collectionCount = Selection.gameObjects.Length - 1;
                else
                {
                    collectionCount--;
                    return;
                }
                
                var recipient = new GameObject("New Referenced Collection");
                Undo.RegisterCreatedObjectUndo(recipient, "Create reference obj.");
                
                var collectionReference = recipient.AddComponent<CollectionReference>();
                Undo.RegisterCreatedObjectUndo(collectionReference, "Add collection reference");
                var serializedObject = new SerializedObject(collectionReference);
                var serializedProperty = serializedObject.GetIterator();
                
                serializedProperty.NextVisible(true);
                serializedProperty.NextVisible(false);
                serializedProperty.NextVisible(false);

                Undo.RecordObject(collectionReference, "Set collection reference");
                foreach (var gameObject in Selection.gameObjects)
                {
                    var elementProperty = serializedProperty.NewElementAtEnd();
                    elementProperty.objectReferenceValue = gameObject;
                }
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void CreateSingleReferenceOn(GameObject gameObject, Object value)
        {
            var singleReference = gameObject.AddComponent<SingleReference>();
            Undo.RegisterCreatedObjectUndo(singleReference, "Add single reference");
            var serializedObject = new SerializedObject(singleReference);
            var serializedProperty = serializedObject.GetIterator();

            serializedProperty.NextVisible(true);
            serializedProperty.NextVisible(false);
            serializedProperty.NextVisible(false);

            Undo.RecordObject(singleReference, "Set single reference");
            serializedProperty.objectReferenceValue = value;
            serializedObject.ApplyModifiedProperties();
        }
    }
}