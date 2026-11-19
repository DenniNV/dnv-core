using Libraries.DNV.CodePresets.Rules.Interfaces;

namespace Libraries.DNV.CodePresets.Commands
{
    public class ConditionalCommand : ICommand
    {
        private readonly ICommand _command;
        private readonly ICondition[] _conditions;

        public bool CanExecute => FindFailedCondition() == null && _command.CanExecute;

        public ConditionalCommand(ICommand command, params ICondition[] conditions)
        {
            _command = command;
            _conditions = conditions;
        }

        public CommandResult Execute()
        {
            var failedCondition = FindFailedCondition();

            if(failedCondition != null)
            {
                return CommandResult.NotAllowed(failedCondition);
            }

            var result = _command.Execute();

            if(result.Status == CommandStatus.Success)
            {
                ResetConditions();
            }

            return result;
        }

        private ICondition FindFailedCondition()
        {
            foreach (var condition in _conditions)
            {
                if(!condition.IsExecuted)
                {
                    return condition;
                }
            }

            return null;
        }

        private void ResetConditions()
        {
            foreach (var condition in _conditions)
            {
                condition.Reset();
            }
        }
    }
}
