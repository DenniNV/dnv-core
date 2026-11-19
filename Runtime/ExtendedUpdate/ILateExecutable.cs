namespace Libraries.DNV.ExtendedUpdate
{
    public interface ILateExecutable : IExecutionTarget
    {
        public void LateExecute();
    }
}