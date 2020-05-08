using System;

namespace Aquila.QueryBuilder
{

    public static class DBExtensinons
    {
        public static DBRawTokenClause GetCompareToken(this CompareType compare)
        {
            switch (compare)
            {
                case CompareType.Equals:
                    return new DBRawTokenClause("=");
                case
                CompareType.GreatThen:
                    return new DBRawTokenClause(">");
                case CompareType.In:
                    return new DBRawTokenClause(SQLTokens.IN);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}