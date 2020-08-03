using System;
using Aquila.Language.Ast.Definitions;
using Aquila.Syntax;
using Aquila.Syntax.Text;

namespace Aquila.Compiler
{
    public static class TypeSyntaxHelper
    {
        public static TypeRef Create(Span info, string typeName)
        {
            throw new Exception();

            // switch (typeName)
            // {
            //     case "int": return new PredefinedTypeSyntax(info, SyntaxKind.IntKeyword);
            //     case "bool": return new PredefinedTypeSyntax(info, SyntaxKind.BoolKeyword);
            //     case "string": return new PredefinedTypeSyntax(info, SyntaxKind.StringKeyword);
            //     case "double": return new PredefinedTypeSyntax(info, SyntaxKind.DoubleKeyword);
            //     case "void": return new PredefinedTypeSyntax(info, SyntaxKind.VoidKeyword);
            //     default: return new NamedTypeSyntax(info, SyntaxKind.Type, typeName);
            // }
        }
    }
}