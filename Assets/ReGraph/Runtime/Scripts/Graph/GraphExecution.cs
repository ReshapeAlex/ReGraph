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

        private long executionId;

        public GraphExecution (long id)
        {
            executionId = id;
            variables = new GraphVariables();
            state = Node.State.Running;
        }
    }
}