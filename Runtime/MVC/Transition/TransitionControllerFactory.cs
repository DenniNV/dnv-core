using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Addressable;
using Libraries.DNV.DependencyInjection;
using Libraries.DNV.MVC.Core;
using Libraries.DNV.MVC.Interfaces;
using UnityEngine;

namespace Libraries.DNV.MVC.Transition
{
    [Serializable]
    public sealed class TransitionControllerFactory : IControllerFactory
    {
        [SerializeField]
        private CustomReferences<TransitionView> _view;

        public async UniTask<IController> Create(ControllerFactoryContext context, CancellationToken cancellationToken)
        {
            var spawnValues = new SpawnValues(Vector3.zero, Quaternion.identity, context.ViewParent);
            var view = await Injector.Get<IAssetProvider>().InstantiateAsync(_view, spawnValues, cancellationToken);
            view.Construct();

            return new TransitionController(view);
        }
    }
}
