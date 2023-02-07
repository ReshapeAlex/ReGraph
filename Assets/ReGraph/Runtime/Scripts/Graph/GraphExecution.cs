using System;
using Sirenix.OdinInspector;

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

        public GraphExecution (long id)
        {
            executionId = id;
            variables = new GraphVariables();
            parameters = new GraphParameters();
            state = Node.State.Running;
        }
    }
}