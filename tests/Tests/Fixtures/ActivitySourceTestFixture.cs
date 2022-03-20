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
        }

        public ActivitySource ActivitySource { get; }

        public void Dispose()
        {
        }
    }
}
