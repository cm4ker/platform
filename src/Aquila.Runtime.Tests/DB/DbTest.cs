using System.Linq;
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
            Assert.NotEmpty(db.GetDescriptors());

            d1.DatabaseName = "some_name";
            d2.DatabaseName = $"Tbl_{d2.DatabaseId}";
            d2.MetadataId = "test";

            db.Save(dc);

            Assert.Equal(1, d1.DatabaseId);
            Assert.Equal(2, d2.DatabaseId);

            DatabaseRuntimeContext db2 = new DatabaseRuntimeContext();

            db2.Load(dc);

            var md = db2.GetMetadata();
            Assert.True(md.GetSemanticMetadata().Any());
        }
    }
}