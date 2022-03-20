using Lamar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelemetryProxy.Extensions.DependencyInjection;
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
                x.AddProxiedSingleton<ITestService, TestService>();
            });

            var service = container.GetInstance<ITestService>();
            Assert.NotNull(service);
            service.DoWork();
        }
    }
}
