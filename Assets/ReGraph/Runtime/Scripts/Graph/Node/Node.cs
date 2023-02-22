using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    public abstract class Node
    {
        public enum State
        {
            Running = 1,
            Failure = 50,
            Stop = 60,
            Success = 100
        }

        [HideIf("@GetType().ToString().Contains(\"RootNode\")")]
        [OnValueChanged("OnEnableChange")]
        [InlineButton("ShowAdvanceSettings", "≡")]
        public bool enabled = true;
        
        [ShowIf("showAdvanceSettings"), BoxGroup("Show Debug Info")]
        [ReadOnly]
        [PropertyOrder(-50)]
        public string guid = System.Guid.NewGuid().ToString();

#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 position;
        [HideInInspector]
        public GraphRunner runner;

        public delegate void NodeDelegate ();
        public event NodeDelegate onEnableChange;
        public void OnEnableChange ()
        {
            onEnableChange?.Invoke();
        }
        
        protected bool showAdvanceSettings;
        private void ShowAdvanceSettings()
        {
            showAdvanceSettings = !showAdvanceSettings;
        }
#endif

        public State Update (GraphExecution execution, int updateId)
        {
            if (!enabled)
                return OnDisabled(execution, updateId);

            bool started = execution.variables.GetStarted(guid, false);
            if (!started)
            {
                OnStart(execution, updateId);
                execution.variables.SetStarted(guid, true);
            }

            State state = OnUpdate(execution, updateId);

            if (state != State.Running)
            {
                OnStop(execution, updateId);
                execution.variables.SetStarted(guid, false);
            }

            return state;
        }
        
        public void Init ()
        {
            OnInit();
        }
        
        public void Reset ()
        {
            OnReset();
        }

        public void Pause (GraphExecution execution)
        {
            OnPause(execution);
        }
        
        public void Unpause (GraphExecution execution)
        {
            OnUnpause(execution);
        }

#if UNITY_EDITOR
        public virtual void OnDrawGizmos () { }
#endif

        protected abstract void OnInit ();
        protected abstract void OnReset ();
        protected abstract void OnPause (GraphExecution execution);
        protected abstract void OnUnpause (GraphExecution execution);
        protected abstract void OnStart (GraphExecution execution, int updateId);
        protected abstract void OnStop (GraphExecution execution, int updateId);
        protected abstract State OnDisabled (GraphExecution execution, int updateId);
        protected abstract State OnUpdate (GraphExecution execution, int updateId);
    }
}