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

            root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a");
            root.ProjectName = "Library";
            root.ProjectVersion = "0.0.0.1";

            var component = new XCComponent()
            {
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            var componentManager = (SingleEntityConfigurationManager)component.ComponentImpl.ComponentManager;
            var storeEditor =
                componentManager.Create()
                .SetName("Store")
                .SetId(Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5"))
                .SetDescription("This is a store entity")
                .SetRealTableName("Obj_0001");
            storeEditor.Type.Initialize();


            var invoiceEditor =
                componentManager.Create()
                .SetName("Invoice")
                .SetId(Guid.Parse("b9fee6cd-a834-4d72-9de5-1fc2087467e5"))
                .SetDescription("This is a invoice entity")
                .SetRealTableName("Obj_0002");
            invoiceEditor.Type.Initialize();

            var departmentEditor =
                componentManager.Create()
                .SetName("Department")
                .SetId(Guid.Parse("953120f4-6e30-4056-9b8b-6eb0c8d406f0"))
                .SetDescription("Some department")
                .SetRealTableName("Obj_0003");
            departmentEditor.Type.Initialize();

            var customEditor =
                componentManager.Create()
                .SetName("Custom")
                .SetId(Guid.Parse("0265f6cd-dccc-4a85-8e14-af65e2c0066f"))
                .SetDescription("Some custom")
                .SetRealTableName("Obj_0004");
            customEditor.Type.Initialize();



            storeEditor.CreateProperty()
                .SetGuid(Guid.Parse("9d865473-58a7-42ff-b4de-17451202064b"))
                .SetName("Invoice")
                .AddType(invoiceEditor.Type)
                .SetDatabaseColumnName("Fld_0003");

            storeEditor.CreateProperty()
                .SetGuid(Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df"))
                .SetName("CompositeProperty")
                .AddType(new XCBinary())
                .AddType(new XCBoolean())
                .AddType(new XCString())
                .AddType(new XCDateTime())
                .SetDatabaseColumnName("Fld_0004");




            invoiceEditor.CreateProperty()
                .SetGuid(Guid.Parse("175d1ade-75f0-416e-bd18-67793f79f176"))
                .SetName("Store")
                .AddType(storeEditor.Type)
                .AddType(departmentEditor.Type)
                .AddType(customEditor.Type)
                .SetDatabaseColumnName("Fld_0002");


            invoiceEditor.CreateProperty()
                .SetGuid(Guid.Parse("4925d3ee-d858-4a96-a65f-1f87c2cf1357"))
                .SetName("CompositeProperty")
                .AddType(new XCBinary())
                .AddType(new XCBoolean())
                .AddType(new XCString())
                .AddType(new XCDateTime())
                .AddType(storeEditor.Type)
                .SetDatabaseColumnName("Fld_0001");

            invoiceEditor.CreateModule()
                .SetText("public int Test(int i) { int _i = i; _i++; return _i; }")
                .SetRelationTypeObject();




            invoiceEditor.CreateCommand()
                .SetGuid(Guid.Parse("8008bbaa-7bc1-4c7d-aa56-3b9f728619ff"))
                .SetName("HelloFromServer")
                .SetDisplayName("Invoke the command")
                .EditModule()
                    .SetText(@"

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
}");




            return root;
        }


        public static XCRoot CreateChangedExampleConfiguration()
        {
            var root = new XCRoot();

            root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a");
            root.ProjectName = "Library";
            root.ProjectVersion = "0.0.0.1";

            var component = new XCComponent()
            {
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            var componentManager = (SingleEntityConfigurationManager)component.ComponentImpl.ComponentManager;
            var storeEditor =
                componentManager.Create()
                .SetName("store")
                .SetId(Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5"))
                .SetDescription("This is a store entity")
                .SetRealTableName("Obj_0001");


            var invoiceEditor =
                componentManager.Create()
                .SetName("invoice")
                .SetId(Guid.Parse("b9fee6cd-a834-4d72-9de5-1fc2087467e5"))
                .SetDescription("This is a invoice entity")
                .SetRealTableName("Obj_0002");

            var departmentEditor =
                componentManager.Create()
                .SetName("department")
                .SetId(Guid.Parse("953120f4-6e30-4056-9b8b-6eb0c8d406f0"))
                .SetDescription("Some department")
                .SetRealTableName("Obj_0003");

            var customEditor =
                componentManager.Create()
                .SetName("custom")
                .SetId(Guid.Parse("0265f6cd-dccc-4a85-8e14-af65e2c0066f"))
                .SetDescription("Some custom")
                .SetRealTableName("Obj_0004");




            storeEditor.CreateProperty()
               .SetGuid(Guid.Parse("9d865473-58a7-42ff-b4de-17451202064b"))
               .SetName("Invoice")
               .AddType(invoiceEditor.Link)
               .SetDatabaseColumnName("Fld_0003");

            storeEditor.CreateProperty()
                .SetGuid(Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df"))
                .SetName("CompositeProperty")
                .AddType(new XCBinary())
                .AddType(new XCBoolean())
                .AddType(new XCString())
                .SetDatabaseColumnName("Fld_0004");




            invoiceEditor.CreateProperty()
                .SetGuid(Guid.Parse("175d1ade-75f0-416e-bd18-67793f79f176"))
                .SetName("Store")
                .AddType(storeEditor.Link)
                .AddType(departmentEditor.Link)
                .AddType(customEditor.Link)
                .SetDatabaseColumnName("Fld_0002");


            invoiceEditor.CreateProperty()
                .SetGuid(Guid.Parse("4925d3ee-d858-4a96-a65f-1f87c2cf1357"))
                .SetName("CompositeProperty")
                .AddType(new XCBinary())
                .AddType(new XCBoolean())
                .AddType(new XCString())
                .AddType(new XCDateTime())
                .AddType(storeEditor.Link)
                .SetDatabaseColumnName("Fld_0001");

            invoiceEditor.CreateModule()
                .SetText("public int Test(int i) { int _i = i; _i++; return _i; }")
                .SetRelationTypeObject();




            invoiceEditor.CreateCommand()
                .SetGuid(Guid.Parse("8008bbaa-7bc1-4c7d-aa56-3b9f728619ff"))
                .SetName("HelloFromServer")
                .SetDisplayName("Invoke the command")
                .EditModule()
                    .SetText(@"

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
}");




            return root;
        }
    }
}