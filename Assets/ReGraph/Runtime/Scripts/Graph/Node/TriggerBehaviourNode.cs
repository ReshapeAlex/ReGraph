using System.Collections;
using System.Collections.Generic;
using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class TriggerBehaviourNode : BehaviourNode
    {
        private const string DEBUG_PREFIX = "Graph Debug";

        public enum ExecutionType
        {
            None,
            EnableIt = 10,
            DisableIt = 11
        }

        [SerializeReference]
        [OnValueChanged("MarkDirty")]
        [ValueDropdown("DrawTriggerListDropdown", ExpandAllMenuItems = true)]
        private string triggerNode;

        [SerializeField]
        [LabelText("Execution")]
        [OnValueChanged("MarkDirty")]
        private ExecutionType executionType;

        public string triggerNodeId => triggerNode;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (string.IsNullOrEmpty(triggerNode) || executionType == ExecutionType.None)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Trigger Behaviour node in " + context.gameObject.name);
            }
            else
            {
                var triggers = Graph.GetChildren(context.graph.RootNode);
                for (int i = 0; i < triggers.Count; i++)
                {
                    if (triggers[i].guid == triggerNode)
                    {
                        if (executionType == ExecutionType.EnableIt)
                            triggers[i].enabled = true;
                        else if (executionType == ExecutionType.DisableIt)
                            triggers[i].enabled = false;
#if UNITY_EDITOR
                        triggers[i].OnEnableChange();
#endif
                        break;
                    }
                }
            }

            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        private IEnumerable DrawTriggerListDropdown ()
        {
            var actionNameListDropdown = new ValueDropdownList<string>();
            if (runner != null && runner.graph != null && runner.graph.nodes != null)
            {
                for (int i = 0; i < runner.graph.nodes.Count; i++)
                {
                    if (runner.graph.nodes[i] is TriggerNode)
                        actionNameListDropdown.Add(runner.graph.nodes[i].GetNodeViewTitle() + " (" + runner.graph.nodes[i].guid + ")", runner.graph.nodes[i].guid);
                }
            }

            return actionNameListDropdown;
        }

        public static string displayName = "Trigger Behaviour Node";
        public static string nodeName = "Trigger";

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
            if (!string.IsNullOrEmpty(triggerNode))
            {
                if (executionType == ExecutionType.EnableIt)
                    return "Enable " + triggerNode;
                else if (executionType == ExecutionType.DisableIt)
                    return "Disable " + triggerNode;
            }

            return string.Empty;
        }
#endif
    }
}