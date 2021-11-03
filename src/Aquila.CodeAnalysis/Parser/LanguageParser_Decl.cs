namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    partial class LanguageParser
    {
        // MemberDecl ParseMemberDecl()
        // {
        //     var type = ParseType();
        //     var modifiers = _pool.Allocate();
        //
        //     ParseModifiers(modifiers, false);
        //     var paramList = this.ParseParenthesizedParameterList();
        //     var attrs = _pool.Allocate<AttributeListSyntax>();
        //     return SyntaxFactory.MethodDecl(attrs.ToList(), modifiers.ToList(),
        //         new PredefinedTypeEx(SyntaxKind.IntKeyword, SyntaxToken.Identifier("int")),
        //         SyntaxToken.StringLiteral("test"), null, null);
        // }
        //
        // TypeEx ParseType()
        // {
        //     var token = EatToken();
        //     switch (token.Kind)
        //     {
        //         case SyntaxKind.IntKeyword:
        //             return SyntaxFactory.PredefinedTypeEx(token);
        //         default:
        //             return SyntaxFactory.NamedTypeEx(token);
        //     }
        // }
    }
}