using Reshape.Unity;
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
            ReDebug.Log("Graph Debug", message);
            base.OnStart(execution, updateId);
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