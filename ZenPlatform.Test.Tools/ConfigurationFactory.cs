using System;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Storage;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;

namespace ZenPlatform.Test.Tools
{
    /// <summary>
    /// Пример конфигурации
    /// </summary>
    public static class ConfigurationFactory
    {
        public static string GetDatabaseConnectionString() => "Host=db1; Username=user; Password=password;";

        public static Project Create()
        {
            var projectMd = new ProjectMD();
            var manager = new MDManager(new TypeManager(), new InMemoryUniqueCounter());
            var project = new Project(projectMd, manager);

            var ce = new ComponentEditor(project);
            var store = ce.CreateObject();
            store.Name = "Store";
            var prop1 = store.CreateProperty();
            prop1.Name = "Property1";

            prop1.SetType(MDTypes.DateTime)
                .SetType(MDTypes.Int)
                .SetType(MDTypes.Boolean)
                .SetType(MDTypes.Numeric(10, 2))
                .SetType(store.GetRef());

            var module = store.CreateModule();
            module.ModuleRelationType = ProgramModuleRelationType.Object;
            module.ModuleText = "public int In(int i) { int _i = i; _i++; return _i; }";

            var command = store.CreateCommand();
            command.Name = "HelloFromServer";
            command.DisplayName = "Some display name";

            command.ModuleText = @"
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
 }

 [ClientCall] 
 public string GetUserNameServer()
 { 
     // Entity.Invoice i = $Entity.Invoice.Create();
     // Entity.Store s = $Entity.Store.Create();
     //
     // i.Name = ""My custom name"";
     // i.CompositeProperty = ""Привет Костя"";
     //
     // s.Name = ""Souths park"";
     // s.Save();    
     //
     // i.Store = s.Link;
     //
     // i.Save();

     return Context.UserName; 
 }

 [Client]
 public void GetUserName()
 {
     GetUserNameServer();
 }
 ";

            ce.Apply();

            return project;
//             var root = new Project(new TypeManager());
//
//             root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a");
//             root.ProjectName = "Library";
//             root.ProjectVersion = "0.0.0.1";
//
//             var comRef = new ComponentRef()
//             {
//                 DllRef = typeof(MDEntity).Assembly.Location,
//                 MDRef = "Entity.xml"
//             };
//
//             comRef.ToComponent(root.TypeManager);
//
//
//             var componentManager = (SingleEntityConfigurationManager) component.ComponentImpl.ComponentManager;
//             var storeEditor =
//                 componentManager.Create()
//                     .SetName("Store")
//                     .SetId(Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5"))
//                     .SetLinkId(Guid.Parse("9c4ab7a8-ccff-407a-806b-b27a7c72702e"))
//                     .SetDescription("This is a store entity")
//                     .SetRealTableName("Obj_0001");
//
//
//             var invoiceEditor =
//                 componentManager.Create()
//                     .SetName("Invoice")
//                     .SetId(Guid.Parse("b9fee6cd-a834-4d72-9de5-1fc2087467e5"))
//                     .SetLinkId(Guid.Parse("688e8cb1-2dd7-4eec-8c88-805516bf0f31"))
//                     .SetDescription("This is a invoice entity")
//                     .SetRealTableName("Obj_0002");
//
//             var departmentEditor =
//                 componentManager.Create()
//                     .SetName("Department")
//                     .SetId(Guid.Parse("953120f4-6e30-4056-9b8b-6eb0c8d406f0"))
//                     .SetLinkId(Guid.Parse("afbf0dfc-8c6f-4634-9dcb-56ca4cf6f647"))
//                     .SetDescription("Some department")
//                     .SetRealTableName("Obj_0003");
//
//             var customEditor =
//                 componentManager.Create()
//                     .SetName("Custom")
//                     .SetId(Guid.Parse("0265f6cd-dccc-4a85-8e14-af65e2c0066f"))
//                     .SetLinkId(Guid.Parse("1635682c-68e9-4c3c-8794-c12cbe9f52ef"))
//                     .SetDescription("Some custom")
//                     .SetRealTableName("Obj_0004");
//
//
//             storeEditor.CreateProperty()
//                 .SetGuid(Guid.Parse("9d865473-58a7-42ff-b4de-17451202064b"))
//                 .SetName("Invoice")
//                 .AddType(invoiceEditor.Link)
//                 .SetDatabaseColumnName("Fld_0003");
//
//             storeEditor.CreateProperty()
//                 .SetGuid(Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df"))
//                 .SetName("CompositeProperty")
//                 .AddType(new XCBinary())
//                 .AddType(new XCBoolean())
//                 .AddType(new XCString())
//                 .AddType(new XCDateTime())
//                 .SetDatabaseColumnName("Fld_0004");
//
//
//             invoiceEditor.CreateProperty()
//                 .SetGuid(Guid.Parse("175d1ade-75f0-416e-bd18-67793f79f176"))
//                 .SetName("Store")
//                 .AddType(storeEditor.Link)
//                 .AddType(departmentEditor.Link)
//                 .AddType(customEditor.Link)
//                 .SetDatabaseColumnName("Fld_0002");
//
//
//             invoiceEditor.CreateProperty()
//                 .SetGuid(Guid.Parse("4925d3ee-d858-4a96-a65f-1f87c2cf1357"))
//                 .SetName("CompositeProperty")
//                 .AddType(new XCBinary() {Size = 150})
//                 .AddType(new XCBoolean())
//                 .AddType(new XCString() {Size = 150})
//                 .AddType(new XCDateTime())
//                 .AddType(storeEditor.Link)
//                 .SetDatabaseColumnName("Fld_0001");
//
//             invoiceEditor.CreateTable()
//                 .SetName("Goods")
//                 .CreateProperty()
//                 .SetGuid(Guid.Parse("74445120-6DA2-476A-AC90-A636CD6CFAED"))
//                 .SetName("Nomenclature")
//                 .AddType(new XCString() {Size = 30})
//                 .SetDatabaseColumnName("Fld_0011");
//
//
//             invoiceEditor.CreateModule()
//                 .SetText("public int Test(int i) { int _i = i; _i++; return _i; }")
//                 .SetRelationTypeObject();
//
//             invoiceEditor.CreateCommand()
//                 .SetGuid(Guid.Parse("8008bbaa-7bc1-4c7d-aa56-3b9f728619ff"))
//                 .SetName("HelloFromServer")
//                 .SetDisplayName("Invoke the command")
//                 .EditModule()
//                 .SetText(@"
// [ClientCall] 
// public int ClientCallProc(int a)
// { 
//     a++;
//     return a; 
// }
//
// [Client]
// public void OnClientClientCallProc()
// {
//     ClientCallProc(10);
// }
//
//
//
// [ClientCall] 
// public string GetUserNameServer()
// { 
//     Entity.Invoice i = $Entity.Invoice.Create();
//     Entity.Store s = $Entity.Store.Create();
//
//     i.Name = ""My custom name"";
//     //i.CompositeProperty = ""Привет Костя"";
//
//     s.Name = ""Souths park"";
//     s.Save();    
//
//     i.Store = s.Link;
//
//     i.Save();
//
//     return Context.UserName; 
// }
//
// [Client]
// public void GetUserName()
// {
//     GetUserNameServer();
// }
// ");
//             return root;
        }
    }
}