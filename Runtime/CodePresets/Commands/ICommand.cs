namespace Libraries.DNV.CodePresets.Commands
{
    public interface ICommandStatus
    {
        public bool CanExecute { get; }
    }

    public interface ICommand : ICommandStatus
    {
        public CommandResult Execute();
    }
}
