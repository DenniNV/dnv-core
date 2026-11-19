namespace Libraries.DNV.CodePresets.Commands
{
    public class HandledCommand : ICommand
    {
        private readonly ICommand _command;
        private readonly ICommandHandler[] _successHandlers;
        private readonly ICommandHandler[] _failureHandlers;

        public bool CanExecute => _command.CanExecute;

        public HandledCommand(ICommand command, ICommandHandler[] successHandlers, ICommandHandler[] failureHandlers)
        {
            _command = command;
            _successHandlers = successHandlers;
            _failureHandlers = failureHandlers;
        }

        public CommandResult Execute()
        {
            var result = _command.Execute();
            var handlers = result.Status == CommandStatus.Success ? _successHandlers : _failureHandlers;

            Notify(handlers, result);

            return result;
        }

        private void Notify(ICommandHandler[] handlers, in CommandResult result)
        {
            foreach (var handler in handlers)
            {
                handler.Handle(result);
            }
        }
    }
}
