using System;
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
            context = new GraphContext(this);
            graph?.Bind(context);
        }

        private void Update ()
        {
            graph?.Update(Time.frameCount);
        }

#if UNITY_EDITOR
        [Button]
        [ShowIf("IsApplicationPlaying")]
        private void Execute ()
        {
            graph?.Execute(DateTime.UtcNow.Millisecond, Time.frameCount);
        }

        private bool IsApplicationPlaying ()
        {
            return Application.isPlaying;
        }

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