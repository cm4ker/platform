using System;
using System.Collections.Generic;
using System.Text;

namespace QueryCompiler
{
    public class DBOrderByClause : IToken
    {
        private List<DBClause> _clauses;

        public DBOrderByClause()
        {
            _clauses = new List<DBClause>();
        }

        public List<DBClause> Clauses => _clauses;

        public void ThenOrderBy(DBClause clause)
        {
            _clauses.Add(clause);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.Append(SQLTokens.ORDER_BY);

            foreach (var clause in _clauses)
            {
                sb.AppendFormat("\t\n{0}", clause.Compile());
            }

            return sb.ToString().Trim();
        }

        public string CompileExpression { get; set; }
    }
}