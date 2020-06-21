using System.Diagnostics;
using System.Threading.Tasks;

namespace CmdWrapper
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly Process _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c ",
                
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
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
