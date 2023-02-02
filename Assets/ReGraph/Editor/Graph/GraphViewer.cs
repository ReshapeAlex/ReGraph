using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

namespace Reshape.ReGraph
{
    public class GraphViewer : GraphView
    {
        public new class UxmlFactory : UxmlFactory<GraphViewer, GraphView.UxmlTraits> { }

        public Action<GraphNodeView> OnNodeSelected;
        public Action<GraphNodeView> OnNodeUnselected;

        SerializedGraph serializer;
        GraphSettings settings;

        public GraphViewer ()
        {
            settings = GraphSettings.GetSettings();

            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new GraphDoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = settings.graphStyle;
            styleSheets.Add(styleSheet);

            viewTransformChanged += OnViewTransformChanged;
        }

        void OnViewTransformChanged (GraphView graphView)
        {
            Vector3 position = contentViewContainer.transform.position;
            Vector3 scale = contentViewContainer.transform.scale;
            serializer?.SetViewTransform(position, scale);
        }

        public GraphNodeView FindNodeView (GraphNode node)
        {
            return GetNodeByGuid(node.guid) as GraphNodeView;
        }

        public void ClearView ()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;
        }

        public void PopulateView (SerializedGraph tree)
        {
            serializer = tree;
            ClearView();
            Debug.Assert(serializer.graph.Created);

            serializer.graph.nodes.ForEach(n => CreateNodeView(n));

            serializer.graph.nodes.ForEach(n =>
            {
                var children = Graph.GetChildren(n);
                children.ForEach(c =>
                {
                    GraphNodeView parentView = FindNodeView(n);
                    GraphNodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });

            contentViewContainer.transform.position = serializer.graph.viewPosition;
            contentViewContainer.transform.scale = serializer.graph.viewScale;
        }

        public override List<Port> GetCompatiblePorts (Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> returnList = new List<Port>();
            List<Port> portList = ports.ToList();
            GraphNodeView startNodeView = (GraphNodeView)startPort.node;
            for (int i = 0; i < portList.Count; i++)
            {
                Port endPort = portList[i];
                if (serializer.graph.Type == Graph.GraphType.BehaviourGraph)
                {
                    GraphNodeView endNodeView = (GraphNodeView)endPort.node;
                    if (startNodeView.node is RootNode && endNodeView.node is TriggerNode == false)
                        continue;
                    if (startNodeView.node is TriggerNode && endNodeView.node is BehaviourNode == false)
                        continue;
                    if (startNodeView.node is BehaviourNode && endNodeView.node is BehaviourNode == false)
                        continue;
                }
                if (endPort.direction != startPort.direction && endPort.node != startPort.node)
                    returnList.Add(endPort);
            }
            return returnList;
        }

        private GraphViewChange OnGraphViewChanged (GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    GraphNodeView nodeView = elem as GraphNodeView;
                    if (nodeView != null)
                    {
                        serializer.DeleteNode(nodeView.node);
                        OnNodeSelected(null);
                    }

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        GraphNodeView parentView = edge.output.node as GraphNodeView;
                        GraphNodeView childView = edge.input.node as GraphNodeView;
                        serializer.RemoveChild(parentView.node, childView.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    GraphNodeView parentView = edge.output.node as GraphNodeView;
                    GraphNodeView childView = edge.input.node as GraphNodeView;
                    serializer.AddChild(parentView.node, childView.node);
                });
            }
            
            nodes.ForEach((n) =>
            {
                GraphNodeView view = n as GraphNodeView;
                List<GraphNode> sorted = view.SortChildren();
                if (sorted != null)
                    serializer.SortChildren(view.node, sorted);
            });

            return graphViewChange;
        }

        public override void BuildContextualMenu (ContextualMenuPopulateEvent evt)
        {
            if (serializer != null && serializer.graph != null)
            {
                Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
                Dictionary<string, System.Type> list = GetContextualList(serializer.graph);
                foreach (var menuItem in list)
                    evt.menu.AppendAction(menuItem.Key, (a) => CreateNode(menuItem.Value, nodePosition));
            }
        }

        /* START - Custom View start here */ 
        public Dictionary<string, System.Type> GetContextualList (Graph graph)
        {
            var list = new Dictionary<string, System.Type>();
            if (graph.Type == Graph.GraphType.BehaviourGraph)
            {
                var types = TypeCache.GetTypesDerivedFrom<TriggerNode>();
                foreach (var type in types)
                    list.Add($"Trigger/{type.Name.Substring(0,type.Name.IndexOf("TriggerNode"))}", type);
                types = TypeCache.GetTypesDerivedFrom<BehaviourNode>();
                foreach (var type in types)
                    list.Add($"Behaviour/{type.Name.Substring(0,type.Name.IndexOf("BehaviourNode"))}", type);
            }
            return list;
        }

        public string GetStyle (GraphNode node)
        {
            if (node is TriggerNode)
            {
                return "trigger";
            }
            else if (node is RootNode)
            {
                return "root";
            }
            else if (node is BehaviourNode)
            {
                return "behaviour";
            }
            return string.Empty;
        }
        /* END - Custom View end here */
        
        public string GetDisableStyle ()
        {
            return "disable";
        }

        public ContextualMenuPopulateEvent GetDeleteAction (ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", (Action<DropdownMenuAction>) (a => this.DeleteSelectionCallback(UnityEditor.Experimental.GraphView.GraphView.AskUser.DontAskUser)),
                (Func<DropdownMenuAction, DropdownMenuAction.Status>) (a => this.canDeleteSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled));
            evt.menu.AppendSeparator();
            return evt;
        }

        void SelectFolder (string path)
        {
            // https://forum.unity.com/threads/selecting-a-folder-in-the-project-via-button-in-editor-window.355357/
            // Check the path has no '/' at the end, if it does remove it,
            // Obviously in this example it doesn't but it might
            // if your getting the path some other way.

            if (path[path.Length - 1] == '/')
                path = path.Substring(0, path.Length - 1);

            // Load object
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

            // Select the object in the project folder
            Selection.activeObject = obj;

            // Also flash the folder yellow to highlight it
            EditorGUIUtility.PingObject(obj);
        }

        void CreateNode (System.Type type, Vector2 position)
        {
            GraphNode node = serializer.CreateNode(type, position);
            CreateNodeView(node);
        }

        void CreateNodeView (GraphNode node)
        {
            GraphNodeView nodeView = new GraphNodeView(serializer, node, this);
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.OnNodeUnselected = OnNodeUnselected;
            AddElement(nodeView);
        }

        public void UpdateNodeStates ()
        {
            nodes.ForEach(n =>
            {
                GraphNodeView view = n as GraphNodeView;
                view.UpdateState();
            });
            if (serializer != null)
                serializer.graph.selectedViewNode = selection;
        }
    }
}