using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Reshape.Reframework
{
    [HideMonoScript]
    public class MouseRotationController : BaseBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        private Vector2 rotateRate;
        private InputActionAsset actionAsset;
        private InputAction action;
        private string actionName;
        private Vector3 rotation;

        public void Initial (Vector2 rate, InputActionAsset input, string inputName)
        {
            rotateRate = rate;
            actionAsset = input;
            actionName = inputName;
        }

        public void Terminate ()
        {
            Destroy(this);
        }

        protected override void Start ()
        {
            if (actionAsset != null)
            {
                action = actionAsset.FindAction(actionName);
                if (action != null)
                {
                    rotation = transform.localEulerAngles;
                    return;
                }
            }

            Terminate();
        }

        protected void Update ()
        {
            var movement = action.ReadValue<Vector2>();
            rotation += new Vector3(movement.y * (Time.deltaTime * rotateRate.y), -movement.x * (Time.deltaTime * rotateRate.x), 0);
            transform.localEulerAngles = rotation;
        }
#endif
    }
}