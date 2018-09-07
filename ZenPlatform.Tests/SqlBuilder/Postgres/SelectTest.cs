using Xunit;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.Tests.SqlBuilder.Postgres
{
 
    public class SelectTest
    {
        [Fact]
        public void SimpleSelect()
        {
            PostgresCompiller c = new PostgresCompiller();

            var query = new SelectQueryNode().From("conf").Select("Data")
                .Where(x => x.Field("BlobName"), "=", x => x.Parameter("BlobName"));

            Assert.Equal("SELECT \"Data\"\nFROM \n    \"conf\"\nWHERE \n    \"BlobName\"=@BlobName",
                c.Compile(query));
        }
    }
}