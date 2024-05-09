using System;
using Microsoft.Extensions.Logging;
using OrderReader.Core.DataModels;
using Serilog;
using Velopack;

namespace OrderReader;

public static class Program
{
    public static ILoggerFactory LoggerFactory { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("../AppData/Logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger);
            var logger = LoggerFactory.CreateLogger("VelopackLogger");
            
            VelopackApp.Build()
                .WithBeforeUpdateFastCallback(_ =>
                {
                    Settings.Initialize();
                    var backupCreated = Settings.BackupConfigs();
                    logger.LogDebug(backupCreated ? "Config backup saved." : "Could not backup the configs.");
                })
                .Run(logger);
            
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}