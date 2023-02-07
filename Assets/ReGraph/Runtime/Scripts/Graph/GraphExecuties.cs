using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reshape.ReGraph
{
    [Serializable]
    public class GraphExecutes
    {
        [SerializeField]
        private List<GraphExecution> executionList;

        public int Count
        {
            get
            {
                if (executionList == null)
                    return 0;
                return executionList.Count;
            }
        }
        
        public GraphExecution Add (long id, TriggerNode.Type triggerType)
        {
            if (executionList == null)
                executionList = new List<GraphExecution>();
            var execution = new GraphExecution(id, triggerType);
            executionList.Add(execution);
            return execution;
        }

        public GraphExecution Get (int index)
        {
            if (executionList == null)
                return null;
            if (index >= executionList.Count)
                return null;
            return executionList[index];
        }

        public void Clear ()
        {
            executionList.Clear();
        }
    }
}