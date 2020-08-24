using Aquila.Syntax.Ast;
using Aquila.Syntax.Text;

namespace Aquila.Syntax
{
    public static class TypeSyntaxHelper
    {
        public static TypeRef Create(Span info, string typeName)
        {
            switch (typeName)
            {
                case "int": return new PredefinedTypeRef(info, SyntaxKind.IntKeyword);
                case "bool": return new PredefinedTypeRef(info, SyntaxKind.BoolKeyword);
                case "string": return new PredefinedTypeRef(info, SyntaxKind.StringKeyword);
                case "double": return new PredefinedTypeRef(info, SyntaxKind.DoubleKeyword);
                // case "void": return new PredefinedTypeRef(info, SyntaxKind.VoidKeyword);
                default: return new NamedTypeRef(info, SyntaxKind.Type, typeName);
            }
        }

        public static TypeRef CreateVoid(Span info)
        {
            return new PredefinedTypeRef(info, SyntaxKind.VoidKeyword);
        }
    }
}