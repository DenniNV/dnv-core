using Libraries.DNV.Addressable;
using Libraries.DNV.DependencyInjection;
using Libraries.DNV.MVC.Interfaces;

namespace Libraries.DNV.MVC.Transition
{
    public sealed class TransitionController : IController
    {
        private IAssetProvider AssetProvider => Injector.Get<IAssetProvider>();

        private readonly TransitionView _view;

        public TransitionController(TransitionView view)
        {
            _view = view;
        }

        public void Show()
        {
            _view.Show();
        }

        public void Hide()
        {
            _view.Hide();
        }

        public void Execute()
        {
        }

        public void Dispose()
        {
            AssetProvider.ReleaseAndDestroy(_view);
        }
    }
}
