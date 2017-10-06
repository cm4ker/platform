using System.Collections.Generic;
using System.Linq;

namespace QueryCompiler
{
    public class DBCompareOperation : DBClause
    {
        private List<DBClause> _clauses;

        public DBCompareOperation()
        {
            _clauses = new List<DBClause>();
        }

        public void AddIsNullClause(DBClause clause)
        {
            var openBracket = new DBFixedTokenClause("(");
            var spaceToten = new DBFixedTokenClause(" ");
            var isToken = new DBFixedTokenClause(SQLTokens.IS);
            var nullToken = new DBFixedTokenClause(SQLTokens.NULL);
            var closeBracket = new DBFixedTokenClause(")");

            _clauses.AddRange(new[] { openBracket, clause, spaceToten, isToken, spaceToten, nullToken, closeBracket });
        }

        public void AddClause(DBClause clause1, CompareType type, DBClause clause2)
        {
            var openBracket = new DBFixedTokenClause("(");
            var comparer = type.GetCompareToken();
            var closeBracket = new DBFixedTokenClause(")");
            var space = new DBFixedTokenClause(" ");
            _clauses.AddRange(new[] { openBracket, clause1, space, comparer, space, clause2, closeBracket });
        }

        public override string Compile(bool recompile = false)
        {
            return _clauses.Aggregate("", (s, clause) => s + clause.Compile());
        }
    }
}