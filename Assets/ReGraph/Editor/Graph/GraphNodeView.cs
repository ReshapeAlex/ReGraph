using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace Reshape.ReGraph
{
    public class GraphNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<GraphNodeView> OnNodeSelected;
        public Action<GraphNodeView> OnNodeUnselected;
        public SerializedGraph serializer;
        public GraphNode node;
        public Port input;
        public Port output;

        public GraphNodeView (SerializedGraph tree, GraphNode node) : base(AssetDatabase.GetAssetPath(GraphSettings.GetSettings().graphNodeXml))
        {
            this.serializer = tree;
            this.node = node;
            this.title = node.GetNodeDisplayTitle();
            this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            //SetupDataBinding();
        }

        private void SetupDataBinding ()
        {
            var nodeProp = serializer.FindNode(serializer.Nodes, node);

            var descriptionProp = nodeProp.FindPropertyRelative("nodeDisplayDescription");

            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.BindProperty(descriptionProp);
        }

        private void SetupClasses ()
        {
            /*if (node is ActionNode)
            {
                AddToClassList("action");
            }
            else if (node is CompositeNode)
            {
                AddToClassList("composite");
            }
            else if (node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
            else*/ if (node is RootNode)
            {
                AddToClassList("root");
            }
        }

        private void CreateInputPorts ()
        {
            /*if (node is ActionNode)
            {
                input = new GraphNodePort(Direction.Input, Port.Capacity.Single);
            }
            else if (node is CompositeNode)
            {
                input = new GraphNodePort(Direction.Input, Port.Capacity.Single);
            }
            else if (node is DecoratorNode)
            {
                input = new GraphNodePort(Direction.Input, Port.Capacity.Single);
            }
            else*/ if (node is RootNode) { }

            if (input != null)
            {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts ()
        {
            /*if (node is ActionNode) { }
            else if (node is CompositeNode)
            {
                output = new GraphNodePort(Direction.Output, Port.Capacity.Multi);
            }
            else if (node is DecoratorNode)
            {
                output = new GraphNodePort(Direction.Output, Port.Capacity.Single);
            }
            else*/ if (node is RootNode)
            {
                output = new GraphNodePort(Direction.Output, Port.Capacity.Single);
            }

            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }

        public override void SetPosition (Rect newPos)
        {
            base.SetPosition(newPos);

            Vector2 position = new Vector2(newPos.xMin, newPos.yMin);
            serializer.SetNodePosition(node, position);
        }

        public override void OnSelected ()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        public override void OnUnselected ()
        {
            base.OnUnselected();
            if (OnNodeUnselected != null)
            {
                OnNodeUnselected.Invoke(this);
            }
        }

        public void SortChildren ()
        {
            /*if (node is CompositeNode composite)
            {
                composite.children.Sort(SortByHorizontalPosition);
            }*/
        }

        private int SortByHorizontalPosition (GraphNode left, GraphNode right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateState ()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            /*if (Application.isPlaying)
            {
                switch (node.state)
                {
                    case GraphNode.State.Running:
                        if (node.started)
                        {
                            AddToClassList("running");
                        }

                        break;
                    case GraphNode.State.Failure:
                        AddToClassList("failure");
                        break;
                    case GraphNode.State.Success:
                        AddToClassList("success");
                        break;
                }
            }*/
        }
    }
}