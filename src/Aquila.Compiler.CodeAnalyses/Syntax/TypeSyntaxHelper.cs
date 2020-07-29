using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Misc;

namespace Aquila.Compiler
{
    public static class TypeSyntaxHelper
    {
        public static TypeSyntax Create(ILineInfo info, string typeName)
        {
            switch (typeName)
            {
                case "int": return new PredefinedTypeSyntax(info, SyntaxKind.IntKeyword);
                case "bool": return new PredefinedTypeSyntax(info, SyntaxKind.BoolKeyword);
                case "string": return new PredefinedTypeSyntax(info, SyntaxKind.StringKeyword);
                case "double": return new PredefinedTypeSyntax(info, SyntaxKind.DoubleKeyword);
                case "void": return new PredefinedTypeSyntax(info, SyntaxKind.VoidKeyword);
                default: return new NamedTypeSyntax(info, SyntaxKind.Type, typeName);
            }
        }
    }
}