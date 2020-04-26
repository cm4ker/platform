using System;

namespace ZenPlatform.QueryBuilder
{
    public static class StandartCompilers
    {

        public static string CompileAliasedObject(string expression, string alias)
        {
            alias = (string.IsNullOrEmpty(alias)) ? "" : $"{SQLTokens.AS} [{alias}]";

            if (string.IsNullOrEmpty(alias))
                return expression;
            return expression + string.Format(" {0}", alias);
        }

        public static string CompileComparer(CompareType compare)
        {
            var result = "";

            switch (compare)
            {
                case CompareType.Equals:
                    result = "="; break;
                default: throw new Exception();
            }

            return result;
        }

        public static object CompileJoin(JoinType joinType)
        {
            var result = "";

            switch (joinType)
            {
                case JoinType.Inner:
                    result = SQLTokens.INNER; break;
                case JoinType.Left:
                    result = SQLTokens.LEFT; break;
                default: throw new Exception();
            }

            return result;
        }
    }
}