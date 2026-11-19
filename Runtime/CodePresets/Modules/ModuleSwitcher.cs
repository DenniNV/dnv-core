using System.Collections.Generic;

namespace Libraries.DNV.CodePresets.Modules
{
    /// <summary>
    /// Keeps exactly one module of a family running: switching stops the current one and runs the next.
    /// </summary>
    public class ModuleSwitcher<TFamily> : IModuleSwitcher<TFamily> where TFamily : class, IRunnable
    {
        private readonly IModuleProvider<TFamily> _provider;

        public TFamily Current { get; private set; }

        public ModuleSwitcher(IReadOnlyList<TFamily> modules)
        {
            _provider = new ModuleRegistry<TFamily>(modules);
        }

        public void SwitchTo<TModule>() where TModule : class, TFamily
        {
            var next = _provider.Get<TModule>();

            Current?.Stop();
            Current = next;
            Current.Run();
        }

        public void StopCurrent()
        {
            Current?.Stop();
            Current = null;
        }
    }
}
