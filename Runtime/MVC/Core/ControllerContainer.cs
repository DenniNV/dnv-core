using System;
using System.Collections.Generic;
using Libraries.DNV.MVC.Interfaces;

namespace Libraries.DNV.MVC.Core
{
    public sealed class ControllerContainer : IControllerContainer
    {
        private readonly List<IController> _controllers = new List<IController>();

        public void Add(IController controller)
        {
            _controllers.Add(controller);
        }

        public T GetController<T>() where T : IController
        {
            var controller = _controllers.Find(x => x is T);

            if (controller == null)
            {
                throw new Exception($"Controller {typeof(T).Name} not found");
            }
            
            return (T)controller;
        }

        public void RemoveController<T>(T controller) where T : IController
        { 
            _controllers.Remove(controller);
        }

        public void Execute()
        {
            foreach (var controller in _controllers)
            {
                controller.Execute();
            }
        }

        public void Dispose()
        {
            foreach (var controller in _controllers)
            {
                controller.Dispose();
            }
        }
    }
}
