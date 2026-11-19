namespace Libraries.DNV.CodePresets.Commands
{
    public class FirstMatchCommand : ICommand
    {
        private readonly ICommand[] _commands;

        public bool CanExecute => FindExecutable() != null;

        public FirstMatchCommand(params ICommand[] commands)
        {
            _commands = commands;
        }

        public CommandResult Execute()
        {
            var command = FindExecutable();

            if(command == null)
            {
                return CommandResult.NotAllowed();
            }

            return command.Execute();
        }

        private ICommand FindExecutable()
        {
            foreach (var command in _commands)
            {
                if(command.CanExecute)
                {
                    return command;
                }
            }

            return null;
        }
    }
}
