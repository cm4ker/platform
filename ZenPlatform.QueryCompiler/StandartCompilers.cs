using System;

namespace ZenPlatform.QueryBuilder
{
    public static class StandartCompilers
    {

        public static string CompileAliasedObject(string expression, string alias)
        {

            var @as = (string.IsNullOrEmpty(alias)) ? "" : SQLTokens.AS;
            alias = (string.IsNullOrEmpty(alias)) ? "" : $"[{alias}]";

            return expression + string.Format(" {0} {1}", @as, alias);
        }


        //public static string SimpleCompiler(string compileExpression, dynamic parameters)
        //{
        //    return Smart.Format(compileExpression, parameters).Trim();
        //}



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