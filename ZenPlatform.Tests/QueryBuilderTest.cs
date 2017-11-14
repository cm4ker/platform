using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using ZenPlatform.QueryBuilder.Queries;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Schema;
using IQueryable = ZenPlatform.QueryBuilder.Queries.IQueryable;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class QueryBuilderTest
    {
        private SqlConnection _connection;
        private string _databaseName = "QueryBuilderTest";
        private string _tableName = "testTable";
        private DBTable _table;
        private decimal _checkValue = 125;
        private decimal _checkValue2 = 777;
        private string _connectionString = @"Data Source=(local);Integrated Security=SSPI;";

        public QueryBuilderTest()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();

            _table = new DBTable(_tableName);
            DBTableField field = new DBTableField(_table, "Key")
            {
                Schema = new DBFieldSchema(DBType.Int, 0, 0, 0, true, true, true, false)
            };
            _table.Fields.Add(field);

            field = new DBTableField(_table, "Name")
            {
                Schema = new DBFieldSchema(DBType.NVarChar,  255, 0, 0, false, false, false, false)
            };
            _table.Fields.Add(field);

            field = new DBTableField(_table, "Value")
            {
                Schema = new DBFieldSchema(DBType.Decimal,  0, 9, 5, false, false, false, true)
            };
            _table.Fields.Add(field);
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

            


            DBCreateTableQuery createTableQuery = new DBCreateTableQuery(_table);

            result = RunQuery(createTableQuery);
            Assert.AreEqual(result, -1);

            /// Insert 
            DBInsertQuery insertQuery = new DBInsertQuery()
            {
                InsertTable = _table
            };

            foreach (var field in _table.Fields.Where(f => !f.Schema.IsKey))
            {
                insertQuery.AddField(field);
            }

            var sqlCommand = new SqlCommand(insertQuery.Compile(),_connection);
            sqlCommand.Parameters.AddWithValue("@Name", "Oleg");
            sqlCommand.Parameters.AddWithValue("@Value", _checkValue);
            Console.WriteLine(insertQuery.Compile());

            result = sqlCommand.ExecuteNonQuery();
            Assert.AreEqual(result, 1);

            // Select, check value
            DBSelectQuery selectQuery = new DBSelectQuery(_table);

            foreach (var field in _table.Fields.Where(f => !f.Schema.IsKey))
            {
                selectQuery.Select(field);
            }

            
            selectQuery.Where("Key", CompareType.Equals);
            Console.WriteLine(selectQuery.Compile());
            sqlCommand = new SqlCommand(selectQuery.Compile(), _connection);
            sqlCommand.Parameters.AddWithValue("Key", 1);
            using (var reader = sqlCommand.ExecuteReader())
            {

                reader.Read();
                var value = reader.GetDecimal(1);
                Assert.AreEqual(value, _checkValue);
            }
            

            /// Alter Table
            DBAlterTableQuery alterTableQuery = new DBAlterTableQuery(_table.Name);

            alterTableQuery.AddColumn(DBType.Decimal, "Value2", 0, 9, 5, false, false, false, true);

            result = RunQuery(alterTableQuery);
            Assert.AreEqual(result, -1);

            var newField = new DBTableField(_table, "Value2")
            {
                Schema = new DBFieldSchema(DBType.Decimal,  0, 9, 5, false, false, false, true)
            };

            _table.Fields.Add(newField);


            insertQuery = new DBInsertQuery()
            {
                InsertTable = _table
            };

            foreach (var field in _table.Fields.Where(f => !f.Schema.IsKey))
            {
                insertQuery.AddField(field);
            }


            sqlCommand = new SqlCommand(insertQuery.Compile(), _connection);
            sqlCommand.Parameters.AddWithValue("@Name", "Oleg");
            sqlCommand.Parameters.AddWithValue("@Value", _checkValue);
            sqlCommand.Parameters.AddWithValue("@Value2", _checkValue2);
            Console.WriteLine(insertQuery.Compile());

            result = sqlCommand.ExecuteNonQuery();
            Assert.AreEqual(result, 1);

            selectQuery = new DBSelectQuery(_table);

            foreach (var field in _table.Fields.Where(f => !f.Schema.IsKey))
            {
                selectQuery.Select(field);
            }


            selectQuery.Where("Key", CompareType.Equals);
            Console.WriteLine(selectQuery.Compile());
            sqlCommand = new SqlCommand(selectQuery.Compile(), _connection);
            sqlCommand.Parameters.AddWithValue("Key", 2);
            using (var reader = sqlCommand.ExecuteReader())
            {

                reader.Read();
                var value = reader.GetDecimal(2);
                Assert.AreEqual(value, _checkValue2);
            }

            //alter column
            alterTableQuery = new DBAlterTableQuery(_table);

            alterTableQuery.AlterColumn(DBType.Decimal, "Value2", 0, 10, 7, false, false, false, true);

            result = RunQuery(alterTableQuery);
            Assert.AreEqual(result, -1);

            selectQuery = new DBSelectQuery(_table);

            foreach (var field in _table.Fields.Where(f => !f.Schema.IsKey))
            {
                selectQuery.Select(field);
            }


            selectQuery.Where("Key", CompareType.Equals);
            Console.WriteLine(selectQuery.Compile());
            sqlCommand = new SqlCommand(selectQuery.Compile(), _connection);
            sqlCommand.Parameters.AddWithValue("Key", 2);
            using (var reader = sqlCommand.ExecuteReader())
            {

                reader.Read();
                var value = reader.GetDecimal(2);
                Assert.AreEqual(value, _checkValue2);
            }


            // drop column
            alterTableQuery = new DBAlterTableQuery(_table);

            alterTableQuery.DropColumn( "Value2");

            result = RunQuery(alterTableQuery);
            Assert.AreEqual(result, -1);


        }

        int RunQuery(IQueryable query)
        {
            return RunQuery(query.Compile());
        }

        SqlDataReader RunQueryReader(IQueryable query)
        {
            return RunQueryReader(query.Compile());
        }

        SqlDataReader RunQueryReader(string command)
        {
            Console.WriteLine(command);
            SqlCommand cmd = new SqlCommand(command, _connection);
            return cmd.ExecuteReader();
        }


        int RunQuery(string command)
        {
            Console.WriteLine(command);
            SqlCommand cmd = new SqlCommand(command, _connection);
            return cmd.ExecuteNonQuery();
        }

    }
}
