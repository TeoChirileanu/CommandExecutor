using System;
using System.Diagnostics;

namespace CmdWrapper
{
    public static class StaticCommandExecutor
    {
        private static readonly ProcessStartInfo StartInfo = new ProcessStartInfo
        {
            FileName = "cmd",
            Arguments = $"/c ",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            ErrorDialog = false
        };

        public static string ExecuteCommand(string command)
        {
            StartInfo.Arguments += command;
            try
            {
                using var process = Process.Start(StartInfo);
                return process?.StandardOutput.ReadToEnd().Trim();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}