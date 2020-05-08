using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;

namespace Aquila.QueryBuilder
{
    public class SqlCompillerBase : ISqlCompiler
    {
        private QueryVisitorBase<string> _visitor;

        private SqlCompillerBase(QueryVisitorBase<string> visitor)
        {
            _visitor = visitor;
        }

        public static ISqlCompiler FormEnum(SqlDatabaseType dbType)
        {
            return new SqlCompillerBase(new SQLVisitorBase());
        }

        public string Compile(SSyntaxNode node)
        {
            return _visitor.Visit(node);
        }

        public string Compile(QueryMachine queryMachine)
        {
            return _visitor.Visit((SSyntaxNode) queryMachine.peek());
        }
    }
}