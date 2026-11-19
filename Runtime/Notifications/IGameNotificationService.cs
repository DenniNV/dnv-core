using System;

namespace Libraries.DNV.Notifications
{
    /// <summary>
    /// Typed messages: a handler receives only the message type it subscribed to. Any type can be a message;
    /// prefer small readonly structs so sending does not allocate.
    /// </summary>
    public interface IGameNotificationService
    {
        void Subscribe<TMessage>(Action<TMessage> handler);
        void Unsubscribe<TMessage>(Action<TMessage> handler);
        void Send<TMessage>(TMessage message);
    }
}
