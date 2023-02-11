using System;
using Reshape.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Reshape.ReGraph
{
    [System.Serializable]
    public class TransformBehaviourNode : BehaviourNode
    {
        [Serializable]
        public struct VectorElement
        {
            [HideLabel]
            [HorizontalGroup(width: 10)]
            public bool enabled;

            [HideLabel]
            [HorizontalGroup]
            [DisableIf("DisableValue")]
            public float value;

            [HideLabel]
            [HorizontalGroup]
            [DisableIf("DisableTransform")]
            public Transform transform;

            private bool DisableValue ()
            {
                if (!enabled)
                    return true;
                if (transform != null)
                    return true;
                return false;
            }

            private bool DisableTransform ()
            {
                if (!enabled)
                    return true;
                return false;
            }
        }

        public enum ExecutionType
        {
            None,
            SetGlobalPosition = 10,
            SetLocalPosition = 11,
            AddGlobalPosition = 12,
            AddLocalPosition = 13,
            SetGlobalRotation = 50,
            SetLocalRotation = 51,
            AddGlobalRotation = 52,
            AddLocalRotation = 53,
            SetGlobalScale = 90,
            SetLocalScale = 91,
            AddGlobalScale = 92,
            AddLocalScale = 93
        }

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        private Transform transform;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [LabelText("Execution")]
        private ExecutionType executionType;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [BoxGroup("X", LabelText = "@XLabel()")]
        [HideLabel]
        [OnInspectorGUI("UpdateValueX")]
        [HideIf("@executionType==ExecutionType.None")]
        private VectorElement x;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [BoxGroup("Y", LabelText = "@YLabel()")]
        [HideLabel]
        [OnInspectorGUI("UpdateValueY")]
        [HideIf("@executionType==ExecutionType.None")]
        private VectorElement y;

        [SerializeField]
        [OnValueChanged("MarkDirty")]
        [BoxGroup("Z", LabelText = "@ZLabel()")]
        [HideLabel]
        [OnInspectorGUI("UpdateValueZ")]
        [HideIf("@executionType==ExecutionType.None")]
        private VectorElement z;

        protected override void OnStart (GraphExecution execution, int updateId)
        {
            if (transform == null || executionType == ExecutionType.None)
            {
                ReDebug.LogWarning("Graph Warning", "Found an empty Transform Behaviour node in " + context.gameObject.name);
            }
            else
            {
                if (executionType is ExecutionType.SetGlobalPosition or ExecutionType.AddGlobalPosition)
                {
                    if (x.enabled && y.enabled && z.enabled)
                    {
                        Vector3 value = Vector3.one;
                        value.x = x.transform == null ? x.value : x.transform.position.x;
                        value.y = y.transform == null ? y.value : y.transform.position.y;
                        value.z = z.transform == null ? z.value : z.transform.position.z;
                        if (executionType is ExecutionType.SetGlobalPosition)
                            transform.SetPosition(value);
                        else if (executionType is ExecutionType.AddGlobalPosition)
                            transform.SetPosition(transform.position + value);
                    }
                    else
                    {
                        if (x.enabled)
                        {
                            var xValue = x.transform == null ? x.value : x.transform.position.x;
                            if (executionType is ExecutionType.SetGlobalPosition)
                                transform.SetPositionX(xValue);
                            else if (executionType is ExecutionType.AddGlobalPosition)
                                transform.SetPositionX(transform.position.x + xValue);
                        }

                        if (y.enabled)
                        {
                            var yValue = y.transform == null ? y.value : y.transform.position.y;
                            if (executionType is ExecutionType.SetGlobalPosition)
                                transform.SetPositionY(yValue);
                            else if (executionType is ExecutionType.AddGlobalPosition)
                                transform.SetPositionY(transform.position.y + yValue);
                        }

                        if (z.enabled)
                        {
                            var zValue = z.transform == null ? z.value : z.transform.position.z;
                            if (executionType is ExecutionType.SetGlobalPosition)
                                transform.SetPositionZ(zValue);
                            else if (executionType is ExecutionType.AddGlobalPosition)
                                transform.SetPositionZ(transform.position.z + zValue);
                        }
                    }
                }
                else if (executionType is ExecutionType.SetLocalPosition or ExecutionType.AddLocalPosition)
                {
                    if (x.enabled && y.enabled && z.enabled)
                    {
                        Vector3 value = Vector3.one;
                        value.x = x.transform == null ? x.value : x.transform.localPosition.x;
                        value.y = y.transform == null ? y.value : y.transform.localPosition.y;
                        value.z = z.transform == null ? z.value : z.transform.localPosition.z;
                        if (executionType is ExecutionType.SetLocalPosition)
                            transform.SetLocalPosition(value);
                        else if (executionType is ExecutionType.AddLocalPosition)
                            transform.SetLocalPosition(transform.localPosition + value);
                    }
                    else
                    {
                        if (x.enabled)
                        {
                            var xValue = x.transform == null ? x.value : x.transform.localPosition.x;
                            if (executionType is ExecutionType.SetLocalPosition)
                                transform.SetLocalPositionX(xValue);
                            else if (executionType is ExecutionType.AddLocalPosition)
                                transform.SetLocalPositionX(transform.localPosition.x + xValue);
                        }

                        if (y.enabled)
                        {
                            var yValue = y.transform == null ? y.value : y.transform.localPosition.y;
                            if (executionType is ExecutionType.SetLocalPosition)
                                transform.SetLocalPositionY(yValue);
                            else if (executionType is ExecutionType.AddLocalPosition)
                                transform.SetLocalPositionY(transform.localPosition.y + yValue);
                        }

                        if (z.enabled)
                        {
                            var zValue = z.transform == null ? z.value : z.transform.localPosition.z;
                            if (executionType is ExecutionType.SetLocalPosition)
                                transform.SetLocalPositionZ(zValue);
                            else if (executionType is ExecutionType.AddLocalPosition)
                                transform.SetLocalPositionZ(transform.localPosition.z + zValue);
                        }
                    }
                }
                else if (executionType is ExecutionType.SetGlobalRotation or ExecutionType.AddGlobalRotation)
                {
                    if (x.enabled && y.enabled && z.enabled)
                    {
                        Vector3 value = Vector3.one;
                        value.x = x.transform == null ? x.value : x.transform.eulerAngles.x;
                        value.y = y.transform == null ? y.value : y.transform.eulerAngles.y;
                        value.z = z.transform == null ? z.value : z.transform.eulerAngles.z;
                        if (executionType is ExecutionType.SetGlobalRotation)
                            transform.SetRotation(value);
                        else if (executionType is ExecutionType.AddGlobalRotation)
                            transform.SetRotation(transform.eulerAngles + value);
                    }
                    else
                    {
                        if (x.enabled)
                        {
                            var xValue = x.transform == null ? x.value : x.transform.eulerAngles.x;
                            if (executionType is ExecutionType.SetGlobalRotation)
                                transform.SetRotationX(xValue);
                            else if (executionType is ExecutionType.AddGlobalRotation)
                                transform.SetRotationX(transform.eulerAngles.x + xValue);
                        }

                        if (y.enabled)
                        {
                            var yValue = y.transform == null ? y.value : y.transform.eulerAngles.y;
                            if (executionType is ExecutionType.SetGlobalRotation)
                                transform.SetRotationY(yValue);
                            else if (executionType is ExecutionType.AddGlobalRotation)
                                transform.SetRotationY(transform.eulerAngles.y + yValue);
                        }

                        if (z.enabled)
                        {
                            var zValue = z.transform == null ? z.value : z.transform.eulerAngles.z;
                            if (executionType is ExecutionType.SetGlobalRotation)
                                transform.SetRotationZ(zValue);
                            else if (executionType is ExecutionType.AddGlobalRotation)
                                transform.SetRotationZ(transform.eulerAngles.z + zValue);
                        }
                    }
                }
                else if (executionType is ExecutionType.SetLocalRotation or ExecutionType.AddLocalRotation)
                {
                    if (x.enabled && y.enabled && z.enabled)
                    {
                        Vector3 value = Vector3.one;
                        value.x = x.transform == null ? x.value : x.transform.localEulerAngles.x;
                        value.y = y.transform == null ? y.value : y.transform.localEulerAngles.y;
                        value.z = z.transform == null ? z.value : z.transform.localEulerAngles.z;
                        if (executionType is ExecutionType.SetLocalRotation)
                            transform.SetLocalRotation(value);
                        else if (executionType is ExecutionType.AddLocalRotation)
                            transform.SetLocalRotation(transform.localEulerAngles + value);
                    }
                    else
                    {
                        if (x.enabled)
                        {
                            var xValue = x.transform == null ? x.value : x.transform.localEulerAngles.x;
                            if (executionType is ExecutionType.SetLocalRotation)
                                transform.SetLocalRotationX(xValue);
                            else if (executionType is ExecutionType.AddLocalRotation)
                                transform.SetLocalRotationX(transform.localEulerAngles.x + xValue);
                        }

                        if (y.enabled)
                        {
                            var yValue = y.transform == null ? y.value : y.transform.localEulerAngles.y;
                            if (executionType is ExecutionType.SetLocalRotation)
                                transform.SetLocalRotationY(yValue);
                            else if (executionType is ExecutionType.AddLocalRotation)
                                transform.SetLocalRotationY(transform.localEulerAngles.y + yValue);
                        }

                        if (z.enabled)
                        {
                            var zValue = z.transform == null ? z.value : z.transform.localEulerAngles.z;
                            if (executionType is ExecutionType.SetLocalRotation)
                                transform.SetLocalRotationZ(zValue);
                            else if (executionType is ExecutionType.AddLocalRotation)
                                transform.SetLocalRotationZ(transform.localEulerAngles.z + zValue);
                        }
                    }
                }
                else if (executionType is ExecutionType.SetGlobalScale or ExecutionType.AddGlobalScale)
                {
                    if (x.enabled && y.enabled && z.enabled)
                    {
                        Vector3 value = Vector3.one;
                        value.x = x.transform == null ? x.value : x.transform.lossyScale.x;
                        value.y = y.transform == null ? y.value : y.transform.lossyScale.y;
                        value.z = z.transform == null ? z.value : z.transform.lossyScale.z;
                        if (executionType is ExecutionType.SetGlobalScale)
                            transform.SetScale(value);
                        else if (executionType is ExecutionType.AddGlobalScale)
                            transform.SetScale(transform.lossyScale + value);
                    }
                    else
                    {
                        if (x.enabled)
                        {
                            var xValue = x.transform == null ? x.value : x.transform.lossyScale.x;
                            if (executionType is ExecutionType.SetGlobalScale)
                                transform.SetScaleX(xValue);
                            else if (executionType is ExecutionType.AddGlobalScale)
                                transform.SetScaleX(transform.lossyScale.x + xValue);
                        }

                        if (y.enabled)
                        {
                            var yValue = y.transform == null ? y.value : y.transform.lossyScale.y;
                            if (executionType is ExecutionType.SetGlobalScale)
                                transform.SetScaleY(yValue);
                            else if (executionType is ExecutionType.AddGlobalScale)
                                transform.SetScaleY(transform.lossyScale.y + yValue);
                        }

                        if (z.enabled)
                        {
                            var zValue = z.transform == null ? z.value : z.transform.lossyScale.z;
                            if (executionType is ExecutionType.SetGlobalScale)
                                transform.SetScaleZ(zValue);
                            else if (executionType is ExecutionType.AddGlobalScale)
                                transform.SetScaleZ(transform.lossyScale.y + zValue);
                        }
                    }
                }
                else if (executionType is ExecutionType.SetLocalScale or ExecutionType.AddLocalScale)
                {
                    if (x.enabled && y.enabled && z.enabled)
                    {
                        Vector3 value = Vector3.one;
                        value.x = x.transform == null ? x.value : x.transform.localScale.x;
                        value.y = y.transform == null ? y.value : y.transform.localScale.y;
                        value.z = z.transform == null ? z.value : z.transform.localScale.z;
                        if (executionType is ExecutionType.SetLocalScale)
                            transform.SetLocalScale(value);
                        else if (executionType is ExecutionType.AddLocalScale)
                            transform.SetLocalScale(transform.localScale + value);
                    }
                    else
                    {
                        if (x.enabled)
                        {
                            var xValue = x.transform == null ? x.value : x.transform.localScale.x;
                            if (executionType is ExecutionType.SetLocalScale)
                                transform.SetLocalScaleX(xValue);
                            else if (executionType is ExecutionType.AddLocalScale)
                                transform.SetLocalScaleX(transform.localScale.x + xValue);
                        }

                        if (y.enabled)
                        {
                            var yValue = y.transform == null ? y.value : y.transform.localScale.y;
                            if (executionType is ExecutionType.SetLocalScale)
                                transform.SetLocalScaleY(yValue);
                            else if (executionType is ExecutionType.AddLocalScale)
                                transform.SetLocalScaleY(transform.localScale.y + yValue);
                        }

                        if (z.enabled)
                        {
                            var zValue = z.transform == null ? z.value : z.transform.localScale.z;
                            if (executionType is ExecutionType.SetLocalScale)
                                transform.SetLocalScaleZ(zValue);
                            else if (executionType is ExecutionType.AddLocalScale)
                                transform.SetLocalScaleZ(transform.localScale.z + zValue);
                        }
                    }
                }
            }

            base.OnStart(execution, updateId);
        }

#if UNITY_EDITOR
        private void UpdateValueX ()
        {
            if (x.enabled)
            {
                if (x.transform != null)
                {
                    var value = 0f;
                    if (executionType == ExecutionType.SetGlobalPosition)
                    {
                        value = x.transform.position.x;
                    }
                    else if (executionType == ExecutionType.SetLocalPosition)
                    {
                        value = x.transform.localPosition.x;
                    }
                    else if (executionType == ExecutionType.SetGlobalRotation)
                    {
                        value = x.transform.eulerAngles.x;
                    }
                    else if (executionType == ExecutionType.SetLocalRotation)
                    {
                        value = x.transform.localEulerAngles.x;
                    }
                    else if (executionType == ExecutionType.SetGlobalScale)
                    {
                        value = x.transform.lossyScale.x;
                    }
                    else if (executionType == ExecutionType.SetLocalScale)
                    {
                        value = x.transform.localScale.x;
                    }

                    x.value = value;
                }
            }
        }

        private void UpdateValueY ()
        {
            if (y.enabled)
            {
                if (y.transform != null)
                {
                    var value = 0f;
                    if (executionType == ExecutionType.SetGlobalPosition)
                    {
                        value = y.transform.position.y;
                    }
                    else if (executionType == ExecutionType.SetLocalPosition)
                    {
                        value = y.transform.localPosition.y;
                    }
                    else if (executionType == ExecutionType.SetGlobalRotation)
                    {
                        value = y.transform.eulerAngles.y;
                    }
                    else if (executionType == ExecutionType.SetLocalRotation)
                    {
                        value = y.transform.localEulerAngles.y;
                    }
                    else if (executionType == ExecutionType.SetGlobalScale)
                    {
                        value = y.transform.lossyScale.y;
                    }
                    else if (executionType == ExecutionType.SetLocalScale)
                    {
                        value = y.transform.localScale.y;
                    }

                    y.value = value;
                }
            }
        }

        private void UpdateValueZ ()
        {
            if (z.enabled)
            {
                if (z.transform != null)
                {
                    var value = 0f;
                    if (executionType == ExecutionType.SetGlobalPosition)
                    {
                        value = z.transform.position.z;
                    }
                    else if (executionType == ExecutionType.SetLocalPosition)
                    {
                        value = z.transform.localPosition.z;
                    }
                    else if (executionType == ExecutionType.SetGlobalRotation)
                    {
                        value = z.transform.eulerAngles.z;
                    }
                    else if (executionType == ExecutionType.SetLocalRotation)
                    {
                        value = z.transform.localEulerAngles.z;
                    }
                    else if (executionType == ExecutionType.SetGlobalScale)
                    {
                        value = z.transform.lossyScale.z;
                    }
                    else if (executionType == ExecutionType.SetLocalScale)
                    {
                        value = z.transform.localScale.z;
                    }

                    z.value = value;
                }
            }
        }

        private string XLabel ()
        {
            switch (executionType)
            {
                case ExecutionType.AddGlobalPosition:
                case ExecutionType.AddGlobalRotation:
                case ExecutionType.AddGlobalScale:
                case ExecutionType.AddLocalPosition:
                case ExecutionType.AddLocalRotation:
                case ExecutionType.AddLocalScale:
                    return "+ X";
            }

            return "X";
        }

        private string YLabel ()
        {
            switch (executionType)
            {
                case ExecutionType.AddGlobalPosition:
                case ExecutionType.AddGlobalRotation:
                case ExecutionType.AddGlobalScale:
                case ExecutionType.AddLocalPosition:
                case ExecutionType.AddLocalRotation:
                case ExecutionType.AddLocalScale:
                    return "+ Y";
            }

            return "Y";
        }

        private string ZLabel ()
        {
            switch (executionType)
            {
                case ExecutionType.AddGlobalPosition:
                case ExecutionType.AddGlobalRotation:
                case ExecutionType.AddGlobalScale:
                case ExecutionType.AddLocalPosition:
                case ExecutionType.AddLocalRotation:
                case ExecutionType.AddLocalScale:
                    return "+ Z";
            }

            return "Z";
        }

        public static string displayName = "Transform Behaviour Node";
        public static string nodeName = "Transform";

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
            if (transform != null)
            {
                string message = "Set ";
                switch (executionType)
                {
                    case ExecutionType.AddGlobalPosition:
                    case ExecutionType.AddGlobalRotation:
                    case ExecutionType.AddGlobalScale:
                    case ExecutionType.AddLocalPosition:
                    case ExecutionType.AddLocalRotation:
                    case ExecutionType.AddLocalScale:
                        message = "Add ";
                        break;
                }

                if (executionType is ExecutionType.SetGlobalPosition or ExecutionType.AddGlobalPosition)
                {
                    message += $"{transform.gameObject.name}'s global position to ";
                }
                else if (executionType is ExecutionType.SetLocalPosition or ExecutionType.AddLocalPosition)
                {
                    message += $"{transform.gameObject.name}'s local position to ";
                }
                else if (executionType is ExecutionType.SetGlobalRotation or ExecutionType.AddGlobalRotation)
                {
                    message += $"{transform.gameObject.name}'s global rotation to ";
                }
                else if (executionType is ExecutionType.SetLocalRotation or ExecutionType.AddLocalRotation)
                {
                    message += $"{transform.gameObject.name}'s local rotation to ";
                }
                else if (executionType is ExecutionType.SetGlobalScale or ExecutionType.AddGlobalScale)
                {
                    message += $"{transform.gameObject.name}'s global scale to ";
                }
                else if (executionType is ExecutionType.SetLocalScale or ExecutionType.AddLocalScale)
                {
                    message += $"{transform.gameObject.name}'s local scale to ";
                }

                if (!x.enabled)
                    message += "NaN, ";
                else if (x.transform != null)
                    message += "Obj, ";
                else
                    message += x.value + ", ";
                if (!y.enabled)
                    message += "NaN, ";
                else if (y.transform != null)
                    message += "Obj, ";
                else
                    message += y.value + ", ";
                if (!z.enabled)
                    message += "NaN";
                else if (z.transform != null)
                    message += "Obj";
                else
                    message += z.value;
                return message;
            }

            return string.Empty;
        }
#endif
    }
}