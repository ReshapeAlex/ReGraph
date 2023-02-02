using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Reshape.ReGraph
{
    [CustomEditor(typeof(GraphRunner))]
    public class GraphRunnerEditor : OdinEditor
    {
        private Graph graphCache;

        public override void OnInspectorGUI ()
        {
            if (!Application.isPlaying)
            {
                if (Tree != null && Tree.UnitySerializedObject != null)
                {
                    if (graphCache == null)
                    {
                        GraphRunner runner = Tree.UnitySerializedObject.targetObject as GraphRunner;
                        if (runner != null)
                            graphCache = runner.graph;
                    }

                    if (graphCache != null)
                    {
                        if (!graphCache.HavePreviewNode() && graphCache.Created)
                        {

                            EditorGUILayout.HelpBox("All graph data in saved in the scene file.", MessageType.Info);
                            if (GUILayout.Button("Edit Graph"))
                                GraphEditorWindow.OpenWindow(this.Tree.UnitySerializedObject);
                            base.OnInspectorGUI();
                            return;
                        }
                    }
                }
            }

            base.OnInspectorGUI();
        }
    }
}