using System.Diagnostics;
using System.Threading.Tasks;

namespace CmdWrapper
{
    public class CommandExecutorAsync : ICommandExecutorAsync
    {
        private readonly Process _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c ", // run command then terminate
                UseShellExecute = false,
                // disable all interaction with the outside world
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                ErrorDialog = false
            }
        };
        public async Task<string> ExecuteCommand(string command)
        {
            _process.StartInfo.Arguments += command;
            
            _process.Start();
            _process.WaitForExit();
            
            var result = await _process.StandardOutput.ReadToEndAsync();
            return result.Trim();
        }

        public void Dispose()
        {
            _process?.Close();
            _process?.Dispose();
        }
    }
}
