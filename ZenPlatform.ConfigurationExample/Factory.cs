using System;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
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
                
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            var store = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();
            store.Guid = Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5");
            var invoice = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();
            invoice.Guid = Guid.Parse("b9fee6cd-a834-4d72-9de5-1fc2087467e5");
            var department = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();
            department.Guid = Guid.Parse("953120f4-6e30-4056-9b8b-6eb0c8d406f0");
            var custom = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();
            custom.Guid = Guid.Parse("0265f6cd-dccc-4a85-8e14-af65e2c0066f");


            department.Name = "Department";
            department.Description = "Some department";
            department.RelTableName = "Obj_0003";

            department.Initialize();


            custom.Name = "Custom";
            custom.Description = "Some custom";
            custom.RelTableName = "Obj_0004";

            custom.Initialize();


            store.Name = "Store";
            store.Description = "This is a store entity";
            store.RelTableName = "Obj_0001";
            var invoiceProp = store.CreateProperty();
            invoiceProp.Guid = Guid.Parse("9d865473-58a7-42ff-b4de-17451202064b");
            invoiceProp.Name = "Invoice";
            invoiceProp.Types.Add(invoice);
            invoiceProp.DatabaseColumnName = "Fld_0003";

            var storeCompositeProp = store.CreateProperty();
            storeCompositeProp.Guid = Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df");
            storeCompositeProp.Name = "CompositeProperty";
            storeCompositeProp.DatabaseColumnName = "Fld_0004";
            storeCompositeProp.Types.Add(new XCBinary());
            storeCompositeProp.Types.Add(new XCBoolean());
            storeCompositeProp.Types.Add(new XCString());
            storeCompositeProp.Types.Add(new XCDateTime());

            store.Initialize();


            var storeProp = invoice.CreateProperty();
            storeProp.Guid = Guid.Parse("175d1ade-75f0-416e-bd18-67793f79f176");
            storeProp.Name = "Store";
            storeProp.Types.Add(store);
            storeProp.Types.Add(department);
            storeProp.Types.Add(custom);
            storeProp.DatabaseColumnName = "Fld_0002";

            var prop = invoice.CreateProperty();
            prop.Guid = Guid.Parse("4925d3ee-d858-4a96-a65f-1f87c2cf1357");
            prop.Name = "CompositeProperty";
            prop.Types.Add(new XCBinary());
            prop.Types.Add(new XCBoolean());
            prop.Types.Add(new XCString());
            prop.Types.Add(new XCDateTime());
            prop.Types.Add(store);

            prop.DatabaseColumnName = "Fld_0001";

            invoice.Name = "Invoice";
            invoice.RelTableName = "Obj_0002";
            invoice.Modules.Add(new XCSingleEntityModule()
            {
                ModuleText = "public int Test(int i) { int _i = i; _i++; return _i; }",
                ModuleRelationType = XCProgramModuleRelationType.Object
            });

            var cmd = invoice.CreateCommand();
            cmd.Guid = Guid.Parse("8008bbaa-7bc1-4c7d-aa56-3b9f728619ff");
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

        public static XCRoot CreateChangedExampleConfiguration()
        {
            var root = new XCRoot();

            root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a"); //Guid.NewGuid();
            root.ProjectName = "Library";
            root.ProjectVersion = "0.0.0.1";

            var component = new XCComponent()
            {

                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            var store = (XCSingleEntity) component.ComponentImpl.ComponentManager.Create();
            store.Guid = Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5");
            var invoice = (XCSingleEntity)component.ComponentImpl.ComponentManager.Create();
            invoice.Guid = Guid.Parse("b9fee6cd-a834-4d72-9de5-1fc2087467e5");
            var department = (XCSingleEntity)component.ComponentImpl.ComponentManager.Create();
            department.Guid = Guid.Parse("953120f4-6e30-4056-9b8b-6eb0c8d406f0");
            var custom = (XCSingleEntity)component.ComponentImpl.ComponentManager.Create();
            custom.Guid = Guid.Parse("0265f6cd-dccc-4a85-8e14-af65e2c0066f");


            department.Name = "Department";
            department.Description = "Some department";
            department.RelTableName = "Obj_0003";

            department.Initialize();


            custom.Name = "Custom";
            custom.Description = "Some custom";
            custom.RelTableName = "Obj_0004";

            custom.Initialize();


            store.Name = "Store";
            store.Description = "This is a store entity";
            store.RelTableName = "Obj_0001";
            var invoiceProp = store.CreateProperty();
            invoiceProp.Guid = Guid.Parse("9d865473-58a7-42ff-b4de-17451202064b");
            invoiceProp.Name = "Invoice";
            invoiceProp.Types.Add(invoice);
            invoiceProp.DatabaseColumnName = "Fld_0003";

            var storeCompositeProp = store.CreateProperty();
            storeCompositeProp.Guid = Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df");
            storeCompositeProp.Name = "CompositeProperty";
            storeCompositeProp.DatabaseColumnName = "Fld_0004";
            storeCompositeProp.Types.Add(new XCBinary());
            storeCompositeProp.Types.Add(new XCBoolean());
            storeCompositeProp.Types.Add(new XCString());
            storeCompositeProp.Types.Add(new XCDateTime());

            store.Initialize();


            var storeProp = invoice.CreateProperty();
            storeProp.Guid = Guid.Parse("175d1ade-75f0-416e-bd18-67793f79f176");
            storeProp.Name = "Store";
            storeProp.Types.Add(store);
            storeProp.Types.Add(department);
            storeProp.Types.Add(custom);
            storeProp.DatabaseColumnName = "Fld_0002";

            var prop = invoice.CreateProperty();
            prop.Guid = Guid.Parse("4925d3ee-d858-4a96-a65f-1f87c2cf1357");
            prop.Name = "CompositeProperty";
            prop.Types.Add(new XCBinary());
            prop.Types.Add(new XCBoolean());
           // prop.Types.Add(new XCString());
            prop.Types.Add(new XCDateTime());
            prop.Types.Add(store);

            prop.DatabaseColumnName = "Fld_0001";

            invoice.Name = "Invoice";
            invoice.RelTableName = "Obj_0002";
            invoice.Modules.Add(new XCSingleEntityModule()
            {
                ModuleText = "public int Test(int i) { int _i = i; _i++; return _i; }",
                ModuleRelationType = XCProgramModuleRelationType.Object
            });

            var cmd = invoice.CreateCommand();
            cmd.Guid = Guid.Parse("8008bbaa-7bc1-4c7d-aa56-3b9f728619ff");
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