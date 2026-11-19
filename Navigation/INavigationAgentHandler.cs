namespace Libraries.DNV.Navigation
{
    public interface INavigationAgentHandler
    {
        void PathBeingReachedHandler();
        void PathIsNotAchievable();
    }
}