using System;
using System.Collections.Generic;

namespace Libraries.DNV.Notifications
{
    public sealed class GameNotificationService : IGameNotificationService
    {
        // Copy-on-write: Send iterates an array that Subscribe/Unsubscribe replace and never mutate,
        // so a handler may (un)subscribe while being notified, and sending allocates nothing.
        private readonly Dictionary<Type, Delegate[]> _handlers = new Dictionary<Type, Delegate[]>();

        public void Subscribe<TMessage>(Action<TMessage> handler)
        {
            var current = GetHandlers<TMessage>();

            if (Array.IndexOf(current, handler) >= 0)
            {
                return;
            }

            var next = new Delegate[current.Length + 1];
            Array.Copy(current, next, current.Length);
            next[current.Length] = handler;

            _handlers[typeof(TMessage)] = next;
        }

        public void Unsubscribe<TMessage>(Action<TMessage> handler)
        {
            var current = GetHandlers<TMessage>();
            var index = Array.IndexOf(current, handler);

            if (index < 0)
            {
                return;
            }

            var next = new Delegate[current.Length - 1];
            Array.Copy(current, 0, next, 0, index);
            Array.Copy(current, index + 1, next, index, current.Length - index - 1);

            _handlers[typeof(TMessage)] = next;
        }

        public void Send<TMessage>(TMessage message)
        {
            foreach (var handler in GetHandlers<TMessage>())
            {
                ((Action<TMessage>)handler).Invoke(message);
            }
        }

        private Delegate[] GetHandlers<TMessage>()
        {
            return _handlers.TryGetValue(typeof(TMessage), out var handlers) ? handlers : Array.Empty<Delegate>();
        }
    }
}
