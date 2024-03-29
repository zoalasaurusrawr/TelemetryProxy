﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TelemetryProxy;
public static class Extensions
{
    public static IServiceCollection AddTelemetryProxy(this IServiceCollection services)
    {
        services.AddOptions();
        var provider = services.BuildServiceProvider();
        var config = provider.GetService<IConfiguration>();

        if (config is null)
        {
            services.AddTelemetryProxy(cfg =>
            {
                cfg.IgnoredNamespaces = new string[0];
                cfg.IgnoredTypes = new string[0];
                cfg.IgnorePrivate = false;
            });
        }
        else
        {
            var options = new TelemetryProxyOptions(new string[0], new string[0]);
            config.GetSection("TelemetryProxy").Bind(options);
            services.Configure<TelemetryProxyOptions>(config.GetSection("TelemetryProxy"));
        }
        return services;
    }

    public static IServiceCollection AddTelemetryProxy(this IServiceCollection services, Action<TelemetryProxyOptions> configure)
    {
        services.AddOptions();
        services.Configure(configure);
        return services;
    }

    public static IServiceCollection AddTracedScoped<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddScoped<TInterface>(factory =>
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<TelemetryProxyOptions>>();
            var target = ActivatorUtilities.CreateInstance<TImplementation>(provider);
            var result = ActivityProxy<TInterface>.CreateWithTarget<TImplementation>(target, options);
            return result;
        });
        return services;
    }

    public static IServiceCollection AddTracedTransient<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddTransient<TInterface>(factory =>
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<TelemetryProxyOptions>>();
            var target = ActivatorUtilities.CreateInstance<TImplementation>(provider);
            var result = ActivityProxy<TInterface>.CreateWithTarget<TImplementation>(target, options);
            return result;
        });
        return services;
    }

    public static IServiceCollection AddTracedSingleton<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddSingleton<TInterface>(factory =>
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<TelemetryProxyOptions>>();
            var target = ActivatorUtilities.CreateInstance<TImplementation>(provider);
            var result = ActivityProxy<TInterface>.CreateWithTarget<TImplementation>(target, options);
            return result;
        });
        return services;
    }

    public static IServiceCollection AddLoggedScoped<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddScoped<TInterface>(factory =>
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<TelemetryProxyOptions>>();
            var target = ActivatorUtilities.CreateInstance<TImplementation>(provider);
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var result = LoggingProxy<TInterface>.CreateWithTarget<TImplementation>(target, loggerFactory, options);
            return result;
        });
        return services;
    }

    public static IServiceCollection AddLoggedTransient<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddTransient<TInterface>(factory =>
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<TelemetryProxyOptions>>();
            var target = ActivatorUtilities.CreateInstance<TImplementation>(provider);
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var result = LoggingProxy<TInterface>.CreateWithTarget<TImplementation>(target, loggerFactory, options);
            return result;
        });
        return services;
    }

    public static IServiceCollection AddLoggedSingleton<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddSingleton<TInterface>(factory =>
        {
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<TelemetryProxyOptions>>();
            var target = ActivatorUtilities.CreateInstance<TImplementation>(provider);
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var result = LoggingProxy<TInterface>.CreateWithTarget<TImplementation>(target, loggerFactory, options);
            return result;
        });
        return services;
    }
}
