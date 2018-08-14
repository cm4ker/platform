using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.Tests.SqlBuilder.Postgres
{
    [TestClass]
    public class SelectTest
    {
        [TestMethod]
        public void SimpleSelect()
        {
            PostgresCompiller c = new PostgresCompiller();

            var query = new SelectQueryNode().From("conf").Select("Data")
                .Where(x => x.Field("BlobName"), "=", x => x.Parameter("BlobName"));

            Assert.AreEqual("SELECT \"Data\"\nFROM \n    \"conf\"\nWHERE \n    \"BlobName\"=@BlobName",
                c.Compile(query));
        }
    }
}