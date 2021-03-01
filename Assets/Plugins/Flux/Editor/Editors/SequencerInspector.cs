using System.Collections;
using System.Collections.Generic;
using Flux.Feedbacks;
using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    [CustomEditor(typeof(Sequencer))]
    public class SequencerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Edit"))
            {
                var window = EditorWindow.GetWindow<SequenceGraph>();
                window.titleContent = EditorGUIUtility.TrTextContentWithIcon("Sequence", "SceneViewFx");
                
                window.Show();
            }
        }
    }
}