using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class PhysicsBehaviourNode : BehaviourNode
    {
        public enum ExecutionType
        {
            None,
            ClearVelocity = 10
        }

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        private Rigidbody rigidbody;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [LabelText("Execution")]
        private ExecutionType executionType;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (rigidbody == null || executionType == ExecutionType.None)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Physics Behaviour node in " + context.gameObject.name);
            }
            else if (executionType == ExecutionType.ClearVelocity)
            {
                rigidbody.velocity = Vector3.zero;
            }

            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        public static string displayName = "Physics Behaviour Node";
        public static string nodeName = "Physics";

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
            if (rigidbody != null)
            {
                if (executionType == ExecutionType.ClearVelocity)
                    return "Clear velocity on " + rigidbody.gameObject.name;
            }

            return string.Empty;
        }
#endif
    }
}