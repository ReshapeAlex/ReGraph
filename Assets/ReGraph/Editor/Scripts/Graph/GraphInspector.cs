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
        [OnInspectorGUI("DisableGUIAfter")]
        [OnValueChanged("SelectRunner")]
        public GraphRunner runner;

        [MenuItem("Tools/Reshape/Graph Inspector")]
        public static void OpenWindow ()
        {
            var window = GetWindow<GraphInspector>();
            Selection.selectionChanged = window.OnSelectionChanged;
            window.Show();
        }

        private void OnSelectionChanged ()
        {
            if (Selection.objects.Length == 1)
            {
                if (Selection.activeGameObject)
                {
                    runner = Selection.activeGameObject.GetComponent<GraphRunner>();
                    if (runner != null)
                    {
                        runner.graph.InitPreviewNode();
                    }
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
        }

        private void SelectRunner ()
        {
            Selection.activeObject = runner;
        }
        
        private void OnInspectorUpdate()
        {
            if (Selection.selectionChanged != OnSelectionChanged)
            {
                Selection.selectionChanged = OnSelectionChanged;
            }
            Repaint();
        }
        
        private void DisableGUIAfter ()
        {
            GUI.enabled = false;
        }
    }
}