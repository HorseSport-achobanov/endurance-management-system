using NTS.Application;
using NTS.Judge.MAUI.Server.ACL;
using NTS.Judge.MAUI.Server.RPC;
using NTS.Judge.MAUI.Server;
using Microsoft.AspNetCore.SignalR;
using Not.Startup;
using Not.Process;

var builder = WebApplication.CreateBuilder(args);

if(args != null)
{
    var parentPid = int.Parse(args[0]);
    ProcessHelper.MonitorParentProcess(parentPid);
}

builder.Services.ConfigureHub();
builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
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
