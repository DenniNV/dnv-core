using Libraries.DNV.Extensions;
using Libraries.DNV.CodePresets.Rules.Interfaces;
using Libraries.DNV.InputTemplate;
using Libraries.DNV.InputTemplate.Template;

namespace Libraries.DNV.InputTemplate.Conditions
{
    public class InputCondition : ICondition
    {
        private readonly ButtonModel _model;
        private readonly InputType _type;

        public bool IsExecuted => _model.IsInputMatchesType(_type);
      
        public InputCondition(ButtonModel model, InputType type)
        {
            _model = model;
            _type = type;
        }
        
        public void Reset()
        {
            _model.Reset();
        }
    }
}