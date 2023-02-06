namespace Reshape.ReGraph
{
    [System.Serializable]
    public class CollisionTriggerNode : TriggerNode
    {
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