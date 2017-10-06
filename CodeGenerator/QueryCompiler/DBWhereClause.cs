using System;
using System.Text;

namespace QueryCompiler
{
    public class DBWhereClause : IToken
    {
        //private List<DBClause> _clauses;
        private DBLogicalClause _logicalClause;
        private DBParameterCollection _parameters;

        public DBWhereClause()
        {
            _parameters = new DBParameterCollection();
            _logicalClause = new DBLogicalClause();
        }

        public DBLogicalClause LogicalClause
        {
            get { return _logicalClause; }
        }

        public DBLogicalOperation Where(DBClause clause1, CompareType type, DBClause clause2)
        {
            return _logicalClause.Where(clause1, type, clause2);
        }

        public DBLogicalOperation Where(DBClause clause)
        {
            return _logicalClause.Where(clause);
        }

        public DBParameterCollection Parameters
        {
            get { return _logicalClause.Parameters; }
        }

        public object Clone()
        {
            throw new NotImplementedException();
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