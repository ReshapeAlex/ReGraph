using System.Collections;
using Reshape.Reframework;
using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class LabelBehaviourNode : BehaviourNode
    {
        public enum ExecutionType
        {
            None,
            StringToLabel = 10,
            VariableToLabel = 11,
            StringToTextMesh = 100,
            VariableToTextMesh = 101,
            StringToTextMeshPro = 200,
            VariableToTextMeshPro = 201
        }

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [LabelText("Execution")]
        [ValueDropdown("TypeChoice")]
        private ExecutionType executionType;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@executionType != ExecutionType.StringToLabel && executionType != ExecutionType.VariableToLabel")]
        [LabelText("Label")]
        private Text textLabel;

        [FormerlySerializedAs("textMeshlabel")]
        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@executionType != ExecutionType.StringToTextMesh && executionType != ExecutionType.VariableToTextMesh")]
        [LabelText("Label")]
        private TextMesh textMeshLabel;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@executionType != ExecutionType.StringToTextMeshPro && executionType != ExecutionType.VariableToTextMeshPro")]
        [LabelText("Label")]
        private TMP_Text textMeshProLabel;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideIf("@executionType != ExecutionType.VariableToLabel && executionType != ExecutionType.VariableToTextMesh && executionType != ExecutionType.VariableToTextMeshPro")]
        private StringVariable variable;

        [SerializeField]
        [TextArea]
        [OnValueChanged("MarkDirty")]
        [HideIf("@executionType != ExecutionType.StringToLabel && executionType != ExecutionType.StringToTextMesh && executionType != ExecutionType.StringToTextMeshPro")]
        [LabelText("String")]
        private string message;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            bool error = false;
            if (executionType is ExecutionType.StringToLabel or ExecutionType.VariableToLabel)
            {
                if (textLabel == null)
                    error = true;
            }
            else if (executionType is ExecutionType.StringToTextMesh or ExecutionType.VariableToTextMesh)
            {
                if (textMeshLabel == null)
                    error = true;
            }
            else if (executionType is ExecutionType.StringToTextMeshPro or ExecutionType.VariableToTextMeshPro)
            {
                if (textMeshProLabel == null)
                    error = true;
            }
            if (executionType is ExecutionType.StringToLabel or ExecutionType.StringToTextMesh or ExecutionType.StringToTextMeshPro)
            {
                if (message == null)
                    error = true;
            }
            else if (executionType is ExecutionType.VariableToLabel or ExecutionType.VariableToTextMesh or ExecutionType.VariableToTextMeshPro)
            {
                if (variable == null)
                    error = true;
            }

            if (error)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Label Behaviour node in " + context.gameObject.name);
            }
            else
            {
                string outputString = string.Empty;
                if (executionType is ExecutionType.StringToLabel or ExecutionType.StringToTextMesh or ExecutionType.StringToTextMeshPro)
                    outputString = message;
                else
                    outputString = variable;

                if (executionType is ExecutionType.StringToLabel or ExecutionType.VariableToLabel)
                    textLabel.text = outputString;
                else if (executionType is ExecutionType.StringToTextMesh or ExecutionType.VariableToTextMesh)
                    textMeshLabel.text = outputString;
                else if (executionType is ExecutionType.StringToTextMeshPro or ExecutionType.VariableToTextMeshPro)
                    textMeshProLabel.text = outputString;
            }

            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        private static IEnumerable TypeChoice = new ValueDropdownList<ExecutionType>()
        {
            {"String To Text", ExecutionType.StringToLabel},
            {"Variable To Text", ExecutionType.VariableToLabel},
            {"String To TextMesh", ExecutionType.StringToTextMesh},
            {"Variable To TextMesh", ExecutionType.VariableToTextMesh},
            {"String To TMPro", ExecutionType.StringToTextMeshPro},
            {"Variable To TMPro", ExecutionType.VariableToTextMeshPro}
        };

        public static string displayName = "Label Behaviour Node";
        public static string nodeName = "Label";

        public override string GetNodeInspectorTitle ()
        {
            return displayName;
        }

        public override string GetNodeViewTitle ()
        {
            return nodeName;
        }

        public override string GetNodeViewDescription ()
        {
            if (executionType is ExecutionType.None)
                return string.Empty;
            if (executionType is ExecutionType.StringToLabel or ExecutionType.VariableToLabel)
                if (textLabel == null)
                    return string.Empty;
            if (executionType is ExecutionType.StringToTextMesh or ExecutionType.VariableToTextMesh)
                if (textMeshLabel == null)
                    return string.Empty;
            if (executionType is ExecutionType.StringToTextMeshPro or ExecutionType.VariableToTextMeshPro)
                if (textMeshProLabel == null)
                    return string.Empty;

            string message = "Set variable to ";
            if (executionType is ExecutionType.StringToLabel or ExecutionType.StringToTextMesh or ExecutionType.StringToTextMeshPro)
                message = "Set string to ";
            if (executionType is ExecutionType.StringToLabel or ExecutionType.VariableToLabel)
                message += "Text";
            else if (executionType is ExecutionType.StringToTextMesh or ExecutionType.VariableToTextMesh)
                message += "Text Mesh";
            else if (executionType is ExecutionType.StringToTextMeshPro or ExecutionType.VariableToTextMeshPro)
                message += "TMPro";
            return message;
        }
#endif
    }
}