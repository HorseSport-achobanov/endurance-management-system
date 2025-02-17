using System.Diagnostics;
using Not.Application.RPC.Clients;
using NTS.Judge.RPC;
using static NTS.Judge.MAUI.Constants;

namespace NTS.Judge.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"))
            .ConfigureJudgeMaui();

        var app = builder.Build();

        ConnectToHub(app.Services);

        return app;
    }

    static void ConnectToHub(IServiceProvider serviceProvider)
    {
        StartHub();
        var connectionsClient = serviceProvider.GetRequiredService<IConnectionsRpcClient>();
        var participationClient = serviceProvider.GetRequiredService<IParticipationRpcClient>();    
        connectionsClient.Connect();
        participationClient.Connect();
    }

    static void StartHub()
    {
        try
        {
            var parentPid = Process.GetCurrentProcess().Id;
            var currentDirectory = Directory.GetCurrentDirectory();
            var info = new ProcessStartInfo
            {
                FileName = Path.Combine(currentDirectory, RELAY_APP_EXE),
                Arguments = parentPid.ToString()
            };

            var hubProcess = Process.Start(info);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
