using Xunit;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.CreateTable;
using System.Text;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.QueryBuilder.Contracts;

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

        public void CreateTableInterfaceTest()
        {
            var table = Query.Create();

            table.Schema();
            table.Column();
        }
    }
}