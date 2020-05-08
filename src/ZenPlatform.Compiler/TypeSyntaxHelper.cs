using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler
{
    public static class TypeSyntaxHelper
    {
        public static TypeSyntax Create(ILineInfo info, string typeName)
        {
            var kind = typeName switch
            {
                "string" => TypeNodeKind.String,
                "int" => TypeNodeKind.Int,
                "uid" => TypeNodeKind.Uid,
                "object" => TypeNodeKind.Object,
                "bool" => TypeNodeKind.Boolean,
                "double" => TypeNodeKind.Double,
                "char" => TypeNodeKind.Char,
                "void" => TypeNodeKind.Void,
                _ => TypeNodeKind.Type
            };

            if (kind == TypeNodeKind.Type) return new SingleTypeSyntax(info, typeName, kind);

            return new PrimitiveTypeSyntax(info, kind);
        }
    }
}