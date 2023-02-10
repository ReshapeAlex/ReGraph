using System;
using System.Collections.Generic;
using System.Linq;
using Reshape.Unity;
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

        private Label descriptionLabel;

        public GraphNodeView (SerializedGraph tree, GraphNode node, GraphViewer viewer) : base(AssetDatabase.GetAssetPath(GraphSettings.GetSettings().graphNodeXml))
        {
            serializer = tree;
            this.node = node;
            this.viewer = viewer;
            if (node != null)
            {
                title = node.GetNodeViewTitle();
                viewDataKey = node.guid;
                style.left = node.position.x;
                style.top = node.position.y;

                CreateInputPorts();
                CreateOutputPorts();
                SetupClasses();
                SetupDataBinding();
            }
            else
            {
                GraphRunner runner = serializer.serializedObject.targetObject as GraphRunner;
                ReDebug.LogWarning("Graph Editor", "System found a null graph node inside " + runner.gameObject.name, false);
            }
        }

        private void SetupDataBinding ()
        {
            var nodeProp = serializer.FindNode(serializer.Nodes, node);

            descriptionLabel = this.Q<Label>("description");
            descriptionLabel.text = node.GetNodeViewDescription();

            Label categoryLabel = this.Q<Label>("category");
            if (node is TriggerNode)
                categoryLabel.text = "Trigger";
            else if (node is BehaviourNode)
                categoryLabel.text = "Behaviour";

            Label connectLabel = this.Q<Label>("connectTo");
            if (node is RootNode)
                connectLabel.text = "Trigger";
            else if (node is TriggerNode)
                connectLabel.text = "Behaviour";
            else if (node is BehaviourNode)
                connectLabel.text = "Behaviour";

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

            if (serializer.graph.selectedViewNode.Count == 1)
                HighlightReference();
            else
                viewer.UnhighlightAllReferenceNode();
        }

        public override void OnUnselected ()
        {
            base.OnUnselected();
            if (OnNodeUnselected != null)
            {
                OnNodeUnselected.Invoke(this);
            }

            if (node != null && node is TriggerBehaviourNode)
            {
                var referenceNode = this.node as TriggerBehaviourNode;
                if (!string.IsNullOrEmpty(referenceNode.triggerNodeId))
                {
                    //-- unhighlight referenceNode
                    viewer.UnhighlightReferenceNode(referenceNode.triggerNodeId);
                }
            }
        }

        public List<GraphNode> SortChildren ()
        {
            if (node is RootNode or TriggerNode or BehaviourNode)
            {
                var gNode = (GraphNode) node;
                List<GraphNode> sorted = gNode.children.ToList();
                sorted.Sort(SortByHorizontalPosition);
                return sorted;
            }

            return null;
        }

        private int SortByHorizontalPosition (GraphNode left, GraphNode right)
        {
            var leftX = left == null ? float.MaxValue : left.position.x;
            var rightX = right == null ? float.MaxValue : right.position.x;
            return leftX < rightX ? -1 : 1;
        }

        public override void BuildContextualMenu (ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GraphNodeView)
            {
                var nodeView = (GraphNodeView) evt.target;
                if (nodeView.node is RootNode == false)
                    evt = viewer.GetDeleteAction(evt);
            }

            base.BuildContextualMenu(evt);
        }

        public void Update ()
        {
            if (node != null && node.dirty)
            {
                descriptionLabel.text = node.GetNodeViewDescription();
                node.dirty = false;

                viewer.UnhighlightAllReferenceNode();
                HighlightReference();
            }

            UpdateState();
        }

        public void HighlightReference ()
        {
            if (node != null && node is TriggerBehaviourNode)
            {
                var referenceNode = this.node as TriggerBehaviourNode;
                if (!string.IsNullOrEmpty(referenceNode.triggerNodeId))
                {
                    //-- highlight referenceNode
                    viewer.HighlightReferenceNode(referenceNode.triggerNodeId);
                }
            }
        }

        public void ApplyRunningHighlight ()
        {
            AddToClassList("running");
        }

        public void UnapplyRunningHighlight ()
        {
            RemoveFromClassList("running");
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