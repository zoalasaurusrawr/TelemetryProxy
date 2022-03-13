using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TelemetryProxy.Extensions.DependencyInjection;
public static class Extensions
{
    public static void AddProxiedScoped<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddScoped<TInterface, TImplementation>(factory =>
        {
            var target = ActivatorUtilities.CreateInstance<TImplementation>(services.BuildServiceProvider());
            var result = ActivityProxy<TInterface>.CreateWithTarget<TImplementation>(target);
            return result;
        });
    }

    public static void AddScopedWithInterception<TInterface, TImplementation, TProxyType>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
        where TProxyType : DispatchProxy
    {
        services.AddScoped<TInterface, TImplementation>(factory =>
        {
            var proxy = (DispatchProxy)ActivatorUtilities.CreateInstance<TProxyType>(services.BuildServiceProvider());
        });
    }
}
