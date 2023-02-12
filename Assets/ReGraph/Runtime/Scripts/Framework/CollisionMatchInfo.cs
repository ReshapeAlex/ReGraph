using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Reshape.Reframework
{
    [System.Serializable]
    public class CollisionMatchInfo
    {
        public string[] specificNames;
        [ValueDropdown("DrawTagDropdown", ExpandAllMenuItems = true)]
        public string[] excludeTags;
        [ValueDropdown("DrawLayerDropdown", ExpandAllMenuItems = true)]
        public string[] excludeLayers;
        [ValueDropdown("DrawTagDropdown", ExpandAllMenuItems = true)]
        public string[] onlyTags;
        [ValueDropdown("DrawLayerDropdown", ExpandAllMenuItems = true)]
        public string[] onlyLayers;
        [HideInInspector]
        public bool inOutDetection;
        [ShowIfGroup("inOutDetection")]
        [FoldoutGroup("inOutDetection/In Out Direction")]
        public bool leftToRight = true;
        [FoldoutGroup("inOutDetection/In Out Direction")]
        public bool rightToLeft = true;
        [FoldoutGroup("inOutDetection/In Out Direction")]
        public bool topToBottom = true;
        [FoldoutGroup("inOutDetection/In Out Direction")]
        public bool bottomToTop = true;
        [HideInInspector]
        public bool uniqueDetection;
        
#if UNITY_EDITOR
        private static IEnumerable DrawTagDropdown()
        {
            ValueDropdownList<string> tagListDropdown = new ValueDropdownList<string>();
            
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                tagListDropdown.Add(UnityEditorInternal.InternalEditorUtility.tags[i], UnityEditorInternal.InternalEditorUtility.tags[i]);
            }
            return tagListDropdown;
        }
        
        private static IEnumerable DrawLayerDropdown()
        {
            ValueDropdownList<string> layerListDropdown = new ValueDropdownList<string>();
            
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
            {
                layerListDropdown.Add(UnityEditorInternal.InternalEditorUtility.layers[i], UnityEditorInternal.InternalEditorUtility.layers[i]);
            }
            return layerListDropdown;
        }
#endif
    }
}