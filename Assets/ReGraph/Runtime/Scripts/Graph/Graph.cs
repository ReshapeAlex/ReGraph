using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
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
        [HideIf("HavePreviewNode")]
        private GraphType type;

        [SerializeReference]
        [HideInInspector]
        private RootNode rootNode;

        [SerializeReference]
        [HideIf("HideNodesList")]
        [DisableIf("@HavePreviewNode() == false")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, Expanded = false, ShowIndexLabels = true, ListElementLabelName = "GetNodeInspectorTitle")]
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

        private static IEnumerable TypeChoice = new ValueDropdownList<GraphType>()
        {
            {"Behaviour Graph", GraphType.BehaviourGraph},
        };

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

        public bool IsApplicationPlaying ()
        {
            return Application.isPlaying;
        }

        public bool HavePreviewNode ()
        {
            return previewSelected;
        }

        public bool ShowWarning ()
        {
            if (selectedViewNode != null)
            {
                if (selectedViewNode.Count > 1)
                {
                    multipleSelectionWarning = "Multiple Graph Nodes Selected!";
                    return true;
                }
                else if (selectedViewNode.Count == 1)
                {
                    if (previewSelected && previewNode == null)
                    {
                        multipleSelectionWarning = "Error Graph Nodes Selected!";
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HidePreviewNode ()
        {
            if (selectedViewNode != null && selectedViewNode.Count > 1)
                return true;
            if (previewNode == null)
                return true;
            return !previewSelected;
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
            return previewNode.GetNodeInspectorTitle();
        }
#endif

        public Graph ()
        {
            executes = new GraphExecutes();
        }

        public GraphNode RootNode => rootNode;

        public GraphType Type => type;

        public bool Created => rootNode != null;
        
        public void Create ()
        {
            rootNode = new RootNode();
            nodes.Add(rootNode);
        }
        
        public void Bind (GraphContext c)
        {
            context = c;
            Traverse(rootNode, node => { node.context = context; });
        }

        public GraphContext Context => context;

        public GraphExecution InitExecute (long id, TriggerNode.Type triggerType)
        {
            if (!Created)
                return null;
            var execution = executes.Add(id, triggerType);
            return execution;
        }
        
        public void RunExecute (GraphExecution execution, int updateId)
        {
            if (!Created)
                return;
            execution.state = rootNode.Update(execution, updateId);
        }

        public void Update (int updateId)
        {
            if (!Created)
                return;
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
            for (int i = 0; i < executes.Count; i++)
                rootNode?.Abort(executes.Get(i));
            Reset();
        }

        public static List<GraphNode> GetChildren (GraphNode parent)
        {
            var children = new List<GraphNode>();
            parent?.GetChildren(ref children);
            return children;
        }

        public static void Traverse (GraphNode node, Action<GraphNode> visitor)
        {
            if (node != null)
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                foreach (var n in children)
                    Traverse(n, visitor);
            }
        }
    }
}