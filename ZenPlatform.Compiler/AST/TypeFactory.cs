using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.AST
{
    public class TypeFactory
    {
        public static IType Create(string token)
        {
            return new PseudoType(token);
        }
    }
}