using System;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using Reshape.Unity;

namespace Reshape.ReGraph
{
    [HideMonoScript]
    [DisallowMultipleComponent]
    public class GraphRunner : BaseBehaviour
    {
        [FoldoutGroup("Settings", expanded: false)]
        [ShowIf("ShowSettings")]
        [DisableIf("DisableSettings")]
        public bool runEvenInactive;

        [FoldoutGroup("Settings")]
        [ShowIf("ShowSettings")]
        [DisableIf("DisableSettings")]
        public bool runEvenDisabled;

        [HideLabel]
        public Graph graph;

        private GraphContext context;

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

        public void TriggerAction (ActionNameChoice type)
        {
            Activate(TriggerNode.Type.ActionTrigger, actionName: type);
        }
        
        public void TriggerCollision (TriggerNode.Type type, GraphRunner runner)
        {
            OnTrigger(type, runner.gameObject);
        }

        protected override void Start ()
        {
            context = new GraphContext(this);
            graph?.Bind(context);
        }

        protected void Update ()
        {
            graph?.Update(Time.frameCount);
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

        private void Activate (TriggerNode.Type type, string actionName = null, long executeId = 0, GameObject interactedGo = null)
        {
            if (!activated)
                return;
            if (executeId == 0)
                executeId = ReTime.currentUtc;
            var execute = graph?.InitExecute(executeId, type);
            if (execute != null)
            {
                if (type == TriggerNode.Type.ActionTrigger)
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
        
        private void OnTrigger(TriggerNode.Type t, GameObject go)
        {
            Activate(t, interactedGo:go);
        }

#if UNITY_EDITOR
        [Button]
        [ShowIf("ShowExecuteButton")]
        private void Execute ()
        {
            Activate(TriggerNode.Type.ActionTrigger, actionName: "Activate");
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