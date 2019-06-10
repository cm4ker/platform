using ZenPlatform.Language.Ast.AST.Builder;

namespace ZenPlatform.Configuration.Data.Contracts
{
    public interface IAstGenerator
    {
        void Build(AstBuilder builder);
    }
}