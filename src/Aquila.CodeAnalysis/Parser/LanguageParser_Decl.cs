using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
{
    partial class LanguageParser
    {
        ModuleDecl ParseModule()
        {
            Debug.Assert(CurrentToken.Kind == SyntaxKind.ModuleKeyword);

            SyntaxToken moduleKeyword = EatToken(SyntaxKind.ModuleKeyword);

            NameEx name = this.ParseQualifiedName();
            if (name.IsMissing && this.PeekToken(1).Kind == SyntaxKind.SemicolonToken)
            {
                //if we can see a semicolon ahead, then the current token was
                //probably supposed to be an identifier
                name = AddTrailingSkippedSyntax(name, this.EatToken());
            }

            SyntaxToken semicolon = this.EatToken(SyntaxKind.SemicolonToken);
            return SyntaxFactory.ModuleDecl(moduleKeyword, name, semicolon);
        }

        ImportDecl ParseImport()
        {
            Debug.Assert(CurrentToken.Kind == SyntaxKind.ImportKeyword);

            SyntaxToken importKeyword = EatToken(SyntaxKind.ImportKeyword);
            SyntaxToken clrKeyword = null;

            if (PeekToken(0).Kind == SyntaxKind.ClrKeyword)
            {
                clrKeyword = EatToken(SyntaxKind.ClrKeyword);
            }


            NameEx name = this.ParseQualifiedName();
            if (name.IsMissing && this.PeekToken(1).Kind == SyntaxKind.SemicolonToken)
            {
                //if we can see a semicolon ahead, then the current token was
                //probably supposed to be an identifier
                name = AddTrailingSkippedSyntax(name, this.EatToken());
            }


            SyntaxToken semicolon = this.EatToken(SyntaxKind.SemicolonToken);
            return SyntaxFactory.ImportDecl(importKeyword, clrKeyword, name, semicolon);
        }

        TypeDecl ParseTypeDecl(SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers)
        {
            Debug.Assert(CurrentToken.Kind == SyntaxKind.TypeKeyword);

            var importKeyword = EatToken(SyntaxKind.TypeKeyword);
            NameEx name = this.ParseQualifiedName();
            if (name.IsMissing && this.PeekToken(1).Kind == SyntaxKind.SemicolonToken)
            {
                //if we can see a semicolon ahead, then the current token was
                //probably supposed to be an identifier
                name = AddTrailingSkippedSyntax(name, this.EatToken());
            }

            var members = _pool.Allocate<FieldDecl>();
            try
            {
                var openBrace = EatToken(SyntaxKind.OpenBraceToken);

                while (CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    var decl = ParseMemberDeclaration(SyntaxKind.TypeDecl);
                    if (decl.Kind == SyntaxKind.FieldDecl)
                        members.Add((FieldDecl)decl);
                    else
                    {
                        //error
                    }
                }

                var closeBrace = EatToken(SyntaxKind.CloseBraceToken);

                return SyntaxFactory.TypeDecl(attributes, modifiers.ToList(), importKeyword, name, openBrace, members,
                    closeBrace);
            }
            finally
            {
                _pool.Free(members);
            }
        }

        FieldDecl ParseFieldDecl(TypeEx type, SyntaxToken identifier)
        {
            var semicolon = EatToken(SyntaxKind.SemicolonToken);
            return SyntaxFactory.FieldDecl(null, null, type, identifier, semicolon);
        }

        MemberDecl ParseFuncDecl(SyntaxKind parentKind, SyntaxList<AttributeListSyntax> attributes,
            SyntaxListBuilder modifiers)
        {
            var fnKeyword = EatToken(SyntaxKind.FnKeyword);

            FuncOwnerSyntax? fos = null;

            if (IsFuncOwnerSyntax())
                fos = ParseFuncOwner();

            // At this point we can either have indexers, methods, or 
            // properties (or something unknown).  Try to break apart
            // the following name and determine what to do from there.
            SyntaxToken identifierOrThisOpt;
            TypeParameterListSyntax typeParameterListOpt;
            this.ParseMemberName(out identifierOrThisOpt, out typeParameterListOpt, isEvent: false);

            var paramList = this.ParseParenthesizedParameterList();

            TypeEx? type = null;
            if (IsPossibleType())
                type = ParseReturnType();

            if (IsNoneOrIncompleteMember(parentKind, attributes, modifiers, type, identifierOrThisOpt,
                    typeParameterListOpt,
                    out var result))
                return result;

            BlockStmt blockBody;
            ArrowExClause expressionBody;
            SyntaxToken semicolon;

            // Method declarations cannot be nested or placed inside async lambdas, and so cannot occur in an
            // asynchronous context. Therefore the IsInAsync state of the parent scope is not saved and
            // restored, just assumed to be false and reset accordingly after parsing the method body.
            Debug.Assert(!IsInAsync);

            IsInAsync = modifiers.Any((int)SyntaxKind.AsyncKeyword);

            this.ParseBlockAndExpressionBodiesWithSemicolon(out blockBody, out expressionBody, out semicolon);

            return _syntaxFactory.FuncDecl(attributes, modifiers.ToList(), fnKeyword, fos, identifierOrThisOpt,
                typeParameterListOpt, paramList, type, blockBody, expressionBody, semicolon);
        }

        private bool IsFuncOwnerSyntax() => PeekToken(0).Kind == SyntaxKind.OpenParenToken;
        private bool IsFunc() => PeekToken(0).Kind == SyntaxKind.FnKeyword;

        FuncOwnerSyntax ParseFuncOwner()
        {
            var openParen = EatToken(SyntaxKind.OpenParenToken);
            var identifier = ParseIdentifierToken();
            var type = ParseType();
            var closeParen = EatToken(SyntaxKind.CloseParenToken);

            //TODO: handle errors

            return _syntaxFactory.FuncOwner(openParen, identifier, type, closeParen);
        }
    }
}