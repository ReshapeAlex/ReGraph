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
                if ( !enabled )
                    if ( !runEvenDisabled )
                        return false;
                if ( !gameObject.activeInHierarchy )
                    if ( !runEvenInactive )
                        return false;
                return true;
            }
        }
        
        public void ActionTrigger(ActionNameChoice type)
        {
            Activate(typeof(ActionTriggerNode), actionName: type);
        }

        protected override void Start ()
        {
            context = new GraphContext(this);
            graph?.Bind(context);
        }

        private void Update ()
        {
            graph?.Update(Time.frameCount);
        }
        
        private void Activate (Type type, string actionName = null, long executeId = 0)
        {
            if (!activated)
                return;
            if (executeId == 0)
                executeId = ReTime.currentUtc;
            var execute = graph?.InitExecute(executeId);
            if (execute != null)
            {
                execute.parameters.actionName = actionName;
                graph?.RunExecute(execute, Time.frameCount);
            }
        }

#if UNITY_EDITOR
        [Button]
        [ShowIf("ShowExecuteButton")]
        private void Execute ()
        {
            Activate(typeof(ActionTriggerNode), actionName: "Activate");
        }

        private bool ShowExecuteButton ()
        {
            return Application.isPlaying && graph?.HavePreviewNode() == false;
        }
        
        private bool ShowSettings ()
        {
            return graph?.HavePreviewNode() == false;
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