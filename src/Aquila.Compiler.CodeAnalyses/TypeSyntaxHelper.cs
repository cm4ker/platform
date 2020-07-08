using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Misc;
using PrimitiveTypeSyntax = Aquila.Language.Ast.PrimitiveTypeSyntax;
using SingleTypeSyntax = Aquila.Language.Ast.SingleTypeSyntax;
using TypeSyntax = Aquila.Language.Ast.TypeSyntax;

namespace Aquila.Compiler
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