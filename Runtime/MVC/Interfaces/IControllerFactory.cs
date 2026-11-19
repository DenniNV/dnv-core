using Libraries.DNV.CodePresets.Factory;
using Libraries.DNV.MVC.Core;

namespace Libraries.DNV.MVC.Interfaces
{
    /// <summary>
    /// Creates a view and returns its controller ready to use. Serialized on UIRoot via [SerializeReference],
    /// so swapping a UI means swapping a factory or its view reference in the inspector.
    /// </summary>
    public interface IControllerFactory : IAsyncFactory<IController, ControllerFactoryContext>
    {
    }
}
