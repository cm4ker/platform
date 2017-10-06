using System;
using System.Collections.Generic;
using System.Text;
using QueryCompiler.Interfaces;

namespace QueryCompiler
{
    public class DBOrderByClause : IDBToken
    {
        private List<DBClause> _clauses;

        public DBOrderByClause()
        {
            _clauses = new List<DBClause>();
        }

        public List<DBClause> Clauses => _clauses;

        public void ThenOrderBy(DBClause clause)
        {
            if (_clauses.Count > 0)
            {
                _clauses.Add(new DBFixedTokenClause(","));
                _clauses.Add(new DBFixedTokenClause(" "));
            }

            _clauses.Add(clause);
        }

        public void ThenOrderByDesc(DBClause clause)
        {
            if (_clauses.Count > 0)
            {
                _clauses.Add(new DBFixedTokenClause(","));
                _clauses.Add(new DBFixedTokenClause(" "));
            }
            _clauses.Add(clause);
            _clauses.Add(new DBFixedTokenClause(" "));
            _clauses.Add(new DBFixedTokenClause(SQLTokens.DESC));
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.Append(SQLTokens.ORDER_BY + " ");

            foreach (var clause in _clauses)
            {
                sb.AppendFormat("{0}", clause.Compile());
            }

            return sb.ToString().Trim();
        }

        public string CompileExpression { get; set; }
    }
}