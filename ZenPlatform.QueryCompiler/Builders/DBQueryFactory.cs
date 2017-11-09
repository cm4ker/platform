using ZenPlatform.QueryBuilder.Queries;
using ZenPlatform.QueryBuilder.Schema;
using System;

namespace ZenPlatform.QueryBuilder
{
    public class DBQueryFactory
    {

        private static DBQueryFactory _instance = new DBQueryFactory();

        public static DBQueryFactory Instance { get { return _instance; } }

        public T GetQuery<T>() where T : IQueryable
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public DBQueryFactory()
        {
           
        }

        public DBSelectQuery GetSelect()
        {
            return new DBSelectQuery();
        }

        public DBUpdateQuery GetUpdate()
        {
            return new DBUpdateQuery();
        }

        public DBDeleteQuery GetDelete()
        {
            return new DBDeleteQuery();
        }

        public DBInsertQuery GetInsert()
        {
            return new DBInsertQuery();
        }

        public DBCreateTableQuery GetCreateTable()
        {
            return new DBCreateTableQuery();
        }

        public DBCreateDatabaseQuery CreateDatabase(string databaseName)
        {
            return new DBCreateDatabaseQuery(databaseName);
        }

        public DBSetIdentityInsert CreateSetIdentity(string tableName, bool isOn)
        {
            return new DBSetIdentityInsert(tableName, isOn);
        }

        public DBTable CreateTable(string name, string alias = "")
        {
            DBTable table = new DBTable(name, alias);
            return table;
        }
        
    }
}
