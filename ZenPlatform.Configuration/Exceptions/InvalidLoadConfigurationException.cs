using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration.Exceptions
{
    public class InvalidLoadConfigurationException : Exception
    {
        public InvalidLoadConfigurationException()
        {

        }

        public InvalidLoadConfigurationException(string message) : base(message)
        {

        }
    }
}
