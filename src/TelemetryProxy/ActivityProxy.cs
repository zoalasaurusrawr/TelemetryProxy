using System.Diagnostics;
using System.Reflection;

namespace TelemetryProxy;

public class ActivityProxy<TInterface> : DispatchProxy, IInterceptionProxy<TInterface> 
    where TInterface : class
{
    public ActivitySource? ActivitySource { get; internal set; }
    public TInterface? Target { get; set; }
    private const string NullValue = "null";
    private const string Primitive = "primitive";
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        try
        {
            if (targetMethod is null)
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

    private void CaptureArgs(Activity activity!!, MethodInfo targetMethod!!, object?[]? args)
    {
        if (args is null)
            return;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var readableArg = GetReadableArgValue(arg);
            activity.SetTag($"arg{i}", readableArg);
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
        var activitySource = new ActivitySource(activitySourceName);
        return Create<TImplementation>(activitySource);
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
        var activitySource = new ActivitySource(activitySourceName);
        return Create<TImplementation>(activitySource, args);
    }

    public static TImplementation CreateWithTarget<TImplementation>(TImplementation target)
        where TImplementation : class, TInterface
    {
        var activitySourceName = GetActivitySourceName<TImplementation>();
        var activitySource = new ActivitySource(activitySourceName);
        return CreateWithTarget<TImplementation>(activitySource, target);
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

    public static TImplementation CreateWithTarget<TImplementation>(ActivitySource activitySource, TImplementation target)
        where TImplementation : class, TInterface
    {
        var proxy = Create<TInterface, ActivityProxy<TInterface>>() as ActivityProxy<TInterface>;

        if (proxy is not null && target is not null)
        {
            proxy.Target = target;
            proxy.ActivitySource = activitySource;
        }
        TInterface result = proxy as TInterface ?? throw new Exception("An error occurred during proxy creation");
        return result as TImplementation ?? throw new Exception($"Could not convert interface: {typeof(TInterface).Name} to implementation: {typeof(TImplementation).Name}");
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
