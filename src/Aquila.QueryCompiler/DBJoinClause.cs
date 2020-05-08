using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquila.QueryBuilder.Interfaces;

namespace Aquila.QueryBuilder
{
    /// <summary>
    /// Represents JOIN clause between two datasources (table\query\udtf)
    /// </summary>
    public class DBJoinClause : IDBToken
    {
        private readonly IDBDataSource _tds;
        private readonly JoinType _jt;
        private DBClause _clause1;
        private DBClause _clause2;
        private string _compileExpression;

        private List<DBClause> _clauses;

        public DBJoinClause(IDBDataSource tds, JoinType jt)
        {
            _tds = tds;
            _jt = jt;
            _clauses = new List<DBClause>();

            _compileExpression = $"{{0}} {SQLTokens.JOIN} {{1}}\n\t{SQLTokens.ON} ";
        }


        public DBClause Clause1
        {
            get { return _clause1; }
            set { _clause1 = value; }
        }

        public DBClause Clause2
        {
            get { return _clause2; }
            set { _clause2 = value; }
        }

        public DBJoinClause On(DBClause clause1, CompareType compare, DBClause clause2)
        {
            var openBracket = new DBRawTokenClause("(");
            var comparer = compare.GetCompareToken();
            var closeBracket = new DBRawTokenClause(")");

            _clauses.AddRange(new[] { openBracket, clause1, comparer, clause2, closeBracket });

            return this;
        }

        public DBJoinClause AndOn(DBClause clause1, CompareType compare, DBClause clause2)
        {
            if (_clauses.Any())
                _clauses.Add(new DBRawTokenClause(SQLTokens.AND));

            return On(clause1, compare, clause2);
        }

        public IDBDataSource DbTableDataSorce
        {
            get { return _tds; }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(_compileExpression, StandartCompilers.CompileJoin(_jt), _tds.Compile());

            foreach (var clause in _clauses)
            {
                sb.AppendFormat("{0} ", clause.Compile());
            }

            return sb.ToString().Trim();
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }
    }


    public enum JoinType
    {
        Inner,
        Left

    }
}