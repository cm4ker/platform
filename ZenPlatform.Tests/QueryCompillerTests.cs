using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Serialization;
using ZenPlatform.QueryCompiler;
using ZenPlatform.QueryCompiler.Queries;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class QueryCompillerTests
    {
        private DBQueryFactory _factory;

        public QueryCompillerTests()
        {
            _factory = new DBQueryFactory();
        }

        [TestMethod]
        public void SelectTest()
        {
            var table = _factory.CreateTable("MyTable", "AliasedTable");
            var f = table.DeclareField("MyField1");

            DBSelectQuery q = _factory.GetSelect();
            q.From(table);
            q.Select(f);

            var compiled = q.Compile();

            var res = @"SELECT
	[AliasedTable].[MyField1]
FROM
	[MyTable] AS [AliasedTable]";
            Assert.AreEqual(compiled.Trim(), res.Trim());
        }

        [TestMethod]
        public void CreateTableTest()
        {
            var q = _factory.GetCreateTable();
            q.Table("Test")
             .Field("SimpleField", QueryCompiler.Schema.DBType.Int, 10, 1, 12, false, false, false, false);

            Debug.WriteLine(q.Compile());
            Console.WriteLine(q.Compile());
        }
    }
}
