namespace ZenPlatform.QueryBuilder
{
    public class DBSelectAliasTransformation : DBCompileTransformation
    {
        private readonly string _alias;


        public DBSelectAliasTransformation(string alias)
        {
            _alias = alias;
        }

        public override string Apply(string compileExpression)
        {
            return $"{compileExpression} {SQLTokens.AS} [{_alias}]";
        }
    }
}