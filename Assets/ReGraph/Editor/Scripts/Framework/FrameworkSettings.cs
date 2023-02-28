using System.Collections.Generic;
using Reshape.Unity;
using Reshape.Unity.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Reshape.ReFramework
{
    [CreateAssetMenu(menuName = "Reshape/ReFramework Settings", order = 23)]
    public class FrameworkSettings : ScriptableObject
    {
        [LabelText("1st Person Controller")]
        public GameObject fpPlayerController;
        
        static FrameworkSettings FindSettings ()
        {
            var guids = AssetDatabase.FindAssets("t:FrameworkSettings");
            if (guids.Length > 1)
            {
                ReDebug.LogWarning("Framework Editor",$"Found multiple settings files, currently is using the first found settings file.", false);
            }

            switch (guids.Length)
            {
                case 0:
                    return null;
                default:
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath<FrameworkSettings>(path);
            }
        }

        internal static FrameworkSettings GetSettings ()
        {
            var settings = FindSettings();
            return settings;
        }

        internal static SerializedObject GetSerializedSettings ()
        {
            return new SerializedObject(GetSettings());
        }

        public static List<T> LoadAssets<T> () where T : UnityEngine.Object
        {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<T> assets = new List<T>();
            foreach (var assetId in assetIds)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetId);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                assets.Add(asset);
            }

            return assets;
        }

        public static List<string> GetAssetPaths<T> () where T : UnityEngine.Object
        {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<string> paths = new List<string>();
            foreach (var assetId in assetIds)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetId);
                paths.Add(path);
            }

            return paths;
        }
        
#if UNITY_EDITOR
        [MenuItem("GameObject/Reshape/1st Person Controller", false, 101)]
        public static void AddFpPlayerController()
        {
            if ( ReEditorHelper.IsInPrefabStage() )
            {
                ReDebug.LogWarning("Not able to do this action when you are editing a prefab!");
                return;
            }
            GameObject[] selected = Selection.gameObjects;
            if ( selected.Length > 0 )
            {
                ReDebug.LogWarning("Not able to do this action when you are selecting gameObject!");
                return;
            }
            
            var settings = FrameworkSettings.GetSettings();
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(settings.fpPlayerController);
            go.name = "First Person Controller";
            ReDebug.Log("Created First Person Controller GameObject!");
            EditorSceneManager.MarkAllScenesDirty();
        }
#endif
    }
}