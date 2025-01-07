﻿using Not.Logging;
using Not.Logging.Builder;
using Serilog;

namespace Not.MAUI.Logging;

public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Configures logging provider and <seealso cref="Microsoft.Extensions.Logging.ILogger{TCategoryName}" /> service implementation
    /// </summary>
    /// <param name="mauiBuilder">MauiAppBuilder instance</param>/param>
    /// <returns>Instance of NotLogBuilder to be used to configure specific loggers</returns>
    public static NLogBuilder ConfigureLogging(this MauiAppBuilder mauiBuilder)
    {
        mauiBuilder.Logging.AddSerilog();
        return mauiBuilder.Services.AddNLogging();
    }
}
