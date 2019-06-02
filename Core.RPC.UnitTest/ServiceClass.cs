using System;
using System.Collections.Generic;
using System.Text;

namespace Core.RPC.Tests
{
    class ServiceClass
    {
        public int value;

        public int Sum(int x, int y)
        {
            value = x + y;
            return value;
        }
    }

    class ServiceClassImpl : IServiceInterface
    {
        public int Sum(int x, int y)
        {
            return x + y;
        }

        public int SomeFunctionOutOfService(int i)
        {
            return i;
        }
    }
}
