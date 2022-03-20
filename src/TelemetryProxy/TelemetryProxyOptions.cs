using Microsoft.Extensions.Options;

namespace TelemetryProxy;

public class TelemetryProxyOptions
{
    public TelemetryProxyOptions()
    {
        IgnoredNamespaces = new string[0];
        IgnoredTypes = new string[0];
        IgnorePrivate = false;
    }

    public TelemetryProxyOptions(string[] ignoredNamespaces, string[] ignoredTypes, bool ignorePrivate = false)
    {
        IgnoredNamespaces = ignoredNamespaces ?? throw new ArgumentNullException(nameof(ignoredNamespaces));
        IgnoredTypes = ignoredTypes ?? throw new ArgumentNullException(nameof(ignoredTypes));
        IgnorePrivate = ignorePrivate;
    }

    public string[] IgnoredNamespaces { get; set; }
    public string[] IgnoredTypes { get; set; }
    public bool IgnorePrivate { get; set; }

    private static string[] _secretNames = { "secret", "password", "connectionstring", "accesskey" };
    public static string[] SecretNames => _secretNames;
    public static IOptions<TelemetryProxyOptions> Default = Options.Create(new TelemetryProxyOptions());
}
