using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Reshape.Unity;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Reshape.ReGraph
{
    [Serializable]
    public class ActionInfo
    {
        public ActionData[] actions;
    }
}