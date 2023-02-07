using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [Serializable]
    public class GraphExecution
    {
        public Node.State state;
        
        [HideLabel, BoxGroup("Variables")]
        public GraphVariables variables;
        [HideLabel, BoxGroup("Parameters")]
        public GraphParameters parameters;

        private long executionId;
        private TriggerNode.Type triggerType;
        
        [ShowInInspector]
        [PropertyOrder(-1)]
        public TriggerNode.Type type => triggerType;

        public GraphExecution (long id, TriggerNode.Type type)
        {
            executionId = id;
            triggerType = type;
            variables = new GraphVariables();
            parameters = new GraphParameters();
            state = Node.State.Running;
        }
    }
}