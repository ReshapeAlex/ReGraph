using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class RootNode : GraphNode
    {
        [SerializeReference]
        [HideInInspector]
        public GraphNode child;

        protected override void OnStart () { }

        protected override void OnStop () { }

        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            //State state = execution.variables.GetState(guid, State.Running);
            
            if (child != null)
                return child.Update(execution, updateId);
            return State.Failure;
        }

#if UNITY_EDITOR
        public RootNode ()
        {
            name = "Root Node";
        }

        public override string GetNodeDisplayTitle()
        {
            return "Start";
        }
#endif
    }
}