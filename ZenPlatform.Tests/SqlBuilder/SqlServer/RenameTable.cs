using Xunit;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.Table;

namespace ZenPlatform.Tests.SqlBuilder.SqlServer
{
    public class RenameTableTest
    {
        private SqlCompillerBase c = new SqlServerCompiller();

        [Fact]
        public void RenameTest()
        {
            var query = new RenameTableQueryNode("test", "new_test", t => t.WithSchema("some_schema"));

            Assert.Equal("sp_rename([some_schema].[test],[new_test])", c.Compile(query));
        }
    }
}