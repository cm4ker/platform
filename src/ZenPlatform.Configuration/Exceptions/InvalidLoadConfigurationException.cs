using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration.Exceptions
{
    /// <summary>
    /// Ошибка загрузки конфигурации
    /// </summary>
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
