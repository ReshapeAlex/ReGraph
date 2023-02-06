using System;
using System.Collections.Generic;

namespace Reshape.ReGraph
{
    [Serializable]
    public class GraphVariables
    {
        public Dictionary<string, Node.State> state;
        public Dictionary<string, bool> started;
        public Dictionary<string, int> intList;

        public GraphVariables ()
        {
            state = new Dictionary<string, Node.State>();
            started = new Dictionary<string, bool>();
            intList = new Dictionary<string, int>();
        }

        public Node.State GetState (string nodeId, Node.State defaultValue)
        {
            if (state.TryGetValue(nodeId, out Node.State outState))
                return outState;
            return defaultValue;
        }
        
        public void SetState (string nodeId, Node.State value)
        {
            if (!state.TryAdd(nodeId, value))
                state[nodeId] = value;
        }
        
        public bool GetStarted (string nodeId, bool defaultValue)
        {
            if (started.TryGetValue(nodeId, out bool outStarted))
                return outStarted;
            return defaultValue;
        }
        
        public void SetStarted (string nodeId, bool value)
        {
            if (!started.TryAdd(nodeId, value))
                started[nodeId] = value;
        }
        
        public int GetInt (string nodeId, int defaultValue = 0)
        {
            if (intList.TryGetValue(nodeId, out int outInt))
                return outInt;
            return defaultValue;
        }
        
        public void SetInt (string nodeId, int value)
        {
            if (!intList.TryAdd(nodeId, value))
                intList[nodeId] = value;
        }
    }
}