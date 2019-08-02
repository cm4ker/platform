using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;

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

        public static XCRoot CreateExampleConfiguration()
        {
            var root = new XCRoot();

            root.ProjectId = Guid.NewGuid();
            root.ProjectName = "Library";
            root.ProjectVersion = "0.0.0.1";

            var component = new XCComponent()
            {
                Blob = new XCBlob("ZenPlatform.EntityComponent.dll"),
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            var store = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();
            store.Name = "Store";
            store.Description = "This is a store entity";
            store.Initialize();

            var prop = new XCSingleEntityProperty();
            prop.Name = "CompositeProperty";
            prop.Types.Add(new XCBinary());
            prop.Types.Add(new XCBoolean());
            prop.Types.Add(new XCString());
            prop.Types.Add(new XCDateTime());
            prop.Types.Add(store);

            prop.DatabaseColumnName = "Fld_0001";

            var invoice = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();
            invoice.Properties.Add(prop);
            invoice.Name = "Invoice";

            invoice.Initialize();

            return root;
        }
    }
}