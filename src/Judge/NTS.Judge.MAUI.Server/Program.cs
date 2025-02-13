﻿using Not.Startup;
using NTS.Application;
using NTS.Judge.MAUI.Server;
using NTS.Judge.MAUI.Server.ACL;
using NTS.Judge.MAUI.Server.RPC;
using Not.ProcessUtility;
using Not.Logging.Builder;
using Not.Filesystem;
using Not.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHub();

if (args.Length > 0)
{
    builder.Services.AddSingleton<ProcessServiceContext>(provider =>
    {
        var parentProcessID = args[0];
        return new ProcessServiceContext(parentProcessID);
    });
    builder.Services.AddHostedService<ProcessService>();
}

builder.ConfigureLogging().AddFilesystemLogger(logFileConfig =>
{
    logFileConfig.Path = FileContextHelper.GetAppDirectory();
    logFileConfig.Name = ContextHelper.ConfigureApplicationName("NTS.Judge.Server");

});

var app = builder.Build();

app.Urls.Add("http://*:11337");

app.MapHub<JudgeRpcHub>(ApplicationConstants.JUDGE_HUB);
app.MapHub<WitnessRpcHub>(Constants.RPC_ENDPOINT); // TODO: change to NtsApplicationConstants.WITNESS_HUB

foreach (var initializer in app.Services.GetServices<IStartupInitializer>())
{
    initializer.RunAtStartup();
}

app.Run();
