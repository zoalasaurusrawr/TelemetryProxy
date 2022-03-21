using System.Diagnostics;

namespace TelemetryProxy
{
    public interface IInterceptionProxy<TInterface> where TInterface : class
    {
        //ActivitySource? ActivitySource { get; }
        TInterface? Target { get; set; }
    }
}