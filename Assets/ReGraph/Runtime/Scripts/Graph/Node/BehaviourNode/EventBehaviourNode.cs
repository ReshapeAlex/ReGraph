using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class EventBehaviourNode : BehaviourNode
    {
        [PropertyOrder(2)]
        [OnValueChanged("MarkDirty")]
        public UnityEvent unityEvent;
        
        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (unityEvent == null || unityEvent.GetPersistentEventCount() <= 0)
                ReDebug.LogWarning("Graph Warning", "Found an empty Event Behaviour node in "+context.gameObject.name);
            else
                unityEvent?.Invoke();
            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        [PropertyOrder(1)]
        [OnValueChanged("MarkDirty")]
        [Tooltip("Only use in Graph Editor")]
        public string devNote;

        public static string displayName = "Event Behaviour Node";
        public static string nodeName = "Event";

        public override string GetNodeInspectorTitle()
        {
            return displayName;
        }

        public override string GetNodeViewTitle()
        {
            return nodeName;
        }
        
        public override string GetNodeViewDescription ()
        {
            if (unityEvent != null)
            {
                if (!string.IsNullOrEmpty(devNote))
                    return "Contain "+unityEvent.GetPersistentEventCount()+" unity events" + "\\n" + devNote;
                else
                    return "Contain "+unityEvent.GetPersistentEventCount()+" unity events";
            }
                
            if (!string.IsNullOrEmpty(devNote))
                return devNote;
            return string.Empty;
        }
#endif
    }
}