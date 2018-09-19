using Xunit;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Conditions;
using ZenPlatform.QueryBuilder.Common.SqlTokens;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Tests.SqlBuilder.Postgres
{

    public class SelectTest
    {
        PostgresCompiller c = new PostgresCompiller();

        [Fact]
        public void SimpleSelect()
        {
            var query = new SelectQueryNode().From("conf").Select("Data")
                .Where(x => x.Field("BlobName"), "=", x => x.Parameter("BlobName"));

            Assert.Equal("SELECT \"Data\"\nFROM \n    \"conf\"\nWHERE \n    \"BlobName\"=@BlobName",
                c.Compile(query));
        }

        [Fact]
        public void SelectConditionfactoryUsing()
        {
            var query = new SelectQueryNode().From("conf").Select("Data")
                .Where(f =>
                        f.And(_ =>
                                new ConditionNode[]
                                {
                                     f.Condition(fel => fel.Field("BlobName"), Tokens.Comparators.E, fer => fer.Parameter("BlobName")),
                                     f.IsNull(ff => ff.Field("SomeField")),
                                     f.Like(fa=>fa.Field("LikeField"), fa=>fa.Raw("'%some pattern%'"))
                                }));

            Assert.Equal("SELECT \"Data\"\nFROM \n    \"conf\"\nWHERE \n    (\"BlobName\"=@BlobName AND \"SomeField\" IS NULL AND \"LikeField\" ILIKE '%some pattern%')",
                c.Compile(query));
        }
    }
}