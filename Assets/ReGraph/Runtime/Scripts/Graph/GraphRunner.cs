using System;
using System.Runtime.CompilerServices;
using Reshape.ReFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using Reshape.Unity;

namespace Reshape.ReGraph
{
    [HideMonoScript]
    [DisallowMultipleComponent]
    public class GraphRunner : BaseBehaviour
    {
        private const string TickName = "GraphRunner";
        
        [FoldoutGroup("Settings", expanded: false)]
        [ShowIf("ShowSettings")]
        [DisableIf("DisableSettings")]
        public bool runEvenInactive;

        [FoldoutGroup("Settings")]
        [ShowIf("ShowSettings")]
        [DisableIf("DisableSettings")]
        public bool runEvenDisabled;

        [FoldoutGroup("Settings")]
        [ShowIf("ShowSettings")]
        [DisableIf("DisableSettings")]
        public bool stopOnDeactivate;

        [HideLabel]
        public Graph graph;

        private GraphContext context;
        private bool disabled;

        public bool activated
        {
            get
            {
                if (this == null)
                    return false;
                if (!enabled)
                    if (!runEvenDisabled)
                        return false;
                if (!gameObject.activeInHierarchy)
                    if (!runEvenInactive)
                        return false;
                return true;
            }
        }
        
        //-----------------------------------------------------------------
        //-- static methods
        //-----------------------------------------------------------------
        
        //-----------------------------------------------------------------
        //-- public methods
        //-----------------------------------------------------------------

        public void TriggerAction (ActionNameChoice type)
        {
            if (type != null)
                Activate(TriggerNode.Type.ActionTrigger, actionName: type);
        }

        public void TriggerCollision (TriggerNode.Type type, GraphRunner runner)
        {
            OnTrigger(type, runner.gameObject);
        }
        
        [SpecialName]
        public void TriggerSpawn (ActionNameChoice type)
        {
            Activate(TriggerNode.Type.GameObjectSpawn, actionName:type);
        }
        
        public void TriggerInput (TriggerNode.Type type, string triggerId)
        {
            Activate(type, actionName:triggerId);
        }
        
        public void TriggerVariable (TriggerNode.Type type, string triggerId)
        {
            Activate(type, actionName:triggerId);
        }

        public void ResumeTrigger (long executionId, int updateId)
        {
            var execution = graph?.FindExecute(executionId);
            if (execution == null)
            {
                ReDebug.LogWarning("Graph Warning", "Trigger " + executionId + " re-activation have not found in " + gameObject.name);
                return;
            }

            if (!activated)
            {
                graph?.StopExecute(execution, Time.frameCount);
                ReDebug.LogWarning("Graph Warning", "Trigger " + executionId + " re-activation being ignored in " + gameObject.name);
                return;
            }

            graph?.ResumeExecute(execution, Time.frameCount);
        }
        
        //-----------------------------------------------------------------
        //-- BaseBehaviour methods
        //-----------------------------------------------------------------
        
        [SpecialName]
        public override void Init ()
        {
            if (graph.HaveRequireInit())
                Activate();
            if (graph.HaveRequireUpdate())
                PlanTick(TickName);
            PlanUninit(); 
            DoneInit();
        }
        
        [SpecialName]
        public override void Tick ()
        {
            if ( !activated )
                return;
            graph?.Update(Time.frameCount);
        }
        
        [SpecialName]
        public override void Uninit ()
        {
            if (graph.HaveRequireUpdate())
                OmitTick();
            if (graph.HaveRequireInit())
                Deactivate();
            DoneUninit();
        }
        
        //-----------------------------------------------------------------
        //-- mono methods
        //-----------------------------------------------------------------
        
        protected void Awake ()
        {
            if (graph != null)
            {
                context = new GraphContext(this);
                graph.Bind(context);
                if (graph.HaveRequireUpdate() || graph.HaveRequireInit())
                {
                    PlanInit();
                }
            }
        }

        protected void OnDisable ()
        {
            Disable();
        }
        
        protected void OnEnable ()
        {
            Enable();
        }

        protected void OnDestroy ()
        {
            if (graph?.Terminated == false)
            {
                OmitUninit();
                if (graph.HaveRequireUpdate())
                    OmitTick();
                if (graph.HaveRequireInit())
                    Deactivate();
            }
        }

        protected void OnTriggerEnter (Collider other)
        {
            OnTrigger(TriggerNode.Type.CollisionEnter, other.gameObject);
        }

        protected void OnTriggerExit (Collider other)
        {
            OnTrigger(TriggerNode.Type.CollisionExit, other.gameObject);
        }

        protected void OnTriggerEnter2D (Collider2D other)
        {
            OnTrigger(TriggerNode.Type.CollisionEnter, other.gameObject);
        }

        protected void OnTriggerExit2D (Collider2D other)
        {
            OnTrigger(TriggerNode.Type.CollisionExit, other.gameObject);
        }
        
        //-----------------------------------------------------------------
        //-- internal methods
        //-----------------------------------------------------------------

        private void Activate (TriggerNode.Type type, string actionName = null, long executeId = 0, GameObject interactedGo = null)
        {
            if (!activated)
            {
                ReDebug.LogWarning("Graph Warning", type + " activation being ignored in " + gameObject.name);
                return;
            }

            if (executeId == 0)
                executeId = ReTime.currentUtc;
            var execute = graph?.InitExecute(executeId, type);
            if (execute != null)
            {
                if (type is TriggerNode.Type.ActionTrigger or TriggerNode.Type.GameObjectSpawn or TriggerNode.Type.InputPress or TriggerNode.Type.InputRelease or TriggerNode.Type.VariableChange)
                {
                    execute.parameters.actionName = actionName;
                    graph?.RunExecute(execute, Time.frameCount);
                }
                else if (type is TriggerNode.Type.CollisionEnter or TriggerNode.Type.CollisionExit or TriggerNode.Type.CollisionStepIn or TriggerNode.Type.CollisionStepOut)
                {
                    execute.parameters.interactedGo = interactedGo;
                    graph?.RunExecute(execute, Time.frameCount);
                }
            }
        }

        private void OnTrigger (TriggerNode.Type t, GameObject go)
        {
            Activate(t, interactedGo: go);
        }

        private void Enable ()
        {
            if (disabled && !stopOnDeactivate)
                graph?.UnpauseExecutes();
            disabled = false;
        }
        
        private void Disable ()
        {
            if (disabled) return;
            disabled = true;
            if (stopOnDeactivate)
                graph?.StopExecutes();
            if (activated) return;
            if (!stopOnDeactivate)
                graph?.PauseExecutes();
        }
        
        private void Activate()
        {
            graph?.Start();
        }
        
        private void Deactivate()
        {
            graph?.Stop();
        }

#if UNITY_EDITOR
        [Button]
        [ShowIf("ShowExecuteButton")]
        private void Execute (string actionName)
        {
            Activate(TriggerNode.Type.ActionTrigger, actionName: actionName);
        }

        private bool ShowExecuteButton ()
        {
            return Application.isPlaying && graph?.HavePreviewNode() == false;
        }

        private bool ShowSettings ()
        {
            return graph?.HavePreviewNode() == false && graph.Created;
        }

        private bool DisableSettings ()
        {
            return Application.isPlaying;
        }

        private void OnDrawGizmosSelected ()
        {
            if (graph == null)
                return;
            Graph.Traverse(graph.RootNode, (n) =>
            {
                if (n.drawGizmos)
                    n.OnDrawGizmos();
            });
        }
#endif
    }
}