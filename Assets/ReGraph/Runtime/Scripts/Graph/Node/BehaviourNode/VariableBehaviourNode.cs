using System.Collections;
using Reshape.ReFramework;
using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class VariableBehaviourNode : BehaviourNode
    {
        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [HideLabel]
        private VariableBehaviourInfo variableBehaviour;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (variableBehaviour.type == VariableBehaviourInfo.Type.None || variableBehaviour.variable == null)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Variable Behaviour node in " + context.gameObject.name);
            }
            else
            {
                bool result = variableBehaviour.Activate();
                for (var i = 0; i < children.Count; ++i)
                {
                    if (children[i] is ConditionNode)
                    {
                        var child = children[i] as ConditionNode;
                        child.MarkExecute(execution, updateId, result);
                    }
                }
            }

            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        public static string displayName = "Variable Behaviour Node";
        public static string nodeName = "Variable";

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
            if (variableBehaviour.type != VariableBehaviourInfo.Type.None && variableBehaviour.variable != null)
            {
                return variableBehaviour.variable.name + " being " + variableBehaviour.type.ToString().SplitCamelCase().ToLower();
            }
            return string.Empty;
        }
#endif
    }

    [System.Serializable]
    public struct VariableBehaviourInfo
    {
        public enum Type
        {
            None = 0,
            SetValue = 1,
            AddValue = 2,
            MinusValue = 3,
            MultiplyValue = 4,
            DivideValue = 5,
            RandomValue = 6,
            CheckCondition = 20
        }

        public enum Condition
        {
            None = 0,
            Equal = 1,
            NotEqual = 2,
            LessThan = 31,
            MoreThan = 32,
            LessThanAndEqual = 33,
            MoreThanAndEqual = 34
        }

        public FloatVariable variable;

        [ValueDropdown("TypeChoice")]
        [OnInspectorGUI("ShowTip")]
        public Type type;

        [ShowIf("@type==Type.CheckCondition")]
        public Condition condition;

        [LabelText("Value")]
        [ShowIf("@type!=Type.RandomValue")]
        public float number;

        public bool Activate ()
        {
            if (variable == null)
                return false;
            if (type == Type.SetValue)
            {
                variable.SetValue(number);
            }
            else if (type == Type.AddValue)
            {
                variable.AddValue(number);
            }
            else if (type == Type.MinusValue)
            {
                variable.MinusValue(number);
            }
            else if (type == Type.DivideValue)
            {
                variable.DivideValue(number);
            }
            else if (type == Type.MultiplyValue)
            {
                variable.MultiplyValue(number);
            }
            else if (type == Type.RandomValue)
            {
                variable.RandomValue();
            }
            else if (type == Type.CheckCondition)
            {
                if (condition == Condition.Equal || condition == Condition.LessThanAndEqual || condition == Condition.MoreThanAndEqual)
                {
                    if (variable.IsEqual(number))
                    {
                        return true;
                    }
                }

                if (condition == Condition.NotEqual)
                {
                    if (!variable.IsEqual(number))
                    {
                        return true;
                    }
                }

                if (condition == Condition.LessThan || condition == Condition.LessThanAndEqual)
                {
                    if (variable < number)
                    {
                        return true;
                    }
                }

                if (condition == Condition.MoreThan || condition == Condition.MoreThanAndEqual)
                {
                    if (variable > number)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

#if UNITY_EDITOR
        private static IEnumerable TypeChoice = new ValueDropdownList<Type>()
        {
            {"Set Value", Type.SetValue},
            {"Add Value", Type.AddValue},
            {"Minus Value", Type.MinusValue},
            {"Multiply Value", Type.MultiplyValue},
            {"Divide Value", Type.DivideValue},
            {"Random Value", Type.RandomValue},
            {"Check Condition", Type.CheckCondition}
        };

        private void ShowTip ()
        {
            if (type == Type.RandomValue)
            {
                EditorGUILayout.HelpBox("Random between 1 to 100", MessageType.Info);
            }
        }
#endif
    }
}