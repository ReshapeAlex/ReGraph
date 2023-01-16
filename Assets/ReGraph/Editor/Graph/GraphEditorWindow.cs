using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Reshape.ReGraph
{
    public class GraphEditorWindow : EditorWindow
    {
        SerializedGraph serializer;
        GraphSettings settings;

        GraphViewer treeView;

        /*WindowInspectorView inspectorView;
        WindowBlackboardView blackboardView;
        WindowOverlayView overlayView;*/
        ToolbarMenu toolbarMenu;
        Label titleLabel;

        [MenuItem("Tools/Reshape/Graph Editor")]
        public static void OpenWindow ()
        {
            GraphEditorWindow wnd = GetWindow<GraphEditorWindow>();
            wnd.titleContent = new GUIContent("GraphEditor");
            wnd.minSize = new Vector2(800, 600);
        }

        public static void OpenWindow (SerializedObject runnerObj)
        {
            GraphEditorWindow wnd = GetWindow<GraphEditorWindow>();
            wnd.titleContent = new GUIContent("Graph Editor");
            wnd.minSize = new Vector2(800, 600);
            wnd.SelectGraph(runnerObj);
        }

        public void CreateGUI ()
        {
            settings = GraphSettings.GetSettings();

            VisualElement root = rootVisualElement;
            var visualTree = settings.graphXml;
            visualTree.CloneTree(root);
            var styleSheet = settings.graphStyle;
            root.styleSheets.Add(styleSheet);

            treeView = root.Q<GraphViewer>();
            //inspectorView = root.Q<InspectorView>();
            //blackboardView = root.Q<BlackboardView>();
            //overlayView = root.Q<OverlayView>("OverlayView");
            toolbarMenu = root.Q<ToolbarMenu>();
            titleLabel = root.Q<Label>("TitleLabel");

            toolbarMenu.RegisterCallback<MouseEnterEvent>((evt) =>
            {
                toolbarMenu.menu.MenuItems().Clear();
                /*var dialogTrees = GraphSettings.GetAssetPaths<Graph>();
                dialogTrees.ForEach(path => {
                    var fileName = System.IO.Path.GetFileName(path);
                    toolbarMenu.menu.AppendAction($"{fileName}", (a) => {
                        var tree = AssetDatabase.LoadAssetAtPath<Graph>(path);
                        SelectTree(tree);
                    });
                });*/
                toolbarMenu.menu.AppendSeparator();
            });

            // Overlay view
            treeView.OnNodeSelected = OnNodeSelectionChanged;
            treeView.OnNodeUnselected = OnNodeUnselectionChanged;
            //overlayView.OnTreeSelected += SelectTree;
            Undo.undoRedoPerformed += OnUndoRedo;

            if (serializer == null)
            {
                //overlayView.Show();
            }
        }

        void OnUndoRedo ()
        {
            if (serializer != null)
            {
                treeView.PopulateView(serializer);
            }
        }

        private void OnSelectionChange ()
        {
            if (Selection.objects.Length == 1)
            {
                if (Selection.activeGameObject)
                {
                    GraphRunner runner = Selection.activeGameObject.GetComponent<GraphRunner>();
                    if (runner)
                    {
                        SelectGraph(new SerializedObject(runner));
                        return;
                    }
                }
            }
            ClearSelection();
        }

        void SelectGraph (SerializedObject runnerObj)
        {
            if (runnerObj == null)
            {
                ClearSelection();
                return;
            }

            serializer = new SerializedGraph(runnerObj);
            serializer.SetViewPreviewNode(null);
            
            if (titleLabel != null)
            {
                GraphRunner runner = runnerObj.targetObject as GraphRunner;
                titleLabel.text = $"Graph from GameObject {runner.gameObject.name} in {runner.gameObject.scene.path}";
            }

            //overlayView.Hide();
            if (treeView != null)
                treeView.PopulateView(serializer);
            //blackboardView.Bind(serializer);
        }

        void ClearSelection ()
        {
            if (serializer != null)
            {
                serializer.SetViewPreviewNode(null);
                serializer.SetViewPreviewSelected(false);
                serializer = null;
            }
            //overlayView.Show();
            if (treeView != null)
                treeView.ClearView();
            //inspectorView.ClearSelection(serializer);
        }

        void OnNodeSelectionChanged (GraphNodeView node)
        {
            if (serializer != null && serializer.graph != null && node != null)
            {
                serializer.SetViewPreviewNode(node.node);
                serializer.SetViewPreviewSelected(true);
                //RepaintInspector("UnityEditor.GameObjectInspector");
            }
            //inspectorView.UpdateSelection(serializer, node);
        }

        void OnNodeUnselectionChanged (GraphNodeView node)
        {
            if (serializer != null)
            {
                serializer.SetViewPreviewSelected(false);
                //RepaintInspector("UnityEditor.GameObjectInspector");
            }
        }
        
        private void OnInspectorUpdate()
        {
            if (Selection.objects.Length == 1)
            {
                if (Selection.activeGameObject)
                {
                    GraphRunner runner = Selection.activeGameObject.GetComponent<GraphRunner>();
                    if (runner != null)
                    {
                        if (serializer == null || runner.graph != serializer.graph)
                        {
                            SelectGraph(new SerializedObject(runner));
                        }
                    }
                }
            }
            treeView?.UpdateNodeStates();
        }

        private void OnEnable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj) {
            switch (obj) {
                case PlayModeStateChange.EnteredEditMode:
                    EditorApplication.delayCall += OnSelectionChange;
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    EditorApplication.delayCall += OnSelectionChange;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void RepaintInspector (string typeName)
        {
            Editor[] ed = (Editor[]) Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; i++)
            {
                if (ed[i].GetType().ToString() == typeName)
                {
                    ed[i].Repaint();
                    return;
                }
            }
        }
    }
}