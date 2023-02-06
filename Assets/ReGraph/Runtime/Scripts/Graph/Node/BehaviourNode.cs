using System.Collections.Generic;
using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public abstract class BehaviourNode : GraphNode
    {
        private string VAR_CHILD;

        private void InitVariables ()
        {
            if (string.IsNullOrEmpty(VAR_CHILD))
                VAR_CHILD = guid + "_child";
        }

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (children == null) return;
            InitVariables();
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i] == null)
                    execution.variables.SetInt(VAR_CHILD + i, (int) State.Success);
                else
                    execution.variables.SetInt(VAR_CHILD + i, (int) State.Running);
            }
        }

        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            if (children == null) return State.Success;

            var stillRunning = false;
            var containFailure = false;
            for (int i = 0; i < children.Count; ++i)
            {
                var state = execution.variables.GetInt(VAR_CHILD + i);
                if (state == (int) State.Running)
                {
                    var status = children[i].Update(execution, updateId);
                    execution.variables.SetInt(VAR_CHILD + i, (int) status);
                    if (status == State.Failure)
                        containFailure = true;
                    else if (status == State.Running)
                        stillRunning = true;
                }
                else if (state == (int) State.Failure)
                {
                    containFailure = true;
                }
            }

            if (stillRunning)
                return State.Running;
            if (containFailure)
                return State.Failure;
            return State.Success;
        }

        protected override void OnStop (GraphExecution execution, int updateId) { }

        protected override void OnReset () { }

        protected override State OnDisabled (GraphExecution execution, int updateId)
        {
            return OnUpdate(execution, updateId);
        }

        public override ChildrenType GetChildrenType ()
        {
            return ChildrenType.Multiple;
        }

        public override void GetChildren (ref List<GraphNode> list)
        {
            if (children != null)
                for (var i = 0; i < children.Count; i++)
                    list.Add(children[i]);
        }
    }
}