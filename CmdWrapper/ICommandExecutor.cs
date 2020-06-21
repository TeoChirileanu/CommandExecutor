namespace CmdWrapper
{
    public interface ICommandExecutor
    {
        void ExecuteCommand(string command);
        string GetExecutionResult();
    }
}