using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Network
{
    public class InvokeException : Exception
    {
        public InvokeException(string message) : base(message)
        {
        }

    }
}
