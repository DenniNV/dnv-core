using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Libraries.DNV.InputTemplate.Template
{
    public class FloatModel
    {
        private readonly InputAction _inputAction;

        public float Value => _inputAction.ReadValue<Vector2>().y;

        public FloatModel(InputAction inputAction)
        {
            _inputAction = inputAction;
        }
    }

    public class Vector2Model
    {
        private readonly InputAction _inputAction;
        public Vector2 Value => _inputAction.ReadValue<Vector2>();

        public Vector2Model(InputAction inputAction)
        {
            _inputAction = inputAction;
        }
    }


    public class ActionModel
    {
        private readonly InputAction _inputAction;

        public event Action IsPressed;
        public event Action IsReleased;

        public ActionModel(InputAction inputAction)
        {
            _inputAction = inputAction;
        }

        public void Dispose()
        {
            _inputAction.performed -= InputActionOnStarted;
            _inputAction.canceled -= InputActionOnCanceled;
        }

        public void AddHandlers()
        {
            _inputAction.performed += InputActionOnStarted;
            _inputAction.canceled += InputActionOnCanceled;
        }

        private void InputActionOnCanceled(InputAction.CallbackContext obj)
        {
            IsReleased?.Invoke();
        }

        private void InputActionOnStarted(InputAction.CallbackContext obj)
        {
            IsPressed?.Invoke();
        }
    }
}