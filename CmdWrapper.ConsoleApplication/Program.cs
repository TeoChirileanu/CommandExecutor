using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;

namespace CmdWrapper.ConsoleApplication
{
    internal static class Program // todo: make async
    {
        private static void Main()
        {
            Run();
        }

        private static void Run()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "watch");
            if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
            
            var watcher = new FileSystemWatcher(tempDir);
            ICommandExecutor commandExecutor = new CommandExecutor();

            var foo = Observable.FromEventPattern(watcher, nameof(watcher.Created))
                .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                .Do(file => Console.WriteLine($"Saw file {file} "))
                .Select(File.ReadAllText)
                .Where(content => !string.IsNullOrWhiteSpace(content))
                .Do(command => Console.WriteLine($"Executing {command}"))
                .Select(command => commandExecutor.ExecuteCommand(command))
                .Subscribe(
                    result => Console.WriteLine($"Result is {result}"),
                    ex => Console.WriteLine($"Got an exception: {ex}"),
                    () => Console.WriteLine($"Stopped watching {tempDir}"));

            Observable.FromEventPattern(watcher, nameof(watcher.Error))
                .Select(data => ((ErrorEventArgs) data.EventArgs).GetException())
                .Subscribe(ex => Console.WriteLine($"Got error: {ex}"));

            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"\nStart watching {tempDir}\n");
            
            var tempFile = Path.Combine(tempDir, Path.GetRandomFileName());
            File.WriteAllText(tempFile, "hostname");

            watcher.Dispose();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }
}