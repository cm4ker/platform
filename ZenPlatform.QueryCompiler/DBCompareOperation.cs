using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.QueryBuilder
{
    public class DBCompareOperation : DBClause
    {
        private List<DBClause> _clauses;

        public DBCompareOperation()
        {
            _clauses = new List<DBClause>();
        }

        public void AddIsNullClause(DBClause clause, bool negotiation = false)
        {
            var openBracket = new DBRawTokenClause("(");
            var spaceToten = new DBRawTokenClause(" ");
            var isToken = new DBRawTokenClause(SQLTokens.IS);
            var nullToken = new DBRawTokenClause(SQLTokens.NULL);
            var closeBracket = new DBRawTokenClause(")");

            if (negotiation)
            {
                var not = new DBRawTokenClause(SQLTokens.NOT);
                var space = new DBRawTokenClause(" ");
                _clauses.AddRange(new[] { not, space });
            }

            _clauses.AddRange(new[] { openBracket, clause, spaceToten, isToken, spaceToten, nullToken, closeBracket });
        }


        public void AddClause(DBClause clause1, CompareType type, DBClause clause2, bool negotiation = false)
        {
            var openBracket = new DBRawTokenClause("(");
            var comparer = type.GetCompareToken();
            var closeBracket = new DBRawTokenClause(")");
            var space = new DBRawTokenClause(" ");
            if (negotiation)
            {
                var not = new DBRawTokenClause(SQLTokens.NOT);
                _clauses.AddRange(new[] { not, space });
            }
            _clauses.AddRange(new[] { openBracket, clause1, space, comparer, space, clause2, closeBracket });
        }

        public override string Compile(bool recompile = false)
        {
            return _clauses.Aggregate("", (s, clause) => s + clause.Compile());
        }
    }
}