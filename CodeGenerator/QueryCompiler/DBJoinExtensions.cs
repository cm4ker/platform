using System;

namespace QueryCompiler
{
    public static class DBJoinExtensions
    {
        public static IDBTableDataSorce Table(this IDBTableDataSourceContainer container, string alias)
        {
            return container.GetTable(alias);
        }

    }

    public static class DBExtensinons
    {
        public static DBFixedTokenClause GetCompareToken(this CompareType compare)
        {
            switch (compare)
            {
                case CompareType.Equals:
                    return new DBFixedTokenClause("=");
                case
                CompareType.GreatThen:
                    return new DBFixedTokenClause(">");
                case CompareType.In:
                    return new DBFixedTokenClause(SQLTokens.IN);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}