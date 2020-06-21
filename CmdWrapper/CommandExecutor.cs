using System.Diagnostics;

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
                UseShellExecute = false,
            }
        };
        public string ExecuteCommand(string command)
        {
            _process.StartInfo.Arguments += command;
            _process.Start();
            _process.WaitForExit();
            return _process.StandardOutput.ReadToEnd().Trim();
        }

        public void Dispose()
        {
            _process?.Close();
            _process?.Dispose();
        }
    }
}
