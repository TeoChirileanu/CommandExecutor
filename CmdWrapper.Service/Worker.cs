using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace CmdWrapper.Service
{
    public class Worker : BackgroundService
    {
        private const string FolderToWatch = @"c:\tmp\watch\";
        private const string FileToWatch = FolderToWatch + "command.txt";
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken _)
        {
            Policy.Handle<Exception>()
                .WaitAndRetryForever(
                    sleepTime => TimeSpan.FromSeconds(sleepTime),
                    (exception, __) => _logger.LogError(exception, exception.Message))
                .Execute(Run);
            
            await Task.CompletedTask;
        }

        private void Run()
        {
            if (!Directory.Exists(FolderToWatch)) Directory.CreateDirectory(FolderToWatch);
            
            _logger.LogInformation($"Watching {FolderToWatch} for {FileToWatch}");

            var watcher = new FileSystemWatcher(FolderToWatch, FileToWatch);

            Observable.FromEventPattern(watcher, nameof(watcher.Changed))
                .Sample(TimeSpan.FromSeconds(1))
                .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                .Select(commandFile => Observable.Return(File.ReadAllText(commandFile)))
                .Switch()
                .Where(command => !string.IsNullOrWhiteSpace(command))
                .Do(command => _logger.LogInformation($">{command}"))
                .Select(command => Observable.Return(ExecuteCommand(command)))
                .Switch()
                .Subscribe(executionResult => _logger.LogInformation(executionResult));

            watcher.EnableRaisingEvents = true;
        }

        private static string ExecuteCommand(string command)
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