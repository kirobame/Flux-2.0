using Flux.Feedbacks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flux.Editor
{
    public class SequenceGraph : EditorWindow
    {
        private Sequencer activeSequencer;
        private SerializedObject serializedObject;
        
        private SequenceGraphView graphView;
        private bool hasBeenFramed;
        
        //---[Lifetime handling]----------------------------------------------------------------------------------------/
        
        [MenuItem("Tools/Flux/Sequence")]
        public static void Open()
        {
            var window = GetWindow<SequenceGraph>();
            window.titleContent = EditorGUIUtility.TrTextContentWithIcon("Sequence", "SceneViewFx");
        }
        
        void OnEnable()
        {
            graphView = new SequenceGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            
            OnSelectionChange();
        }
        void OnDisable()
        {
            if (activeSequencer != null) graphView.Unload();
            rootVisualElement.Remove(graphView);
        }

        //---[View shift]-----------------------------------------------------------------------------------------------/
        
        void OnSelectionChange()
        {
            if (Selection.activeGameObject == null || !Selection.activeGameObject.TryGetComponent<Sequencer>(out var sequencer)) return;

            if (activeSequencer != null)
            {
                serializedObject = null;
                activeSequencer = null;
                
                graphView.Unload();
            }
            
            activeSequencer = sequencer;
            serializedObject = new SerializedObject(activeSequencer);
            
            graphView.Load(activeSequencer, serializedObject);
            hasBeenFramed = false;
        }
        
        //---[Utilities]------------------------------------------------------------------------------------------------/

        void OnGUI()
        {
            var rect = new Rect(Vector2.zero, position.size);
            rect.DrawBorders(5.0f, Color.red);
            
            if (hasBeenFramed) return;

            graphView.FrameAll();
            hasBeenFramed = true;
        }
    }
}