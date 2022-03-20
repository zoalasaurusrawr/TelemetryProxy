using OpenTelemetry.Resources;
using System;
using System.Diagnostics;

namespace Tests.Fixtures
{
    public class ActivitySourceTestFixture : IDisposable
    {
        public ActivitySourceTestFixture()
        {
            ActivitySource.AddActivityListener(new ActivityListener());
            ActivitySource = new ActivitySource(nameof(ActivitySourceTestFixture));
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService("Samples");
            ResourceBuilder = resourceBuilder;
        }

        public ActivitySource ActivitySource { get; }
        public ResourceBuilder ResourceBuilder { get; }

        public void Dispose()
        {
        }
    }
}
