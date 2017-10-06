using System;

namespace QueryCompiler
{
    public class DbSelectAliasTransformation : DBCompileTransformation
    {
        private readonly string _alias;


        public DbSelectAliasTransformation(string alias)
        {
            _alias = alias;
        }

        public override string Apply(string compileExpression)
        {
            return $"{compileExpression} {SQLTokens.AS} [{_alias}]";
        }
    }
}