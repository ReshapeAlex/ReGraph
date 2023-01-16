using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Reshape.ReGraph
{
    [Serializable]
    public class Graph
    {
        [SerializeReference]
        [HideInInspector]
        public RootNode rootNode;

        [SerializeReference]
        [HideIf("HideNodesList")]
        [DisableIf("@HavePreviewNode() == false")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        public List<GraphNode> nodes = new List<GraphNode>();
        
        [SerializeField, ReadOnly, HideLabel]
        [HideIf("@IsApplicationPlaying() == false")]
        private GraphExecutes executes;

        private GraphContext context;

#if UNITY_EDITOR
        [HideInInspector]
        public Vector3 viewPosition = new Vector3(300, 200);
        [HideInInspector]
        public Vector3 viewScale = Vector3.one;
        [SerializeReference]
        [HideIf("HidePreviewNode")]
        [HideLabel]
        [HideDuplicateReferenceBox]
        [BoxGroup("PreviewNode", GroupName = "@GetPreviewNodeName()")]
        [OnInspectorDispose("UnselectedPreviewNode")]
        public GraphNode previewNode;
        [HideInInspector]
        public bool previewSelected;
        [ShowIfGroup("SelectionWarning", Condition = "ShowWarning")]
        [BoxGroup("SelectionWarning/Hide", ShowLabel = false)]
        [HideLabel]
        [DisplayAsString]
        public string multipleSelectionWarning = "Multiple Dialog Tree selected!";
        [HideInInspector]
        public int selectedNodeCount;

        public bool IsApplicationPlaying ()
        {
            return Application.isPlaying;
        }
        
        public bool HavePreviewNode ()
        {
            if (previewNode == null)
                return false;
            return previewSelected;
        }

        public bool ShowWarning ()
        {
            if (selectedNodeCount > 1)
            {
                multipleSelectionWarning = "Multiple Graph Nodes Selected!"; 
                return true;
            }
            return false;
        }

        public bool HidePreviewNode ()
        {
            if (selectedNodeCount > 1)
                return true;
            return HavePreviewNode() == false;
        }

        public bool HideNodesList ()
        {
            if (rootNode == null)
                return true;
            return HavePreviewNode();
        }

        public void UnselectedPreviewNode ()
        {
            previewNode = null;
        }

        public string GetPreviewNodeName ()
        {
            if (previewNode == null)
                return String.Empty;
            return previewNode.GetType().ToString();
        }
#endif

        public Graph ()
        {
            executes = new GraphExecutes();
        }

        public void Create ()
        {
            rootNode = new RootNode();
            nodes.Add(rootNode);
            nodes.Add(new RootNode());
        }

        public void Bind (GraphContext context)
        {
            this.context = context;
            Traverse(rootNode, node => { node.context = context; });
        }

        public void Execute (long id, int updateId)
        {
            var execution = executes.Add(id);
            execution.state = rootNode.Update(execution, updateId);
        }

        public void Update (int updateId)
        {
            for (int i = 0; i < executes.Count; i++)
            {
                var execution = executes.Get(i);
                if (execution.state == Node.State.Running)
                    execution.state = rootNode.Update(execution, updateId);
            }
        }

        public void Reset ()
        {
            executes.Clear();
            Traverse(rootNode, node => { node.Reset(); });
        }

        public void Stop ()
        {
            rootNode.Abort();
            Reset();
        }

        public static List<GraphNode> GetChildren (GraphNode parent)
        {
            List<GraphNode> children = new List<GraphNode>();

            /*if (parent is DecoratorNode decorator && decorator.child != null)
            {
                children.Add(decorator.child);
            }*/

            if (parent is RootNode {child: { }} rootNode)
            {
                children.Add(rootNode.child);
            }

            /*if (parent is CompositeNode composite)
            {
                return composite.children;
            }*/

            return children;
        }

        public static void Traverse (GraphNode node, System.Action<GraphNode> visitor)
        {
            if (node != null)
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visitor));
            }
        }
    }
}