using System.Collections.Generic;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public abstract class TriggerNode : GraphNode
    {
        protected override void OnStart (GraphExecution execution, int updateId) { }

        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            return State.Failure;
        }

        protected override void OnStop (GraphExecution execution, int updateId) { }

        protected override void OnReset () { }

        protected override State OnDisabled (GraphExecution execution, int updateId)
        {
            return State.Failure;
        }

        public override ChildrenType GetChildrenType ()
        {
            return ChildrenType.Single;
        }

        public override void GetChildren (ref List<GraphNode> list)
        {
            if (children != null && children.Count > 0)
                list.Add(children[0]);
        }
    }
}