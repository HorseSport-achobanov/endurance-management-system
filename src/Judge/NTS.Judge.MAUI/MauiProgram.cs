using System.Diagnostics;
using NTS.Judge.RPC;

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
        var judgeClient = serviceProvider.GetRequiredService<IJudgeRpcClient>();
        judgeClient.Connect();
    }

    static void StartHub()
    {
        try
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var info = new ProcessStartInfo
            {
                FileName = Path.Combine(currentDirectory, "NTS.Judge.MAUI.Server.exe"),
            };

            var hubProcess = Process.Start(info);
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                if (hubProcess != null && !hubProcess.HasExited)
                {
                    hubProcess.CloseMainWindow();
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
