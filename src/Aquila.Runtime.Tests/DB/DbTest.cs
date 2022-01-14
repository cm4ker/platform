using System.Data;
using System.IO;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Assemlies;
using Aquila.Core.Authentication;
using Aquila.Core.Sessions;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Library.Scripting;
using Aquila.Metadata;
using SqlInMemory;
using Xunit;

namespace Aquila.Runtime.Tests.DB
{
    public class DbTest
    {
        [Fact]
        public void TestDbProvider()
        {
            var cs = "Data Source=.;Initial Catalog=TestDb;Integrated Security=true";
            SqlInMemoryDb.Create(cs);
            Assert.True(SqlHelper.DatabaseExists(cs));
            MigrationRunner.Migrate(cs, SqlDatabaseType.SqlServer);

            DatabaseRuntimeContext db = new DatabaseRuntimeContext();
            DataConnectionContext dc =
                new DataConnectionContext(SqlDatabaseType.SqlServer, cs, IsolationLevel.ReadCommitted);


            var d1 = db.Descriptors.CreateDescriptor(dc);
            var d2 = db.Descriptors.CreateDescriptor(dc);

            AqContext.IScriptingProvider provider = new ScriptingProvider();

            var code =
                @"
import Entity;

[endpoint] public static int endpoint_test() { return 100500; }
[endpoint] public static datetime current_time() { return get_date(); }
[endpoint] public static int echo(int ping) { return ping; }
[endpoint] public static int create_invoice_with_store() 
{
    var store = StoreManager.create();
    store.Name = ""My store"";
    store.save();
    var obj = InvoiceManager.create();
    obj.Name = ""test"";
    obj.ComplexProperty = ""Complex"";
    obj.ComplexProperty = 10;
    obj.ComplexProperty = store.link;
    obj.Store = store.link;
    obj.save();
    
    var q = query();
    q.text = ""FROM Entity.Invoice SELECT ComplexProperty"";
    q.set_param(""p0"", 1);

    var r = q.exec();

    if(r.read())
    {
        var id = r[""ComplexProperty""];
        return 1;
    }

    return 0;
}
";

            var script = provider.CreateScript(new AqContext.ScriptOptions()
            {
                IsSubmission = false,
                EmitDebugInformation = true,
                Location = new Location("unknown", 0, 0),
                //AdditionalReferences = AdditionalReferences,
            }, code, TestMetadata.GetTestMetadata());

            db.PendingFiles.SaveFile(dc,
                new FileDescriptor
                {
                    Name = "AssemblyName",
                    Type = FileType.MainAssembly,
                }, script.Image.ToArray());

            Assert.NotEmpty(db.Descriptors.GetEntityDescriptors());

            d1.DatabaseName = "some_name";
            d2.DatabaseName = $"Tbl_{d2.DatabaseId}";
            d2.MetadataId = "test";

            db.PendingMetadata.SetMetadata(TestMetadata.GetTestMetadata());
            db.SaveAll(dc);

            //NOTE: First 0x100 type ids are reserved 
            Assert.Equal(257, d1.DatabaseId);
            Assert.Equal(258, d2.DatabaseId);

            DatabaseRuntimeContext db2 = new DatabaseRuntimeContext();

            db2.LoadAll(dc);
            Assert.True(db.PendingMetadata.GetMetadata().Metadata.Any());
            
        }
    }
}