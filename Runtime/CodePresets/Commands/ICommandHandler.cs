namespace Libraries.DNV.CodePresets.Commands
{
    public interface ICommandHandler
    {
        public void Handle(in CommandResult result);
    }
}
