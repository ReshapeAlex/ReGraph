using System;
using System.Collections;
using Reshape.Reframework;
using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class ActionBehaviourNode : BehaviourNode
    {
        [SerializeField]
        [OnValueChanged("MarkDirty")]
        private GraphRunner graph;
        [SerializeField]
        [ValueDropdown("DrawActionNameListDropdown", ExpandAllMenuItems = true)]
        [OnValueChanged("MarkDirty")]
        private ActionNameChoice actionName;
        
        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (graph == null || actionName == null)
                ReDebug.LogWarning("Graph Warning", "Found an empty Action Behaviour node in "+context.gameObject.name);
            else
                graph?.TriggerAction(actionName);
            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        private static IEnumerable DrawActionNameListDropdown ()
        {
            return ActionNameChoice.GetActionNameListDropdown();
        }
        
        public static string displayName = "Action Behaviour Node";
        public static string nodeName = "Action";

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
            if (graph != null && actionName != null)
                return "Execute "+actionName+" in graph of "+graph.gameObject.name;
            return string.Empty;
        }
#endif
    }
}