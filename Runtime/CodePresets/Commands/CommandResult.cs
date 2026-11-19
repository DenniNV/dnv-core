using Libraries.DNV.CodePresets.Rules.Interfaces;

namespace Libraries.DNV.CodePresets.Commands
{
    public readonly struct CommandResult
    {
        public CommandStatus Status { get; }
        public ICondition FailedCondition { get; }

        private CommandResult(CommandStatus status, ICondition failedCondition)
        {
            Status = status;
            FailedCondition = failedCondition;
        }

        public static CommandResult Success()
        {
            return new CommandResult(CommandStatus.Success, null);
        }

        public static CommandResult NotAllowed()
        {
            return new CommandResult(CommandStatus.NotAllowed, null);
        }

        public static CommandResult NotAllowed(ICondition failedCondition)
        {
            return new CommandResult(CommandStatus.NotAllowed, failedCondition);
        }

        public static CommandResult Failed()
        {
            return new CommandResult(CommandStatus.Failed, null);
        }
    }
}
