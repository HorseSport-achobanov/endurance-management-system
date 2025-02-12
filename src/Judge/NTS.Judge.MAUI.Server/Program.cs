using Not.Startup;
using NTS.Application;
using NTS.Judge.MAUI.Server;
using NTS.Judge.MAUI.Server.ACL;
using NTS.Judge.MAUI.Server.RPC;
using Not.Logging.Builder;
using Not.Filesystem;
using Not.Contexts;

var builder = WebApplication.CreateBuilder();

builder.Services.ConfigureHub();

builder.ConfigureLogging().AddFilesystemLogger(logFileConfig =>
{
    logFileConfig.Path = FileContextHelper.GetAppDirectory();
    logFileConfig.Name = ContextHelper.ConfigureApplicationName("NTS.Judge.Server");

});



var app = builder.Build();

app.Urls.Add("http://*:11337");

app.MapHub<JudgeRpcHub>(ApplicationConstants.JUDGE_HUB);
app.MapHub<WitnessRpcHub>(Constants.RPC_ENDPOINT); // TODO: change to NtsApplicationConstants.WITNESS_HUB

//var a = app.Services.GetRequiredService<IHubContext<WitnessRpcHub, IEmsClientProcedures>>();
//Console.WriteLine(a.Groups);

foreach (var initializer in app.Services.GetServices<IStartupInitializer>())
{
    initializer.RunAtStartup();
}

app.Run();
