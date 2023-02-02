namespace Reshape.ReGraph
{
    [System.Serializable]
    public class CollisionTriggerNode : TriggerNode
    {
        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            if (children != null)
                if (children.Count > 0)
                    return children[0].Update(execution, updateId);
            return State.Failure;
        }
        
#if UNITY_EDITOR
        public static string displayName = "Collision Trigger Node";
        public static string nodeName = "Collision";
        
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