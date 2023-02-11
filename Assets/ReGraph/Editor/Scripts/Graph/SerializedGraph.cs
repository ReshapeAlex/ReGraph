using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Reshape.ReGraph
{
    public class SerializedGraph
    {
        readonly public SerializedObject serializedObject;
        readonly public GraphRunner runner;
        readonly public Graph graph;

        const string sPropRootNode = "graph.rootNode";
        const string sPropNodes = "graph.nodes";
        const string sPropBlackboard = "blackboard";
        const string sPropGuid = "guid";
        const string sPropChild = "child";
        const string sPropChildren = "children";
        const string sPropPosition = "position";
        const string sViewTransformPosition = "graph.viewPosition";
        const string sViewTransformScale = "graph.viewScale";

        private SerializedProperty nodes;

        public SerializedProperty RootNode
        {
            get { return serializedObject.FindProperty(sPropRootNode); }
        }

        public SerializedProperty Nodes
        {
            get
            {
                if (nodes == null)
                {
                    SerializedProperty graph = serializedObject.FindProperty(sPropNodes);
                    nodes = serializedObject.FindProperty(sPropNodes);
                }

                return nodes;
            }
        }

        public SerializedProperty Blackboard
        {
            get { return serializedObject.FindProperty(sPropBlackboard); }
        }

        // Start is called before the first frame update
        public SerializedGraph (SerializedObject tree)
        {
            serializedObject = tree;
            runner = serializedObject.targetObject as GraphRunner;
            graph = runner.graph;
            nodes = null;
        }

        public void Save ()
        {
            serializedObject.ApplyModifiedProperties();
        }

        public SerializedProperty FindNode (SerializedProperty array, GraphNode node)
        {
            if (node == null) return null;
            if (array.serializedObject == null) return null;
            for (int i = 0; i < array.arraySize; ++i)
            {
                var current = array.GetArrayElementAtIndex(i);
                if (current.managedReferenceValue != null && current.FindPropertyRelative(sPropGuid).stringValue == node.guid)
                    return current;
            }

            return null;
        }

        public void SetViewTransform (Vector3 position, Vector3 scale)
        {
            serializedObject.FindProperty(sViewTransformPosition).vector3Value = position;
            serializedObject.FindProperty(sViewTransformScale).vector3Value = scale;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void SetNodePosition (GraphNode node, Vector2 position)
        {
            var nodeProp = FindNode(Nodes, node);
            if (nodeProp != null)
            {
                Vector2 ori = nodeProp.FindPropertyRelative(sPropPosition).vector2Value;
                if (Vector2.Distance(ori, position) > 2f)
                {
                    nodeProp.FindPropertyRelative(sPropPosition).vector2Value = position;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public void SetViewPreviewNode (GraphNode node)
        {
            graph.previewNode = node;
            //serializedObject.FindProperty("graph.previewNode").managedReferenceValue = node;
            //serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void SetViewPreviewSelected (bool selected)
        {
            graph.previewSelected = selected;
            //serializedObject.FindProperty("graph.previewSelected").boolValue = selected;
            //serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void DeleteNode (SerializedProperty array, GraphNode node)
        {
            if (node == null)
            {
                for (int i = 0; i < array.arraySize; ++i)
                {
                    var current = array.GetArrayElementAtIndex(i);
                    if (current.managedReferenceValue == null)
                    {
                        array.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }

                return;
            }

            for (int i = 0; i < array.arraySize; ++i)
            {
                var current = array.GetArrayElementAtIndex(i);
                if (current.managedReferenceValue != null)
                {
                    if (current.FindPropertyRelative(sPropGuid).stringValue == node.guid)
                    {
                        array.DeleteArrayElementAtIndex(i);
                        return;
                    }
                }
            }
        }

        public GraphNode CreateNodeInstance (System.Type type)
        {
            GraphNode node = System.Activator.CreateInstance(type) as GraphNode;
            node.guid = GUID.Generate().ToString();
            return node;
        }

        SerializedProperty AppendArrayElement (SerializedProperty arrayProperty)
        {
            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            return arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
        }

        public GraphNode CreateNode (System.Type type, Vector2 position)
        {
            GraphNode node = CreateNodeInstance(type);
            node.position = position;
            node.runner = runner;

            SerializedProperty newNode = AppendArrayElement(Nodes);
            newNode.managedReferenceValue = node;

            serializedObject.ApplyModifiedProperties();

            return node;
        }

        public void SetRootNode (RootNode node)
        {
            RootNode.managedReferenceValue = node;
            serializedObject.ApplyModifiedProperties();
        }

        public void DeleteNode (GraphNode node)
        {
            /*SerializedProperty nodesProperty = Nodes;
            for (int i = 0; i < nodesProperty.arraySize; ++i)
            {
                var prop = nodesProperty.GetArrayElementAtIndex(i);
                var guid = prop.FindPropertyRelative(sPropGuid).stringValue;*/
                DeleteNode(Nodes, node);
                serializedObject.ApplyModifiedProperties();
            /*}*/
        }

        public void SortChildren (GraphNode node, List<GraphNode> sorted)
        {
            var nodeProperty = FindNode(Nodes, node);
            for (var i = 0; i < sorted.Count; i++)
            {
                if (sorted[i] == null)
                    continue;
                var childrenProperty = nodeProperty.FindPropertyRelative(sPropChildren);
                if (childrenProperty != null)
                {
                    for (var j = 0; j < childrenProperty.arraySize; ++j)
                    {
                        var current = childrenProperty.GetArrayElementAtIndex(j);
                        if (current.managedReferenceValue != null)
                        {
                            if (current.FindPropertyRelative(sPropGuid).stringValue == sorted[i].guid)
                            {
                                childrenProperty.MoveArrayElement(j, i);
                                serializedObject.ApplyModifiedProperties();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void AddChild (GraphNode parent, GraphNode child)
        {
            var parentProperty = FindNode(Nodes, parent);
            var childrenProperty = parentProperty.FindPropertyRelative(sPropChildren);
            if (childrenProperty != null)
            {
                //-- clean up null in children property
                DeleteNode(childrenProperty, null);
                
                SerializedProperty newChild = AppendArrayElement(childrenProperty);
                newChild.managedReferenceValue = child;
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void RemoveChild (GraphNode parent, GraphNode child)
        {
            var parentProperty = FindNode(Nodes, parent);
            var childrenProperty = parentProperty.FindPropertyRelative(sPropChildren);
            if (childrenProperty != null)
            {
                DeleteNode(childrenProperty, child);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}