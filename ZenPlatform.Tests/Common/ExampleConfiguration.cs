using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Tests.Common
{
    /// <summary>
    /// Пример конфигурации
    /// </summary>
    public static class ExampleConfiguration
    {
        private const string ConfigurationPath = "../../../../Build/Debug/ExampleConfiguration/Configuration";

        public static XCRoot GetExample()
        {
            return XCRoot.Load(new XCFileSystemStorage(ConfigurationPath, "Project.xml"));
        }
    }
}
