﻿using System;
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
            
            var watcher = new FileSystemWatcher(tempDir, "command.txt");

            var disposable = Observable.FromEventPattern(watcher, nameof(watcher.Changed))
                .Sample(TimeSpan.FromMilliseconds(500))
                .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                .Select(GetContentFromFile)
                .Switch()
                .Where(command => !string.IsNullOrWhiteSpace(command))
                .Do(command => Console.WriteLine($">{command}"))
                .Select(ExecuteCommandAndGetResult)
                .Switch()
                .Subscribe(Console.WriteLine, ex => Console.WriteLine($"Got an error: {ex}"));

            Console.WriteLine($"\nStart watching {tempDir}\n");
            watcher.EnableRaisingEvents = true;

            var tempFile = Path.Combine(tempDir, "command.txt");
            File.WriteAllText(tempFile, "echo foo");
            Thread.Sleep(1000);
            File.WriteAllText(tempFile, "hostname");
            Thread.Sleep(1000);

            Console.WriteLine($"Stop watching {tempDir}");
            disposable.Dispose();
            watcher.Dispose();
        }

        private static IObservable<string> ExecuteCommandAndGetResult(string command)
        {
            var subject = new BehaviorSubject<string>(string.Empty);
            try { subject.OnNext(StaticCommandExecutor.ExecuteCommand(command)); }
            catch (Exception e) { subject.OnError(e); }
            return subject.AsObservable();
        }

        private static IObservable<string> GetContentFromFile(string file)
        {
            var subject = new BehaviorSubject<string>(string.Empty);
            try { subject.OnNext(File.ReadAllText(file)); }
            catch (Exception e) { subject.OnError(e); }
            return subject.AsObservable();
        }
    }
}