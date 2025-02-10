using Not.Exceptions;

namespace Not.Contexts;

public static class ContextHelper
{
    public static string ConfigureApplicationName(string applicationName)
    {
        _applicationName = applicationName;
        return _applicationName;
    }

    static string? _applicationName;
}
