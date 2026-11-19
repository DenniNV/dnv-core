using System;
using Libraries.DNV.Core;
using Libraries.DNV.DependencyInjection.Interfaces;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection.Installers
{
    [Serializable]
    public class UIInstaller : IInstaller
    {
        [SerializeField] private UIRoot _uiRoot;

        public void Bind(Resolver container)
        {
            container.RegistrationInstance(_uiRoot);
        }
    }
}
