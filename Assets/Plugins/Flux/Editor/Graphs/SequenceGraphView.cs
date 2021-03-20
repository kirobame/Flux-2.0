using System;
using System.Collections.Generic;
using System.Linq;
using Flux.Feedbacks;
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
            styleSheets.Add(Resources.Load<StyleSheet>("SequenceGraph"));
            
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var grid = new GridBackground();
            VisualElement borders = new IMGUIContainer(() =>
            {
                var rect = new Rect(Vector2.zero, window.position.size);
                var thickness = 3.0f;
                var color = "#383838".ToColor();
                
                EditorGUI.DrawRect(new Rect(Vector2.zero, new Vector2(rect.width, thickness)), color);
                EditorGUI.DrawRect(new Rect(Vector2.zero, new Vector2(thickness, rect.height)), color);
                EditorGUI.DrawRect(new Rect(new Vector2(rect.xMax - thickness, rect.yMin), new Vector2(thickness, rect.height)), color);
                EditorGUI.DrawRect(new Rect(new Vector2(rect.xMin, rect.yMax - thickness), new Vector2(rect.width, thickness)), color);
            });
            grid.Add(borders);
            
            Insert(0, grid);
            grid.StretchToParentSize();

            cachedNodes = new List<SequenceNode>();
            cachedEdges = new List<Edge>();

            serializeGraphElements += SerializeNode;
            canPasteSerializedData += CanPaste;
            unserializeAndPaste += UnserializeAndPaste;
            
            searchWindow = ScriptableObject.CreateInstance<TypeSearchWindow>();
            searchWindow.Initialize("Create node", typeof(Effect), window);
            searchWindow.onSelectEntry += (val, pos) => AddNode((Type) val, contentViewContainer.WorldToLocal(pos));
            nodeCreationRequest += ctxt => SearchWindow.Open(new SearchWindowContext(ctxt.screenMousePosition), searchWindow);
            
            deleteSelection += DeleteSelection;
            graphViewChanged += OnChange;
        }
        
        //---[Data]-----------------------------------------------------------------------------------------------------/

        private SerializedProperty serializedProperty;
        private SerializedProperty rootProperty;
        private SerializedProperty arrayProperty;
        
        private TypeSearchWindow searchWindow;

        private List<SequenceNode> cachedNodes;
        private List<Edge> cachedEdges;

        //---[Loading]--------------------------------------------------------------------------------------------------/
        
        public void Load(SerializedProperty serializedProperty)
        {
            this.serializedProperty = serializedProperty.Copy();

            serializedProperty.Next(true);
            rootProperty = serializedProperty.Copy();

            CreateRootNode();

            serializedProperty.Next(false);
            arrayProperty = serializedProperty.Copy();
            
            FetchNodes();
            FrameAll();
        }
        private void CreateRootNode()
        {
            var rootNode = CreateNodeFrom(rootProperty, "Root", false);
            rootNode.index = -1;
            rootNode.capabilities = Capabilities.Ascendable | Capabilities.Collapsible | Capabilities.Selectable;
            
            AddNode(rootNode);
        }
        private void FetchNodes()
        {
            for (var i = 0; i < arrayProperty.arraySize; i++)
            {
                var subProperty = arrayProperty.GetArrayElementAtIndex(i);
                var node = CreateNodeFrom(subProperty);
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

            foreach (var edge in cachedEdges) RemoveElement(edge);
            cachedEdges.Clear();
            
            foreach (var node in cachedNodes) RemoveElement(node);
            cachedNodes.Clear();

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        
        //---[Node creation]--------------------------------------------------------------------------------------------/

        public void AddNode(Type nodeType, Vector2 position)
        {
            if (rootProperty == null) return;

            var subProperty = arrayProperty.NewElementAtEnd();
            subProperty.managedReferenceValue = (Effect)Activator.CreateInstance(nodeType);

            var copy = subProperty.Copy();
            copy.Next(true);
            
            copy.rectValue = new Rect(position, new Vector2(250, 150));
            serializedProperty.serializedObject.ApplyModifiedProperties();

            var node = CreateNodeFrom(subProperty, nodeType.Name);
            node.index = arrayProperty.arraySize - 1;
            
            AddNode(node);
        }
        private void CopyNode(int sourceIndex)
        {
            var sourceProperty = arrayProperty.GetArrayElementAtIndex(sourceIndex);
            var subProperty = arrayProperty.NewElementAtEnd();

            var nodeType = sourceProperty.GetManagedType();
            subProperty.managedReferenceValue = (Effect)Activator.CreateInstance(nodeType);
            sourceProperty.CopyTo(subProperty);
            
            var copy = subProperty.Copy();
            copy.Next(true);
            
            sourceProperty.Next(true);
            var rect = sourceProperty.rectValue;
            copy.rectValue = new Rect(rect.position + new Vector2(50, 50), rect.size);

            serializedProperty.serializedObject.ApplyModifiedProperties();
            
            var node = CreateNodeFrom(subProperty, nodeType.Name);
            node.index = arrayProperty.arraySize - 1;
            
            AddNode(node);
        }
        private SequenceNode CreateNodeFrom(SerializedProperty property, string overrideName = "", bool hasInput = true)
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
                propertyField.Bind(property.serializedObject);
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
            serializedProperty.serializedObject.ApplyModifiedProperties();

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

        string SerializeNode(IEnumerable<GraphElement> elements)
        {
            var data = string.Empty;
            foreach (var element in elements)
            {
                if (!(element is SequenceNode node)) continue;
                data += $"{node.index}/";
            }

            if (data != string.Empty) data = data.Substring(0, data.Length - 1);
            return data;
        }
        bool CanPaste(string data) => data != string.Empty;
        void UnserializeAndPaste(string operation, string data)
        {
            if (operation != "Paste" || rootProperty == null) return;

            foreach (var subData in data.Split('/'))
            {
                var index = int.Parse(subData);
                CopyNode(index);
            }
        }
        
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