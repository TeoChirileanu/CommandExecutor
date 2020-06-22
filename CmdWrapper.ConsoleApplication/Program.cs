using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace CmdWrapper.ConsoleApplication
{
    internal static class Program
    {
        private static void Main() => Run();

        private static void Run()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "watch");
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            Directory.CreateDirectory(tempDir);
            
            var watcher = new FileSystemWatcher(tempDir);

            var disposable = Observable.FromEventPattern(watcher, nameof(watcher.Created))
                .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                .Select(GetContentFromFile)
                .Switch()
                .Where(content => !string.IsNullOrWhiteSpace(content))
                .Do(command => Console.WriteLine($">{command}"))
                .Select(ExecuteCommandAndGetResult)
                .Switch()
                .Subscribe(Console.WriteLine, ex => Console.WriteLine($"Got an error: {ex}"));

            Console.WriteLine($"\nStart watching {tempDir}\n");
            watcher.EnableRaisingEvents = true;

            var tempFile = Path.Combine(tempDir, Path.GetRandomFileName());
            File.WriteAllText(tempFile, "ping localhost");

            Console.ReadKey();

            Console.WriteLine($"Stop watching {tempDir}");
            disposable.Dispose();
            watcher.Dispose();
        }

        private static BehaviorSubject<string> ExecuteCommandAndGetResult(string command)
        {
            var subject = new BehaviorSubject<string>(string.Empty);
            try
            {
                subject.OnNext(StaticCommandExecutor.ExecuteCommand(command));
            }
            catch (Exception e)
            {
                subject.OnError(e);
            }

            return subject;
        }

        private static BehaviorSubject<string> GetContentFromFile(string file)
        {
            var subject = new BehaviorSubject<string>(string.Empty);
            try { subject.OnNext(File.ReadAllText(file)); }
            catch (Exception e) { subject.OnError(e); }
            return subject;
        }
    }
}