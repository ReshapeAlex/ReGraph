using System.Collections.Generic;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class RootNode : GraphNode
    {
        private static string VAR_CURRENT;

        private void InitVariables ()
        {
            if (string.IsNullOrEmpty(VAR_CURRENT))
                VAR_CURRENT = guid + VAR_CURRENT;
        }
        
        protected override void OnStart (GraphExecution execution, int updateId)
        {
            InitVariables();
            execution.variables.SetInt(VAR_CURRENT, 0);
        }

        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            if (children != null)
            {
                int current = execution.variables.GetInt(VAR_CURRENT);
                for (int i = current; i < children.Count; ++i)
                {
                    current = i;
                    var child = children[current];
                    if (child != null)
                    {
                        switch (child.Update(execution, updateId))
                        {
                            case State.Running:
                                return State.Running;
                            case State.Success:
                                return State.Success;
                            case State.Failure:
                                continue;
                        }
                    }
                }
            }
            return State.Failure;
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

#if UNITY_EDITOR
        public static string displayName = "Start Node";
        public static string nodeName = "Start";

        public override string GetNodeInspectorTitle()
        {
            return displayName;
        }

        public override string GetNodeViewTitle()
        {
            return nodeName;
        }
#endif
    }
}