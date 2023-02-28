using System.Collections;
using Reshape.ReFramework;
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
            Show = 10,
            Hide = 11,
            EnableComponent = 30,
            DisableComponent = 31,
            Spawn = 50,
            Expel = 51
        }

        private enum GoType
        {
            None,
            WithRunner,
            WithoutRunner
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

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [ShowIf("ShowLocation")]
        [LabelText("Spawn Location")]
        private Transform location;

        [SerializeField]
        [ValueDropdown("DrawActionNameListDropdown", ExpandAllMenuItems = true)]
        [OnValueChanged("MarkDirty")]
        [ShowIf("ShowLocation")]
        [LabelText("OnSpawn Action")]
        private ActionNameChoice actionName;

        private GoType spawnType;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (gameObject == null || executionType == ExecutionType.None)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty GameObject Behaviour node in " + context.gameObject.name);
            }
            else if (executionType is ExecutionType.DisableComponent or ExecutionType.EnableComponent)
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
                        var comp = (Renderer) component;
                        comp.enabled = value;
                    }
                    else if (component is Collider)
                    {
                        var comp = (Collider) component;
                        comp.enabled = value;
                    }
                    else if (component is Behaviour)
                    {
                        var comp = (Behaviour) component;
                        comp.enabled = value;
                    }
                }
            }
            else if (executionType == ExecutionType.Show)
            {
                gameObject.SetActiveOpt(true);
            }
            else if (executionType == ExecutionType.Hide)
            {
                gameObject.SetActiveOpt(false);
            }
            else if (executionType == ExecutionType.Spawn)
            {
                GameObject go = null;
                if (location != null)
                    go = context.runner.TakeFromPool(gameObject, location);
                else
                    go = context.runner.TakeFromPool(gameObject, context.transform);
                if (go != null && actionName != null)
                {
                    if (spawnType == GoType.None)
                    {
                        GraphRunner gr = go.GetComponent<GraphRunner>();
                        if (gr != null)
                        {
                            spawnType = GoType.WithRunner;
                            gr.TriggerSpawn(actionName);
                        }
                        else
                        {
                            spawnType = GoType.WithoutRunner;
                        }
                    }
                    else if (spawnType == GoType.WithRunner)
                    {
                        GraphRunner gr = go.GetComponent<GraphRunner>();
                        gr.TriggerSpawn(actionName);
                    }
                }
            }
            else if (executionType == ExecutionType.Expel)
            {
                context.runner.InsertIntoPool(gameObject, true);
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

        private bool ShowLocation ()
        {
            if (executionType is ExecutionType.Spawn)
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

        private static IEnumerable DrawActionNameListDropdown ()
        {
            return ActionNameChoice.GetActionNameListDropdown();
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
                else if (executionType == ExecutionType.Show)
                {
                    return "Show " + gameObject.name;
                }
                else if (executionType == ExecutionType.Hide)
                {
                    return "Hide " + gameObject.name;
                }
                else if (executionType == ExecutionType.Spawn)
                {
                    if (actionName != null)
                        return "Spawn " + gameObject.name + " with " + actionName + " action";
                    else
                        return "Spawn " + gameObject.name;
                }
                else if (executionType == ExecutionType.Expel)
                {
                    return "Expel " + gameObject.name;
                }
            }

            return string.Empty;
        }
#endif
    }
}