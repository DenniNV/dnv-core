using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.MVC.Interfaces
{
    /// <summary>
    /// Receives its view and data ready-made from an <see cref="IControllerFactory"/>.
    /// Holds the UI state; the view only displays what the controller gives it, so views stay swappable.
    /// </summary>
    public interface IController : IExecutable
    {
        public void Show();
        public void Hide();
        public void Dispose();
    }
}
