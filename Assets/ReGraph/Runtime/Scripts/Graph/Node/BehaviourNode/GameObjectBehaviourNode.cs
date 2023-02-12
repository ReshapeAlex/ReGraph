using System.Collections;
using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class GameObjectBehaviourNode : BehaviourNode
    {
        public enum ExecutionType
        {
            None,
            ShowIt = 10,
            HideIt = 11,
            EnableComponent = 30,
            DisableComponent = 31
        }

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        private GameObject gameObject;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [LabelText("Execution")]
        private ExecutionType executionType;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [ShowIf("ShowComponent")]
        [ValueDropdown("DrawComponentListDropdown", ExpandAllMenuItems = true)]
        private Component component;


        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (gameObject == null || executionType == ExecutionType.None)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty GameObject Behaviour node in " + context.gameObject.name);
            }
            else
            {
                if (executionType is ExecutionType.DisableComponent or ExecutionType.EnableComponent)
                {
                    if (component == null)
                    {
                        ReDebug.LogWarning("Graph Warning", "Found an empty GameObject Behaviour node in " + context.gameObject.name);
                    }
                    else
                    {
                        bool value = executionType == ExecutionType.EnableComponent;
                        if (component is Renderer)
                        {
                            var comp = (Renderer)component;
                            comp.enabled = value;
                        }
                        else if (component is Collider)
                        {
                            var comp = (Collider)component;
                            comp.enabled = value;
                        }
                        else if (component is Behaviour)
                        {
                            var comp = (Behaviour)component;
                            comp.enabled = value;
                        }
                    }
                }
                else if (executionType == ExecutionType.ShowIt)
                {
                    gameObject.SetActiveOpt(true);
                }
                else if (executionType == ExecutionType.HideIt)
                {
                    gameObject.SetActiveOpt(false);
                }
            }

            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        private bool ShowComponent ()
        {
            if (executionType is ExecutionType.DisableComponent or ExecutionType.EnableComponent)
                return true;
            return false;
        }

        private IEnumerable DrawComponentListDropdown ()
        {
            var actionNameListDropdown = new ValueDropdownList<Component>();
            if (gameObject != null)
            {
                var components = gameObject.GetComponents<Component>();
                foreach (var comp in components)
                {
                    if (comp is Collider or Renderer or Behaviour)
                        actionNameListDropdown.Add(comp.GetType().ToString(), comp);
                }
            }

            return actionNameListDropdown;
        }

        public static string displayName = "GameObject Behaviour Node";
        public static string nodeName = "GameObject";

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
            if (gameObject != null)
            {
                if (executionType == ExecutionType.DisableComponent && component != null)
                {
                    return "Disable " + component.name + " in " + gameObject.name;
                }
                else if (executionType == ExecutionType.EnableComponent && component != null)
                {
                    return "Enable " + component.name + " in " + gameObject.name;
                }
                else if (executionType == ExecutionType.ShowIt)
                {
                    return "Show " + gameObject.name;
                }
                else if (executionType == ExecutionType.HideIt)
                {
                    return "Hide " + gameObject.name;
                }
            }

            return string.Empty;
        }
#endif
    }
}