using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class QueryBuilderTest
    {
        private SqlConnection _connection;
        private string _databaseName = "QueryBuilderTest";
        private string _tableName = "testTable";
        private string _connectionString = @"Data Source=(local);Integrated Security=SSPI;";

        public QueryBuilderTest()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        [TestMethod]
        public void QueryBuilder()
        {
            DBDropDatabaseQuery dropDatabaseQuery = new DBDropDatabaseQuery(_databaseName)
            {
                DropIfExist = true
            };

            int result = RunQuery(dropDatabaseQuery);
            Assert.AreEqual(result, -1);


            DBCreateDatabaseQuery createDatabaseQuery = new DBCreateDatabaseQuery(_databaseName);
            result = RunQuery(createDatabaseQuery);
            Assert.AreEqual(result, -1);
            
            result = RunQuery($"USE {_databaseName}");

            DBDropTableQuery dropTableQuery = new DBDropTableQuery(_tableName)
            {
                DropIfExist = true
            };

            result = RunQuery(dropTableQuery);

            DBCreateTableQuery createTableQuery = new DBCreateTableQuery(_tableName);
            createTableQuery.Field("Key", ZenPlatform.QueryBuilder.Schema.DBType.Int, 0, 0, 0, true, true, true, false);
            createTableQuery.Field("Name", ZenPlatform.QueryBuilder.Schema.DBType.NVarChar, 255, 0, 0, false, false, false, false);
            createTableQuery.Field("Value", ZenPlatform.QueryBuilder.Schema.DBType.Decimal, 0, 9, 5, false, false, false, true);

            result = RunQuery(createTableQuery);
            Assert.AreEqual(result, -1);

        }

        int RunQuery(IQueryable query)
        {
            return RunQuery(query.Compile());
        }

        int RunQuery(string command)
        {
            SqlCommand cmd = new SqlCommand(command, _connection);
            return cmd.ExecuteNonQuery();
        }



        ~QueryBuilderTest()
        {
            _connection.Close();
        }

    }
}
