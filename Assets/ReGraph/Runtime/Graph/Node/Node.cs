using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    public abstract class Node
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        [HideInInspector]
        public string guid = System.Guid.NewGuid().ToString();
        [HideIf("@GetType().ToString().Contains(\"RootNode\")")]
        public bool enabled = true;

#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 position;
#endif

        public State Update (GraphExecution execution, int updateId)
        {
            if (!enabled)
                return OnDisabled(execution, updateId);
            
            bool started = execution.variables.GetStarted(guid, false);
            if (!started)
            {
                OnStart();
                execution.variables.SetStarted(guid, true);
            }

            State state = OnUpdate(execution, updateId);
            execution.variables.SetState(guid, state);

            if (state != State.Running)
            {
                OnStop();
                execution.variables.SetStarted(guid, false);
            }

            return state;
        }

        public virtual void Reset () { }

#if UNITY_EDITOR
        public virtual void OnDrawGizmos () { }
#endif

        protected abstract void OnStart ();
        protected abstract void OnStop ();
        protected abstract State OnDisabled (GraphExecution execution, int updateId);
        protected abstract State OnUpdate (GraphExecution execution, int updateId);
    }
}