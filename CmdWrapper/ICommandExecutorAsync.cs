using System;
using System.Threading.Tasks;

namespace CmdWrapper
{
    public interface ICommandExecutorAsync : IDisposable
    {
        Task<string> ExecuteCommand(string command);
    }
}