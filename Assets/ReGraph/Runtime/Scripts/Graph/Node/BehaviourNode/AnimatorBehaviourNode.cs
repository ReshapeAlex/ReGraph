using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class AnimatorBehaviourNode : BehaviourNode
    {
        public enum ExecutionType
        {
            None,
            SetBoolTrue = 10,
            SetBoolFalse = 11,
            SetTrigger = 20
        }

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [LabelText("Execution")]
        [ValueDropdown("TypeChoice")]
        private ExecutionType executionType;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@executionType == ExecutionType.None")]
        private Animator animator;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@executionType == ExecutionType.None")]
        private string parameter;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (animator == null || executionType is ExecutionType.None || string.IsNullOrEmpty(parameter))
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Animator Behaviour node in " + context.gameObject.name);
            }
            else
            {
                if (executionType == ExecutionType.SetBoolTrue)
                {
                    animator.SetBool(parameter, true);
                }
                else if (executionType == ExecutionType.SetBoolFalse)
                {
                    animator.SetBool(parameter, false);
                }
                else if (executionType == ExecutionType.SetTrigger)
                {
                    animator.SetTrigger(parameter);
                }
            }

            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        private static IEnumerable TypeChoice = new ValueDropdownList<ExecutionType>()
        {
            {"Tick Bool", ExecutionType.SetBoolTrue},
            {"Untick Bool", ExecutionType.SetBoolFalse},
            {"Call Trigger", ExecutionType.SetTrigger}
        };

        public static string displayName = "Animator Behaviour Node";
        public static string nodeName = "Animator";

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
            if (animator != null && executionType is ExecutionType.None == false && !string.IsNullOrEmpty(parameter))
            {
                if (executionType is ExecutionType.SetBoolTrue)
                    return "Tick " + parameter + " Bool on " + animator.gameObject.name;
                if (executionType is ExecutionType.SetBoolFalse)
                    return "Untick " + parameter + " Bool on " + animator.gameObject.name;
                if (executionType is ExecutionType.SetTrigger)
                    return "Call " + parameter + " Trigger on " + animator.gameObject.name;
            }

            return string.Empty;
        }
#endif
    }
}