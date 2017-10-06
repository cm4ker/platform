using System;
using System.Collections.Generic;
using System.Text;
using QueryCompiler.Interfaces;

namespace QueryCompiler
{
    public class DBGroupByClause : IDBToken
    {
        public List<DBClause> Clauses => _clauses;

        private List<DBClause> _clauses;

        public DBGroupByClause()
        {
            _clauses = new List<DBClause>();
        }

        public void ThenGroupBy(DBClause clause)
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
            sb.Append(SQLTokens.GROUP_BY);

            foreach (var clause in _clauses)
            {
                sb.AppendFormat("\n\t{0}", clause.Compile());
            }

            return sb.ToString().Trim();
        }

        public string CompileExpression { get; set; }
    }
}