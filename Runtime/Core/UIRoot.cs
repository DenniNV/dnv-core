using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Logging;
using Libraries.DNV.MVC.Core;
using Libraries.DNV.MVC.Interfaces;
using UnityEngine;

namespace Libraries.DNV.Core
{
    /// <summary>
    /// Builds every controller from its factory. The library knows no game UI:
    /// a game adds its screens as factories in the inspector.
    /// </summary>
    public sealed class UIRoot : MonoBehaviour
    {
        [SerializeReference] [SubclassSelector]
        private List<IControllerFactory> _controllerFactories = new List<IControllerFactory>();

        private readonly ControllerContainer _controllers = new ControllerContainer();

        public async UniTask Build(CancellationToken token)
        {
            var context = new ControllerFactoryContext(transform);

            foreach(var factory in _controllerFactories)
            {
                if(factory == null)
                {
                    Log.Warning(LogCategory.Core, $"[{nameof(UIRoot)}] Empty controller factory slot, skipped.");
                    continue;
                }

                _controllers.Add(await factory.Create(context, token));
            }
        }

        public TController Get<TController>() where TController : IController
        {
            return _controllers.GetController<TController>();
        }

        public void Dispose()
        {
            _controllers.Dispose();
        }

        private void Update()
        {
            _controllers.Execute();
        }
    }
}
