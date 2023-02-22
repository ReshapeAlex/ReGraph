using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using Reshape.Reframework;
using Reshape.Unity;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class InputTriggerNode : TriggerNode
    {
#if ENABLE_INPUT_SYSTEM
        [ValueDropdown("TriggerTypeChoice")]
        [OnValueChanged("MarkDirty")]
        public Type triggerType;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@triggerType==Type.None")]
        private InputActionAsset inputAction;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@triggerType==Type.None")]
        [ValueDropdown("InputActionNameChoice", ExpandAllMenuItems = false, AppendNextDrawer = true)]
        private string inputName;

#elif UNITY_EDITOR
        [DisplayAsString]
        [HideLabel]
        public string importWarning = "Please import Input System at Package Manager";
#endif

        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            State state = execution.variables.GetState(guid, State.Running);
            if (state == State.Running)
            {
                if (execution.type is Type.InputPress or Type.InputRelease && execution.type == triggerType)
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
            if (inputAction != null && triggerType != Type.None)
            {
                InputAction action = inputAction.FindAction(inputName);
                if (action != null)
                {
                    inputAction.Enable();
                    if (triggerType == Type.InputPress)
                    {
                        action.performed += callbackContext => 
                        {
                            context.runner.TriggerInput(Type.InputPress, TriggerId);
                        };
                    }
                    else if (triggerType == Type.InputRelease)
                    {
                        action.canceled += callbackContext => 
                        {
                            context.runner.TriggerInput(Type.InputRelease, TriggerId);
                        };
                    }
                }
                else
                {
                    ReDebug.LogWarning("Graph Warning", "Found an empty Input Trigger node in " + context.gameObject.name);
                }
            }
            else
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Input Trigger node in " + context.gameObject.name);
            }
        }

        protected override void OnReset ()
        {
            inputAction.Disable();
        }

        public override bool IsRequireInit ()
        {
            if (inputAction == null || string.IsNullOrEmpty(inputName) || triggerType == Type.None)
                return false;
            return true;
        }

#if UNITY_EDITOR
        private IEnumerable InputActionNameChoice ()
        {
            ValueDropdownList<string> menu = new ValueDropdownList<string>();
            if (inputAction != null)
            {
                for (int i = 0; i < inputAction.actionMaps.Count; i++)
                {
                    string mapName = inputAction.actionMaps[i].name;
                    for (int j = 0; j < inputAction.actionMaps[i].actions.Count; j++)
                    {
                        menu.Add(mapName + "//" + inputAction.actionMaps[i].actions[j].name, inputAction.actionMaps[i].actions[j].name);
                    }
                }
            }

            return menu;
        }

        private IEnumerable TriggerTypeChoice ()
        {
            ValueDropdownList<Type> menu = new ValueDropdownList<Type>();
            menu.Add("Input Press", Type.InputPress);
            menu.Add("Input Release", Type.InputRelease);
            return menu;
        }

        public static string displayName = "Input Trigger Node";
        public static string nodeName = "Input";

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
            if (inputAction != null && !string.IsNullOrEmpty(inputName) && triggerType != Type.None)
            {
                if (triggerType == Type.InputPress)
                    return "Press " + inputName;
                if (triggerType == Type.InputRelease)
                    return "Release " + inputName;
            }
            return string.Empty;
        }
#endif
    }
}