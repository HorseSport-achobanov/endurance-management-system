using System.Diagnostics;

namespace Not.Contexts;

public static class ContextHelper
{
    public static string ConfigureApplicationName(string applicationName)
    {
        _applicationName = applicationName;
        return _applicationName;
    }

    public static string ConfigureApplicationName() 
    {
        var currentProcess = Process.GetCurrentProcess();
        return _applicationName = currentProcess.ProcessName;
    }

    static string? _applicationName;
}
