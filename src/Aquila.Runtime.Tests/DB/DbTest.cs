using System.Data;
using System.IO;
using System.Linq;
using Aquila.Core.Assemlies;
using Aquila.Data;
using Aquila.Initializer;
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

            db.Files.SaveFile(dc,
                new FileDescriptor
                {
                    Name = "AssemblyName",
                    Type = FileType.MainAssembly,
                }, File.ReadAllBytes("C:\\test\\test_aq.dll"));

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
        }
    }
}