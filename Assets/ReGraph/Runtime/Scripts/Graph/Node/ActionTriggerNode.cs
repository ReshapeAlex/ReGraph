using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class ActionTriggerNode : TriggerNode
    {
        [SerializeField]
        [ValueDropdown("DrawActionName1ListDropdown", ExpandAllMenuItems = true)]
        [OnValueChanged("MarkDirty")]
        private ActionNameChoice actionName;

        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            if (actionName != null)
                if (execution.parameters.actionName.Equals(actionName))
                    return base.OnUpdate(execution, updateId);
            return State.Failure;
        }

#if UNITY_EDITOR
        private static IEnumerable DrawActionName1ListDropdown ()
        {
            return ActionNameChoice.GetActionNameListDropdown();
        }

        public static string displayName = "Action Trigger Node";
        public static string nodeName = "Action";

        public override string GetNodeInspectorTitle ()
        {
            return displayName;
        }

        public override string GetNodeViewTitle ()
        {
            return nodeName;
        }
        
        public override string GetNodeViewDescription ()
        {
            return actionName == null ? String.Empty : "Action Name : "+actionName;
        }
#endif
    }
}