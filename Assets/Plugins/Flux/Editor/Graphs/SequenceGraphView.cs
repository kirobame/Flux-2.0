using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flux.Editor
{
    public class SequenceGraphView : GraphView
    {
        private SerializedObject serializedObject;
        private SerializedProperty rootProperty;
        private SerializedProperty arrayProperty;

        private EditorWindow window;
        private NodeSearchWindow searchWindow;
        
        public SequenceGraphView(EditorWindow window)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("SequenceGraph"));
            
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Initialize(window, this);
            nodeCreationRequest += ctxt => SearchWindow.Open(new SearchWindowContext(ctxt.screenMousePosition), searchWindow);
            
            deleteSelection += DeleteSelection;
        }

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
        
        public void Load(Sequencer sequencer, SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
            
            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);
            for (var i = 0; i < 10; i++) serializedProperty.Next(false);
            rootProperty = serializedProperty.Copy();

            CreateRootNode();

            serializedProperty.Next(false);
            arrayProperty = serializedProperty.Copy();
            
            FetchNodes();
        }
        private void CreateRootNode()
        {
            var rootNode = CreateNodeFrom(rootProperty, serializedObject, "Root", false);
            rootNode.index = -1;
            rootNode.capabilities = Capabilities.Ascendable | Capabilities.Collapsible | Capabilities.Selectable;
            AddElement(rootNode);
        }
        private void FetchNodes()
        {
            var cachedNodes = new Node[arrayProperty.arraySize];
            
            for (var i = 0; i < arrayProperty.arraySize; i++)
            {
                var subProperty = arrayProperty.GetArrayElementAtIndex(i);
                var node = CreateNodeFrom(subProperty, serializedObject);
                node.index = i;

                cachedNodes[i] = node;
                AddElement(node);
            }

            nodes.ForEach(node =>
            {
                var sequenceNode = (SequenceNode)node;
                
                var subProperty =  GetPropertyFor(sequenceNode);
                subProperty.Next(true);
                subProperty.Next(false);

                for (var i = 0; i < subProperty.arraySize; i++)
                {
                    var indexProperty = subProperty.GetArrayElementAtIndex(i);
                    var edge = new Edge()
                    {
                        output = sequenceNode.outputContainer.Q<Port>(),
                        input = cachedNodes[indexProperty.intValue].inputContainer.Q<Port>()
                        
                    };
                    
                    edge?.input.Connect(edge);
                    edge?.output.Connect(edge);
                    
                    Add(edge);
                }
            });
        }
        
        public void Unload()
        {
            nodes.ForEach(node =>
            {
                var sequenceNode = (SequenceNode)node;
                var subProperty = GetPropertyFor(sequenceNode);
                
                subProperty.Next(true);
                subProperty.rectValue = sequenceNode.GetPosition();
                
                subProperty.Next(false);
                subProperty.arraySize = 0;
            });

            edges.ForEach(edge =>
            {
                var inputNode = (SequenceNode)edge.input.node;
                var outputNode = (SequenceNode)edge.output.node;
                
                var outputProperty = GetPropertyFor(outputNode);;
                outputProperty.Next(true);
                outputProperty.Next(false);

                var indexProperty = outputProperty.NewElementAtEnd();
                indexProperty.intValue = inputNode.index;
            });

            serializedObject.ApplyModifiedProperties();
        }

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
            AddElement(node);
        }
        private SequenceNode CreateNodeFrom(SerializedProperty property, SerializedObject serializedObject, string overrideName = "", bool hasInput = true)
        {
            var copy = property.Copy();
            copy.Next(true);

            var name = overrideName;
            if (overrideName == string.Empty)
            {
                name = property.managedReferenceFullTypename.Split(' ').Last();
                if (name.Contains('.')) name = name.Split('.').Last();
            }
            var node = new SequenceNode() { title = name };

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
            
            var rect = copy.rectValue;
            node.SetPosition(rect);
            
            return node;
        }

        void DeleteSelection(string operationName, AskUser askUser)
        {
            Debug.Log(operationName);
        }
        
        /*private readonly Vector2 defaultSize = new Vector2(200, 150);
        
        public SequenceGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("SequenceGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            
            AddElement(GenerateEntryPoint());
        }

        public SequenceNode CreateSequenceNode(string nodeName)
        {
            var sequenceNode = new SequenceNode()
            {
                title = "Part",
                Guid = GUID.Generate().ToString(),
                content = "Wow more stuff"
            };

            var inputPort = GeneratePort(sequenceNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            sequenceNode.inputContainer.Add(inputPort);
            
            var button = new Button(() =>
            {
                AddChoicePort(sequenceNode);
            });
            button.text = "New choice";
            sequenceNode.titleContainer.Add(button);
            
            sequenceNode.RefreshExpandedState();
            sequenceNode.RefreshPorts();
            
            sequenceNode.SetPosition(new Rect(Vector2.zero, defaultSize));
            AddElement(sequenceNode);
            
            return sequenceNode;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node) compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private Port GeneratePort(SequenceNode node, Direction direction, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
        }
        
        private SequenceNode GenerateEntryPoint()
        {
            var node = new SequenceNode()
            {
                title = "Start",
                Guid = GUID.Generate().ToString(),
                content = "Hello world!",
                entryPoint = true
            };

            var port = GeneratePort(node, Direction.Output);
            port.portName = "Next";
            node.outputContainer.Add(port);
            
            node.RefreshExpandedState();
            node.RefreshPorts();
            
            node.SetPosition(new Rect(100, 200, 100, 150));
            return node;
        }

        private void AddChoicePort(SequenceNode node)
        {
            var port = GeneratePort(node, Direction.Output);
            var outputCount = node.outputContainer.Query("connector").ToList().Count;
            port.name = $"Choice - {outputCount}";
            
            node.outputContainer.Add(port);
                        
            node.RefreshExpandedState();
            node.RefreshPorts();
        }*/
    }
}