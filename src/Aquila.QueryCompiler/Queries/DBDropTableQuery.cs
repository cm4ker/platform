using System;
using System.Text;

namespace Aquila.QueryBuilder.Queries
{
    public class DBDropTableQuery : IQueryable
    {
        private string _tableName;

        public bool DropIfExist { get; set; }

        public DBDropTableQuery(string tableName)
        {
            Table(tableName);
        }

        public void Table(string tableName)
        {
            _tableName = tableName;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2}[{3}]", SQLTokens.DROP, SQLTokens.TABLE, DropIfExist ? $"{SQLTokens.IF} {SQLTokens.EXISTS} " : "", _tableName);
            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}
