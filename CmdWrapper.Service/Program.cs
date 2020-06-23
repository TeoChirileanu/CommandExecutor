using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CmdWrapper.Service
{
    public static class Program
    {
        internal const string LogFile = @"c:\tmp\watch\log.txt";
        
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                // run in advance: PS:> New-EventLog -LogName CommandExecutorLogs -Source Executor
                .WriteTo.EventLog("Executor", "CommandExecutorLogs")
                .WriteTo.File(LogFile)
                .WriteTo.Console()
                .WriteTo.Trace()
                .CreateLogger();
            
            try
            {
                Log.Information("Worker started");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Dunno what happened, but I'm dead");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindowsService()
                .ConfigureServices((_, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}