using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TelemetryProxy.Extensions.DependencyInjection;
public static class Extensions
{
    public static void AddProxiedScoped<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddScoped<TInterface>(factory =>
        {
            var target = ActivatorUtilities.CreateInstance<TImplementation>(services.BuildServiceProvider());
            var result = ActivityProxy<TInterface>.CreateWithTarget<TImplementation>(target);
            return result;
        });
    }

    public static void AddProxiedTransient<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddTransient<TInterface>(factory =>
        {
            var target = ActivatorUtilities.CreateInstance<TImplementation>(services.BuildServiceProvider());
            var result = ActivityProxy<TInterface>.CreateWithTarget<TImplementation>(target);
            return result;
        });
    }

    public static void AddProxiedSingleton<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddSingleton<TInterface>(factory =>
        {
            var target = ActivatorUtilities.CreateInstance<TImplementation>(services.BuildServiceProvider());
            var result = ActivityProxy<TInterface>.CreateWithTarget<TImplementation>(target);
            return result;
        });
    }
}
