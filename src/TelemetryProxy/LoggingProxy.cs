using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace TelemetryProxy;

public class LoggingProxy<TInterface> : DispatchProxy, IInterceptionProxy<TInterface>
    where TInterface : class
{
    public IOptions<TelemetryProxyOptions> Options { get; internal set; } = TelemetryProxyOptions.Default;
    protected ILoggerFactory LoggerFactory { get; private set; } = new LoggerFactory();
    protected ILogger Logger { get; private set; } = new LoggerFactory().CreateLogger<TInterface>();

    public TInterface? Target { get; set; }
    private const string NullValue = "null";
    private const string Primitive = "primitive";
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        var stringBuilder = new StringBuilder();

        try
        {
            if (targetMethod is null)
                return null;
            if (IsNamespaceIgnored(targetMethod) || IsTypeIgnored(targetMethod) || IsAccessibilityIgnored(targetMethod))
                return null;

            Logger = LoggerFactory.CreateLogger(targetMethod?.DeclaringType?.Name ?? typeof(TInterface).Name);

            stringBuilder.AppendLine(targetMethod?.Name ?? "Unknown");

            if (Logger != null)
                CaptureArgs(stringBuilder, targetMethod, args);

            var result = targetMethod.Invoke(Target, args);
            Logger.LogInformation(stringBuilder.ToString());
            return result;
        }
        catch (TargetInvocationException targetInvocationException)
        {
            if (targetInvocationException.InnerException is not null)
            {
                Logger.LogError(targetInvocationException.InnerException, stringBuilder.ToString());
                throw targetInvocationException.InnerException;
            }
            return null;
        }
        catch
        {
            throw;
        }
    }

    private bool IsNamespaceIgnored(MethodInfo? method)
    {
        if (method is null)
            return true;

        var declaringTypeNamespace = method.DeclaringType?.Namespace ?? string.Empty;
        return Options.Value.IgnoredNamespaces.Contains(declaringTypeNamespace) ||
            Options.Value.IgnoredNamespaces.Any(_ => _.StartsWith(declaringTypeNamespace));
    }

    private bool IsTypeIgnored(MethodInfo? method)
    {
        if (method is null)
            return true;

        var declaringTypeName = method.DeclaringType?.Name ?? string.Empty;
        return Options.Value.IgnoredNamespaces.Contains(declaringTypeName) ||
            Options.Value.IgnoredNamespaces.Any(_ => _.StartsWith(declaringTypeName));
    }

    private bool IsAccessibilityIgnored(MethodInfo? method)
    {
        if (method is null)
            return true;

        if (method.IsPrivate && Options.Value.IgnorePrivate)
            return true;

        return false;
    }

    private void CaptureArgs(StringBuilder stringBuilder!!, MethodInfo targetMethod!!, object?[]? args)
    {
        if (args is null)
            return;

        stringBuilder.AppendLine("\tParameters:");

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var argName = $"arg{i}";
            var parameters = targetMethod.GetParameters();
            if (i < parameters.Length)
                argName = parameters[i].Name;

            var readableArg = GetReadableArgValue(arg);
            stringBuilder.AppendLine($"\t\t{argName}: {readableArg}");
        }
    }

    private string GetReadableArgValue(object? arg)
    {
        var type = arg?.GetType() ?? typeof(object);
        if (arg is null)
            return NullValue;
        if (type.IsPrimitive)
            return arg.ToString() ?? Primitive;

        return arg.ToString() ?? $"[{type.Name}]";
    }

    public static TInterface Create<TImplementation>()
        where TImplementation : TInterface
    {
        var activitySourceName = GetActivitySourceName<TImplementation>();
        return Create<TImplementation>(TelemetrySource.ActivitySource, new LoggerFactory());
    }

    public static TInterface Create<TImplementation>(ActivitySource activitySource, ILoggerFactory loggerFactory)
        where TImplementation : TInterface
    {
        var proxy = Create<TInterface, LoggingProxy<TInterface>>() as LoggingProxy<TInterface>;
        if (proxy is not null)
        {
            proxy.Target = Activator.CreateInstance<TImplementation>();
            proxy.LoggerFactory = loggerFactory;
        }
        TInterface result = proxy as TInterface ?? throw new Exception("An error occurred during proxy creation");
        return result;
    }

    public static TInterface Create<TImplementation>(ILoggerFactory loggerFactory, params object?[]? args)
        where TImplementation : TInterface
    {
        var activitySourceName = GetActivitySourceName<TImplementation>();
        return Create<TImplementation>(TelemetrySource.ActivitySource, loggerFactory, args);
    }

    public static TInterface CreateWithTarget<TImplementation>(TImplementation target!!, ILoggerFactory loggerFactory)
        where TImplementation : class, TInterface
    {
        var activitySourceName = GetActivitySourceName<TImplementation>();
        return CreateWithTarget<TImplementation>(TelemetrySource.ActivitySource, target, loggerFactory, TelemetryProxyOptions.Default);
    }

    public static TInterface CreateWithTarget<TImplementation>(TImplementation target!!, ILoggerFactory loggerFactory, IOptions<TelemetryProxyOptions> options!!)
        where TImplementation : class, TInterface
    {
        var activitySourceName = GetActivitySourceName<TImplementation>();
        return CreateWithTarget<TImplementation>(TelemetrySource.ActivitySource, target, loggerFactory, options);
    }

    public static TInterface Create<TImplementation>(ActivitySource activitySource, ILoggerFactory loggerFactory, params object?[]? args)
        where TImplementation : TInterface
    {
        var proxy = Create<TInterface, LoggingProxy<TInterface>>() as LoggingProxy<TInterface>;
        TImplementation? target = (TImplementation?)Activator.CreateInstance(typeof(TImplementation), args);

        if (proxy is not null && target is not null)
        {
            proxy.Target = target;
            proxy.LoggerFactory = loggerFactory;
        }
        TInterface result = proxy as TInterface ?? throw new Exception("An error occurred during proxy creation");
        return result;
    }

    public static TInterface CreateWithTarget<TImplementation>(ActivitySource activitySource!!, TImplementation target!!, ILoggerFactory loggerFactory, IOptions<TelemetryProxyOptions> options!!)
        where TImplementation : class, TInterface
    {
        var proxy = Create<TInterface, LoggingProxy<TImplementation>>() as LoggingProxy<TImplementation>;

        if (proxy is not null && target is not null)
        {
            proxy.Target = target;
            proxy.Options = options;
            proxy.LoggerFactory = loggerFactory;
        }
        TInterface result = proxy as TInterface ?? throw new Exception("An error occurred during proxy creation");
        return result;// as TImplementation ?? throw new Exception($"Could not convert interface: {typeof(TInterface).Name} to implementation: {typeof(TImplementation).Name}");
    }
    /// <summary>
    /// Try to get a suitable name for the activity source by
    /// first trying to get the assembly name for the implementation
    /// and if null, then the type name, and then the entry assembly name
    /// and finally string.empty
    /// 
    /// TODO: Should throw exception if we get to string.empty?
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    private static string GetActivitySourceName<TImplementation>()
        where TImplementation : TInterface
    {
        var type = typeof(TInterface);
        return type?.Assembly?.FullName ?? type?.Name ?? Assembly.GetEntryAssembly()?.FullName ?? string.Empty;
    }
}