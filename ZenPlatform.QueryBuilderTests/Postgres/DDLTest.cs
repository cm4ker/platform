using Xunit;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.CreateTable;
using System.Text;

namespace ZenPlatform.Tests.SqlBuilder.Postgres
{
    public class DDLTest
    {
        [Fact]
        public void CreateTableTest()
        {
            var c = new CreateTableQueryNode("test", "SomeTableName");
            c.WithColumn("Column1", t => t.Boolean());
                
                

            var sc = new PostgresCompiller();

            var script = sc.Compile(c);
            var actual = "CREATE TABLE \"test\".\"SomeTableName\"(\"Column1\" \"bool\")";
            Assert.Equal(actual, script);
        }
    }
}