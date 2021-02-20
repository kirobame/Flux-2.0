using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flux.Editor
{
    public class SequenceGraphView : GraphView
    {
        public SequenceGraphView(EditorWindow window)
        {
            this.window = window;
            styleSheets.Add(Resources.Load<StyleSheet>("SequenceGraph"));
            
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            cachedNodes = new List<SequenceNode>();
            cachedEdges = new List<Edge>();
            
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Initialize(window, this);
            nodeCreationRequest += ctxt => SearchWindow.Open(new SearchWindowContext(ctxt.screenMousePosition), searchWindow);
            
            deleteSelection += DeleteSelection;
            graphViewChanged += OnChange;
        }
        
        //---[Data]-----------------------------------------------------------------------------------------------------/
        
        private int id;
        private SerializedObject serializedObject;
        private SerializedProperty rootProperty;
        private SerializedProperty arrayProperty;

        private EditorWindow window;
        private NodeSearchWindow searchWindow;

        private List<SequenceNode> cachedNodes;
        private List<Edge> cachedEdges;

        //---[Loading]--------------------------------------------------------------------------------------------------/
        
        public void Load(Sequencer sequencer, SerializedObject serializedObject)
        {
            id = sequencer.GetInstanceID();
            this.serializedObject = serializedObject;
            
            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);
            for (var i = 0; i < 10; i++) serializedProperty.Next(false);
            rootProperty = serializedProperty.Copy();

            CreateRootNode();

            serializedProperty.Next(false);
            arrayProperty = serializedProperty.Copy();
            
            FetchNodes();
            FrameAll();
        }
        private void CreateRootNode()
        {
            var rootNode = CreateNodeFrom(rootProperty, serializedObject, "Root", false);
            rootNode.index = -1;
            rootNode.capabilities = Capabilities.Ascendable | Capabilities.Collapsible | Capabilities.Selectable;
            
            AddNode(rootNode);
        }
        private void FetchNodes()
        {
            for (var i = 0; i < arrayProperty.arraySize; i++)
            {
                var subProperty = arrayProperty.GetArrayElementAtIndex(i);
                var node = CreateNodeFrom(subProperty, serializedObject);
                node.index = i;

                AddNode(node);
            }

            foreach (var node in cachedNodes)
            {
                var subProperty = GetPropertyFor(node);
                subProperty.Next(true);
                subProperty.Next(false);
                
                for (var i = 0; i < subProperty.arraySize; i++)
                {
                    var indexProperty = subProperty.GetArrayElementAtIndex(i);
                    var edge = new Edge()
                    {
                        output = node.outputContainer.Q<Port>(),
                        input = cachedNodes[indexProperty.intValue + 1].inputContainer.Q<Port>()

                    };
                    
                    edge?.input.Connect(edge);
                    edge?.output.Connect(edge);

                    cachedEdges.Add(edge);
                    Add(edge);
                }
            }
        }
        
        //---[Unloading]------------------------------------------------------------------------------------------------/
        
        public void Unload()
        {
            if (id == 0) return;
            if (serializedObject == null) serializedObject = new SerializedObject((Sequencer)EditorUtility.InstanceIDToObject(id));
            
            foreach (var node in cachedNodes)
            {
                var subProperty = GetPropertyFor(node);
                
                subProperty.Next(true);
                subProperty.rectValue = node.GetPosition();
                
                subProperty.Next(false);
                subProperty.arraySize = 0;
            }

            foreach (var edge in cachedEdges)
            {
                var inputNode = (SequenceNode)edge.input.node;
                var outputNode = (SequenceNode)edge.output.node;
                
                var outputProperty = GetPropertyFor(outputNode);;
                outputProperty.Next(true);
                outputProperty.Next(false);

                var indexProperty = outputProperty.NewElementAtEnd();
                indexProperty.intValue = inputNode.index;
            }

            serializedObject.ApplyModifiedProperties();

            foreach (var edge in cachedEdges) RemoveElement(edge);
            cachedEdges.Clear();
            
            foreach (var node in cachedNodes) RemoveElement(node);
            cachedNodes.Clear();
        }
        
        //---[Node creation]--------------------------------------------------------------------------------------------/

        public void AddNode(Type nodeType, Vector2 position)
        {
            if (serializedObject == null) return;

            var subProperty = arrayProperty.NewElementAtEnd();
            subProperty.managedReferenceValue = (Effect)Activator.CreateInstance(nodeType);

            var copy = subProperty.Copy();
            copy.Next(true);
            
            copy.rectValue = new Rect(position, new Vector2(250, 150));
            serializedObject.ApplyModifiedProperties();

            var node = CreateNodeFrom(subProperty, serializedObject, nodeType.Name);
            node.index = arrayProperty.arraySize - 1;
            
            AddNode(node);
        }
        private SequenceNode CreateNodeFrom(SerializedProperty property, SerializedObject serializedObject, string overrideName = "", bool hasInput = true)
        {
            var copy = property.Copy();
            copy.Next(true);
            var rect = copy.rectValue;

            var name = overrideName;
            if (overrideName == string.Empty)
            {
                name = property.managedReferenceFullTypename.Split(' ').Last();
                if (name.Contains('.')) name = name.Split('.').Last();
            }
            
            var node = new SequenceNode() { title = name };
            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            if (hasInput)
            {
                var inPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
                inPort.portName = "In";
                node.inputContainer.Add(inPort);
            }
            
            var outPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            outPort.portName = "Out";
            node.outputContainer.Add(outPort);
            
            node.RefreshExpandedState();
            node.RefreshPorts();
            
            var box = new Box();
            var shouldAddPropertyBox = false;
            box.styleSheets.Add(Resources.Load<StyleSheet>("Box"));

            var propertyName = property.GetName();
            var propertyStyle = Resources.Load<StyleSheet>("Label");
            
            while (copy.NextVisible(false))
            {
                var parentName = copy.GetParentName();
                if (propertyName != parentName) break;
                
                var propertyField = new PropertyField(copy);
                propertyField.styleSheets.Add(propertyStyle);
                propertyField.Bind(serializedObject);
                box.Add(propertyField);
                shouldAddPropertyBox = true;
            }
            
            if (shouldAddPropertyBox) node.mainContainer.Add(box);
            node.SetPosition(rect);
            
            return node;
        }
        
        //---[Utilities]------------------------------------------------------------------------------------------------/

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port => { if (startPort != port && startPort.node != port.node) compatiblePorts.Add(port); });

            return compatiblePorts;
        }
        
        private SerializedProperty GetPropertyFor(SequenceNode node)
        {
            if (node.index == -1) return rootProperty.Copy();
            else return arrayProperty.GetArrayElementAtIndex(node.index);
        }
        
        private void AddNode(SequenceNode node)
        {
            cachedNodes.Add(node);
            AddElement(node);
        }
        private void RemoveNode(SequenceNode node)
        {
            arrayProperty.DeleteArrayElementAtIndex(node.index);
            serializedObject.ApplyModifiedProperties();

            cachedNodes.Remove(node);
            RemoveElement(node);
        }

        private void RemoveEdge(Edge edge)
        {
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            
            cachedEdges.Remove(edge);
            RemoveElement(edge);
        }
        
        //---[Callbacks]------------------------------------------------------------------------------------------------/
        
        GraphViewChange OnChange(GraphViewChange change)
        {
            if (change.edgesToCreate != null) foreach (var edge in change.edgesToCreate) cachedEdges.Add(edge);
            return change;
        }
        void DeleteSelection(string operationName, AskUser askUser)
        {
            for (var i = 0; i < selection.Count; i++)
            {
                if (selection[i] is Edge removedEdge) RemoveEdge(removedEdge);
                else if (selection[i] is SequenceNode sequenceNode)
                {
                    var matchingEdges = cachedEdges.Where(edge => edge.input.node == sequenceNode || edge.output.node == sequenceNode).ToArray();
                    foreach (var edge in matchingEdges) RemoveEdge(edge);

                    var index = sequenceNode.index;
                    RemoveNode(sequenceNode);

                    foreach (var node in cachedNodes) if (node.index > index) node.index--;
                }
            }
        }
    }
}