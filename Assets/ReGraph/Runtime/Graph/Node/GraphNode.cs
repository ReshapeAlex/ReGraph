using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    [HideReferenceObjectPicker]
    public abstract class GraphNode : Node
    {
        [HideInInspector]
        public GraphContext context;

#if UNITY_EDITOR
        [DisplayAsString]
        [HideLabel]
        public string name;
        [HideInInspector]
        [TextArea]
        public string description;
        [HideInInspector]
        public bool drawGizmos = false;
        [HideInInspector]
        public string nodeDisplayDescription;
#endif

        public void Abort ()
        {
            Graph.Traverse(this, (node) =>
            {
                node.OnStop();
            });
        }

#if UNITY_EDITOR
        public abstract string GetNodeDisplayTitle ();
#endif
    }
}