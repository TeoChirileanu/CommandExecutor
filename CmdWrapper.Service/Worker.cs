using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CmdWrapper.Service
{
    public class Worker : BackgroundService
    {
        private const string FolderToWatch = @"c:\tmp\watch";
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                CreateFolderToWatchIfNeeded();
                _logger.LogInformation($"Watching {FolderToWatch}");
                using var watcher = new FileSystemWatcher(FolderToWatch) {EnableRaisingEvents = true};
                using var pipeline = Observable.FromEventPattern(watcher, nameof(watcher.Changed))
                    .Sample(TimeSpan.FromMilliseconds(500))
                    .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                    .Select(GetContentFromFile)
                    .Switch()
                    .Where(command => !string.IsNullOrWhiteSpace(command))
                    .Do(command => _logger.LogInformation($">{command}"))
                    .Select(ExecuteCommandAndGetResult)
                    .Switch()
                    .Subscribe(
                        executionResult => _logger.LogInformation(executionResult),
                        ex => _logger.LogError(ex, "Got an error"));
                await Task.Delay(-1, cancellationToken);
            }
        }

        private void CreateFolderToWatchIfNeeded()
        {
            try { if (!Directory.Exists(FolderToWatch)) Directory.CreateDirectory(FolderToWatch); }
            catch (Exception e) {_logger.LogError(e.ToString());}
        }
        
        private static IObservable<string> GetContentFromFile(string file)
        {
            var subject = new BehaviorSubject<string>(string.Empty);
            try { subject.OnNext(File.ReadAllText(file)); }
            catch (Exception e) { subject.OnError(e); }
            return subject.AsObservable();
        }
        
        private static IObservable<string> ExecuteCommandAndGetResult(string command)
        {
            var subject = new BehaviorSubject<string>(string.Empty);
            try { subject.OnNext(ExecuteCommand(command)); }
            catch (Exception e) { subject.OnError(e); }
            return subject.AsObservable();
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
            var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd();
            return output?.Trim();
        }
    }
}