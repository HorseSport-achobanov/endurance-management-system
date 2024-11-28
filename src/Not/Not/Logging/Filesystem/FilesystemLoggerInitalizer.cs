﻿using Not.Startup;
using Serilog;

namespace Not.Logging.Filesystem;

public class FilesystemLoggerInitalizer : IStartupInitializer
{
    readonly IFilesystemLoggerConfiguration _configuration;

    public FilesystemLoggerInitalizer(IFilesystemLoggerConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void RunAtStartup()
    {
        LoggingHelper.Validate();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File($"{_configuration.Path}/log.txt")
            .CreateLogger();
    }
}
