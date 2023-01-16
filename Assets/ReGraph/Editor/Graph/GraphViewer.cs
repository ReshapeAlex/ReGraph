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
        private string[] baseNode = new string[] {"ActionNode", "CompositeNode", "DecoratorNode"};

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
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new GraphDoubleClickSelection());

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
            Debug.Assert(serializer.graph.rootNode != null);

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
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
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
                view.SortChildren();
            });

            return graphViewChange;
        }

        public override void BuildContextualMenu (ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            //evt.menu.AppendSeparator();

            Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);

            /*var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
                evt.menu.AppendAction($"[Action]/{type.Name}", (a) => CreateNode(type, nodePosition));
            types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
                evt.menu.AppendAction($"[Composite]/{type.Name}", (a) => CreateNode(type, nodePosition));
            types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
                evt.menu.AppendAction($"[Decorator]/{type.Name}", (a) => CreateNode(type, nodePosition));*/

            if (serializer != null && serializer.graph != null)
            {
                List<string> allowNodeTypes = new List<string>();
                /*if (serializer.graph.GetType().Equals(typeof(DialogGraph)))
                {
                    allowNodeTypes.Add("DialogMessage");
                    allowNodeTypes.Add("DialogChoice");
                    allowNodeTypes.Add("DialogAction");
                    allowNodeTypes.Add("DebugNode");
                }*/

                var types = TypeCache.GetTypesDerivedFrom<GraphNode>();
                foreach (var type in types)
                {
                    bool add = false;
                    if (allowNodeTypes != null)
                    {
                        if (allowNodeTypes.Count == 0)
                            add = true;
                        else if (allowNodeTypes.Contains(type.Name))
                            add = true;
                    }
                    else
                    {
                        add = true;
                    }

                    if (add && baseNode.Contains(type.Name))
                        add = false;
                    //if (add)
                    //    evt.menu.AppendAction($"{StringExtensions.SplitCamelCase(type.Name)}", (a) => CreateNode(type, nodePosition));
                }
            }
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
            GraphNodeView nodeView = new GraphNodeView(serializer, node);
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
            if (serializer != null )
                serializer.graph.selectedNodeCount = selection.Count;
        }
    }
}