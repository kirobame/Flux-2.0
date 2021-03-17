using Flux.Feedbacks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flux.Editor
{
    public class SequenceGraph : EditorWindow
    {
        private SerializedProperty serializedProperty;
        
        private SequenceGraphView graphView;
        private bool hasBeenFramed;
        
        //---[Lifetime handling]----------------------------------------------------------------------------------------/

        void OnEnable()
        {
            graphView = new SequenceGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
        void OnDisable()
        {
            if (serializedProperty != null) graphView.Unload();
            rootVisualElement.Remove(graphView);
        }

        //---[View shift]-----------------------------------------------------------------------------------------------/

        public void Initialize(SerializedProperty serializedProperty)
        {
            if (this.serializedProperty  != null)
            {
                this.serializedProperty  = null;
                graphView.Unload();
            }

            this.serializedProperty = serializedProperty;
                
            graphView.Load(serializedProperty.Copy());
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