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
           // var c = new CreateTableQueryNode("test", "SomeTableName");
            //c.WithColumn("Column1", t => t.Boolean());

           // var sc = new PostgresCompiller();

           // var script = sc.Compile(c);
           // var actual = "CREATE TABLE \"test\".\"SomeTableName\"(\"Column1\" \"BOOLEAN\")";
            //  Assert.Equal(actual, script);
            /*
            var resultNode = ExpressionBuilder.Create().Table("TEST")
                .WithColumn("Column1").AsInt()
                .WithColumn("Coulumn2").AsVarchar(100)
                .Build();
                */
            /*
            BaseSqlNodeVisitor visitor = new BaseSqlNodeVisitor();
            StringBuilder stringBuilder = new StringBuilder();
            visitor.VisitNode(resultNode, stringBuilder);


            //var result = stringBuilder.ToString();

            int a = 10;*/
        }
    }
}