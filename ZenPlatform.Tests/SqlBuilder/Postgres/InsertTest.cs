using Xunit;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Common.Operations;
using ZenPlatform.QueryBuilder.DML.Insert;

namespace ZenPlatform.Tests.SqlBuilder.Postgres
{
    public class InsertTest
    {
        private PostgresCompiller _compiller = new PostgresCompiller();

        [Fact]
        public void SimpleInsert()
        {
            var query = new InsertQueryNode()
                .InsertInto("config")
                .WithFieldAndValue(x => x.Field("Field1"),
                    x => x.Parameter("value1"))
                .WithFieldAndValue(x => x.Field("Field2"),
                    x => x.Parameter("value2"));

            var cmd = _compiller.Compile(query);

            var expectedCmd = "INSERT INTO \"config\"(\"Field1\", \"Field2\") VALUES(@value1, @value2)";

            Assert.Equal(expectedCmd, cmd);
        }
    }
}