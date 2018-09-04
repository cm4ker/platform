using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.CreateDatabase;

namespace ZenPlatform.Tests.SqlBuilder.Postgres
{
    [TestClass]
    public class DatabaseTest
    {
        private PostgresCompiller _compiller = new PostgresCompiller();
        
        
        [TestMethod]
        public void CreateDatabaseTest()
        {
            var q = new CreateDatabaseQueryNode("TestDatabase");

            var query = _compiller.Compile(q);

            Assert.AreEqual("CREATE DATABASE \"TestDatabase\"", query);
        }

        [TestMethod]
        public void DropDatabaseTest()
        {
            var q = new DropDatabaseQueryNode("TestDatabase");

            var query = _compiller.Compile(q);

            Assert.AreEqual("DROP DATABASE \"TestDatabase\"", query);
        }
    }
    
    
}