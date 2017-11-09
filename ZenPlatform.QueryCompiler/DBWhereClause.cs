using System.Text;
using ZenPlatform.QueryBuilder.Interfaces;

namespace ZenPlatform.QueryBuilder
{
    public class DBWhereClause : IDBToken
    {
        //private List<DBClause> _clauses;
        private DBLogicalClause _logicalClause;

        public DBWhereClause()
        {

            _logicalClause = new DBLogicalClause();
        }

        public DBLogicalClause LogicalClause
        {
            get { return _logicalClause; }
        }

        public DBLogicalOperation Where(DBClause clause1, CompareType type, DBClause clause2)
        {
            return _logicalClause.Where(clause1, type, clause2, false);
        }

        public DBLogicalOperation WhereNot(DBClause clause1, CompareType type, DBClause clause2)
        {
            return _logicalClause.Where(clause1, type, clause2, true);
        }

        public DBParameterCollection Parameters
        {
            get { return _logicalClause.Parameters; }
        }

        public void Clear()
        {
            _logicalClause = new DBLogicalClause();
        }

        public object Clone()
        {
            var clone = this.MemberwiseClone() as DBWhereClause;
            clone._logicalClause = _logicalClause.Clone() as DBLogicalClause;

            return clone;
        }

        public string Compile(bool recompile = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(SQLTokens.WHERE);

            sb.AppendFormat("{0} ", _logicalClause.Compile());

            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}