using Xunit;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.CreateDatabase;

namespace ZenPlatform.Tests.SqlBuilder.Postgres
{

    public class DatabaseTest
    {
        private PostgresCompiller _compiller = new PostgresCompiller();


        [Fact]
        public void CreateDatabaseTest()
        {
            var q = new CreateDatabaseQueryNode("TestDatabase");

            var query = _compiller.Compile(q);

            Assert.Equal("CREATE DATABASE \"TestDatabase\"", query);
        }

        [Fact]
        public void DropDatabaseTest()
        {
            var q = new DropDatabaseQueryNode("TestDatabase");

            var query = _compiller.Compile(q);

            Assert.Equal("DROP DATABASE \"TestDatabase\"", query);
        }
    }


}