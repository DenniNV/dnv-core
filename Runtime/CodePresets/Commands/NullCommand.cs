namespace Libraries.DNV.CodePresets.Commands
{
    public class NullCommand : ICommand
    {
        public static readonly NullCommand Instance = new NullCommand();

        public bool CanExecute => false;

        private NullCommand()
        {
        }

        public CommandResult Execute()
        {
            return CommandResult.Failed();
        }
    }
}
