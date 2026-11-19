namespace Libraries.DNV.CodePresets.Modules
{
    public interface IModuleSwitcher<TFamily> where TFamily : class, IRunnable
    {
        TFamily Current { get; }
        void SwitchTo<TModule>() where TModule : class, TFamily;
        void StopCurrent();
    }
}
