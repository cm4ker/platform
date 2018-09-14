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
    public static class Factory
    {
        private const string ConfigurationPath = "../../../../Build/Debug/ExampleConfiguration/Configuration";

        public static XCRoot GetExampleConfigutaion()
        {
            return XCRoot.Load(new XCFileSystemStorage(ConfigurationPath, "Project.xml"));
        }

        public static string GetDatabaseConnectionString() => "Host=db1; Username=user; Password=password;";
    }
}
