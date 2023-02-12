using System;
using Reshape.Unity;
using Sirenix.OdinInspector;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class WaitBehaviourNode : BehaviourNode
    {
        public const string VAR_CALLBACK = "_callback";

        [OnValueChanged("MarkDirty")]
        public float time;

        private string callbackKey;

        private void InitVariables ()
        {
            if (string.IsNullOrEmpty(callbackKey))
                callbackKey = guid + VAR_CALLBACK;
        }

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (time <= 0)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Wait Behaviour node in " + context.gameObject.name);
            }
            else
            {
                InitVariables();
                execution.variables.SetInt(callbackKey, 0);
                context.runner.Wait(execution.id.ToString(), time, OnWaitComplete, Array.Empty<string>());
            }

            base.OnStart(execution, updateId);

            void OnWaitComplete (string[] paramStr)
            {
                execution.variables.SetInt(callbackKey, 1);
                context.runner.ResumeTrigger(execution.id, updateId);
            }
        }

        protected override State OnUpdate (GraphExecution execution, int updateId)
        {
            if (time <= 0)
                return base.OnUpdate(execution, updateId);
            if (execution.variables.GetInt(callbackKey) == 1)
                return base.OnUpdate(execution, updateId);
            return State.Running;
        }

        protected override void OnStop (GraphExecution execution, int updateId)
        {
            bool started = execution.variables.GetStarted(guid, false);
            if (started)
            {
                var callback = execution.variables.GetInt(callbackKey);
                if (callback == 0)
                    context.runner.CancelWait(execution.id.ToString());
            }
            base.OnStop(execution, updateId);
        }

        protected override void OnPause (GraphExecution execution)
        {
            bool started = execution.variables.GetStarted(guid, false);
            if (started)
            {
                var callback = execution.variables.GetInt(callbackKey);
                if (callback == 0)
                    context.runner.StopWait(execution.id.ToString());
            }

            base.OnPause(execution);
        }

        protected override void OnUnpause (GraphExecution execution)
        {
            bool started = execution.variables.GetStarted(guid, false);
            if (started)
            {
                var callback = execution.variables.GetInt(callbackKey);
                if (callback == 0)
                    context.runner.ResumeWait(execution.id.ToString());
            }

            base.OnPause(execution);
        }

        public override bool IsRequireUpdate ()
        {
            return enabled;
        }

#if UNITY_EDITOR
        public static string displayName = "Wait Behaviour Node";
        public static string nodeName = "Wait";

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
            return "Wait " + time + " seconds";
        }
#endif
    }
}