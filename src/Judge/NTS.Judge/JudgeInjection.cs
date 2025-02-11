using Microsoft.Extensions.DependencyInjection;
using Not.Startup;
using NTS.Domain.Core.StaticOptions;

namespace NTS.Judge;

public static class JudgeInjection
{
    /// <summary>
    /// Necessary to be called directly from UI project, otherwise the runtime treeshakes this
    /// DLL off, because no resources are explicitly referenced.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureJudge(this IServiceCollection services)
    {
        return services.AddSingleton<IStartupInitializer, StaticOption>();
    }
}
