using Libraries.DNV.CodePresets.Rules.Interfaces;
using UnityEngine.InputSystem.Controls;

namespace Libraries.DNV.InputTemplate.Conditions
{
    public class DebugInputCondition : ICondition
    {
        private readonly KeyControl _keyControl;
        public bool IsExecuted => _keyControl.wasPressedThisFrame;
        
        public DebugInputCondition(KeyControl keyControl)
        {
            _keyControl = keyControl;
        }
        
        public void Reset()
        {
        }
    }
}