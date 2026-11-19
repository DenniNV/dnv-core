using UnityEngine.InputSystem;

namespace Libraries.DNV.InputTemplate.Template
{
    public class ButtonModel 
    {
        private readonly InputAction _inputAction;
        public bool IsReleased => _inputAction.WasReleasedThisFrame();
        public bool IsPressed => _inputAction.WasPressedThisFrame();
        public bool IsHold => _inputAction.IsPressed();
        
        public ButtonModel(InputAction inputAction)
        {
            _inputAction = inputAction;
        }

        public void Reset()
        {
            _inputAction.Reset();
        }
    }
}