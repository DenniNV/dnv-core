using System.Collections.Generic;
using Libraries.DNV.CodePresets.Rules.Interfaces;

namespace Libraries.DNV.CodePresets.Commands
{
    public class CommandBuilder
    {
        private readonly ICommand _command;
        private readonly List<ICondition> _conditions = new List<ICondition>();
        private readonly List<ICommandHandler> _successHandlers = new List<ICommandHandler>();
        private readonly List<ICommandHandler> _failureHandlers = new List<ICommandHandler>();

        private CommandBuilder(ICommand command)
        {
            _command = command;
        }

        public static CommandBuilder For(ICommand command)
        {
            return new CommandBuilder(command);
        }

        public CommandBuilder When(params ICondition[] conditions)
        {
            _conditions.AddRange(conditions);

            return this;
        }

        public CommandBuilder OnSuccess(params ICommandHandler[] handlers)
        {
            _successHandlers.AddRange(handlers);

            return this;
        }

        public CommandBuilder OnFailed(params ICommandHandler[] handlers)
        {
            _failureHandlers.AddRange(handlers);

            return this;
        }

        public ICommand Build()
        {
            var command = _conditions.Count == 0
                ? _command
                : new ConditionalCommand(_command, _conditions.ToArray());

            if(_successHandlers.Count == 0 && _failureHandlers.Count == 0)
            {
                return command;
            }

            return new HandledCommand(command, _successHandlers.ToArray(), _failureHandlers.ToArray());
        }
    }
}
