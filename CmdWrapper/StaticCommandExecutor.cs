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
                
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,

                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false
            };

            using var process = new Process {StartInfo = startInfo};
            process.Start();
            
            var stdout = process.StandardOutput.ReadToEnd(); 
            var stderr = process.StandardError.ReadToEnd();
            var result = $"{stdout}\n{stderr}";

            process.Kill(true);
            process.Close();

            return result;
        }
    }
}