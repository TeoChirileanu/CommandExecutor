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

            string result;
            try
            {
                _process.Start();
                _process.WaitForExit();
                result = _process.StandardOutput.ReadToEnd();
            }
            catch (Exception e)
            {
                var errorMessage = $"Got an error: {e}";
                Console.WriteLine(errorMessage);
                return errorMessage;
            }
            
            return result.Trim();
        }

        public void Dispose()
        {
            _process?.Close();
            _process?.Dispose();
        }
    }
}