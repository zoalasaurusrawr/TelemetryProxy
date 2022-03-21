using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using System;
using System.Diagnostics;

namespace Tests.Fixtures
{
    public class ProxyTestFixture : IDisposable
    {
        public ProxyTestFixture()
        {
            ActivitySource.AddActivityListener(new ActivityListener());
            ActivitySource = new ActivitySource(nameof(ProxyTestFixture));
            LoggerFactory = new LoggerFactory();
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService("Samples");
            ResourceBuilder = resourceBuilder;
        }

        public ActivitySource ActivitySource { get; }
        public ResourceBuilder ResourceBuilder { get; }
        public ILoggerFactory LoggerFactory { get; }

        public void Dispose()
        {
        }
    }
}
