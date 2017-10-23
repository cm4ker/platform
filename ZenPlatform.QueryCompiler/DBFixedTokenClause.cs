namespace ZenPlatform.QueryCompiler
{
    public class DBFixedTokenClause : DBClause
    {
        private readonly string _token;

        public DBFixedTokenClause(string token)
        {
            _token = token;
        }


        public override string Compile(bool recompile = false)
        {
            return _token;
        }
    }
}