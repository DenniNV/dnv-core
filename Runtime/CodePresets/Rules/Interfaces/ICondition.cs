namespace Libraries.DNV.CodePresets.Rules.Interfaces
{
    public interface ICondition
    {
        bool IsExecuted { get; }
        public void Reset();
    }

    public static class ConditionExtension
    {
        public static bool IsExecuted(this ICondition[] self)
        {
            foreach (var item in self)
            {
                if (!item.IsExecuted)
                {
                    return false;
                }
            }

            return true;
        }
    } 
  
}