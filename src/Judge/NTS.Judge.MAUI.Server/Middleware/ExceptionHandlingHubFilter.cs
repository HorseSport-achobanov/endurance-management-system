using Microsoft.AspNetCore.SignalR;
using Not.Logging;
using Not.Notify;

namespace NTS.Judge.MAUI.Server.Middleware;

public class ExceptionHandlingHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
    HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        Console.WriteLine($"Calling hub method '{invocationContext.HubMethodName}'");
        try
        {
            return await next(invocationContext);
        }
        catch (HubException ex)
        {
            HandleHubException(ex, invocationContext.HubMethodName);
            return null;
        }
    }

    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        try
        {
            return next(context);
        }
        catch (HubException ex)
        {
            HandleHubException(ex, "OnConnectedAsync");
            return Task.FromException(ex);
        }
    }

    public Task OnDisconnectedAsync(
        HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
    {
        try
        {
            return next(context, exception);
        }
        catch (HubException ex)
        {
            HandleHubException(ex, "OnDisconnectedAsync");
            return Task.FromException(ex);
        }
    }

    public void HandleHubException(HubException hubException, string methodName)
    {
        NotifyHelper.Error(hubException);
        var logMessage =
                    $"An error {hubException.Message} was thrown calling {methodName} " +
                    $"at {hubException.Source} with trace \n {hubException.StackTrace}";
        LoggingHelper.Error(logMessage);
    }
}
