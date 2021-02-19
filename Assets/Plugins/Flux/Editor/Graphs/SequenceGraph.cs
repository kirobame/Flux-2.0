using System;
using System.Collections;
using System.Collections.Generic;
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
        
        [MenuItem("Tools/Flux/Sequence")]
        public static void Open()
        {
            var window = GetWindow<SequenceGraph>();
            window.titleContent = new GUIContent("Sequence");
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
            graphView.Unload();
            rootVisualElement.Remove(graphView);
        }

        void OnSelectionChange()
        {
            if (Selection.activeGameObject == null || !Selection.activeGameObject.TryGetComponent<Sequencer>(out var sequencer))
            {
                UnloadIfNecessary();
                return;
            }

            UnloadIfNecessary();
            
            activeSequencer = sequencer;
            serializedObject = new SerializedObject(activeSequencer);
            
            graphView.Load(activeSequencer, serializedObject);
        }
        private void UnloadIfNecessary()
        {
            if (activeSequencer == null) return;
            
            serializedObject = null;
            activeSequencer = null;
                
            graphView.Unload();
        }

        /*private SequenceGraphView graphView;
        private string fileName = "New sequence";

        private Mono mono;
        
        [MenuItem("Tools/Flux/Sequence")]
        public static void Open()
        {
            var window = GetWindow<SequenceGraph>();
            window.titleContent = new GUIContent("Sequence");
        }

        void OnEnable()
        {
            mono = FindObjectOfType<Mono>();
            
            //ConstructGraph();
            GenerateToolbar();
        }
        void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        private void ConstructGraph()
        {
            graphView = new SequenceGraphView()
            {
                name = "Sequence graph"
            };

            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            Debug.Log(mono);
            var serializedObject = new SerializedObject(mono);
            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.NextVisible(true);
            
            serializedProperty.NextVisible(false);
            var propField = new PropertyField(serializedProperty);
            propField.Bind(serializedObject);
            toolbar.Add(propField);
            
            serializedProperty.NextVisible(false);
            if (serializedProperty.objectReferenceValue != null)
            {
                var subObject = new SerializedObject(serializedProperty.objectReferenceValue);
                var subProp = subObject.GetIterator();
                
                subProp.NextVisible(true);
                subProp.NextVisible(false);
                
                for (var i = 0; i < subProp.arraySize; i++)
                {
                    var subProperty = subProp.GetArrayElementAtIndex(i);
                    var subField = new PropertyField(subProperty, $"Item - 0{i + 1}");
                    subField.Bind(subObject);
                    rootVisualElement.Add(subField);
                }
            }
            else
            {
                serializedProperty.NextVisible(true);
                for (var i = 0; i < serializedProperty.arraySize; i++)
                {
                    var subProperty = serializedProperty.GetArrayElementAtIndex(i);
                    var subField = new PropertyField(subProperty, $"Item - 0{i + 1}");
                    subField.Bind(serializedObject);
                    rootVisualElement.Add(subField);
                }
            }
            
            /*serializedProperty.NextVisible(false);
            for (var i = 0; i < serializedProperty.arraySize; i++)
            {
                var subProperty = serializedProperty.GetArrayElementAtIndex(i);
                var subField = new PropertyField(subProperty, $"Item - 0{i + 1}");
                subField.Bind(serializedObject);
                rootVisualElement.Add(subField);
            }
            
            rootVisualElement.Add(toolbar);
            
            /*var fileNameField = new TextField("Name :");
            fileNameField.SetValueWithoutNotify(fileName);
            fileNameField.MarkDirtyRepaint();
            fileNameField.RegisterValueChangedCallback(evt =>
            {
                fileName = evt.newValue;
            });
            toolbar.Add(fileNameField);
            
            toolbar.Add(new Button(SaveData) { text = "Save Data"});
            toolbar.Add(new Button(LoadData) { text = "Load Data"});
            
            var nodeCreate = new Button(() =>
            {
                graphView.CreateSequenceNode("New node");
            });
            nodeCreate.text = "Create Node";

            toolbar.Add(nodeCreate);
            rootVisualElement.Add(toolbar);
        }

        private void SaveData()
        {
            
        }
        private void LoadData()
        {
            
        }*/
    }
}