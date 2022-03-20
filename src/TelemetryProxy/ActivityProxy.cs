using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;

namespace TelemetryProxy;

public class ActivityProxy<TInterface> : DispatchProxy, IInterceptionProxy<TInterface>
    where TInterface : class
{
    public ActivitySource? ActivitySource { get; internal set; } = TelemetrySource.ActivitySource;
    public IOptions<TelemetryProxyOptions> Options { get; internal set; } = TelemetryProxyOptions.Default;
    public TInterface? Target { get; set; }
    private const string NullValue = "null";
    private const string Primitive = "primitive";
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        try
        {
            if (targetMethod is null)
                return null;
            if (IsNamespaceIgnored(targetMethod) || IsTypeIgnored(targetMethod) || IsAccessibilityIgnored(targetMethod))
                return null;

            using var activity = ActivitySource?.StartActivity(targetMethod.Name);
            if (activity != null)
                CaptureArgs(activity, targetMethod, args);

            var result = targetMethod.Invoke(Target, args);
            return result;
        }
        catch (TargetInvocationException targetInvocationException)
        {
            if (targetInvocationException.InnerException is not null)
                throw targetInvocationException.InnerException;

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

    private void CaptureArgs(Activity activity!!, MethodInfo targetMethod!!, object?[]? args)
    {
        if (args is null)
            return;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var argName = $"arg{i}";
            var parameters = targetMethod.GetParameters();
            if (i < parameters.Length)
                argName = parameters[i].Name;

            var readableArg = GetReadableArgValue(arg);
            activity.SetTag(argName, readableArg);
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
        return Create<TImplementation>(TelemetrySource.ActivitySource);
    }

    public static TInterface Create<TImplementation>(ActivitySource activitySource)
        where TImplementation : TInterface
    {
        var proxy = Create<TInterface, ActivityProxy<TInterface>>() as ActivityProxy<TInterface>;
        if (proxy is not null)
        {
            proxy.Target = Activator.CreateInstance<TImplementation>();
            proxy.ActivitySource = activitySource;
        }
        TInterface result = proxy as TInterface ?? throw new Exception("An error occurred during proxy creation");
        return result;
    }

    public static TInterface Create<TImplementation>(params object?[]? args)
        where TImplementation : TInterface
    {
        var activitySourceName = GetActivitySourceName<TImplementation>();
        return Create<TImplementation>(TelemetrySource.ActivitySource, args);
    }

    public static TInterface CreateWithTarget<TImplementation>(TImplementation target!!)
        where TImplementation : class, TInterface
    {
        var activitySourceName = GetActivitySourceName<TImplementation>();
        return CreateWithTarget<TImplementation>(TelemetrySource.ActivitySource, target, TelemetryProxyOptions.Default);
    }

    public static TInterface CreateWithTarget<TImplementation>(TImplementation target!!, IOptions<TelemetryProxyOptions> options!!)
        where TImplementation : class, TInterface
    {
        var activitySourceName = GetActivitySourceName<TImplementation>();
        return CreateWithTarget<TImplementation>(TelemetrySource.ActivitySource, target, options);
    }

    public static TInterface Create<TImplementation>(ActivitySource activitySource, params object?[]? args)
        where TImplementation : TInterface
    {
        var proxy = Create<TInterface, ActivityProxy<TInterface>>() as ActivityProxy<TInterface>;
        TImplementation? target = (TImplementation?)Activator.CreateInstance(typeof(TImplementation), args);

        if (proxy is not null && target is not null)
        {
            proxy.Target = target;
            proxy.ActivitySource = activitySource;
        }
        TInterface result = proxy as TInterface ?? throw new Exception("An error occurred during proxy creation");
        return result;
    }

    public static TInterface CreateWithTarget<TImplementation>(ActivitySource activitySource!!, TImplementation target!!, IOptions<TelemetryProxyOptions> options!!)
        where TImplementation : class, TInterface
    {
        var proxy = Create<TInterface, ActivityProxy<TImplementation>>() as ActivityProxy<TImplementation>;

        if (proxy is not null && target is not null)
        {
            proxy.Target = target;
            proxy.ActivitySource = activitySource;
            proxy.Options = options;
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
