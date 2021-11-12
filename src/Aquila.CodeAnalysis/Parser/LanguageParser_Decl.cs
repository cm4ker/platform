using System.Diagnostics;

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
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

        ExtendDecl ParseExtend()
        {
            Debug.Assert(CurrentToken.Kind == SyntaxKind.ExtendKeyword);

            var importKeyword = EatToken(SyntaxKind.ExtendKeyword);
            NameEx name = this.ParseQualifiedName();
            if (name.IsMissing && this.PeekToken(1).Kind == SyntaxKind.SemicolonToken)
            {
                //if we can see a semicolon ahead, then the current token was
                //probably supposed to be an identifier
                name = AddTrailingSkippedSyntax(name, this.EatToken());
            }

            var members = _pool.Allocate<MethodDecl>();
            try
            {
                var openBrace = EatToken(SyntaxKind.OpenBraceToken);

                while (CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    var decl = ParseMemberDeclaration(SyntaxKind.ExtendDecl);
                    if (decl.Kind == SyntaxKind.MethodDecl)
                        members.Add((MethodDecl)decl);
                }

                var closeBrace = EatToken(SyntaxKind.CloseBraceToken);

                return SyntaxFactory.ExtendDecl(importKeyword, name, openBrace, members.ToList(), closeBrace);
            }
            finally
            {
                _pool.Free(members);
            }
        }
    }
}
