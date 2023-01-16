using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Reshape.ReGraph
{
    [CustomEditor(typeof(GraphRunner))]
    public class GraphRunnerEditor : OdinEditor
    {
        private Graph graphCache;
        
        public override void OnInspectorGUI()
        {
            if (this.Tree != null && this.Tree.UnitySerializedObject != null)
            {
                if (graphCache == null)
                {
                    GraphRunner runner = this.Tree.UnitySerializedObject.targetObject as GraphRunner;
                    graphCache = runner.graph;
                }
                if (graphCache != null && !graphCache.HavePreviewNode())
                {
                    if (GUILayout.Button("Edit graph"))
                    {
                        GraphEditorWindow.OpenWindow(this.Tree.UnitySerializedObject);
                    }
                }
            }
            base.OnInspectorGUI();
        }
    }
}