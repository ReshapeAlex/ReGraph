using System;
using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using Reshape.Reframework;
using UnityEngine.InputSystem;
#endif

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class InputBehaviourNode : BehaviourNode
    {
        public enum ExecutionType
        {
            None,
            MouseRotationEnable = 10,
            MouseRotationDisable = 11
        }

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [LabelText("Execution")]
        [ValueDropdown("TypeChoice")]
        private ExecutionType executionType;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        private GameObject gameObject;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [LabelText("@ParamVectorTwo1Name()")]
        [HideIf("HideParamVectorTwo1")]
        private Vector2 paramVectorTwo1;

#if ENABLE_INPUT_SYSTEM
        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("HideParamInputAction")]
        private InputActionAsset inputAction;
#endif
        
        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("HideParamCameraView")]
        private Camera cameraView;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [LabelText("@ParamString1Name()")]
        [HideIf("HideParamString1")]
        [ValueDropdown("ParamString1Choice", ExpandAllMenuItems = false, AppendNextDrawer = true)]
        private string paramString1;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (executionType is ExecutionType.None)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Input Behaviour node in " + context.gameObject.name);
            }
            else if (executionType is ExecutionType.MouseRotationEnable)
            {
#if ENABLE_INPUT_SYSTEM
                if (gameObject == null || inputAction == null || string.IsNullOrEmpty(paramString1))
                {
                    ReDebug.LogWarning("Graph Warning", "Found an empty Input Behaviour node in " + context.gameObject.name);
                }
                else
                {
                    MouseRotationController inpect = gameObject.GetComponent<MouseRotationController>();
                    if (inpect == null)
                        inpect = gameObject.AddComponent<MouseRotationController>();
                    inpect.Initial(paramVectorTwo1, inputAction, paramString1, cameraView);
                }
#endif
            }
            else if (executionType is ExecutionType.MouseRotationDisable)
            {
                if (gameObject == null)
                {
                    ReDebug.LogWarning("Graph Warning", "Found an empty Input Behaviour node in " + context.gameObject.name);
                }
                else
                {
                    MouseRotationController inpect = gameObject.GetComponent<MouseRotationController>();
                    if (inpect != null)
                        inpect.Terminate();
                }
            }

            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        private IEnumerable ParamString1Choice ()
        {
            ValueDropdownList<string> menu = new ValueDropdownList<string>();
            if (executionType is ExecutionType.MouseRotationEnable or ExecutionType.MouseRotationDisable)
            {
#if ENABLE_INPUT_SYSTEM
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
#endif
            }

            return menu;
        }

        private string ParamVectorTwo1Name ()
        {
            if (executionType is ExecutionType.MouseRotationEnable)
                return "Rotate Speed";
            return string.Empty;
        }

        private string ParamString1Name ()
        {
            if (executionType is ExecutionType.MouseRotationEnable)
                return "Input Name";
            return string.Empty;
        }

        private bool HideParamInputAction ()
        {
            if (executionType is ExecutionType.MouseRotationEnable)
                return false;
            return true;
        }
        
        private bool HideParamCameraView ()
        {
#if ENABLE_INPUT_SYSTEM
            if (executionType is ExecutionType.MouseRotationEnable)
                return false;
#endif
            return true;
        }

        private bool HideParamVectorTwo1 ()
        {
#if ENABLE_INPUT_SYSTEM
            if (executionType is ExecutionType.MouseRotationEnable)
                return false;
#endif
            return true;
        }

        private bool HideParamString1 ()
        {
#if ENABLE_INPUT_SYSTEM
            if (executionType is ExecutionType.MouseRotationEnable)
                return false;
#endif
            return true;
        }

        private static IEnumerable TypeChoice = new ValueDropdownList<ExecutionType>()
        {
            {"Enable Mouse To Rotation", ExecutionType.MouseRotationEnable},
            {"Disable Mouse To Rotation", ExecutionType.MouseRotationDisable},
        };

        public static string displayName = "Input Behaviour Node";
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
            if (executionType is ExecutionType.MouseRotationEnable)
            {
#if ENABLE_INPUT_SYSTEM
                if (inputAction != null && !string.IsNullOrEmpty(paramString1) && gameObject != null)
                    return "Enable Mouse Control Rotation on " + gameObject.name;
#endif
            }
            else if (executionType is ExecutionType.MouseRotationDisable)
            {
                return "Disable Mouse Control Rotation on " + gameObject.name;
            }

            return string.Empty;
        }
#endif
    }
}