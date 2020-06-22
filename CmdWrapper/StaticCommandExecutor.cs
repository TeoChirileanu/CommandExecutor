using System;
using System.Diagnostics;

namespace CmdWrapper
{
    public static class StaticCommandExecutor
    {
        public static string ExecuteCommand(string command)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c {command}",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                ErrorDialog = false
            };
            return Process.Start(startInfo)?.StandardOutput.ReadToEnd().Trim();
        }
    }
}