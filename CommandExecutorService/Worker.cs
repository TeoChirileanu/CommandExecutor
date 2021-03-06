using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommandExecutorService
{
    public class Worker : BackgroundService
    {
        private const string FolderToWatch = @"c:\tmp\watch\";
        private const string FileToWatch = "command.txt";
        
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _logger.LogInformation("Service started");
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _logger.LogInformation($"Watching {FolderToWatch} for {FileToWatch}");
            while (!token.IsCancellationRequested)
            {
                var watcher = new FileSystemWatcher();
                IDisposable observable = null;
                try
                {
                    if (!Directory.Exists(FolderToWatch)) Directory.CreateDirectory(FolderToWatch);
                    
                    watcher.Path = FolderToWatch;
                    watcher.Filter = FileToWatch;
                    watcher.EnableRaisingEvents = true;

                    observable = Observable.FromEventPattern(watcher, nameof(watcher.Changed))
                        .Sample(TimeSpan.FromSeconds(1))
                        .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                        .Select(commandFile => Observable.Return(File.ReadAllText(commandFile)))
                        .Switch()
                        .Where(command => !string.IsNullOrWhiteSpace(command))
                        .Do(command => _logger.LogInformation($">{command}"))
                        .Select(command => Observable.Return(ExecuteCommand(command)))
                        .Switch()
                        .Subscribe(executionResult => _logger.LogInformation(executionResult));

                    await File.WriteAllTextAsync(FileToWatch, string.Empty, token);
                    // watch incoming files raising errors as necessary
                    await Task.Delay(TimeSpan.FromMinutes(1), token);
                    // dispose after waiting so a new fresh start can be made
                    observable.Dispose();
                    watcher.Dispose();
                }
                catch (TaskCanceledException)
                {
                    await StopAsync(token);
                    observable?.Dispose();
                    watcher.Dispose();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Got an error : {e.Message}");
                    observable?.Dispose();
                    watcher.Dispose();

                    await Task.Delay(TimeSpan.FromSeconds(10), token);
                    await ExecuteAsync(token);
                }
            }
        }

        private static string ExecuteCommand(string command)
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
            process.WaitForExit();
            
            var stdout = process.StandardOutput.ReadToEnd(); 
            var stderr = process.StandardError.ReadToEnd();
            var result = $"{stdout}\n{stderr}";

            process.Kill(true);
            process.Close();

            return result;
        }
    }
}