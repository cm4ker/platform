using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;

namespace ZenPlatform.QueryBuilder
{
    public enum SqlDatabaseType
    {
        SqlServer,
        Postgres,
        MySql,
        Oracle
    }

    public class SqlCompillerBase: ISqlCompiler
    {
        private QueryVisitorBase<string> _visitor;
        private SqlCompillerBase(QueryVisitorBase<string> visitor)
        {
            _visitor = visitor;
        }
        public static ISqlCompiler FormEnum(SqlDatabaseType dbtype)
        {
            return new SqlCompillerBase(new SQLVisitorBase());
        }

        public string Compile(SSyntaxNode node)
        {
            return _visitor.Visit(node);
        }

        public string Compile(QueryMachine queryMachine)
        {
            return _visitor.Visit((SSyntaxNode)queryMachine.Peek());
        }
    }
}