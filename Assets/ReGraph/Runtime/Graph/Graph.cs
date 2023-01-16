using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace Reshape.ReGraph
{
    [Serializable]
    public class Graph
    {
        public enum GraphType
        {
            None,
            BehaviourGraph = 10
        }

        [SerializeField]
        [ValueDropdown("TypeChoice")]
        [DisableIf("@Created")]
        private GraphType type;

        [SerializeReference]
        [HideInInspector]
        private RootNode rootNode;

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
        [HideDuplicateReferenceBox]
        [HideLabel]
        [BoxGroup("PreviewNode", GroupName = "@GetPreviewNodeName()")]
        public GraphNode previewNode;

        [HideInInspector]
        public bool previewSelected;

        [ShowIfGroup("SelectionWarning", Condition = "ShowWarning")]
        [BoxGroup("SelectionWarning/Hide", ShowLabel = false)]
        [HideLabel]
        [DisplayAsString]
        public string multipleSelectionWarning = "Multiple Dialog Tree selected!";

        [HideInInspector]
        public List<ISelectable> selectedViewNode;

        [ShowIf("@Created == false")]
        [Button]
        public void CreateGraph ()
        {
            if (type == GraphType.None)
            {
                EditorUtility.DisplayDialog("Create Graph", "Please select a graph type before click on Create Graph button", "OK");
            }
            else
            {
                Create();
            }
        }

        private static IEnumerable TypeChoice = new ValueDropdownList<GraphType>()
        {
            {"Behaviour Graph", GraphType.BehaviourGraph},
        };

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
            if (selectedViewNode != null && selectedViewNode.Count > 1)
            {
                multipleSelectionWarning = "Multiple Graph Nodes Selected!";
                return true;
            }

            return false;
        }

        public bool HidePreviewNode ()
        {
            if (selectedViewNode != null && selectedViewNode.Count > 1)
                return true;
            return HavePreviewNode() == false;
        }

        public bool HideNodesList ()
        {
            if (rootNode == null)
                return true;
            return HavePreviewNode();
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

        public GraphNode RootNode
        {
            get { return rootNode; }
        }

        public GraphType Type
        {
            get { return type; }
        }

        public bool Created
        {
            get { return rootNode != null; }
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