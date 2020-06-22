using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CmdWrapper.ConsoleApplication
{
    internal static class Program
    {
        private static void Main() => Run();

        private static void Run()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "watch");
            if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
            
            var watcher = new FileSystemWatcher(tempDir) {EnableRaisingEvents = true};
            Console.WriteLine($"\nStart watching {tempDir}\n");

            Observable.FromEventPattern(watcher, nameof(watcher.Created))
                .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                .Do(file => Console.WriteLine($"Saw file {file} "))
                .Select(File.ReadAllText) // todo: make async
                .Where(content => !string.IsNullOrWhiteSpace(content))
                .Subscribe(
                    content => Console.Write($"having content {content}"),
                    ex => Console.WriteLine(ex.ToString()));

            foreach (var _ in Enumerable.Range(0, 10))
            {
                var tempFile = Path.Combine(tempDir, Path.GetRandomFileName());
                var randomContent = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                File.WriteAllText(tempFile, randomContent);
            }
            
            watcher.Dispose();
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            Console.WriteLine($"\nStopped watching {tempDir}\n");
        }
    }
}