using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder
{
    public interface ISqlCompiler
    {
        string Compile(SSyntaxNode node);

        string Compile(QueryMachine queryMachine);
    }
}