using System;
using System.Collections.Generic;
using System.Linq;
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
        public GraphViewer viewer;
        public Port input;
        public Port output;

        public GraphNodeView (SerializedGraph tree, GraphNode node, GraphViewer viewer) : base(AssetDatabase.GetAssetPath(GraphSettings.GetSettings().graphNodeXml))
        {
            this.serializer = tree;
            this.node = node;
            this.viewer = viewer;
            this.title = node.GetNodeViewTitle();
            this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            SetupDataBinding();
        }

        private void SetupDataBinding ()
        {
            var nodeProp = serializer.FindNode(serializer.Nodes, node);

            var descriptionProp = nodeProp.FindPropertyRelative("nodeDisplayDescription");

            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.BindProperty(descriptionProp);

            this.node.onEnableChange -= OnEnableChange;
            this.node.onEnableChange += OnEnableChange;
        }

        private void OnEnableChange ()
        {
            if (this.node.enabled)
                RemoveFromClassList(viewer.GetDisableStyle());
            else
                RemoveFromClassList(viewer.GetStyle(node));
            SetupClasses();
        }

        private void SetupClasses ()
        {
            if (!this.node.enabled)
                AddToClassList(viewer.GetDisableStyle());
            else
                AddToClassList(viewer.GetStyle(node));
        }

        private void CreateInputPorts ()
        {
            if (node is RootNode == false)
                input = new GraphNodePort(Direction.Input, Port.Capacity.Single);
            if (input != null)
            {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts ()
        {
            if (node.GetChildrenType() == GraphNode.ChildrenType.Single)
                output = new GraphNodePort(Direction.Output, Port.Capacity.Single);
            else if (node.GetChildrenType() == GraphNode.ChildrenType.Multiple)
                output = new GraphNodePort(Direction.Output, Port.Capacity.Multi);
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

        public List<GraphNode> SortChildren ()
        {
            if (node is RootNode root)
            {
                List<GraphNode> sorted = root.children.ToList();
                sorted.Sort(SortByHorizontalPosition);
                return sorted;
            }
            return null;
        }

        private int SortByHorizontalPosition (GraphNode left, GraphNode right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GraphNodeView)
            {
                var nodeView = (GraphNodeView) evt.target;
                if (nodeView.node is RootNode == false)
                    evt = viewer.GetDeleteAction(evt);
            }
            base.BuildContextualMenu(evt);
        }

        public void UpdateState ()
        {
            /*RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (Application.isPlaying)
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