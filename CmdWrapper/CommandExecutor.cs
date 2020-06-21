using System.Diagnostics;

namespace CmdWrapper
{
    public class CommandExecutor : ICommandExecutor
    {
        public string ExecuteCommand(string command)
        {
            using var process = new Process
            {
                StartInfo =
                {
                    FileName = @"C:\Windows\System32\cmd.exe",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    ErrorDialog = false
                }
            };
            process.StartInfo.Arguments = $"/c {command}";
            process.Start();
            var result = process.StandardOutput.ReadToEnd().Trim();
            process.Close();
            return result;
        }
    }
}
