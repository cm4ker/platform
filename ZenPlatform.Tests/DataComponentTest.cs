using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.DataComponent;
using ZenPlatform.DataComponent.QueryBuilders;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class DataComponentTest
    {

        private PObjectType _pObject;

        public DataComponentTest()
        {
            _pObject = new PSimpleObjectType("test");
            var property = new PProperty(_pObject)
            {
                Name = "test_property"
            };
            property.Types.Add(new PString());
            _pObject.Properties.Add(property);

            property = new PProperty(_pObject)
            {
                Name = "test_property2",
            };
            property.Types.Add(new PGuid());
            _pObject.Properties.Add(property);
            _pObject.TableName = "test_table";
        }

        [TestMethod]
        public void QueryBuilderTest()
        {
            var q = new DBSelectQuery("teble1").Select("qwe", "vdvdfv");

            var q2 = new DBSelectQuery();
            q2.From(q,"t1").Select("tttt","ewrewr");



            Assert.AreEqual(1, 1);

            Console.WriteLine(q.Compile());

        }

        [TestMethod]
        public void AlterTableQueryBuilderTest()
        {
            var query = new DBAlterTableQuery("testTable");

            query.DropColumn("testField");
            query.DropColumn("testField2");

            var s = query.Compile();

        }

        [TestMethod]
        public void SelectQueryBuilderTest()
        {
            var builder = new QueryBuilderComponent(_pObject);

            var query = builder.GetSelect();
            Assert.IsInstanceOfType(query, typeof(DBSelectQuery));

            string sql = @"SELECT
	[test_table].[Id]  ,
	[test_table].[test_property]  ,
	[test_table].[test_property2]  
FROM
	[test_table]  
	
WHERE
([test_table].[Id] = @Id)";

            Assert.AreEqual(query.Compile().Trim(), sql.Trim());

            Console.WriteLine(query.Compile());
        }

        [TestMethod]
        public void UpdateQueryBuilderTest()
        {
            var builder = new QueryBuilderComponent(_pObject);

            var query = builder.GetUpdate();
            Assert.IsInstanceOfType(query, typeof(DBUpdateQuery));

            string sql = @"UPDATE
	[test_table]
SET
	test_property = @test_property
	,test_property2 = @test_property2
	


WHERE
([test_table].[Id] = @Id)";

            Assert.AreEqual(query.Compile().Trim(), sql.Trim());

            Console.WriteLine(query.Compile());
        }

        [TestMethod]
        public void DeleteQueryBuilderTest()
        {

            var builder = new QueryBuilderComponent(_pObject);

            var query = builder.GetDelete();
            Assert.IsInstanceOfType(query, typeof(DBDeleteQuery));

            string sql = @"DELETE
	[test_table]

WHERE
([test_table].[Id] = @Id)";

            Assert.AreEqual(query.Compile().Trim(), sql.Trim());

            Console.WriteLine(query.Compile());
        }


        [TestMethod]
        public void InsertQueryBuilderTest()
        {

            var builder = new QueryBuilderComponent(_pObject);

            var query = builder.GetInsert();
            Assert.IsInstanceOfType(query, typeof(DBInsertQuery));

            string sql = @"INSERT INTO
	 test_table(test_property,test_property2)
VALUES
	(@test_property,@test_property2)";

            Assert.AreEqual(query.Compile().Trim(), sql.Trim());

            Console.WriteLine(query.Compile());
        }
    }
}
