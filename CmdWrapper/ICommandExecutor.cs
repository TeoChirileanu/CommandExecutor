using System;
using System.Threading.Tasks;

namespace CmdWrapper
{
    public interface ICommandExecutor : IDisposable
    {
        Task<string> ExecuteCommand(string command);
    }
}