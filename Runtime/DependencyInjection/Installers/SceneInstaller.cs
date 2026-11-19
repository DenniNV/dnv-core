using System;
using Libraries.DNV.DependencyInjection.Interfaces;
using Libraries.DNV.Notifications;

namespace Libraries.DNV.DependencyInjection.Installers
{
    [Serializable]
    public class SceneInstaller : IInstaller
    {
        private IGameNotificationService _gameNotificationService;

        public void Bind(Resolver container)
        {
            InternalBuild();
            container.RegistrationInstance(_gameNotificationService);
        }

        private void InternalBuild()
        {
            _gameNotificationService = new GameNotificationService();
        }
    }
}