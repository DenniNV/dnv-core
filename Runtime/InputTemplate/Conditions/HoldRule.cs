using Libraries.DNV.Extensions;
using Libraries.DNV.CodePresets.Rules.Interfaces;
using Libraries.DNV.CodePresets.Sequence;
using Libraries.DNV.InputTemplate;
using Libraries.DNV.InputTemplate.Template;

namespace Libraries.DNV.InputTemplate.Conditions
{
    public class HoldRule : ICondition
    {
        private readonly ButtonModel _model;
        private readonly Delay _delay;

        public bool IsExecuted => IsDelay();

        public HoldRule(ButtonModel model, Delay delay)
        {
            _model = model;
            _delay = delay;
        }

        public void Reset()
        {
            _delay.Reset();
        }

        private bool IsDelay()
        {
            if (_model.IsInputMatchesType(InputType.Hold))
            {
                if (_delay.IsExecuted)
                {
                    return true;
                }

                return false;
            }
            
            _delay.Reset();
            return false;
        }
    }
}