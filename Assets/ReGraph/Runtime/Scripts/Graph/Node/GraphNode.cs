using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    [HideReferenceObjectPicker]
    public abstract class GraphNode : Node
    {
        public enum ChildrenType
        {
            Single,
            Multiple,
            None
        }

        [SerializeReference]
        [ShowIf("ShowChildren"), BoxGroup("Show Debug Info")]
        [ReadOnly]
        [ListDrawerSettings(ListElementLabelName = "guid")]
        public List<GraphNode> children = new List<GraphNode>();

        [HideInInspector]
        public GraphContext context;
        
        public void Abort (GraphExecution execution)
        {
            Graph.Traverse(this, (node) => { node.OnStop(execution, 0); });
        }

#if UNITY_EDITOR
        [HideInInspector]
        [TextArea]
        public string description;
        [HideInInspector]
        public bool drawGizmos = false;
        [HideInInspector]
        public string nodeDisplayDescription;

        private bool ShowChildren ()
        {
            if (GetType().ToString().Contains("RootNode"))
                return true;
            return showAdvanceSettings;
        }
        
        public abstract string GetNodeInspectorTitle ();
        public abstract string GetNodeViewTitle ();
#endif

        public abstract ChildrenType GetChildrenType ();
        public abstract void GetChildren (ref List<GraphNode> list);
    }
}