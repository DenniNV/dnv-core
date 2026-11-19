namespace Libraries.DNV.CodePresets.Factory
{
    public interface IFactory<out TItems, in TContext> where TContext : struct where TItems : class
    {
        public TItems Create(TContext context);
    }
}