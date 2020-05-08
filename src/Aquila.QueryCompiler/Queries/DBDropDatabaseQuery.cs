using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.QueryBuilder.Queries
{
    public class DBDropDatabaseQuery: IQueryable
    {
        public bool DropIfExist { get; set; }
        private string _databaseName;

        public DBDropDatabaseQuery()
        {

        }

          public DBDropDatabaseQuery(string databaseName)
        {
            _databaseName = databaseName;
        }

        public void Database(string databaseName)
        {
            _databaseName = databaseName;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2}[{3}]", SQLTokens.DROP, SQLTokens.DATABASE, DropIfExist ? $"{SQLTokens.IF} {SQLTokens.EXISTS} " : "",  _databaseName);
            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}
