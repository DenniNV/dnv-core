namespace Libraries.DNV.Navigation
{
    public interface INavigationRegenerator
    {
        void RegeneratePath();
        public void ChangePathGeneratorTo<TGenerator>() where TGenerator : IPathGenerator;
    }
}