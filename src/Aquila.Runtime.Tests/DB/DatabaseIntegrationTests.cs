using System.Data.SqlClient;
using Aquila.Core;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;
using Aquila.Runtime.Querying;
using Npgsql;
using Xunit;
using Xunit.Abstractions;

namespace Aquila.Runtime.Tests.DB
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }


    [Collection("Database collection")]
    public class DatabaseIntegrationTests
    {
        DatabaseFixture fixture;
        private readonly ITestOutputHelper _logger;

        public DatabaseIntegrationTests(DatabaseFixture fixture, ITestOutputHelper logger)
        {
            this.fixture = fixture;
            _logger = logger;
        }

        [Fact(Skip = "mssql")]
        public void InsertEntityQueryGenerationTest()
        {
            var md = fixture.Context.MetadataProvider;
            var invoice = md.GetSemanticByName("Entity.Invoice");

            Assert.NotNull(invoice);

            var actual = CRUDQueryGenerator.CompileInsert(invoice, fixture.Context, out var q);

            Assert.NotNull(q);

            var expect =
                @"INSERT INTO Tbl_257(Fld_261, Fld_262_T, Fld_262_R, Fld_262_I, Fld_262_S, Fld_263, Fld_264, Fld_265)
(SELECT T0.A0,
T0.A1_T,
T0.A1_R,
T0.A1_I,
T0.A1_S,
T0.A2,
T0.A3,
T0.A4
FROM
(SELECT @p0 as A0,
@p1_T as A1_T,
@p1_R as A1_R,
@p1_I as A1_I,
@p1_S as A1_S,
@p2 as A2,
@p3 as A3,
@p4 as A4
) as T0
WHERE
CASE WHEN  EXISTS (SELECT 1
FROM
(SELECT 1 as _sec_fld
) as _sec_dummy
WHERE
'CreateMe' = T0.A3
) THEN 0 
 ELSE 2147483647
 END + 2147483647 = 2147483647
)";

            Assert.Equal(expect.ReplaceLineEndings(), actual.ReplaceLineEndings());
        }

        [Fact(Skip = "mssql")]
        public void DeleteEntityQueryGenerationTest()
        {
            var md = fixture.Context.MetadataProvider;
            var invoice = md.GetSemanticByName("Entity.Invoice");

            Assert.NotNull(invoice);

            var actual = CRUDQueryGenerator.CompileDelete(invoice, fixture.Context, out var q);

            Assert.NotNull(q);

            var expect =
                @"DELETE T0
FROM
Tbl_257 as T0
WHERE
(T0.Fld_261 = @p0 AND CASE WHEN  EXISTS (SELECT 1
FROM
(SELECT 1 as _sec_fld
) as _sec_dummy
WHERE
'DeleteMe' = T0.Fld_264
) THEN 0 
 ELSE 2147483647
 END + 2147483647 = 2147483647)
";

            Assert.Equal(expect.ReplaceLineEndings(), actual.ReplaceLineEndings());
        }

        [Fact(Skip = "mssql")]
        public void UpdateEntityQueryGenerationTest()
        {
            var md = fixture.Context.MetadataProvider;
            var invoice = md.GetSemanticByName("Entity.Invoice");

            Assert.NotNull(invoice);

            var actual = CRUDQueryGenerator.CompileUpdate(invoice, fixture.Context, out var q);
            var expected = @"UPDATE T0
SET T0.Fld_261 = T1.A0, T0.Fld_262_T = T1.A1_T, T0.Fld_262_S = T1.A1_S, T0.Fld_262_I = T1.A1_I, T0.Fld_262_R = T1.A1_R, T0.Fld_263 = T1.A2, T0.Fld_264 = T1.A3, T0.Fld_265 = T1.A4
FROM
Tbl_257 as T0
CROSS JOIN (SELECT @p0 as A0,
@p1_T as A1_T,
@p1_R as A1_R,
@p1_I as A1_I,
@p1_S as A1_S,
@p2 as A2,
@p3 as A3,
@p4 as A4
) as T1 
WHERE
(T0.Fld_261 = T1.A0 AND CASE WHEN  (EXISTS (SELECT 1
FROM
(SELECT 1 as _sec_fld
) as _sec_dummy
WHERE
'UpdateMe' = T0.Fld_264
) AND EXISTS (SELECT 1
FROM
(SELECT 1 as _sec_fld
) as _sec_dummy
WHERE
'UpdateMe' = T0.Fld_264
)) THEN 0 
 ELSE 2147483647
 END + 2147483647 = 2147483647)
";
            Assert.Equal(expected.ReplaceLineEndings(), actual.ReplaceLineEndings());
        }

        [Fact(Skip = "mssql")]
        public void SelectEntityQueryGenerationTest()
        {
            var md = fixture.Context.MetadataProvider;
            var invoice = md.GetSemanticByName("Entity.Invoice");

            Assert.NotNull(invoice);

            var actual = CRUDQueryGenerator.CompileSelect(invoice, fixture.Context, out var q, out _);
            var expected = @"SELECT T0.Fld_261,
T0.Fld_262_T,
T0.Fld_262_R,
T0.Fld_262_I,
T0.Fld_262_S,
T0.Fld_263,
T0.Fld_264,
T0.Fld_265
FROM
Tbl_257 as T0
WHERE
T0.Fld_261 = @p0
";

            _logger.WriteLine(actual);

            Assert.Equal(expected.ReplaceLineEndings(), actual.ReplaceLineEndings());
        }
    }
}