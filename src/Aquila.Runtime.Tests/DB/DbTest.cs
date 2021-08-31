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
            DataConnectionContext dc = new DataConnectionContext(SqlDatabaseType.SqlServer, cs);

            var d1 = db.CreateDescriptor(dc);
            var d2 = db.CreateDescriptor(dc);

            db.SaveAssembly(dc,
                new AssemblyDescriptor
                {
                    Name = "AssemblyName",
                    Type = AssemblyType.Server,
                    AssemblyHash = "AsmHash",
                    ConfigurationHash = "Hash"
                }, File.ReadAllBytes("C:\\test\\test_aq.dll"));

            Assert.NotEmpty(db.GetEntityDescriptors());

            d1.DatabaseName = "some_name";
            d2.DatabaseName = $"Tbl_{d2.DatabaseId}";
            d2.MetadataId = "test";

            db.Save(dc);

            //NOTE: First 0x100 type ids are reserved 
            Assert.Equal(257, d1.DatabaseId);
            Assert.Equal(258, d2.DatabaseId);

            DatabaseRuntimeContext db2 = new DatabaseRuntimeContext();

            db2.Load(dc);

            var md = db2.GetMetadata();
            Assert.True(md.GetSemanticMetadata().Any());
        }
    }
}