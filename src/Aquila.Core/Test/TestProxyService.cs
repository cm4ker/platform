using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.Network
{
    public class TestProxyService : ITestProxyService
    {
        public int Sum(int a, int b)
        {
            return a + b;
        }
    }
}
