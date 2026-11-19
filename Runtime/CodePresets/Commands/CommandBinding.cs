using System;

namespace Libraries.DNV.CodePresets.Commands
{
    public class CommandBinding : IDisposable
    {
        private readonly Action _unsubscribe;

        private CommandBinding(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public static CommandBinding Create(Action<Action> subscribe, Action<Action> unsubscribe, ICommand command)
        {
            void Handler() => command.Execute();

            subscribe(Handler);

            return new CommandBinding(() => unsubscribe(Handler));
        }

        public static CommandBinding Create<TArgument>(Action<Action<TArgument>> subscribe,
            Action<Action<TArgument>> unsubscribe, Func<TArgument, ICommand> selector)
        {
            void Handler(TArgument argument) => selector(argument).Execute();

            subscribe(Handler);

            return new CommandBinding(() => unsubscribe(Handler));
        }

        public void Dispose()
        {
            _unsubscribe();
        }
    }
}
