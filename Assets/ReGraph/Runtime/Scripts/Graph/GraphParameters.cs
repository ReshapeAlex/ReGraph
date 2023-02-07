using System;
using UnityEngine;

namespace Reshape.ReGraph
{
    [Serializable]
    public class GraphParameters
    {
        public string actionName;
        public GameObject interactedGo;

        public GraphParameters ()
        {
            actionName = string.Empty;
        }
    }
}