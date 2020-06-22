using System;

namespace CmdWrapper
{
    public interface ICommandExecutor : IDisposable
    {
        string ExecuteCommand(string command);
    }
}