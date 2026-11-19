using Libraries.DNV.CodePresets.Commands;
using Libraries.DNV.CodePresets.Rules.Interfaces;
using Libraries.DNV.Logging;

namespace Libraries.DNV.CodePresets.Commands
{
    public class LogCommandHandler : ICommandHandler
    {
        private readonly string _commandName;

        public LogCommandHandler(string commandName)
        {
            _commandName = commandName;
        }

        public void Handle(in CommandResult result)
        {
            Log.Info(LogCategory.Commands, $"{_commandName} -> {result.Status} {Describe(result.FailedCondition)}");
        }

        private string Describe(ICondition condition)
        {
            return condition == null ? string.Empty : $"blocked by {condition.GetType().Name}";
        }
    }
}
