using Lamar;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelemetryProxy;
using Tests.Fixtures;
using Xunit;

namespace Tests
{
    public class ContainerTests : IClassFixture<ActivitySourceTestFixture>
    {
        public ActivitySourceTestFixture Fixture { get; }

        public ContainerTests(ActivitySourceTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void AddScoped_Succeeds()
        {
            var container = new Container(x =>
            {
                x.AddOpenTelemetryTracing((builder) => builder
                    .SetResourceBuilder(Fixture.ResourceBuilder)
                    .AddSource(TelemetrySource.ActivitySource.Name)
                    .AddConsoleExporter()
                );
                x.AddTelemetryProxy();
                x.AddProxiedScoped<ITestService, TestService>();
            });

            var service = container.GetInstance<ITestService>();
            Assert.NotNull(service);
            service.DoWork();
        }

        [Fact]
        public void AddTransient_Succeeds()
        {
            var container = new Container(x =>
            {
                x.AddOpenTelemetryTracing((builder) => builder
                    .SetResourceBuilder(Fixture.ResourceBuilder)
                    .AddSource(TelemetrySource.ActivitySource.Name)
                    .AddConsoleExporter()
                );
                x.AddTelemetryProxy();
                x.AddProxiedTransient<ITestService, TestService>();
            });

            var service = container.GetInstance<ITestService>();
            Assert.NotNull(service);
            service.DoWork();
        }

        [Fact]
        public void AddSingleton_Succeeds()
        {
            var container = new Container(x =>
            {
                x.AddOpenTelemetryTracing((builder) => builder
                    .SetResourceBuilder(Fixture.ResourceBuilder)
                    .AddSource(TelemetrySource.ActivitySource.Name)
                    .AddConsoleExporter()
                );
                x.AddTelemetryProxy();
                x.AddProxiedSingleton<ITestService, TestService>();
            });

            var service = container.GetInstance<ITestService>();
            Assert.NotNull(service);
            service.DoWork();
        }
    }
}
