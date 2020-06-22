using System;
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
        public string ExecuteCommand(string command)
        {
            _process.StartInfo.Arguments += command;

            try
            {
                _process.Start();
                _process.WaitForExit();
                return _process.StandardOutput.ReadToEnd().Trim();
            }
            catch (Exception e)
            {
                return $"Got an error: {e}";
            }
        }

        public void Dispose()
        {
            _process?.Close();
            _process?.Dispose();
        }
    }
}