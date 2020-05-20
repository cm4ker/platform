using System;
using Aquila.Configuration;
using Aquila.Configuration.Common;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Configuration.Storage;
using Aquila.Configuration.Structure;
using Aquila.Core.Contracts.Configuration;
using Aquila.EntityComponent.Configuration;
using dnlib.DotNet;
using STCE = Aquila.SerializableTypeComponent.Configuration.Editors.ComponentEditor;
using WSE = Aquila.WebServiceComponent.Configuration.Editors.ComponentEditor;

namespace Aquila.Test.Tools
{
    /// <summary>
    /// Пример конфигурации
    /// </summary>
    public static class ConfigurationFactory
    {
        public static string GetDatabaseConnectionString() => "Host=db1; Username=user; Password=password;";

        public static Project Create()
        {
            var projectMd = new ProjectMD
            {
                ProjectName = "Library",
                ProjectVersion = "1.0.0.0"
            };
            var manager = new MDManager(new TypeManager(), new InMemoryUniqueCounter());
            var project = new Project(projectMd, manager);

            #region EntityComponent

            {
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

                var table1 = store.CreateTable();
                table1.Name = "ExampleTable";

                var tableProp1 = table1.CreateProperty();
                tableProp1.Name = "TableProperty";
                tableProp1.SetType(MDTypes.String(100));
                tableProp1.SetType(store.GetRef());

                var i = store.CreateInterface();
                i.Markup =
                    @"<p:StoreEditorForm 
                xmlns:p=""clr-namespace:Entity;assembly=LibraryServer""    
                xmlns=""clr-namespace:Aquila.Avalonia.Wrapper;assembly=Aquila.Avalonia.Wrapper"">
  <UXGroup Orientation=""Vertical"">
    <UXTextBox />
    <UXTextBox />
    <UXGroup Orientation = ""Horizontal""> 
        <UXCheckBox />
<UXCheckBox />
<UXCheckBox />
<UXCheckBox />
    </UXGroup>
  </UXGroup>
</p:StoreEditorForm>";
                i.Name = "Editor";

                var module = store.CreateModule();
                module.ModuleName = "Module1";
                module.ModuleRelationType = ProgramModuleRelationType.Object;
                module.ModuleText = @"
public void In(int i) {  }
public void In(string s) {  }
public void Overload() 
{
    In(100);
    Save();
}
";

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

 public void ExecuteSql()
 {
    var a = $Query();
    a.Text = ""FROM Entity.Store SELECT Id"";
    var r = a.ExecuteReader();

    while(r.Read())
    {
        var g = (uid)r.Id;
    }
 }


 [Client]
 public void OnClientClientCallProc()
 {
     ClientCallProc(10);
 }

[ClientCall] 
 public void CreateAndSaveStore()
 {
    Store s = $Entity.Store.Create();
    s.Name = ""Souths park"";
    s.Save();   
 }


 [ClientCall] 
 public string GetUserNameServer()
 { 
     CreateAndSaveStore();

     return Context.UserName; 
 }

 [ClientCall] 
 public Store GetStore()
 { 
     Store s = $Entity.Store.Create();
     s.Name = ""Souths park"";
        
     return s; 
 }

[ClientCall]
public Store UpdateName(Store income)
{
    income.Name = ""Changed!!!!"";
    return income;
}

 [Client]
 public void GetUserName()
 {
     GetUserNameServer();
 }
 ";
                ce.Apply();
            }

            #endregion

            #region SerializableTypes

            var sce = new STCE(project);

            var type1 = sce.CreateType();
            type1.Name = "ST1";
            var st1Name = type1.CreateProperty();
            st1Name.Name = "Name";
            st1Name.SetType(MDTypes.String(200));
            st1Name.IsArray = true;

            sce.Apply();

            #endregion

            #region WebServices

            var wse = new WSE(project);

            var ws1 = wse.CreateType();
            ws1.Name = "WebService1";
            var ws1Main = ws1.CreateModule();
            ws1Main.ModuleName = "Main";
            ws1Main.ModuleText = @"
using Entity;

[Operation]
public string CreateNewStore(int selector)
{
     var e = $Entity.Store.Create();

     var row = e.ExampleTable.Add();
     row.TableProperty = ""String value for this"";

     e.Name = ""Simple store"";
     e.Property1 = selector;
     e.Save();

     if(selector > 0)
         return ""above zero"" + e.Id;
     else
         return ""below zero"" + e.Id;
}


[Operation]
public int GetStoreMaxValue()
{
    var a = $Query();
    a.Text = ""FROM Entity.Store SELECT A=CAST( Property1 as int) ORDER BY 1 DESC"";
    var r = a.ExecuteReader();

    if(r.Read())
    {
        return (int)r.A;
    }

    return 0;
}


";
            wse.Apply();

            #endregion

            return project;
        }
    }
}