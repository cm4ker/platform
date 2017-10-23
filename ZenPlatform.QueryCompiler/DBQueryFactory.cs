using ZenPlatform.QueryCompiler.Queries;
using ZenPlatform.QueryCompiler.Schema;

namespace ZenPlatform.QueryCompiler
{
    public class DBQueryFactory
    {
        //private readonly DBContext _context;
        //private readonly DBSchemaManager _schemaManager;

        public DBQueryFactory()//DBContext context)
        {
            //_context = context;
            //_schemaManager = new DBSchemaManager(_context);
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
