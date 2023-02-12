using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reshape.Reframework
{
    public class VariableScriptableObject : ScriptableObject
    {
        public delegate void CommonDelegate();
        [PropertyOrder(100)]
        public bool persistent;
        
        [HideInInspector]
        public event CommonDelegate onEarlyChange;
        //[HideInInspector]
        public event CommonDelegate onChange;
        //[HideInInspector]
        public event CommonDelegate onReset;

        private bool linked;
        
        public virtual void OnEarlyChanged()
        {
            onEarlyChange?.Invoke();
        }

        public virtual void OnChanged()
        {
            if (!linked && !persistent)
            {
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
                SceneManager.sceneUnloaded += OnSceneUnloaded;
            }
            linked = true;
            onChange?.Invoke();
        }

        public void OnSceneUnloaded(Scene scene)
        {
            onReset?.Invoke();
        }
    }
}