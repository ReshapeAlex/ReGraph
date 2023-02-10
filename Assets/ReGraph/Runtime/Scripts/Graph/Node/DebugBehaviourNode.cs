using System;
using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class DebugBehaviourNode : BehaviourNode
    {
        private const string DEBUG_PREFIX = "Graph Debug";
        [SerializeField]
        [TextArea]
        [OnValueChanged("MarkDirty")]
        private string message;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (string.IsNullOrEmpty(message))
                ReDebug.LogWarning(DEBUG_PREFIX, "Found an invalid Debug Behaviour node in "+context.gameObject.name);
            else
                ReDebug.Log(DEBUG_PREFIX, message);
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
        
        public override string GetNodeViewDescription ()
        {
            if (!string.IsNullOrEmpty(message))
                return "[Log] " + message;
            return string.Empty;
        }
#endif
    }
}