using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Misc;

namespace Aquila.Compiler
{
    public static class TypeSyntaxHelper
    {
        public static TypeSyntax Create(ILineInfo info, string typeName)
        {
            return new TypeSyntax(info, SyntaxKind.Type, typeName);
        }
    }
}