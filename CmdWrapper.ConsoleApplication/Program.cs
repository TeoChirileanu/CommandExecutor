using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;

namespace CmdWrapper.ConsoleApplication
{
    internal static class Program
    {
        private static void Main()
        {
            Run();
        }

        private static void Run()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "watch");
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            Directory.CreateDirectory(tempDir);
            
            var watcher = new FileSystemWatcher(tempDir);

            var disposable = Observable.FromEventPattern(watcher, nameof(watcher.Created))
                .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                .Do(file => Console.WriteLine($"Saw file {file} "))
                .Select(File.ReadAllText)
                .Where(content => !string.IsNullOrWhiteSpace(content))
                .Do(command => Console.WriteLine($"Executing {command}"))
                .Select(command => Observable.Return(StaticCommandExecutor.ExecuteCommand(command)))
                .Switch()
                .Subscribe(result => Console.WriteLine($"Result is {result}"));

            Observable.FromEventPattern(watcher, nameof(watcher.Error))
                .Select(data => ((ErrorEventArgs) data.EventArgs).GetException())
                .Subscribe(ex => Console.WriteLine($"Got error: {ex}"));

            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"\nStart watching {tempDir}\n");
            
            var tempFile = Path.Combine(tempDir, Path.GetRandomFileName());
            File.WriteAllText(tempFile, "hostname");
            
            Thread.Sleep(TimeSpan.FromMilliseconds(100));

            Console.WriteLine($"Stop watching {tempDir}");
            disposable.Dispose();
            watcher.Dispose();
        }
    }
}