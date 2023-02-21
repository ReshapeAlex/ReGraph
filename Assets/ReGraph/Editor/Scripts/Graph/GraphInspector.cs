using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Reshape.ReGraph
{
    public class GraphInspector : OdinEditorWindow
    {
        [InlineEditor(Expanded = true)]
        [PropertyOrder(-3)]
        [OnInspectorGUI("DrawGenerateAllButton")]
        public GraphRunner runner;

        [MenuItem("Tools/Reshape/Open Graph Inspector")]
        public static void OpenWindow ()
        {
            var window = GetWindow<GraphInspector>();
            window.Show();
        }

        private void DrawGenerateAllButton ()
        {
            GUI.enabled = false;
        }
        
        private void OnInspectorUpdate()
        {
            if (Selection.objects.Length == 1)
            {
                if (Selection.activeGameObject)
                {
                    runner = Selection.activeGameObject.GetComponent<GraphRunner>();
                }
                else
                {
                    runner = null;
                }
            }
            else
            {
                runner = null;
            }
            Repaint();
        }
    }
}