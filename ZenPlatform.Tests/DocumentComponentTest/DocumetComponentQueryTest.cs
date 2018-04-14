using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.DocumentComponent;
using ZenPlatform.DocumentComponent.QueryBuilders;

namespace ZenPlatform.Tests.DocumentComponentTest
{
    [TestClass]
    public class DocumentComponentQueryTest
    {
        public PDocumentObjectType _object;
        public DocumentQueryBuilder _queryBuilder;
        public DocumentComponentQueryTest()
        {
            var component = ConfigurationFactory.CreateDocumentComponent();
            _object = ConfigurationFactory.CreateInvoice(component);
            _queryBuilder = new DocumentQueryBuilder();
        }

        [TestMethod]
        public void SingleSelectTest()
        {
            var query = _queryBuilder.SelectSingleObject(_object);

            var sql = query.Compile().Replace("\n", "").Replace("\r", "");

            var normalSql =
@"SELECT
	[Invoices].[Id]
	,[Invoices].[StartDate]
	,[Invoices].[SomeNumber]
FROM
	[Invoices]
WHERE 
	 (@Id = [Invoices].[Id])".Replace("\n", "").Replace("\r", "");
            Console.WriteLine(sql);

            Assert.AreEqual(sql, normalSql);
        }
    }
}
