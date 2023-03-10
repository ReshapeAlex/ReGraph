using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using Reshape.ReFramework;
using Reshape.Unity;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class VariableTriggerNode : TriggerNode
    {
        [ValueDropdown("TriggerTypeChoice")]
        [OnValueChanged("MarkDirty")]
        public Type triggerType;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@triggerType==Type.None")]
        public VariableScriptableObject variable;

        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            State state = execution.variables.GetState(guid, State.Running);
            if (state == State.Running)
            {
                if (execution.type is Type.VariableChange && execution.type == triggerType)
                {
                    if (execution.parameters.actionName == TriggerId)
                    {
                        execution.variables.SetState(guid, State.Success);
                        state = State.Success;
                    }
                    else
                    {
                        execution.variables.SetState(guid, State.Failure);
                        state = State.Failure;
                    }
                }
                else
                {
                    execution.variables.SetState(guid, State.Failure);
                    state = State.Failure;
                }
            }

            if (state == State.Success)
                return base.OnUpdate(execution, updateId);
            return State.Failure;
        }

        protected override void OnInit ()
        {
            if (variable != null && triggerType == Type.VariableChange)
            {
                variable.onChange += OnValueChanged;
            }
            else
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Variable Trigger node in " + context.gameObject.name);
            }
        }

        private void OnValueChanged ()
        {
            context.runner.TriggerVariable(Type.VariableChange, TriggerId);
        }

        protected override void OnReset ()
        {
            variable.onChange -= OnValueChanged;
        }

        public override bool IsRequireInit ()
        {
            if (variable == null || triggerType == Type.None)
                return false;
            return true;
        }

#if UNITY_EDITOR
        private IEnumerable TriggerTypeChoice ()
        {
            ValueDropdownList<Type> menu = new ValueDropdownList<Type>();
            menu.Add("Value Change", Type.VariableChange);
            return menu;
        }

        public static string displayName = "Variable Trigger Node";
        public static string nodeName = "Variable";

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
            if (triggerType == Type.VariableChange && variable != null)
                return variable.name + "'s value changed";
            return string.Empty;
        }
#endif
    }
}