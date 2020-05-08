using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder
{
    public interface ISqlCompiler
    {
        string Compile(SSyntaxNode node);

        string Compile(QueryMachine queryMachine);
    }
}