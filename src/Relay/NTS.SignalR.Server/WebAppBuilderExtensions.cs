using Not.Logging.Builder;
using Not.Logging;
using Serilog;

namespace NTS.SignalR.Server;

public static class WebAppBuilderExtensions
{
    public static NLogBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
        builder.Logging.AddSerilog();
        return builder.Services.AddNLogging();
    }
}
