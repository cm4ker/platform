using System;
using SmartFormat;

namespace QueryCompiler
{
    public static class StandartCompilers
    {

        public static string CompileAliasedObject(string compileExpression, dynamic parameters)
        {

            var @as = (string.IsNullOrEmpty(parameters.Alias)) ? "" : SQLTokens.AS;
            var alias = (string.IsNullOrEmpty(parameters.Alias)) ? "" : $"[{parameters.Alias}]";

            return Smart.Format(compileExpression + " {1} {2}", parameters, @as, alias).Trim();
        }


        public static string SimpleCompiler(string compileExpression, dynamic parameters)
        {
            return Smart.Format(compileExpression, parameters).Trim();
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