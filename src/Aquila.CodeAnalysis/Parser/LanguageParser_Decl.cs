using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    partial class LanguageParser
    {
        ImportDecl ParseImport()
        {
            Debug.Assert(CurrentToken.Kind == SyntaxKind.ImportKeyword);

            var importKeyword = EatToken(SyntaxKind.ImportKeyword);
            NameEx name = this.ParseQualifiedName();
            if (name.IsMissing && this.PeekToken(1).Kind == SyntaxKind.SemicolonToken)
            {
                //if we can see a semicolon ahead, then the current token was
                //probably supposed to be an identifier
                name = AddTrailingSkippedSyntax(name, this.EatToken());
            }

            SyntaxToken semicolon = this.EatToken(SyntaxKind.SemicolonToken);
            return SyntaxFactory.ImportDecl(importKeyword, name, semicolon);
        }
    }

    partial class LanguageParser
    {
        
    }
}