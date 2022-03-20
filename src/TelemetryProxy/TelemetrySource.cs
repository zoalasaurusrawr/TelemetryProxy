using System.Diagnostics;
using System.Reflection;

namespace TelemetryProxy
{
    public static class TelemetrySource
    {
        public static ActivitySource ActivitySource { get; } = new ActivitySource(GetActivitySourceName());

        private static string GetActivitySourceName()
        {
            return Assembly.GetEntryAssembly()?.GetName().Name ?? Assembly.GetExecutingAssembly()?.GetName()?.Name ?? nameof(TelemetryProxy);
        }
    }
}
