using System;

namespace Libraries.DNV.MVC.Interfaces
{
    public interface IControllerContainer : IDisposable
    {
        public T GetController<T>() where T : IController;
        public void RemoveController<T>(T controller) where T : IController;
        public void Add(IController controller);
    }
}
