namespace Reshape.ReGraph
{
    [System.Serializable]
    public class ActionTriggerNode : TriggerNode
    {
#if UNITY_EDITOR
        public static string displayName = "Action Trigger Node";
        public static string nodeName = "Action";
        
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