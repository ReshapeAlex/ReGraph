using UnityEngine;

namespace Reshape.ReGraph
{
    public class GraphContext
    {
        public GameObject gameObject;
        public Transform transform;
        
        public GraphContext(GameObject gameObject)
        {
            this.gameObject = gameObject;
            CreateFromGameObject();
        }

        public void CreateFromGameObject()
        {
            transform = gameObject.transform;
        }
    }
}