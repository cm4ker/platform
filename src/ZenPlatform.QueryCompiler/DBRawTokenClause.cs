namespace ZenPlatform.QueryBuilder
{
    public class DBRawTokenClause : DBClause
    {
        private readonly string _token;

        public DBRawTokenClause(string token)
        {
            _token = token;
        }

        public override string Compile(bool recompile = false)
        {
            return _token;
        }
    }
}