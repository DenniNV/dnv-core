using Cysharp.Threading.Tasks;
using Libraries.DNV.Addressable;
using Libraries.DNV.CodePresets.State;
using Libraries.DNV.CodePresets.State.Interfaces;
using Libraries.DNV.ContentManagerSystem;
using Libraries.DNV.DependencyInjection;
using Libraries.DNV.DependencyInjection.Context;
using UnityEngine;

namespace Libraries.DNV.Core
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private int _frameRate = 60;

        [SerializeField] private CustomReferences<ProjectContext> _projectContext;
        [SerializeField] private CustomReferences<UIContext> _uiContext;
        [SerializeField] private CustomReferences<ProjectDisposer> _projectDisposer;

        [SerializeReference] [SubclassSelector]
        private IGameStatesFactory _statesFactory = new DefaultGameStatesFactory();

        private AssetProvider _assetProvider;

        private void Awake()
        {
            Enter().Forget();
        }

        private async UniTask Enter()
        {
            CreateAddressableContainer();

            var projectContext = await _assetProvider.InstantiateAsync(_projectContext, SpawnValues.Default, destroyCancellationToken);
            var uiContext = await _assetProvider.InstantiateAsync(_uiContext, SpawnValues.Default, destroyCancellationToken);
            var projectDisposer = await _assetProvider.InstantiateAsync(_projectDisposer, SpawnValues.Default, destroyCancellationToken);

            await projectDisposer.Build(destroyCancellationToken);
            await projectContext.Build(destroyCancellationToken);
            await uiContext.Build(destroyCancellationToken);

            DontDestroyOnLoad(projectContext);
            DontDestroyOnLoad(uiContext);
            DontDestroyOnLoad(projectDisposer);

            IStateMachine gsm = Injector.Get<GameStateMachine>();
            var statesFactory = _statesFactory ?? new DefaultGameStatesFactory();
            gsm.Registration(statesFactory.Create(gsm));

            gsm.SwitchTo<InitializeState>();
            Application.targetFrameRate = _frameRate;
        }

        private void CreateAddressableContainer()
        {
            var resolver = new Resolver.Entry();
            _assetProvider = new AssetProvider();

            resolver.RegistrationInstance<IAssetProvider>(_assetProvider);

            ResolverRegistry.Add(resolver);
        }
    }
}