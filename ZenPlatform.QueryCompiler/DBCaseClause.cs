using System.Text;

namespace ZenPlatform.QueryCompiler
{
    public class DBCaseClause : DBClause
    {
        private DBClause _thenClause;
        private DBClause _elseClause;
        private DBLogicalClause _logicalClause;

        public DBCaseClause(DBLogicalClause op, DBClause then, DBClause @else)
        {
            _thenClause = then;
            _elseClause = @else;
            _logicalClause = op;
        }

        public override string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7}",
                SQLTokens.CASE, SQLTokens.WHEN, _logicalClause.Compile(), SQLTokens.THEN, _thenClause.Compile(),
                SQLTokens.ELSE, _elseClause.Compile(), SQLTokens.END);
            return sb.ToString();
        }
    }
}