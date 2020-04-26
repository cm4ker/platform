using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.QueryBuilder
{
    public class DBQueryCompiler
    {

        public DBSelectQuery CreateSelect()
        {
            return new DBSelectQuery();
        }

        public DBUpdateQuery CreateUpdate()
        {
            return new DBUpdateQuery();
        }

        public DBDeleteQuery CreateDelete()
        {
            return new DBDeleteQuery();
        }

        public DBInsertQuery CreateInsert()
        {
            return new DBInsertQuery();
        }

        public DBSetIdentityInsert CreateSetIdentity(string tableName, bool isOn)
        {
            return new DBSetIdentityInsert(tableName, isOn);
        }

        //public DBTable CreateTable(string name, string alias = "")
        //{

        //    DBTable table = new DBTable(name, _schemaManager, alias);
        //    return table;
        //}
    }
}
