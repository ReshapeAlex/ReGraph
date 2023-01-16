using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [HideMonoScript]
    [DisallowMultipleComponent]
    public class GraphRunner : MonoBehaviour
    {
        [HideLabel]
        public Graph graph;

        private GraphContext context;
    
        private void Start ()
        {
            context = new GraphContext(gameObject);
            graph?.Bind(context);
        }

        private void Update ()
        {
            graph?.Update(Time.frameCount);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected ()
        {
            if (graph == null)
                return;
            Graph.Traverse(graph.RootNode, (n) =>
            {
                if (n.drawGizmos)
                    n.OnDrawGizmos();
            });
        }
#endif
    }
}