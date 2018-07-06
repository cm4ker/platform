namespace ZenPlatform.QueryBuilder
{
    public class PostgresCompiller : SqlCompillerBase
    {
        public override string StartNameLiteral { get; } = "\"";
        public override string EndNameLiteral { get; } = "\"";
    }
}