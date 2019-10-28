using System;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;

namespace ZenPlatform.ConfigurationExample
{
    /// <summary>
    /// Пример конфигурации
    /// </summary>
    public static class Factory
    {
        public static string GetDatabaseConnectionString() => "Host=db1; Username=user; Password=password;";

        public static XCRoot CreateExampleConfiguration()
        {
            var root = new XCRoot();

            root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a"); //Guid.NewGuid();
            root.ProjectName = "Library";
            root.ProjectVersion = "0.0.0.1";

            var component = new XCComponent()
            {
                Blob = new XCBlob("ZenPlatform.EntityComponent.dll"),
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            var store = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();
            var invoice = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();

            store.Name = "Store";
            store.Description = "This is a store entity";
            store.Initialize();

            var storeProp = invoice.CreateProperty();
            storeProp.Name = "Store";
            storeProp.Types.Add(store);

            var prop = invoice.CreateProperty();
            prop.Name = "CompositeProperty";
            prop.Types.Add(new XCBinary());
            prop.Types.Add(new XCBoolean());
            prop.Types.Add(new XCString());
            prop.Types.Add(new XCDateTime());
            prop.Types.Add(store);

            prop.DatabaseColumnName = "Fld_0001";

            invoice.Name = "Invoice";

            invoice.Modules.Add(new XCSingleEntityModule()
            {
                ModuleText = "public int Test(int i) { int _i = i; _i++; return _i; }",
                ModuleRelationType = XCProgramModuleRelationType.Object
            });

            var cmd = invoice.CreateCommand();
            cmd.Name = "HelloFromServer";
            cmd.Module.ModuleText = @"

[ClientCall] 
public int ClientCallProc(int a)
{ 
    a++; 
    return a; 
}

[Client]
public void OnClientClientCallProc()
{
    ClientCallProc(10);
}";
            cmd.DisplayName = "Invoke the command";

            invoice.Initialize();

            return root;
        }
    }
}