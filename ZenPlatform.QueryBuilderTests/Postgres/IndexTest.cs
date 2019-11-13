using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xunit;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.Index;

namespace ZenPlatform.Tests.SqlBuilder.Postgres
{
    public class IndexTest
    {
        private PostgresCompiller _compiller = new PostgresCompiller();

        [Fact]
        public void CreateIndexTest()
        {
            var q = new CreateIndexQueryNode("NewIndex")
                .WithTable("IndexedTable", o => o.WithSchema("customSchema")
                    .WithColumn("Col1", co => co.WithDirection(ListSortDirection.Descending))
                    .WithColumn("Col2", null));
            var expected =
                "CREATE INDEX \"NewIndex\" ON \"customSchema\".\"IndexedTable\"(\"Col1\" DESC, \"Col2\" ASC)";

            var actual = _compiller.Compile(q);

            Assert.Equal(expected, actual);
        }
    }
}