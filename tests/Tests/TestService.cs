using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal interface ITestService
    {
        void DoWork();
        void DoWork(object arg);
        void DoNullableWork(object arg, object? arg2 = null);
    }

    internal class TestService : ITestService
    {
        public TestService()
            : this(new object())
        {
        }

        public TestService(object arg)
        {
            _arg = arg;
        }

        private readonly object _arg;

        public void DoWork()
        {
            
        }

        public void DoWork(object arg)
        {

        }

        public void DoNullableWork(object arg, object? arg2 = null)
        {

        }
    }
}
