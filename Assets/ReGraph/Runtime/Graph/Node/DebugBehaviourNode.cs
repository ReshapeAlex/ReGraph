using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class DebugBehaviourNode : BehaviourNode
    {
        [SerializeField]
        [TextArea]
        private string message;
        
        protected override void OnStart (GraphExecution execution, int updateId)
        {
            Debug.Log(message);
        }

#if UNITY_EDITOR
        public static string displayName = "Debug Behaviour Node";
        public static string nodeName = "Debug";

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