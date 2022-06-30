// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Aquila.CodeAnalysis.Errors;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
{
    using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

    internal partial class LanguageParser : SyntaxParser
    {
        // list pools - allocators for lists that are used to build sequences of nodes. The lists
        // can be reused (hence pooled) since the syntax factory methods don't keep references to
        // them

        private readonly SyntaxListPool _pool = new SyntaxListPool(); // Don't need to reset this.

        private readonly SyntaxFactoryContext _syntaxFactoryContext; // Fields are resettable.
        private readonly ContextAwareSyntax _syntaxFactory; // Has context, the fields of which are resettable.

        private int _recursionDepth;
        private TerminatorState _termState; // Resettable
        private bool _isInTry; // Resettable
        private bool _checkedTopLevelStatementsFeatureAvailability; // Resettable

        // NOTE: If you add new state, you should probably add it to ResetPoint as well.

        internal LanguageParser(
            Lexer lexer,
            Aquila.CodeAnalysis.AquilaSyntaxNode oldTree,
            IEnumerable<TextChangeRange> changes,
            LexerMode lexerMode = LexerMode.Syntax,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(lexer, lexerMode, oldTree, changes, allowModeReset: false,
                preLexIfNotIncremental: true, cancellationToken: cancellationToken)
        {
            _syntaxFactoryContext = new SyntaxFactoryContext();
            _syntaxFactory = new ContextAwareSyntax(_syntaxFactoryContext);
        }

        private static bool IsSomeWord(SyntaxKind kind)
        {
            return kind == SyntaxKind.IdentifierToken || SyntaxFacts.IsKeywordKind(kind);
        }

        // Parsing rule terminating conditions.  This is how we know if it is 
        // okay to abort the current parsing rule when unexpected tokens occur.

        [Flags]
        internal enum TerminatorState
        {
            EndOfFile = 0,
            IsNamespaceMemberStartOrStop = 1 << 0,
            IsAttributeDeclarationTerminator = 1 << 1,
            IsPossibleAggregateClauseStartOrStop = 1 << 2,
            IsPossibleMemberStartOrStop = 1 << 3,
            IsEndOfReturnType = 1 << 4,
            IsEndOfParameterList = 1 << 5,
            IsEndOfFieldDeclaration = 1 << 6,
            IsPossibleEndOfVariableDeclaration = 1 << 7,
            IsEndOfTypeArgumentList = 1 << 8,
            IsPossibleStatementStartOrStop = 1 << 9,
            IsEndOfFixedStatement = 1 << 10,
            IsEndOfTryBlock = 1 << 11,
            IsEndOfCatchClause = 1 << 12,
            IsEndOfFilterClause = 1 << 13,
            IsEndOfCatchBlock = 1 << 14,
            IsEndOfDoWhileExpression = 1 << 15,
            IsEndOfForStatementArgument = 1 << 16,
            IsEndOfDeclarationClause = 1 << 17,
            IsEndOfArgumentList = 1 << 18,
            IsSwitchSectionStart = 1 << 19,
            IsEndOfTypeParameterList = 1 << 20,
            IsEndOfMethodSignature = 1 << 21,
            IsEndOfNameInExplicitInterface = 1 << 22,
            IsEndOfFunctionPointerParameterList = 1 << 23,
            IsEndOfFunctionPointerParameterListErrored = 1 << 24,
            IsEndOfFunctionPointerCallingConvention = 1 << 25,
            IsEndOfRecordSignature = 1 << 26,
        }

        private const int LastTerminatorState = (int)TerminatorState.IsEndOfRecordSignature;

        private bool IsTerminator()
        {
            if (this.CurrentToken.Kind == SyntaxKind.EndOfFileToken)
            {
                return true;
            }

            for (int i = 1; i <= LastTerminatorState; i <<= 1)
            {
                switch (_termState & (TerminatorState)i)
                {
                    case TerminatorState.IsNamespaceMemberStartOrStop when this.IsNamespaceMemberStartOrStop():
                    case TerminatorState.IsAttributeDeclarationTerminator when this.IsAttributeDeclarationTerminator():
                    case TerminatorState.IsPossibleAggregateClauseStartOrStop
                        when this.IsPossibleAggregateClauseStartOrStop():
                    case TerminatorState.IsPossibleMemberStartOrStop when this.IsPossibleMemberStartOrStop():
                    case TerminatorState.IsEndOfReturnType when this.IsEndOfReturnType():
                    case TerminatorState.IsEndOfParameterList when this.IsEndOfParameterList():
                    case TerminatorState.IsEndOfFieldDeclaration when this.IsEndOfFieldDeclaration():
                    case TerminatorState.IsPossibleEndOfVariableDeclaration
                        when this.IsPossibleEndOfVariableDeclaration():
                    case TerminatorState.IsEndOfTypeArgumentList when this.IsEndOfTypeArgumentList():
                    case TerminatorState.IsPossibleStatementStartOrStop when this.IsPossibleStatementStartOrStop():
                    case TerminatorState.IsEndOfFixedStatement when this.IsEndOfFixedStatement():
                    case TerminatorState.IsEndOfTryBlock when this.IsEndOfTryBlock():
                    case TerminatorState.IsEndOfCatchClause when this.IsEndOfCatchClause():
                    case TerminatorState.IsEndOfFilterClause when this.IsEndOfFilterClause():
                    case TerminatorState.IsEndOfCatchBlock when this.IsEndOfCatchBlock():
                    case TerminatorState.IsEndOfForStatementArgument when this.IsEndOfForStatementArgument():
                    case TerminatorState.IsEndOfDeclarationClause when this.IsEndOfDeclarationClause():
                    case TerminatorState.IsEndOfArgumentList when this.IsEndOfArgumentList():
                    case TerminatorState.IsSwitchSectionStart when this.IsPossibleSwitchSection():
                    case TerminatorState.IsEndOfTypeParameterList when this.IsEndOfTypeParameterList():
                    case TerminatorState.IsEndOfMethodSignature when this.IsEndOfMethodSignature():
                    case TerminatorState.IsEndOfNameInExplicitInterface when this.IsEndOfNameInExplicitInterface():
                    case TerminatorState.IsEndOfFunctionPointerParameterList
                        when this.IsEndOfFunctionPointerParameterList(errored: false):
                    case TerminatorState.IsEndOfFunctionPointerParameterListErrored
                        when this.IsEndOfFunctionPointerParameterList(errored: true):
                    case TerminatorState.IsEndOfFunctionPointerCallingConvention
                        when this.IsEndOfFunctionPointerCallingConvention():
                    case TerminatorState.IsEndOfRecordSignature when this.IsEndOfRecordSignature():
                        return true;
                }
            }

            return false;
        }

        private static Aquila.CodeAnalysis.AquilaSyntaxNode GetOldParent(Aquila.CodeAnalysis.AquilaSyntaxNode node)
        {
            return node != null ? node.Parent : null;
        }

        private struct FileBody
        {
            public ModuleDecl ModuleDecl = null;
            
            public SyntaxListBuilder<ImportDecl> Imports;
            public SyntaxListBuilder<MemberDecl> Members;
            
            public FileBody(SyntaxListPool pool)
            {
                Imports = pool.Allocate<ImportDecl>();
                Members = pool.Allocate<MemberDecl>();
            }

            internal void Free(SyntaxListPool pool)
            {
                pool.Free(Imports);
                pool.Free(Members);
            }
        }

        internal CompilationUnitSyntax ParseCompilationUnit()
        {
            return ParseWithStackGuard(
                ParseCompilationUnitCore,
                () => SyntaxFactory.CompilationUnit(
                    null,
                    new SyntaxList<ImportDecl>(),
                    new SyntaxList<MemberDecl>(),
                    SyntaxFactory.Token(SyntaxKind.EndOfFileToken)));
        }

        internal CompilationUnitSyntax ParseCompilationUnitCore()
        {
            SyntaxToken tmp = null;
            SyntaxListBuilder initialBadNodes = null;
            var body = new FileBody(_pool);
            try
            {
                this.ParseFileBody(ref tmp, ref body, ref initialBadNodes, SyntaxKind.CompilationUnit);

                var eof = this.EatToken(SyntaxKind.EndOfFileToken);
                var result =
                    _syntaxFactory.CompilationUnit(body.ModuleDecl, body.Imports, body.Members, eof);

                if (initialBadNodes != null)
                {
                    // attach initial bad nodes as leading trivia on first token
                    result = AddLeadingSkippedSyntax(result, initialBadNodes.ToListNode());
                    _pool.Free(initialBadNodes);
                }

                return result;
            }
            finally
            {
                body.Free(_pool);
            }
        }

        internal TNode ParseWithStackGuard<TNode>(Func<TNode> parseFunc, Func<TNode> createEmptyNodeFunc)
            where TNode : AquilaSyntaxNode
        {
            // If this value is non-zero then we are nesting calls to ParseWithStackGuard which should not be 
            // happening.  It's not a bug but it's inefficient and should be changed.
            Debug.Assert(_recursionDepth == 0);

            try
            {
                return parseFunc();
            }
            catch (InsufficientExecutionStackException)
            {
                return CreateForGlobalFailure(lexer.TextWindow.Position, createEmptyNodeFunc());
            }
        }

        private TNode CreateForGlobalFailure<TNode>(int position, TNode node) where TNode : AquilaSyntaxNode
        {
            // Turn the complete input into a single skipped token. This avoids running the lexer, and therefore
            // the preprocessor directive parser, which may itself run into the same problem that caused the
            // original failure.
            var builder = new SyntaxListBuilder(1);
            builder.Add(SyntaxFactory.BadToken(null, lexer.TextWindow.Text.ToString(), null));
            var fileAsTrivia = _syntaxFactory.SkippedTokensTrivia(builder.ToList<SyntaxToken>());
            node = AddLeadingSkippedSyntax(node, fileAsTrivia);
            ForceEndOfFile(); // force the scanner to report that it is at the end of the input.
            return AddError(node, position, 0, ErrorCode.ERR_InsufficientStack);
        }

        private static bool IsPossibleStartOfTypeDeclaration(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PubKeyword:
                case SyntaxKind.SealedKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.OpenBracketToken:
                    return true;
                default:
                    return false;
            }
        }

        private void AddSkippedFileText(
            ref SyntaxToken openBraceOrSemicolon,
            ref FileBody body,
            ref SyntaxListBuilder initialBadNodes,
            AquilaSyntaxNode skippedSyntax)
        {
            if (body.Imports.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Imports, skippedSyntax);
            }
            else if (body.Members.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Members, skippedSyntax);
            }
            // else if (body.Components.Count > 0)
            // {
            //     AddTrailingSkippedSyntax(body.Components, skippedSyntax);
            // }
            // else if (body.Extends.Count > 0)
            // {
            //     AddTrailingSkippedSyntax(body.Extends, skippedSyntax);
            // }
            else if (openBraceOrSemicolon != null)
            {
                openBraceOrSemicolon = AddTrailingSkippedSyntax(openBraceOrSemicolon, skippedSyntax);
            }
            else
            {
                if (initialBadNodes == null)
                {
                    initialBadNodes = _pool.Allocate();
                }

                initialBadNodes.AddRange(skippedSyntax);
            }
        }

        // Parts of a namespace declaration in the order they can be defined.
        private enum FileParts
        {
            None = 0,
            Module = 1,
            Imports = 2,
            Types = 3,
            Methods = 4,
            Components = 5,
            Extends = 6,
            TopLevelStatementsAfterTypesAndNamespaces = 7,
        }

        private void ParseFileBody(ref SyntaxToken openBraceOrSemicolon, ref FileBody body,
            ref SyntaxListBuilder initialBadNodes, SyntaxKind parentKind)
        {
            // "top-level" expressions and statements should never occur inside an asynchronous context
            Debug.Assert(!IsInAsync);

            bool isGlobal = openBraceOrSemicolon == null;

            var saveTerm = _termState;
            _termState |= TerminatorState.IsNamespaceMemberStartOrStop;
            FileParts seen = FileParts.None;
            var pendingIncompleteMembers = _pool.Allocate<MemberDecl>();
            bool reportUnexpectedToken = true;

            try
            {
                while (true)
                {
                    switch (this.CurrentToken.Kind)
                    {
                        case SyntaxKind.CloseBraceToken:

                            // incomplete members must be processed before we add any nodes to the body:
                            ReduceIncompleteMembers(ref pendingIncompleteMembers, ref openBraceOrSemicolon,
                                ref body, ref initialBadNodes);

                            var token = this.EatToken();
                            token = this.AddError(token,
                                IsScript
                                    ? ErrorCode.ERR_GlobalDefinitionOrStatementExpected
                                    : ErrorCode.ERR_EOFExpected);

                            this.AddSkippedFileText(ref openBraceOrSemicolon, ref body, ref initialBadNodes,
                                token);
                            reportUnexpectedToken = true;
                            break;

                        case SyntaxKind.ModuleKeyword:
                            body.ModuleDecl = ParseModule();
                            seen = FileParts.Module;
                            break;

                        case SyntaxKind.ImportKeyword:
                            var import = ParseImport();
                            body.Imports.Add(import);
                            seen = FileParts.Imports;
                            break;

                        case SyntaxKind.EndOfFileToken:
                            // This token marks the end of a namespace body
                            return;

                        default:
                        {
                            var decl = ParseMemberDeclaration(SyntaxKind.CompilationUnit);
                            if (decl != null)
                            {
                                body.Members.Add(decl);
                            }
                            else
                            {
                                //no progress
                                EatToken();
                            }

                            break;
                        }
                    }
                }
            }
            finally
            {
                _termState = saveTerm;

                // adds pending incomplete nodes:
                AddIncompleteMembers(ref pendingIncompleteMembers, ref body);
                _pool.Free(pendingIncompleteMembers);
            }

            MemberDecl adjustStateAndReportStatementOutOfOrder(ref FileParts seen,
                MemberDecl memberOrStatement)
            {
                switch (memberOrStatement.Kind)
                {
                    case SyntaxKind.GlobalStatement:
                        if (seen < FileParts.Components)
                        {
                            seen = FileParts.Components;
                        }
                        else if (seen == FileParts.Extends)
                        {
                            seen = FileParts.TopLevelStatementsAfterTypesAndNamespaces;

                            if (!IsScript)
                            {
                                memberOrStatement = this.AddError(memberOrStatement,
                                    ErrorCode.ERR_TopLevelStatementAfterNamespaceOrType);
                            }
                        }

                        break;

                    case SyntaxKind.NamespaceDeclaration:
                    case SyntaxKind.FileScopedNamespaceDeclaration:
                    case SyntaxKind.EnumDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.DelegateDeclaration:
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.RecordStructDeclaration:
                        if (seen < FileParts.Extends)
                        {
                            seen = FileParts.Extends;
                        }

                        break;

                    default:
                        if (seen < FileParts.Components)
                        {
                            seen = FileParts.Components;
                        }

                        break;
                }

                return memberOrStatement;
            }
        }

        private static void AddIncompleteMembers(ref SyntaxListBuilder<MemberDecl> incompleteMembers,
            ref FileBody body)
        {
            if (incompleteMembers.Count > 0)
            {
                body.Members.AddRange(incompleteMembers);
                incompleteMembers.Clear();
            }
        }

        private void ReduceIncompleteMembers(
            ref SyntaxListBuilder<MemberDecl> incompleteMembers,
            ref SyntaxToken openBraceOrSemicolon,
            ref FileBody body,
            ref SyntaxListBuilder initialBadNodes)
        {
            for (int i = 0; i < incompleteMembers.Count; i++)
            {
                this.AddSkippedFileText(ref openBraceOrSemicolon, ref body, ref initialBadNodes,
                    incompleteMembers[i]);
            }

            incompleteMembers.Clear();
        }

        private bool IsPossibleNamespaceMemberDeclaration()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.NamespaceKeyword:
                    return true;
                case SyntaxKind.IdentifierToken:
                    return IsPartialInNamespaceMemberDeclaration();
                default:
                    return IsPossibleStartOfTypeDeclaration(this.CurrentToken.Kind);
            }
        }

        private bool IsPartialInNamespaceMemberDeclaration()
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword)
            {
                if (this.IsPartialType())
                {
                    return true;
                }
                else if (this.PeekToken(1).Kind == SyntaxKind.NamespaceKeyword)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsEndOfNamespace()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseBraceToken;
        }

        private bool IsNamespaceMemberStartOrStop()
        {
            return this.IsEndOfNamespace()
                   || this.IsPossibleNamespaceMemberDeclaration();
        }

        private NameEqualsSyntax ParseNameEquals()
        {
            Debug.Assert(this.IsNamedAssignment());
            return _syntaxFactory.NameEquals(
                _syntaxFactory.IdentifierEx(this.ParseIdentifierToken()),
                this.EatToken(SyntaxKind.EqualsToken));
        }


        private bool IsPossibleAttributeDeclaration()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBracketToken;
        }

        private SyntaxList<AttributeListSyntax> ParseAttributeDeclarations()
        {
            var attributes = _pool.Allocate<AttributeListSyntax>();
            try
            {
                var saveTerm = _termState;
                _termState |= TerminatorState.IsAttributeDeclarationTerminator;

                while (this.IsPossibleAttributeDeclaration())
                {
                    var attribute = this.ParseAttributeDeclaration();
                    attributes.Add(attribute);
                }

                _termState = saveTerm;

                return attributes.ToList();
            }
            finally
            {
                _pool.Free(attributes);
            }
        }


        private bool IsAttributeDeclarationTerminator()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseBracketToken
                   || this.IsPossibleAttributeDeclaration(); // start of a new one...
        }

        private AttributeListSyntax ParseAttributeDeclaration()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.AttributeList)
            {
                return (AttributeListSyntax)this.EatNode();
            }

            var openBracket = this.EatToken(SyntaxKind.OpenBracketToken);

            // // Check for optional location :
            // AttributeTargetSpecifierSyntax attrLocation = null;
            if (IsSomeWord(this.CurrentToken.Kind) && this.PeekToken(1).Kind == SyntaxKind.ColonToken)
            {
                // var id = ConvertToKeyword(this.EatToken());
                // var colon = this.EatToken(SyntaxKind.ColonToken);
                // attrLocation = _syntaxFactory.AttributeTargetSpecifier(id, colon);
            }

            var attributes = _pool.AllocateSeparated<AttributeSyntax>();
            try
            {
                // if (attrLocation != null && attrLocation.Identifier.ToAttributeLocation() == AttributeLocation.Module)
                // {
                //     attrLocation = CheckFeatureAvailability(attrLocation, MessageID.IDS_FeatureModuleAttrLoc);
                // }

                this.ParseAttributes(attributes);
                var closeBracket = this.EatToken(SyntaxKind.CloseBracketToken);
                var declaration = _syntaxFactory.AttributeList(openBracket, attributes, closeBracket);

                return declaration;
            }
            finally
            {
                _pool.Free(attributes);
            }
        }

        private void ParseAttributes(SeparatedSyntaxListBuilder<AttributeSyntax> nodes)
        {
            // always expect at least one attribute
            nodes.Add(this.ParseAttribute());

            // remaining attributes
            while (this.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
            {
                if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // comma is optional, but if it is present it may be followed by another attribute
                    nodes.AddSeparator(this.EatToken());

                    // check for legal trailing comma
                    if (this.CurrentToken.Kind == SyntaxKind.CloseBracketToken)
                    {
                        break;
                    }

                    nodes.Add(this.ParseAttribute());
                }
                else if (this.IsPossibleAttribute())
                {
                    // report missing comma
                    nodes.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                    nodes.Add(this.ParseAttribute());
                }
                else if (this.SkipBadAttributeListTokens(nodes, SyntaxKind.IdentifierToken) == PostSkipAction.Abort)
                {
                    break;
                }
            }
        }

        private PostSkipAction SkipBadAttributeListTokens(SeparatedSyntaxListBuilder<AttributeSyntax> list,
            SyntaxKind expected)
        {
            Debug.Assert(list.Count > 0);
            SyntaxToken tmp = null;
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref tmp, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleAttribute(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseBracketToken || p.IsTerminator(),
                expected);
        }

        private bool IsPossibleAttribute()
        {
            return this.IsTrueIdentifier();
        }

        private AttributeSyntax ParseAttribute()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.Attribute)
            {
                return (AttributeSyntax)this.EatNode();
            }

            var name = this.ParseQualifiedName();

            //var argList = this.ParseAttributeArgumentList();
            return _syntaxFactory.Attribute(name, null);
        }


        private PostSkipAction SkipBadAttributeArgumentTokens(ref SyntaxToken openParen,
            SeparatedSyntaxListBuilder<AttributeArgumentSyntax> list, SyntaxKind expected)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref openParen, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleAttributeArgument(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseParenToken || p.IsTerminator(),
                expected);
        }

        private bool IsPossibleAttributeArgument()
        {
            return this.IsPossibleExpression();
        }


        private static DeclarationModifiers GetModifier(SyntaxToken token)
            => GetModifier(token.Kind, token.ContextualKind);

        internal static DeclarationModifiers GetModifier(SyntaxKind kind, SyntaxKind contextualKind)
        {
            switch (kind)
            {
                case SyntaxKind.PubKeyword:
                    return DeclarationModifiers.Public;
                case SyntaxKind.InternalKeyword:
                    return DeclarationModifiers.Internal;
                case SyntaxKind.ProtectedKeyword:
                    return DeclarationModifiers.Protected;
                case SyntaxKind.PrivateKeyword:
                    return DeclarationModifiers.Private;
                case SyntaxKind.SealedKeyword:
                    return DeclarationModifiers.Sealed;
                case SyntaxKind.AbstractKeyword:
                    return DeclarationModifiers.Abstract;
                case SyntaxKind.StaticKeyword:
                    return DeclarationModifiers.Static;
                case SyntaxKind.VirtualKeyword:
                    return DeclarationModifiers.Virtual;
                case SyntaxKind.ExternKeyword:
                    return DeclarationModifiers.Extern;
                case SyntaxKind.NewKeyword:
                    return DeclarationModifiers.New;
                case SyntaxKind.OverrideKeyword:
                    return DeclarationModifiers.Override;
                case SyntaxKind.ReadOnlyKeyword:
                    return DeclarationModifiers.ReadOnly;
                case SyntaxKind.VolatileKeyword:
                    return DeclarationModifiers.Volatile;
                case SyntaxKind.UnsafeKeyword:
                    return DeclarationModifiers.Unsafe;
                case SyntaxKind.PartialKeyword:
                    return DeclarationModifiers.Partial;
                case SyntaxKind.AsyncKeyword:
                    return DeclarationModifiers.Async;
                case SyntaxKind.RefKeyword:
                    return DeclarationModifiers.Ref;
                case SyntaxKind.IdentifierToken:
                    switch (contextualKind)
                    {
                        case SyntaxKind.PartialKeyword:
                            return DeclarationModifiers.Partial;
                        case SyntaxKind.AsyncKeyword:
                            return DeclarationModifiers.Async;
                    }

                    goto default;
                default:
                    return DeclarationModifiers.None;
            }
        }

        private void ParseModifiers(SyntaxListBuilder tokens, bool forAccessors)
        {
            while (true)
            {
                var newMod = GetModifier(this.CurrentToken);
                if (newMod == DeclarationModifiers.None)
                {
                    break;
                }

                SyntaxToken modTok;
                switch (newMod)
                {
                    case DeclarationModifiers.Partial:
                        var nextToken = PeekToken(1);
                        var isPartialType = this.IsPartialType();
                        var isPartialMember = this.IsPartialMember();
                        if (isPartialType || isPartialMember)
                        {
                            // Standard legal cases.
                            modTok = ConvertToKeyword(this.EatToken());
                            modTok = CheckFeatureAvailability(modTok,
                                isPartialType ? MessageID.IDS_FeaturePartialTypes : MessageID.IDS_FeaturePartialMethod);
                        }
                        else if (nextToken.Kind == SyntaxKind.NamespaceKeyword)
                        {
                            // Error reported in binding
                            modTok = ConvertToKeyword(this.EatToken());
                        }
                        else if (
                            nextToken.Kind == SyntaxKind.EnumKeyword ||
                            nextToken.Kind == SyntaxKind.DelegateKeyword ||
                            (IsPossibleStartOfTypeDeclaration(nextToken.Kind) &&
                             GetModifier(nextToken) != DeclarationModifiers.None))
                        {
                            // Misplaced partial
                            // TODO(https://github.com/dotnet/roslyn/issues/22439):
                            // We should consider moving this check into binding, but avoid holding on to trees
                            modTok = AddError(ConvertToKeyword(this.EatToken()), ErrorCode.ERR_PartialMisplaced);
                        }
                        else
                        {
                            return;
                        }

                        break;

                    case DeclarationModifiers.Ref:
                        // 'ref' is only a modifier if used on a ref struct
                        // it must be either immediately before the 'struct'
                        // keyword, or immediately before 'partial struct' if
                        // this is a partial ref struct declaration
                    {
                        var next = PeekToken(1);
                        if (isStructOrRecordKeyword(next) ||
                            (next.ContextualKind == SyntaxKind.PartialKeyword &&
                             isStructOrRecordKeyword(PeekToken(2))))
                        {
                            modTok = this.EatToken();
                            modTok = CheckFeatureAvailability(modTok, MessageID.IDS_FeatureRefStructs);
                        }
                        else if (forAccessors && this.IsPossibleAccessorModifier())
                        {
                            // Accept ref as a modifier for properties and event accessors, to produce an error later during binding.
                            modTok = this.EatToken();
                        }
                        else
                        {
                            return;
                        }

                        break;
                    }

                    case DeclarationModifiers.Async:
                        if (!ShouldAsyncBeTreatedAsModifier(parsingStatementNotDeclaration: false))
                        {
                            return;
                        }

                        modTok = ConvertToKeyword(this.EatToken());
                        modTok = CheckFeatureAvailability(modTok, MessageID.IDS_FeatureAsync);
                        break;

                    default:
                        modTok = this.EatToken();
                        break;
                }

                tokens.Add(modTok);
            }

            bool isStructOrRecordKeyword(SyntaxToken token)
            {
                if (token.Kind == SyntaxKind.StructKeyword)
                {
                    return true;
                }

                if (token.ContextualKind == SyntaxKind.RecordKeyword)
                {
                    // This is an unusual use of LangVersion. Normally we only produce errors when the langversion
                    // does not support a feature, but in this case we are effectively making a language breaking
                    // change to consider "record" a type declaration in all ambiguous cases. To avoid breaking
                    // older code that is not using C# 9 we conditionally parse based on langversion
                    return IsFeatureEnabled(MessageID.IDS_FeatureRecords);
                }

                return false;
            }
        }

        private bool ShouldAsyncBeTreatedAsModifier(bool parsingStatementNotDeclaration)
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.AsyncKeyword);

            // Adapted from CParser::IsAsyncMethod.

            if (IsNonContextualModifier(PeekToken(1)))
            {
                // If the next token is a (non-contextual) modifier keyword, then this token is
                // definitely the async keyword
                return true;
            }

            // Some of our helpers start at the current token, so we'll have to advance for their
            // sake and then backtrack when we're done.  Don't leave this block without releasing
            // the reset point.
            ResetPoint resetPoint = GetResetPoint();

            try
            {
                this.EatToken(); //move past contextual 'async'

                if (!parsingStatementNotDeclaration &&
                    (this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword))
                {
                    this.EatToken(); // "partial" doesn't affect our decision, so look past it.
                }

                // Comment directly from CParser::IsAsyncMethod.
                // ... 'async' [partial] <typedecl> ...
                // ... 'async' [partial] <event> ...
                // ... 'async' [partial] <implicit> <operator> ...
                // ... 'async' [partial] <explicit> <operator> ...
                // ... 'async' [partial] <typename> <operator> ...
                // ... 'async' [partial] <typename> <membername> ...
                // DEVNOTE: Although we parse async user defined conversions, operators, etc. here,
                // anything other than async methods are detected as erroneous later, during the define phase

                if (!parsingStatementNotDeclaration)
                {
                    var ctk = this.CurrentToken.Kind;
                    if (IsPossibleStartOfTypeDeclaration(ctk) ||
                        ctk == SyntaxKind.EventKeyword ||
                        ((ctk == SyntaxKind.ExplicitKeyword || ctk == SyntaxKind.ImplicitKeyword) &&
                         PeekToken(1).Kind == SyntaxKind.OperatorKeyword))
                    {
                        return true;
                    }
                }

                if (ScanType() != ScanTypeFlags.NotType)
                {
                    // We've seen "async TypeName".  Now we have to determine if we should we treat 
                    // 'async' as a modifier.  Or is the user actually writing something like 
                    // "public async Goo" where 'async' is actually the return type.

                    if (IsPossibleMemberName())
                    {
                        // we have: "async Type X" or "async Type this", 'async' is definitely a 
                        // modifier here.
                        return true;
                    }

                    var currentTokenKind = this.CurrentToken.Kind;

                    // The file ends with "async TypeName", it's not legal code, and it's much 
                    // more likely that this is meant to be a modifier.
                    if (currentTokenKind == SyntaxKind.EndOfFileToken)
                    {
                        return true;
                    }

                    // "async TypeName }".  In this case, we just have an incomplete member, and 
                    // we should definitely default to 'async' being considered a return type here.
                    if (currentTokenKind == SyntaxKind.CloseBraceToken)
                    {
                        return true;
                    }

                    // "async TypeName void". In this case, we just have an incomplete member before
                    // an existing member.  Treat this 'async' as a keyword.
                    if (SyntaxFacts.IsPredefinedType(this.CurrentToken.Kind))
                    {
                        return true;
                    }

                    // "async TypeName public".  In this case, we just have an incomplete member before
                    // an existing member.  Treat this 'async' as a keyword.
                    if (IsNonContextualModifier(this.CurrentToken))
                    {
                        return true;
                    }

                    // "async TypeName class". In this case, we just have an incomplete member before
                    // an existing type declaration.  Treat this 'async' as a keyword.
                    if (IsTypeDeclarationStart())
                    {
                        return true;
                    }

                    // "async TypeName namespace". In this case, we just have an incomplete member before
                    // an existing namespace declaration.  Treat this 'async' as a keyword.
                    if (currentTokenKind == SyntaxKind.NamespaceKeyword)
                    {
                        return true;
                    }

                    if (!parsingStatementNotDeclaration && currentTokenKind == SyntaxKind.OperatorKeyword)
                    {
                        return true;
                    }
                }
            }
            finally
            {
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }

            return false;
        }

        private static bool IsNonContextualModifier(SyntaxToken nextToken)
        {
            return GetModifier(nextToken) != DeclarationModifiers.None &&
                   !SyntaxFacts.IsContextualKeyword(nextToken.ContextualKind);
        }

        private bool IsPartialType()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword);
            var nextToken = this.PeekToken(1);
            switch (nextToken.Kind)
            {
                case SyntaxKind.StructKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.TypeKeyword:
                    return true;
            }

            if (nextToken.ContextualKind == SyntaxKind.RecordKeyword)
            {
                // This is an unusual use of LangVersion. Normally we only produce errors when the langversion
                // does not support a feature, but in this case we are effectively making a language breaking
                // change to consider "record" a type declaration in all ambiguous cases. To avoid breaking
                // older code that is not using C# 9 we conditionally parse based on langversion
                return IsFeatureEnabled(MessageID.IDS_FeatureRecords);
            }

            return false;
        }

        private bool IsPartialMember()
        {
            // note(cyrusn): this could have been written like so:
            //
            //  return
            //    this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword &&
            //    this.PeekToken(1).Kind == SyntaxKind.VoidKeyword;
            //
            // However, we want to be lenient and allow the user to write 
            // 'partial' in most modifier lists.  We will then provide them with
            // a more specific message later in binding that they are doing 
            // something wrong.
            //
            // Some might argue that the simple check would suffice.
            // However, we'd like to maintain behavior with 
            // previously shipped versions, and so we're keeping this code.

            // Here we check for:
            //   partial ReturnType MemberName
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword);
            var point = this.GetResetPoint();
            try
            {
                this.EatToken(); // partial

                if (this.ScanType() == ScanTypeFlags.NotType)
                {
                    return false;
                }

                return IsPossibleMemberName();
            }
            finally
            {
                this.Reset(ref point);
                this.Release(ref point);
            }
        }

        private bool IsPossibleMemberName()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.IdentifierToken:
                    if (this.CurrentToken.ContextualKind == SyntaxKind.GlobalKeyword &&
                        this.PeekToken(1).Kind == SyntaxKind.UsingKeyword)
                    {
                        return false;
                    }

                    return true;
                case SyntaxKind.ThisKeyword:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// checks for modifiers whose feature is not available
        /// </summary>
        private void CheckForVersionSpecificModifiers(SyntaxListBuilder modifiers, SyntaxKind kind, MessageID feature)
        {
            for (int i = 0, n = modifiers.Count; i < n; i++)
            {
                if (modifiers[i].RawKind == (int)kind)
                {
                    modifiers[i] = CheckFeatureAvailability(modifiers[i], feature);
                }
            }
        }

        private void SkipBadMemberListTokens(ref SyntaxToken openBrace, SyntaxListBuilder members)
        {
            if (members.Count > 0)
            {
                var tmp = members[members.Count - 1];
                this.SkipBadMemberListTokens(ref tmp);
                members[members.Count - 1] = tmp;
            }
            else
            {
                GreenNode tmp = openBrace;
                this.SkipBadMemberListTokens(ref tmp);
                openBrace = (SyntaxToken)tmp;
            }
        }

        private void SkipBadMemberListTokens(ref GreenNode previousNode)
        {
            int curlyCount = 0;
            var tokens = _pool.Allocate();
            try
            {
                bool done = false;

                // always consume at least one token.
                var token = this.EatToken();
                token = this.AddError(token, ErrorCode.ERR_InvalidMemberDecl, token.Text);
                tokens.Add(token);

                while (!done)
                {
                    SyntaxKind kind = this.CurrentToken.Kind;

                    // If this token can start a member, we're done
                    if (CanStartMember(kind) &&
                        !(kind == SyntaxKind.DelegateKeyword && (this.PeekToken(1).Kind == SyntaxKind.OpenBraceToken ||
                                                                 this.PeekToken(1).Kind == SyntaxKind.OpenParenToken)))
                    {
                        done = true;
                        continue;
                    }

                    // <UNDONE>  UNDONE: Seems like this makes sense, 
                    // but if this token can start a namespace element, but not a member, then
                    // perhaps we should bail back up to parsing a namespace body somehow...</UNDONE>

                    // Watch curlies and look for end of file/close curly
                    switch (kind)
                    {
                        case SyntaxKind.OpenBraceToken:
                            curlyCount++;
                            break;

                        case SyntaxKind.CloseBraceToken:
                            if (curlyCount-- == 0)
                            {
                                done = true;
                                continue;
                            }

                            break;

                        case SyntaxKind.EndOfFileToken:
                            done = true;
                            continue;

                        default:
                            break;
                    }

                    tokens.Add(this.EatToken());
                }

                previousNode = AddTrailingSkippedSyntax((AquilaSyntaxNode)previousNode, tokens.ToListNode());
            }
            finally
            {
                _pool.Free(tokens);
            }
        }

        private bool IsPossibleMemberStartOrStop()
        {
            return this.IsPossibleMemberStart() || this.CurrentToken.Kind == SyntaxKind.CloseBraceToken;
        }

        private bool IsPossibleAggregateClauseStartOrStop()
        {
            return this.CurrentToken.Kind == SyntaxKind.ColonToken
                   || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                   || this.IsCurrentTokenWhereOfConstraintClause();
        }

        private bool IsCurrentTokenWhereOfConstraintClause()
        {
            return
                this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword &&
                this.PeekToken(1).Kind == SyntaxKind.IdentifierToken &&
                this.PeekToken(2).Kind == SyntaxKind.ColonToken;
        }


        private bool IsPossibleTypeParameterConstraint()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.NewKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.DefaultKeyword:
                    return true;
                case SyntaxKind.IdentifierToken:
                    return this.IsTrueIdentifier();
                default:
                    return IsPredefinedType(this.CurrentToken.Kind);
            }
        }

        private bool IsPossibleMemberStart()
        {
            return CanStartMember(this.CurrentToken.Kind);
        }

        private static bool CanStartMember(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.BoolKeyword:
                case SyntaxKind.ByteKeyword:
                case SyntaxKind.CharKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.DecimalKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.DoubleKeyword:
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.EventKeyword:
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.FixedKeyword:
                case SyntaxKind.FloatKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.DatetimeKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.LongKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.ObjectKeyword:
                case SyntaxKind.OverrideKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PubKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.SByteKeyword:
                case SyntaxKind.SealedKeyword:
                case SyntaxKind.ShortKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.StringKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.UIntKeyword:
                case SyntaxKind.ULongKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.UShortKeyword:
                case SyntaxKind.VirtualKeyword:
                case SyntaxKind.VoidKeyword:
                case SyntaxKind.VolatileKeyword:
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.TildeToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.ImplicitKeyword:
                case SyntaxKind.ExplicitKeyword:
                case SyntaxKind.OpenParenToken: //tuple
                case SyntaxKind.RefKeyword:
                    return true;

                default:
                    return false;
            }
        }

        private bool IsTypeDeclarationStart()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.DelegateKeyword when !IsFunctionPointerStart():
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.TypeKeyword:
                    return true;

                case SyntaxKind.IdentifierToken:
                    if (CurrentToken.ContextualKind == SyntaxKind.RecordKeyword)
                    {
                        // This is an unusual use of LangVersion. Normally we only produce errors when the langversion
                        // does not support a feature, but in this case we are effectively making a language breaking
                        // change to consider "record" a type declaration in all ambiguous cases. To avoid breaking
                        // older code that is not using C# 9 we conditionally parse based on langversion
                        return IsFeatureEnabled(MessageID.IDS_FeatureRecords);
                    }

                    return false;

                default:
                    return false;
            }
        }

        private bool CanReuseMemberDeclaration(SyntaxKind kind, bool isGlobal)
        {
            switch (kind)
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.FileScopedNamespaceDeclaration:
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    return true;
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.MethodDeclaration:
                    if (!isGlobal || IsScript)
                    {
                        return true;
                    }

                    // We can reuse original nodes if they came from the global context as well.
                    return (this.CurrentNode.Parent is Syntax.CompilationUnitSyntax);

                case SyntaxKind.GlobalStatement:
                    return isGlobal;

                default:
                    return false;
            }
        }


        private bool IsMisplacedModifier(SyntaxListBuilder modifiers, SyntaxList<AttributeListSyntax> attributes,
            TypeEx type, out MemberDecl result)
        {
            if (GetModifier(this.CurrentToken) != DeclarationModifiers.None &&
                this.CurrentToken.ContextualKind != SyntaxKind.PartialKeyword &&
                this.CurrentToken.ContextualKind != SyntaxKind.AsyncKeyword &&
                IsComplete(type))
            {
                var misplacedModifier = this.CurrentToken;
                type = this.AddError(
                    type,
                    type.FullWidth + misplacedModifier.GetLeadingTriviaWidth(),
                    misplacedModifier.Width,
                    ErrorCode.ERR_BadModifierLocation,
                    misplacedModifier.Text);

                result = _syntaxFactory.IncompleteMember(attributes, modifiers.ToList(), type);
                return true;
            }

            result = null;
            return false;
        }


        private bool IsNoneOrIncompleteMember(SyntaxKind parentKind, SyntaxList<AttributeListSyntax> attributes,
            SyntaxListBuilder modifiers, TypeEx? type,
            SyntaxToken identifierOrThisOpt, TypeParameterListSyntax typeParameterListOpt,
            out MemberDecl result)
        {
            var typeIsMissing = type?.IsMissing ?? true;

            if (identifierOrThisOpt == null && typeParameterListOpt == null)
            {
                if (attributes.Count == 0 && modifiers.Count == 0 && typeIsMissing && type.Kind != SyntaxKind.RefType)
                {
                    // we haven't advanced, the caller needs to consume the tokens ahead
                    result = null;
                    return true;
                }

                var incompleteMember =
                    _syntaxFactory.IncompleteMember(attributes, modifiers.ToList(), typeIsMissing ? null : type);
                if (incompleteMember.ContainsDiagnostics)
                {
                    result = incompleteMember;
                }
                else if (parentKind is SyntaxKind.NamespaceDeclaration or SyntaxKind.FileScopedNamespaceDeclaration ||
                         parentKind == SyntaxKind.CompilationUnit && !IsScript)
                {
                    result = this.AddErrorToLastToken(incompleteMember, ErrorCode.ERR_NamespaceUnexpected);
                }
                else
                {
                    //the error position should indicate CurrentToken
                    result = this.AddError(
                        incompleteMember,
                        incompleteMember.FullWidth + this.CurrentToken.GetLeadingTriviaWidth(),
                        this.CurrentToken.Width,
                        ErrorCode.ERR_InvalidMemberDecl,
                        this.CurrentToken.Text);
                }

                return true;
            }

            result = null;
            return false;
        }


        // Returns null if we can't parse anything (even partially).
        internal MemberDecl ParseMemberDeclaration(SyntaxKind parentKind)
        {
            _recursionDepth++;
            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
            var result = ParseMemberDeclarationCore(parentKind);
            _recursionDepth--;
            return result;
        }

        /// <summary>
        /// Changes in this function should be mirrored in <see cref="ParseMemberDeclarationOrStatementCore"/>.
        /// Try keeping structure of both functions similar to simplify this task. The split was made to 
        /// reduce the stack usage during recursive parsing.
        /// </summary>
        /// <returns>Returns null if we can't parse anything (even partially).</returns>
        private MemberDecl ParseMemberDeclarationCore(SyntaxKind parentKind)
        {
            // "top-level" expressions and statements should never occur inside an asynchronous context
            Debug.Assert(!IsInAsync);
            // Debug.Assert(parentKind != SyntaxKind.CompilationUnit);

            cancellationToken.ThrowIfCancellationRequested();

            // don't reuse members if they were previously declared under a different type keyword kind
            if (this.IsIncrementalAndFactoryContextMatches)
            {
                if (CanReuseMemberDeclaration(CurrentNodeKind, isGlobal: false))
                {
                    return (MemberDecl)this.EatNode();
                }
            }

            var modifiers = _pool.Allocate();

            var saveTermState = _termState;

            try
            {
                var attributes = this.ParseAttributeDeclarations();

                this.ParseModifiers(modifiers, forAccessors: false);

                if (IsFunc())
                {
                    var funcDecl = ParseFuncDecl(parentKind, attributes, modifiers);
                    if (parentKind != SyntaxKind.CompilationUnit)
                        this.AddError(funcDecl, ErrorCode.ERR_BadDirectivePlacement);
                    return funcDecl;
                }

                if (IsTypeDeclarationStart())
                {
                    var typeDecl = ParseTypeDecl(attributes, modifiers);
                    if (parentKind != SyntaxKind.CompilationUnit)
                        this.AddError(typeDecl, ErrorCode.ERR_BadDirectivePlacement);
                    return typeDecl;
                }

                // Everything that's left -- methods, fields, properties, 
                // indexers, and non-conversion operators -- starts with a type 
                // (possibly void).
                TypeEx type = ParseReturnType();

                var afterTypeResetPoint = this.GetResetPoint();
                MemberDecl result;
                try
                {
                    // Check for misplaced modifiers.  if we see any, then consider this member
                    // terminated and restart parsing.
                    if (IsMisplacedModifier(modifiers, attributes, type, out result))
                    {
                        return result;
                    }


                    parse_member_name: ;

                    // At this point we can either have indexers, methods, or 
                    // properties (or something unknown).  Try to break apart
                    // the following name and determine what to do from there.
                    SyntaxToken identifierOrThisOpt;
                    TypeParameterListSyntax typeParameterListOpt;
                    this.ParseMemberName(out identifierOrThisOpt, out typeParameterListOpt, isEvent: false);

                    // First, check if we got absolutely nothing.  If so, then 
                    // We need to consume a bad member and try again.
                    if (IsNoneOrIncompleteMember(parentKind, attributes, modifiers, type,
                            identifierOrThisOpt, typeParameterListOpt, out result))
                    {
                        return result;
                    }

                    Debug.Assert(identifierOrThisOpt != null);

                    // check availability of readonly members feature for indexers, properties and methods
                    CheckForVersionSpecificModifiers(modifiers, SyntaxKind.ReadOnlyKeyword,
                        MessageID.IDS_FeatureReadOnlyMembers);


                    if (parentKind == SyntaxKind.TypeDecl)
                    {
                        return ParseFieldDecl(type, identifierOrThisOpt);
                    }

                    // treat anything else as a method.
                    return this.ParseMethodDeclaration(attributes, modifiers, type, identifierOrThisOpt,
                        typeParameterListOpt);
                }
                finally
                {
                    this.Release(ref afterTypeResetPoint);
                }
            }
            finally
            {
                _pool.Free(modifiers);
                _termState = saveTermState;
            }
        }

        private bool IsFieldDeclaration(bool isEvent)
        {
            if (this.CurrentToken.Kind != SyntaxKind.IdentifierToken)
            {
                return false;
            }

            if (this.CurrentToken.ContextualKind == SyntaxKind.GlobalKeyword &&
                this.PeekToken(1).Kind == SyntaxKind.UsingKeyword)
            {
                return false;
            }

            // Treat this as a field, unless we have anything following that
            // makes us:
            //   a) explicit
            //   b) generic
            //   c) a property
            //   d) a method (unless we already know we're parsing an event)
            var kind = this.PeekToken(1).Kind;
            switch (kind)
            {
                case SyntaxKind.DotToken: // Goo.     explicit
                case SyntaxKind.ColonColonToken: // Goo::    explicit
                case SyntaxKind.DotDotToken: // Goo..    explicit
                case SyntaxKind.LessThanToken: // Goo<     explicit or generic method
                case SyntaxKind.OpenBraceToken: // Goo {    property
                case SyntaxKind.EqualsGreaterThanToken: // Goo =>   property
                    return false;
                case SyntaxKind.OpenParenToken: // Goo(     method
                    return isEvent;
                default:
                    return true;
            }
        }

        private bool IsOperatorKeyword()
        {
            return
                this.CurrentToken.Kind == SyntaxKind.ImplicitKeyword ||
                this.CurrentToken.Kind == SyntaxKind.ExplicitKeyword ||
                this.CurrentToken.Kind == SyntaxKind.OperatorKeyword;
        }

        public static bool IsComplete(AquilaSyntaxNode node)
        {
            if (node == null)
            {
                return false;
            }

            foreach (var child in node.ChildNodesAndTokens().Reverse())
            {
                var token = child as SyntaxToken;
                if (token == null)
                {
                    return IsComplete((AquilaSyntaxNode)child);
                }

                if (token.IsMissing)
                {
                    return false;
                }

                if (token.Kind != SyntaxKind.None)
                {
                    return true;
                }

                // if token was optional, consider the next one..
            }

            return true;
        }


        /// <summary>
        /// Parses any block or expression bodies that are present. Also parses
        /// the trailing semicolon if one is present.
        /// </summary>
        private void ParseBlockAndExpressionBodiesWithSemicolon(
            out BlockStmt blockBody,
            out ArrowExClause expressionBody,
            out SyntaxToken semicolon,
            bool parseSemicolonAfterBlock = true,
            MessageID requestedExpressionBodyFeature = MessageID.IDS_FeatureExpressionBodiedMethod)
        {
            // Check for 'forward' declarations with no block of any kind
            if (this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                blockBody = null;
                expressionBody = null;
                semicolon = this.EatToken(SyntaxKind.SemicolonToken);
                return;
            }

            blockBody = null;
            expressionBody = null;

            if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                blockBody = this.ParseMethodOrAccessorBodyBlock(attributes: default, isAccessorBody: false);
            }

            if (this.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                throw new NotSupportedException("");

                // Debug.Assert(requestedExpressionBodyFeature == MessageID.IDS_FeatureExpressionBodiedMethod
                //              || requestedExpressionBodyFeature == MessageID.IDS_FeatureExpressionBodiedAccessor
                //              || requestedExpressionBodyFeature == MessageID.IDS_FeatureExpressionBodiedDeOrConstructor,
                //     "Only IDS_FeatureExpressionBodiedMethod, IDS_FeatureExpressionBodiedAccessor or IDS_FeatureExpressionBodiedDeOrConstructor can be requested");
                // expressionBody = this.ParseArrowExpressionClause();
                // expressionBody = CheckFeatureAvailability(expressionBody, requestedExpressionBodyFeature);
            }

            semicolon = null;
            // Expression-bodies need semicolons and native behavior
            // expects a semicolon if there is no body
            if (expressionBody != null || blockBody == null)
            {
                semicolon = this.EatToken(SyntaxKind.SemicolonToken);
            }
            // Check for bad semicolon after block body
            else if (parseSemicolonAfterBlock && this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                semicolon = this.EatTokenWithPrejudice(ErrorCode.ERR_UnexpectedSemicolon);
            }
        }

        private bool IsEndOfTypeParameterList()
        {
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                // void Goo<T (
                return true;
            }

            if (this.CurrentToken.Kind == SyntaxKind.ColonToken)
            {
                // class C<T :
                return true;
            }

            if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                // class C<T {
                return true;
            }

            if (IsCurrentTokenWhereOfConstraintClause())
            {
                // class C<T where T :
                return true;
            }

            return false;
        }

        private bool IsEndOfMethodSignature()
        {
            return this.CurrentToken.Kind == SyntaxKind.SemicolonToken ||
                   this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private bool IsEndOfRecordSignature()
        {
            return this.CurrentToken.Kind == SyntaxKind.SemicolonToken ||
                   this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private bool IsEndOfNameInExplicitInterface()
        {
            return this.CurrentToken.Kind == SyntaxKind.DotToken ||
                   this.CurrentToken.Kind == SyntaxKind.ColonColonToken;
        }

        private bool IsEndOfFunctionPointerParameterList(bool errored) => this.CurrentToken.Kind ==
                                                                          (errored
                                                                              ? SyntaxKind.CloseParenToken
                                                                              : SyntaxKind.GreaterThanToken);

        private bool IsEndOfFunctionPointerCallingConvention() =>
            this.CurrentToken.Kind == SyntaxKind.CloseBracketToken;


        private MethodDecl ParseMethodDeclaration(
            SyntaxList<AttributeListSyntax> attributes,
            SyntaxListBuilder modifiers,
            TypeEx type,
            SyntaxToken identifier,
            TypeParameterListSyntax typeParameterList)
        {
            // Parse the name (it could be qualified)
            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfMethodSignature;

            var paramList = this.ParseParenthesizedParameterList();

            try
            {
                _termState = saveTerm;

                BlockStmt blockBody;
                ArrowExClause expressionBody;
                SyntaxToken semicolon;

                // Method declarations cannot be nested or placed inside async lambdas, and so cannot occur in an
                // asynchronous context. Therefore the IsInAsync state of the parent scope is not saved and
                // restored, just assumed to be false and reset accordingly after parsing the method body.
                Debug.Assert(!IsInAsync);

                IsInAsync = modifiers.Any((int)SyntaxKind.AsyncKeyword);

                this.ParseBlockAndExpressionBodiesWithSemicolon(out blockBody, out expressionBody, out semicolon);

                IsInAsync = false;

                return _syntaxFactory.MethodDecl(
                    attributes,
                    modifiers.ToList(),
                    type,
                    identifier,
                    typeParameterList,
                    paramList,
                    blockBody,
                    expressionBody,
                    semicolon);
            }
            finally
            {
            }
        }

        private TypeEx ParseReturnType()
        {
            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfReturnType;
            var type = this.ParseTypeOrVoid();
            _termState = saveTerm;
            return type;
        }

        private bool IsEndOfReturnType()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.SemicolonToken:
                    return true;
                default:
                    return false;
            }
        }

        private ExprSyntax ParsePossibleRefExpression()
        {
            // var refKeyword = (SyntaxToken)null;
            // if (this.CurrentToken.Kind == SyntaxKind.RefKeyword &&
            //     // check for lambda expression with explicit ref return type: `ref int () => { ... }`
            //     !this.IsPossibleLambdaExpression(Precedence.Expression))
            // {
            //     refKeyword = this.EatToken();
            //     refKeyword = CheckFeatureAvailability(refKeyword, MessageID.IDS_FeatureRefLocalsReturns);
            // }

            var expression = this.ParseExpressionCore();
            // if (refKeyword != null)
            // {
            //     expression = _syntaxFactory.RefExpression(refKeyword, expression);
            // }

            return expression;
        }

        private bool IsPossibleAccessor()
        {
            return this.CurrentToken.Kind == SyntaxKind.IdentifierToken
                   || IsPossibleAttributeDeclaration()
                   || SyntaxFacts.GetAccessorDeclarationKind(this.CurrentToken.ContextualKind) != SyntaxKind.None
                   || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken // for accessor blocks w/ missing keyword
                   || this.CurrentToken.Kind == SyntaxKind.SemicolonToken // for empty body accessors w/ missing keyword
                   || IsPossibleAccessorModifier();
        }

        private bool IsPossibleAccessorModifier()
        {
            // We only want to accept a modifier as the start of an accessor if the modifiers are
            // actually followed by "get/set/add/remove".  Otherwise, we might thing think we're 
            // starting an accessor when we're actually starting a normal class member.  For example:
            //
            //      class C {
            //          public int Prop { get { this.
            //          private DateTime x;
            //
            // We don't want to think of the "private" in "private DateTime x" as starting an accessor
            // here.  If we do, we'll get totally thrown off in parsing the remainder and that will
            // throw off the rest of the features that depend on a good syntax tree.
            // 
            // Note: we allow all modifiers here.  That's because we want to parse things like
            // "abstract get" as an accessor.  This way we can provide a good error message
            // to the user that this is not allowed.

            if (GetModifier(this.CurrentToken) == DeclarationModifiers.None)
            {
                return false;
            }

            var peekIndex = 1;
            while (GetModifier(this.PeekToken(peekIndex)) != DeclarationModifiers.None)
            {
                peekIndex++;
            }

            var token = this.PeekToken(peekIndex);
            if (token.Kind == SyntaxKind.CloseBraceToken || token.Kind == SyntaxKind.EndOfFileToken)
            {
                // If we see "{ get { } public }
                // then we will think that "public" likely starts an accessor.
                return true;
            }

            switch (token.ContextualKind)
            {
                case SyntaxKind.GetKeyword:
                case SyntaxKind.SetKeyword:
                case SyntaxKind.InitKeyword:
                case SyntaxKind.AddKeyword:
                case SyntaxKind.RemoveKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private enum PostSkipAction
        {
            Continue,
            Abort
        }


        private PostSkipAction SkipBadSeparatedListTokensWithExpectedKind<T, TNode>(
            ref T startToken,
            SeparatedSyntaxListBuilder<TNode> list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            SyntaxKind expected)
            where T : AquilaSyntaxNode
            where TNode : AquilaSyntaxNode
        {
            // We're going to cheat here and pass the underlying SyntaxListBuilder of "list" to the helper method so that
            // it can append skipped trivia to the last element, regardless of whether that element is a node or a token.
            GreenNode trailingTrivia;
            var action = this.SkipBadListTokensWithExpectedKindHelper(list.UnderlyingBuilder, isNotExpectedFunction,
                abortFunction, expected, out trailingTrivia);
            if (trailingTrivia != null)
            {
                startToken = AddTrailingSkippedSyntax(startToken, trailingTrivia);
            }

            return action;
        }

        private PostSkipAction SkipBadListTokensWithErrorCode<T, TNode>(
            ref T startToken,
            SyntaxListBuilder<TNode> list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            ErrorCode error)
            where T : AquilaSyntaxNode
            where TNode : AquilaSyntaxNode
        {
            GreenNode trailingTrivia;
            var action = this.SkipBadListTokensWithErrorCodeHelper(list, isNotExpectedFunction, abortFunction, error,
                out trailingTrivia);
            if (trailingTrivia != null)
            {
                startToken = AddTrailingSkippedSyntax(startToken, trailingTrivia);
            }

            return action;
        }

        /// <remarks>
        /// WARNING: it is possible that "list" is really the underlying builder of a SeparateSyntaxListBuilder,
        /// so it is important that we not add anything to the list.
        /// </remarks>
        private PostSkipAction SkipBadListTokensWithExpectedKindHelper(
            SyntaxListBuilder list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            SyntaxKind expected,
            out GreenNode trailingTrivia)
        {
            if (list.Count == 0)
            {
                return SkipBadTokensWithExpectedKind(isNotExpectedFunction, abortFunction, expected,
                    out trailingTrivia);
            }
            else
            {
                GreenNode lastItemTrailingTrivia;
                var action = SkipBadTokensWithExpectedKind(isNotExpectedFunction, abortFunction, expected,
                    out lastItemTrailingTrivia);
                if (lastItemTrailingTrivia != null)
                {
                    AddTrailingSkippedSyntax(list, lastItemTrailingTrivia);
                }

                trailingTrivia = null;
                return action;
            }
        }

        private PostSkipAction SkipBadListTokensWithErrorCodeHelper<TNode>(
            SyntaxListBuilder<TNode> list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            ErrorCode error,
            out GreenNode trailingTrivia) where TNode : AquilaSyntaxNode
        {
            if (list.Count == 0)
            {
                return SkipBadTokensWithErrorCode(isNotExpectedFunction, abortFunction, error, out trailingTrivia);
            }
            else
            {
                GreenNode lastItemTrailingTrivia;
                var action = SkipBadTokensWithErrorCode(isNotExpectedFunction, abortFunction, error,
                    out lastItemTrailingTrivia);
                if (lastItemTrailingTrivia != null)
                {
                    AddTrailingSkippedSyntax(list, lastItemTrailingTrivia);
                }

                trailingTrivia = null;
                return action;
            }
        }

        private PostSkipAction SkipBadTokensWithExpectedKind(
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            SyntaxKind expected,
            out GreenNode trailingTrivia)
        {
            var nodes = _pool.Allocate();
            try
            {
                bool first = true;
                var action = PostSkipAction.Continue;
                while (isNotExpectedFunction(this))
                {
                    if (abortFunction(this))
                    {
                        action = PostSkipAction.Abort;
                        break;
                    }

                    var token = (first && !this.CurrentToken.ContainsDiagnostics)
                        ? this.EatTokenWithPrejudice(expected)
                        : this.EatToken();
                    first = false;
                    nodes.Add(token);
                }

                trailingTrivia = (nodes.Count > 0) ? nodes.ToListNode() : null;
                return action;
            }
            finally
            {
                _pool.Free(nodes);
            }
        }

        private PostSkipAction SkipBadTokensWithErrorCode(
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            ErrorCode errorCode,
            out GreenNode trailingTrivia)
        {
            var nodes = _pool.Allocate();
            try
            {
                bool first = true;
                var action = PostSkipAction.Continue;
                while (isNotExpectedFunction(this))
                {
                    if (abortFunction(this))
                    {
                        action = PostSkipAction.Abort;
                        break;
                    }

                    var token = (first && !this.CurrentToken.ContainsDiagnostics)
                        ? this.EatTokenWithPrejudice(errorCode)
                        : this.EatToken();
                    first = false;
                    nodes.Add(token);
                }

                trailingTrivia = (nodes.Count > 0) ? nodes.ToListNode() : null;
                return action;
            }
            finally
            {
                _pool.Free(nodes);
            }
        }

        private SyntaxToken EatAccessorSemicolon()
            => this.EatToken(SyntaxKind.SemicolonToken,
                IsFeatureEnabled(MessageID.IDS_FeatureExpressionBodiedAccessor)
                    ? ErrorCode.ERR_SemiOrLBraceOrArrowExpected
                    : ErrorCode.ERR_SemiOrLBraceExpected);

        private SyntaxKind GetAccessorKind(SyntaxToken accessorName)
        {
            switch (accessorName.ContextualKind)
            {
                case SyntaxKind.GetKeyword: return SyntaxKind.GetAccessorDeclaration;
                case SyntaxKind.SetKeyword: return SyntaxKind.SetAccessorDeclaration;
                case SyntaxKind.InitKeyword: return SyntaxKind.InitAccessorDeclaration;
                case SyntaxKind.AddKeyword: return SyntaxKind.AddAccessorDeclaration;
                case SyntaxKind.RemoveKeyword: return SyntaxKind.RemoveAccessorDeclaration;
            }

            return SyntaxKind.UnknownAccessorDeclaration;
        }


        internal ParameterListSyntax ParseParenthesizedParameterList()
        {
            if (this.IsIncrementalAndFactoryContextMatches &&
                CanReuseParameterList(this.CurrentNode as Aquila.CodeAnalysis.Syntax.ParameterListSyntax))
            {
                return (ParameterListSyntax)this.EatNode();
            }

            var parameters = _pool.AllocateSeparated<ParameterSyntax>();

            try
            {
                var openKind = SyntaxKind.OpenParenToken;
                var closeKind = SyntaxKind.CloseParenToken;

                SyntaxToken open;
                SyntaxToken close;
                this.ParseParameterList(out open, parameters, out close, openKind, closeKind);
                return _syntaxFactory.ParameterList(open, parameters, close);
            }
            finally
            {
                _pool.Free(parameters);
            }
        }

        private void ParseParameterList(
            out SyntaxToken open,
            SeparatedSyntaxListBuilder<ParameterSyntax> nodes,
            out SyntaxToken close,
            SyntaxKind openKind,
            SyntaxKind closeKind)
        {
            open = this.EatToken(openKind);

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfParameterList;

            if (this.CurrentToken.Kind != closeKind)
            {
                tryAgain:
                if (this.IsPossibleParameter() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // first parameter
                    var parameter = this.ParseParameter();
                    nodes.Add(parameter);

                    // additional parameters
                    while (true)
                    {
                        if (this.CurrentToken.Kind == closeKind)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleParameter())
                        {
                            nodes.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                            parameter = this.ParseParameter();
                            if (parameter.IsMissing && this.IsPossibleParameter())
                            {
                                // ensure we always consume tokens
                                parameter = AddTrailingSkippedSyntax(parameter, this.EatToken());
                            }

                            nodes.Add(parameter);
                            continue;
                        }
                        else if (this.SkipBadParameterListTokens(ref open, nodes, SyntaxKind.CommaToken, closeKind) ==
                                 PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadParameterListTokens(ref open, nodes, SyntaxKind.IdentifierToken, closeKind) ==
                         PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }

            _termState = saveTerm;
            close = this.EatToken(closeKind);
        }


        private ParameterSyntax ParseParameter()
        {
            if (this.IsIncrementalAndFactoryContextMatches &&
                CanReuseParameter(this.CurrentNode as Aquila.CodeAnalysis.Syntax.ParameterSyntax))
            {
                return (ParameterSyntax)this.EatNode();
            }

            var attributes = this.ParseAttributeDeclarations();

            var modifiers = _pool.Allocate();
            try
            {
                this.ParseParameterModifiers(modifiers);

                TypeEx type;
                SyntaxToken name;
                if (this.CurrentToken.Kind != SyntaxKind.ArgListKeyword)
                {
                    name = this.ParseIdentifierToken();
                    type = this.ParseType(mode: ParseTypeMode.Parameter);


                    // // When the user type "int goo[]", give them a useful error
                    // if (this.CurrentToken.Kind == SyntaxKind.OpenBracketToken &&
                    //     this.PeekToken(1).Kind == SyntaxKind.CloseBracketToken)
                    // {
                    //     var open = this.EatToken();
                    //     var close = this.EatToken();
                    //     open = this.AddError(open, ErrorCode.ERR_BadArraySyntax);
                    //     name = AddTrailingSkippedSyntax(name, SyntaxList.List(open, close));
                    // }
                }
                else
                {
                    // We store an __arglist parameter as a parameter with null type and whose 
                    // .Identifier has the kind ArgListKeyword.
                    type = null;
                    name = this.EatToken(SyntaxKind.ArgListKeyword);
                }

                EqualsValueClauseSyntax def = null;
                if (this.CurrentToken.Kind == SyntaxKind.EqualsToken)
                {
                    var equals = this.EatToken(SyntaxKind.EqualsToken);
                    var value = this.ParseExpressionCore();
                    def = _syntaxFactory.EqualsValueClause(equals, value: value);
                    def = CheckFeatureAvailability(def, MessageID.IDS_FeatureOptionalParameter);
                }

                return _syntaxFactory.Parameter(attributes, modifiers.ToList(), name, type, def);
            }
            finally
            {
                _pool.Free(modifiers);
            }
        }

        private static bool CanReuseParameterList(Aquila.CodeAnalysis.Syntax.ParameterListSyntax list)
        {
            if (list == null)
            {
                return false;
            }

            if (list.OpenParenToken.IsMissing)
            {
                return false;
            }

            if (list.CloseParenToken.IsMissing)
            {
                return false;
            }

            foreach (var parameter in list.Parameters)
            {
                if (!CanReuseParameter(parameter))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanReuseBracketedParameterList(Aquila.CodeAnalysis.Syntax.BracketedParameterListSyntax list)
        {
            if (list == null)
            {
                return false;
            }

            if (list.OpenBracketToken.IsMissing)
            {
                return false;
            }

            if (list.CloseBracketToken.IsMissing)
            {
                return false;
            }

            foreach (var parameter in list.Parameters)
            {
                if (!CanReuseParameter(parameter))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsEndOfParameterList()
        {
            return this.CurrentToken.Kind is SyntaxKind.CloseParenToken or SyntaxKind.CloseBracketToken
                or SyntaxKind.SemicolonToken;
        }

        private PostSkipAction SkipBadParameterListTokens(
            ref SyntaxToken open, SeparatedSyntaxListBuilder<ParameterSyntax> list, SyntaxKind expected,
            SyntaxKind closeKind)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref open, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleParameter(),
                p => p.CurrentToken.Kind == closeKind || p.IsTerminator(),
                expected);
        }

        private bool IsPossibleParameter()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.OpenBracketToken: // attribute
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.OpenParenToken: // tuple
                case SyntaxKind.DelegateKeyword when IsFunctionPointerStart(): // Function pointer type
                    return true;

                case SyntaxKind.IdentifierToken:
                    return this.IsTrueIdentifier();

                default:
                    return IsParameterModifier(this.CurrentToken.Kind) || IsPredefinedType(this.CurrentToken.Kind);
            }
        }

        private static bool CanReuseParameter(Aquila.CodeAnalysis.Syntax.ParameterSyntax parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            // cannot reuse a node that possibly ends in an expression
            if (parameter.Default != null)
            {
                return false;
            }

            // cannot reuse lambda parameters as normal parameters (parsed with
            // different rules)
            Aquila.CodeAnalysis.AquilaSyntaxNode parent = parameter.Parent;
            if (parent != null)
            {
                if (parent.Kind() == SyntaxKind.SimpleLambdaExpression)
                {
                    return false;
                }

                Aquila.CodeAnalysis.AquilaSyntaxNode grandparent = parent.Parent;
                if (grandparent != null && grandparent.Kind() == SyntaxKind.ParenthesizedLambdaExpression)
                {
                    Debug.Assert(parent.Kind() == SyntaxKind.ParameterList);
                    return false;
                }
            }

            return true;
        }

        private static bool IsParameterModifier(SyntaxKind kind, bool isFunctionPointerParameter = false)
        {
            switch (kind)
            {
                case SyntaxKind.ThisKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.ParamsKeyword:
                case SyntaxKind.ReadOnlyKeyword when isFunctionPointerParameter:
                    return true;
            }

            return false;
        }

        private void ParseParameterModifiers(SyntaxListBuilder modifiers, bool isFunctionPointerParameter = false)
        {
            while (IsParameterModifier(this.CurrentToken.Kind, isFunctionPointerParameter))
            {
                var modifier = this.EatToken();

                switch (modifier.Kind)
                {
                    case SyntaxKind.ThisKeyword:
                        modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureExtensionMethod);
                        if (this.CurrentToken.Kind == SyntaxKind.RefKeyword ||
                            this.CurrentToken.Kind == SyntaxKind.InKeyword)
                        {
                            modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureRefExtensionMethods);
                        }

                        break;

                    case SyntaxKind.RefKeyword:
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                        {
                            modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureRefExtensionMethods);
                        }

                        break;
                    }

                    case SyntaxKind.InKeyword:
                    {
                        modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureReadOnlyReferences);

                        if (this.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                        {
                            modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureRefExtensionMethods);
                        }

                        break;
                    }
                }

                modifiers.Add(modifier);
            }
        }


        private TNode EatUnexpectedTrailingSemicolon<TNode>(TNode decl) where TNode : AquilaSyntaxNode
        {
            // allow for case of one unexpected semicolon...
            if (this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                var semi = this.EatToken();
                semi = this.AddError(semi, ErrorCode.ERR_UnexpectedSemicolon);
                decl = AddTrailingSkippedSyntax(decl, semi);
            }

            return decl;
        }


        private bool IsEndOfFieldDeclaration()
        {
            return this.CurrentToken.Kind == SyntaxKind.SemicolonToken;
        }

        private void ParseVariableDeclarators(
            TypeEx type,
            VariableFlags flags,
            SeparatedSyntaxListBuilder<VariableInit> variables,
            bool variableDeclarationsExpected,
            bool allowLocalFunctions,
            SyntaxList<AttributeListSyntax> attributes,
            SyntaxList<SyntaxToken> mods)
        {
            variables.Add(
                this.ParseVariableDeclarator(
                    type,
                    flags,
                    isFirst: true,
                    allowLocalFunctions: allowLocalFunctions,
                    attributes: attributes,
                    mods: mods));

            while (true)
            {
                if (this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                {
                    break;
                }
                else if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    variables.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                    variables.Add(
                        this.ParseVariableDeclarator(
                            type,
                            flags,
                            isFirst: false,
                            allowLocalFunctions: false,
                            attributes: attributes,
                            mods: mods));
                }
                else if (!variableDeclarationsExpected ||
                         this.SkipBadVariableListTokens(variables, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                {
                    break;
                }
            }
        }

        private PostSkipAction SkipBadVariableListTokens(SeparatedSyntaxListBuilder<VariableInit> list,
            SyntaxKind expected)
        {
            AquilaSyntaxNode tmp = null;
            Debug.Assert(list.Count > 0);
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref tmp, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken,
                p => p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsTerminator(),
                expected);
        }

        [Flags]
        private enum VariableFlags
        {
            Fixed = 0x01,
            Const = 0x02,
            Local = 0x04
        }

        private static SyntaxTokenList GetOriginalModifiers(Aquila.CodeAnalysis.AquilaSyntaxNode decl)
        {
            if (decl != null)
            {
                switch (decl.Kind())
                {
                    // case SyntaxKind.FieldDeclaration:
                    //     return ((Aquila.CodeAnalysis.Syntax.FieldDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.MethodDeclaration:
                        return ((Aquila.CodeAnalysis.Syntax.MethodDecl)decl).Modifiers;
                    // case SyntaxKind.ConstructorDeclaration:
                    //     return ((Aquila.CodeAnalysis.Syntax.ConstructorDeclarationSyntax)decl).Modifiers;
                    // case SyntaxKind.DestructorDeclaration:
                    //     return ((Aquila.CodeAnalysis.Syntax.DestructorDeclarationSyntax)decl).Modifiers;
                    // case SyntaxKind.PropertyDeclaration:
                    //     return ((Aquila.CodeAnalysis.Syntax.PropertyDeclarationSyntax)decl).Modifiers;
                    // case SyntaxKind.EventFieldDeclaration:
                    //     return ((Aquila.CodeAnalysis.Syntax.EventFieldDeclarationSyntax)decl).Modifiers;
                    // case SyntaxKind.AddAccessorDeclaration:
                    // case SyntaxKind.RemoveAccessorDeclaration:
                    // case SyntaxKind.GetAccessorDeclaration:
                    // case SyntaxKind.SetAccessorDeclaration:
                    // case SyntaxKind.InitAccessorDeclaration:
                    //     return ((Aquila.CodeAnalysis.Syntax.AccessorDeclarationSyntax)decl).Modifiers;
                    // case SyntaxKind.ClassDeclaration:
                    // case SyntaxKind.StructDeclaration:
                    // case SyntaxKind.InterfaceDeclaration:
                    // case SyntaxKind.RecordDeclaration:
                    // case SyntaxKind.RecordStructDeclaration:
                    //     return ((Aquila.CodeAnalysis.Syntax.TypeDeclarationSyntax)decl).Modifiers;
                    // case SyntaxKind.DelegateDeclaration:
                    //     return ((Aquila.CodeAnalysis.Syntax.DelegateDeclarationSyntax)decl).Modifiers;
                }
            }

            return default(SyntaxTokenList);
        }

        private static bool WasFirstVariable(Aquila.CodeAnalysis.Syntax.VariableInit variable)
        {
            var parent = GetOldParent(variable) as Aquila.CodeAnalysis.Syntax.VariableDecl;
            if (parent != null)
            {
                return parent.Variables[0] == variable;
            }

            return false;
        }

        private static VariableFlags GetOriginalVariableFlags(Aquila.CodeAnalysis.Syntax.VariableInit old)
        {
            var parent = GetOldParent(old);
            var mods = GetOriginalModifiers(parent);
            VariableFlags flags = default(VariableFlags);
            if (mods.Any(SyntaxKind.FixedKeyword))
            {
                flags |= VariableFlags.Fixed;
            }

            if (mods.Any(SyntaxKind.ConstKeyword))
            {
                flags |= VariableFlags.Const;
            }

            if (parent != null && (parent.Kind() == SyntaxKind.VariableDeclaration ||
                                   parent.Kind() == SyntaxKind.LocalDeclarationStatement))
            {
                flags |= VariableFlags.Local;
            }

            return flags;
        }

        private static bool CanReuseVariableDeclarator(Syntax.VariableInit old, VariableFlags flags,
            bool isFirst)
        {
            if (old == null)
            {
                return false;
            }

            SyntaxKind oldKind;

            return (flags == GetOriginalVariableFlags(old))
                   && (isFirst == WasFirstVariable(old))
                   && old.Initializer == null // can't reuse node that possibly ends in an expression
                   && (oldKind = GetOldParent(old).Kind()) != SyntaxKind.VariableDeclaration // or in a method body
                   && oldKind != SyntaxKind.LocalDeclarationStatement;
        }

        private VariableInit ParseVariableDeclarator(
            TypeEx parentType,
            VariableFlags flags,
            bool isFirst,
            bool allowLocalFunctions,
            SyntaxList<AttributeListSyntax> attributes,
            SyntaxList<SyntaxToken> mods,
            bool isExpressionContext = false)
        {
            if (this.IsIncrementalAndFactoryContextMatches &&
                CanReuseVariableDeclarator(this.CurrentNode as Aquila.CodeAnalysis.Syntax.VariableInit, flags, isFirst))
            {
                return (VariableInit)this.EatNode();
            }

            if (!isExpressionContext)
            {
                // Check for the common pattern of:
                //
                // C                    //<-- here
                // Console.WriteLine();
                //
                // Standard greedy parsing will assume that this should be parsed as a variable
                // declaration: "C Console".  We want to avoid that as it can confused parts of the
                // system further up.  So, if we see certain things following the identifier, then we can
                // assume it's not the actual name.  
                // 
                // So, if we're after a newline and we see a name followed by the list below, then we
                // assume that we're accidentally consuming too far into the next statement.
                //
                // <dot>, <arrow>, any binary operator (except =), <question>.  None of these characters
                // are allowed in a normal variable declaration.  This also provides a more useful error
                // message to the user.  Instead of telling them that a semicolon is expected after the
                // following token, then instead get a useful message about an identifier being missing.
                // The above list prevents:
                //
                // C                    //<-- here
                // Console.WriteLine();
                //
                // C                    //<-- here 
                // Console->WriteLine();
                //
                // C 
                // A + B;
                //
                // C 
                // A ? B : D;
                //
                // C 
                // A()
                var resetPoint = this.GetResetPoint();
                try
                {
                    var currentTokenKind = this.CurrentToken.Kind;
                    if (currentTokenKind == SyntaxKind.IdentifierToken && !parentType.IsMissing)
                    {
                        var isAfterNewLine = parentType.GetLastToken().TrailingTrivia
                            .Any((int)SyntaxKind.EndOfLineTrivia);
                        if (isAfterNewLine)
                        {
                            int offset, width;
                            this.GetDiagnosticSpanForMissingToken(out offset, out width);

                            this.EatToken();
                            currentTokenKind = this.CurrentToken.Kind;

                            var isNonEqualsBinaryToken =
                                currentTokenKind != SyntaxKind.EqualsToken &&
                                SyntaxFacts.IsBinaryExpressionOperatorToken(currentTokenKind);

                            if (currentTokenKind == SyntaxKind.DotToken ||
                                currentTokenKind == SyntaxKind.OpenParenToken ||
                                currentTokenKind == SyntaxKind.MinusGreaterThanToken ||
                                isNonEqualsBinaryToken)
                            {
                                var isPossibleLocalFunctionToken =
                                    currentTokenKind == SyntaxKind.OpenParenToken ||
                                    currentTokenKind == SyntaxKind.LessThanToken;

                                // Make sure this isn't a local function
                                if (!isPossibleLocalFunctionToken || !IsLocalFunctionAfterIdentifier())
                                {
                                    var missingIdentifier = CreateMissingIdentifierToken();
                                    missingIdentifier = this.AddError(missingIdentifier, offset, width,
                                        ErrorCode.ERR_IdentifierExpected);

                                    return _syntaxFactory.VariableInit(missingIdentifier, null, null);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    this.Reset(ref resetPoint);
                    this.Release(ref resetPoint);
                }
            }

            // NOTE: Diverges from Dev10.
            //
            // When we see parse an identifier and we see the partial contextual keyword, we check
            // to see whether it is already attached to a partial class or partial method
            // declaration.  However, in the specific case of variable declarators, Dev10
            // specifically treats it as a variable name, even if it could be interpreted as a
            // keyword.
            var name = this.ParseIdentifierToken();
            BracketedArgumentListSyntax argumentList = null;
            EqualsValueClauseSyntax initializer = null;
            TerminatorState saveTerm = _termState;
            bool isFixed = (flags & VariableFlags.Fixed) != 0;
            bool isConst = (flags & VariableFlags.Const) != 0;
            bool isLocal = (flags & VariableFlags.Local) != 0;

            // Give better error message in the case where the user did something like:
            //
            // X x = 1, Y y = 2; 
            // using (X x = expr1, Y y = expr2) ...
            //
            // The superfluous type name is treated as variable (it is an identifier) and a missing ',' is injected after it.
            if (!isFirst && this.IsTrueIdentifier())
            {
                name = this.AddError(name, ErrorCode.ERR_MultiTypeInDeclaration);
            }

            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.EqualsToken:
                    if (isFixed)
                    {
                        goto default;
                    }

                    var equals = this.EatToken();

                    SyntaxToken refKeyword = null;
                    if (isLocal && !isConst &&
                        this.CurrentToken.Kind == SyntaxKind.RefKeyword
                       )
                    {
                        refKeyword = this.EatToken();
                        refKeyword = CheckFeatureAvailability(refKeyword, MessageID.IDS_FeatureRefLocalsReturns);
                    }

                    var init = this.ParseVariableInitializer();
                    // if (refKeyword != null)
                    // {
                    //     init = _syntaxFactory.RefExpression(refKeyword, init);
                    // }

                    initializer = _syntaxFactory.EqualsValueClause(equals, init);
                    break;

                // case SyntaxKind.LessThanToken:
                //     if (allowLocalFunctions && isFirst)
                //     {
                //         localFunction = TryParseLocalFunctionStatementBody(attributes, mods, parentType, name);
                //         if (localFunction != null)
                //         {
                //             return null;
                //         }
                //     }
                //
                //     goto default;
                //
                // case SyntaxKind.OpenParenToken:
                //     if (allowLocalFunctions && isFirst)
                //     {
                //         localFunction = TryParseLocalFunctionStatementBody(attributes, mods, parentType, name);
                //         if (localFunction != null)
                //         {
                //             return null;
                //         }
                //     }
                //
                //     // Special case for accidental use of C-style constructors
                //     // Fake up something to hold the arguments.
                //     _termState |= TerminatorState.IsPossibleEndOfVariableDeclaration;
                //     argumentList = this.ParseBracketedArgumentList();
                //     _termState = saveTerm;
                //     argumentList = this.AddError(argumentList, ErrorCode.ERR_BadVarDecl);
                //     break;

                case SyntaxKind.OpenBracketToken:
                    bool sawNonOmittedSize;
                    _termState |= TerminatorState.IsPossibleEndOfVariableDeclaration;
                    var specifier = this.ParseArrayRankSpecifier(sawNonOmittedSize: out sawNonOmittedSize);
                    _termState = saveTerm;
                    var open = specifier.OpenBracketToken;
                    var sizes = specifier.Sizes;
                    var close = specifier.CloseBracketToken;
                    if (isFixed && !sawNonOmittedSize)
                    {
                        close = this.AddError(close, ErrorCode.ERR_ValueExpected);
                    }

                    var args = _pool.AllocateSeparated<ArgumentSyntax>();
                    try
                    {
                        var withSeps = sizes.GetWithSeparators();
                        foreach (var item in withSeps)
                        {
                            var expression = item as ExprSyntax;
                            if (expression != null)
                            {
                                bool isOmitted = expression.Kind == SyntaxKind.OmittedArraySizeExpression;
                                if (!isFixed && !isOmitted)
                                {
                                    expression = this.AddError(expression, ErrorCode.ERR_ArraySizeInDeclaration);
                                }

                                args.Add(_syntaxFactory.Argument(null, refKindKeyword: null, expression));
                            }
                            else
                            {
                                args.AddSeparator((SyntaxToken)item);
                            }
                        }

                        argumentList = _syntaxFactory.BracketedArgumentList(open, args, close);
                        if (!isFixed)
                        {
                            argumentList = this.AddError(argumentList, ErrorCode.ERR_CStyleArray);
                            // If we have "int x[] = new int[10];" then parse the initializer.
                            if (this.CurrentToken.Kind == SyntaxKind.EqualsToken)
                            {
                                goto case SyntaxKind.EqualsToken;
                            }
                        }
                    }
                    finally
                    {
                        _pool.Free(args);
                    }

                    break;

                default:
                    if (isConst)
                    {
                        name = this.AddError(name,
                            ErrorCode.ERR_ConstValueRequired); // Error here for missing constant initializers
                    }
                    else if (isFixed)
                    {
                        if (parentType.Kind == SyntaxKind.ArrayType)
                        {
                            // They accidentally put the array before the identifier
                            name = this.AddError(name, ErrorCode.ERR_FixedDimsRequired);
                        }
                        else
                        {
                            goto case SyntaxKind.OpenBracketToken;
                        }
                    }

                    break;
            }

            if (string.IsNullOrWhiteSpace(name.GetValueText()))
            {
                throw new Exception("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            }

            return _syntaxFactory.VariableInit(name, argumentList, initializer);
        }

        // Is there a local function after an eaten identifier?
        private bool IsLocalFunctionAfterIdentifier()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.OpenParenToken ||
                         this.CurrentToken.Kind == SyntaxKind.LessThanToken);
            var resetPoint = this.GetResetPoint();

            try
            {
                var typeParameterListOpt = this.ParseTypeParameterList();
                var paramList = ParseParenthesizedParameterList();

                if (!paramList.IsMissing &&
                    (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken ||
                     this.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken ||
                     this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword))
                {
                    return true;
                }

                return false;
            }
            finally
            {
                Reset(ref resetPoint);
                Release(ref resetPoint);
            }
        }

        private bool IsPossibleEndOfVariableDeclaration()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.CommaToken:
                case SyntaxKind.SemicolonToken:
                    return true;
                default:
                    return false;
            }
        }

        private ExprSyntax ParseVariableInitializer()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return this.ParseArrayInitializer();
                default:
                    return this.ParseExpressionCore();
            }
        }


        private bool IsPossibleVariableInitializer()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBraceToken || this.IsPossibleExpression();
        }


        private bool IsPossibleEnumMemberDeclaration()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBracketToken || this.IsTrueIdentifier();
        }

        private bool IsDotOrColonColon()
        {
            return this.CurrentToken.Kind == SyntaxKind.DotToken ||
                   this.CurrentToken.Kind == SyntaxKind.ColonColonToken;
        }

        private IdentifierEx CreateMissingIdentifierName()
        {
            return _syntaxFactory.IdentifierEx(CreateMissingIdentifierToken());
        }


        private static SyntaxToken CreateMissingIdentifierToken()
        {
            return SyntaxFactory.MissingToken(SyntaxKind.IdentifierToken);
        }

        [Flags]
        private enum NameOptions
        {
            None = 0,

            InExpression =
                1 << 0, // Used to influence parser ambiguity around "<" and generics vs. expressions. Used in ParseSimpleName.

            InTypeList =
                1 << 1, // Allows attributes to appear within the generic type argument list. Used during ParseInstantiation.

            PossiblePattern =
                1 << 2, // Used to influence parser ambiguity around "<" and generics vs. expressions on the right of 'is'
            AfterIs = 1 << 3,
            DefinitePattern = 1 << 4,
            AfterOut = 1 << 5,
            AfterTupleComma = 1 << 6,
            FirstElementOfPossibleTupleLiteral = 1 << 7,
            InUnionType = 1 << 8
        }

        /// <summary>
        /// True if current identifier token is not really some contextual keyword
        /// </summary>
        /// <returns></returns>
        private bool IsTrueIdentifier()
        {
            if (this.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                if (!IsCurrentTokenPartialKeywordOfPartialMethodOrType() &&
                    !IsCurrentTokenQueryKeywordInQuery() &&
                    !IsCurrentTokenWhereOfConstraintClause())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// True if the given token is not really some contextual keyword.
        /// This method is for use in executable code, as it treats <c>partial</c> as an identifier.
        /// </summary>
        private bool IsTrueIdentifier(SyntaxToken token)
        {
            return
                token.Kind == SyntaxKind.IdentifierToken &&
                !(this.IsInQuery && IsTokenQueryContextualKeyword(token));
        }

        private IdentifierEx ParseIdentifierName(ErrorCode code = ErrorCode.ERR_IdentifierExpected)
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.IdentifierName)
            {
                if (!SyntaxFacts.IsContextualKeyword(((Aquila.CodeAnalysis.Syntax.IdentifierEx)this.CurrentNode)
                        .Identifier
                        .Kind()))
                {
                    return (IdentifierEx)this.EatNode();
                }
            }

            var tk = ParseIdentifierToken(code);
            return SyntaxFactory.IdentifierEx(tk);
        }

        private SyntaxToken ParseIdentifierToken(ErrorCode code = ErrorCode.ERR_IdentifierExpected)
        {
            var ctk = this.CurrentToken.Kind;
            if (ctk == SyntaxKind.IdentifierToken)
            {
                // Error tolerance for IntelliSense. Consider the following case: [EditorBrowsable( partial class Goo {
                // } Because we're parsing an attribute argument we'll end up consuming the "partial" identifier and
                // we'll eventually end up in a pretty confused state.  Because of that it becomes very difficult to
                // show the correct parameter help in this case.  So, when we see "partial" we check if it's being used
                // as an identifier or as a contextual keyword.  If it's the latter then we bail out.  See
                // Bug: vswhidbey/542125
                if (IsCurrentTokenPartialKeywordOfPartialMethodOrType() || IsCurrentTokenQueryKeywordInQuery())
                {
                    var result = CreateMissingIdentifierToken();
                    result = this.AddError(result, ErrorCode.ERR_InvalidExprTerm, this.CurrentToken.Text);
                    return result;
                }

                SyntaxToken identifierToken = this.EatToken();

                if (this.IsInAsync && identifierToken.ContextualKind == SyntaxKind.AwaitKeyword)
                {
                    identifierToken = this.AddError(identifierToken, ErrorCode.ERR_BadAwaitAsIdentifier);
                }

                return identifierToken;
            }
            else
            {
                var name = CreateMissingIdentifierToken();
                name = this.AddError(name, code);
                return name;
            }
        }

        private bool IsCurrentTokenQueryKeywordInQuery()
        {
            return this.IsInQuery && this.IsCurrentTokenQueryContextualKeyword;
        }

        private bool IsCurrentTokenPartialKeywordOfPartialMethodOrType()
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword)
            {
                if (this.IsPartialType() || this.IsPartialMember())
                {
                    return true;
                }
            }

            return false;
        }

        private TypeParameterListSyntax ParseTypeParameterList()
        {
            if (this.CurrentToken.Kind != SyntaxKind.LessThanToken)
            {
                return null;
            }

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfTypeParameterList;
            try
            {
                var parameters = _pool.AllocateSeparated<TypeParameterSyntax>();
                var open = this.EatToken(SyntaxKind.LessThanToken);
                open = CheckFeatureAvailability(open, MessageID.IDS_FeatureGenerics);

                // first parameter
                parameters.Add(this.ParseTypeParameter());

                // remaining parameter & commas
                while (true)
                {
                    if (this.CurrentToken.Kind == SyntaxKind.GreaterThanToken ||
                        this.IsCurrentTokenWhereOfConstraintClause())
                    {
                        break;
                    }
                    else if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        parameters.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                        parameters.Add(this.ParseTypeParameter());
                    }
                    else if (this.SkipBadTypeParameterListTokens(parameters, SyntaxKind.CommaToken) ==
                             PostSkipAction.Abort)
                    {
                        break;
                    }
                }

                var close = this.EatToken(SyntaxKind.GreaterThanToken);

                return _syntaxFactory.TypeParameterList(open, parameters, close);
            }
            finally
            {
                _termState = saveTerm;
            }
        }

        private PostSkipAction SkipBadTypeParameterListTokens(SeparatedSyntaxListBuilder<TypeParameterSyntax> list,
            SyntaxKind expected)
        {
            AquilaSyntaxNode tmp = null;
            Debug.Assert(list.Count > 0);
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref tmp, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken,
                p => p.CurrentToken.Kind == SyntaxKind.GreaterThanToken || p.IsTerminator(),
                expected);
        }

        private TypeParameterSyntax ParseTypeParameter()
        {
            if (this.IsCurrentTokenWhereOfConstraintClause())
            {
                return _syntaxFactory.TypeParameter(
                    default(SyntaxList<AttributeListSyntax>),
                    varianceKeyword: null,
                    this.AddError(CreateMissingIdentifierToken(), ErrorCode.ERR_IdentifierExpected));
            }

            var attrs = default(SyntaxList<AttributeListSyntax>);
            if (this.CurrentToken.Kind == SyntaxKind.OpenBracketToken &&
                this.PeekToken(1).Kind != SyntaxKind.CloseBracketToken)
            {
                var saveTerm = _termState;
                _termState = TerminatorState.IsEndOfTypeArgumentList;
                attrs = this.ParseAttributeDeclarations();
                _termState = saveTerm;
            }

            SyntaxToken varianceToken = null;
            if (this.CurrentToken.Kind == SyntaxKind.InKeyword ||
                this.CurrentToken.Kind == SyntaxKind.OutKeyword)
            {
                varianceToken = CheckFeatureAvailability(this.EatToken(), MessageID.IDS_FeatureTypeVariance);
            }

            return _syntaxFactory.TypeParameter(attrs, varianceToken, this.ParseIdentifierToken());
        }

        private SimpleNameEx ParseSimpleName(NameOptions options = NameOptions.None)
        {
            var id = this.ParseIdentifierName();
            if (id.Identifier.IsMissing)
            {
                return id;
            }

            // You can pass ignore generics if you don't even want the parser to consider generics at all.
            // The name parsing will then stop at the first "<". It doesn't make sense to pass both Generic and IgnoreGeneric.

            SimpleNameEx name = id;
            if (this.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                var pt = this.GetResetPoint();
                var kind = this.ScanTypeArgumentList(options);
                this.Reset(ref pt);
                this.Release(ref pt);

                if (kind == ScanTypeArgumentListKind.DefiniteTypeArgumentList ||
                    (kind == ScanTypeArgumentListKind.PossibleTypeArgumentList &&
                     (options & NameOptions.InTypeList) != 0))
                {
                    Debug.Assert(this.CurrentToken.Kind == SyntaxKind.LessThanToken);
                    SyntaxToken open;
                    var types = _pool.AllocateSeparated<TypeEx>();
                    SyntaxToken close;
                    this.ParseTypeArgumentList(out open, types, out close);
                    name = _syntaxFactory.GenericEx(id.Identifier,
                        _syntaxFactory.TypeArgumentList(open, types, close));
                    _pool.Free(types);
                }
            }

            return name;
        }

        private enum ScanTypeArgumentListKind
        {
            NotTypeArgumentList,
            PossibleTypeArgumentList,
            DefiniteTypeArgumentList
        }

        private ScanTypeArgumentListKind ScanTypeArgumentList(NameOptions options)
        {
            if (this.CurrentToken.Kind != SyntaxKind.LessThanToken)
            {
                return ScanTypeArgumentListKind.NotTypeArgumentList;
            }

            if ((options & NameOptions.InExpression) == 0)
            {
                return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
            }

            // We're in an expression context, and we have a < token.  This could be a 
            // type argument list, or it could just be a relational expression.  
            //
            // Scan just the type argument list portion (i.e. the part from < to > ) to
            // see what we think it could be.  This will give us one of three possibilities:
            //
            //      result == ScanTypeFlags.NotType.
            //
            // This is absolutely not a type-argument-list.  Just return that result immediately.
            //
            //      result != ScanTypeFlags.NotType && isDefinitelyTypeArgumentList.
            //
            // This is absolutely a type-argument-list.  Just return that result immediately
            // 
            //      result != ScanTypeFlags.NotType && !isDefinitelyTypeArgumentList.
            //
            // This could be a type-argument list, or it could be an expression.  Need to see
            // what came after the last '>' to find out which it is.

            // Scan for a type argument list. If we think it's a type argument list
            // then assume it is unless we see specific tokens following it.
            SyntaxToken lastTokenOfList = null;
            ScanTypeFlags possibleTypeArgumentFlags = ScanPossibleTypeArgumentList(
                ref lastTokenOfList, out bool isDefinitelyTypeArgumentList);

            if (possibleTypeArgumentFlags == ScanTypeFlags.NotType)
            {
                return ScanTypeArgumentListKind.NotTypeArgumentList;
            }

            if (isDefinitelyTypeArgumentList)
            {
                return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
            }

            // If we did not definitively determine from immediate syntax that it was or
            // was not a type argument list, we must have scanned the entire thing up through
            // the closing greater-than token. In that case we will disambiguate based on the
            // token that follows it.
            Debug.Assert(lastTokenOfList.Kind == SyntaxKind.GreaterThanToken);

            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.ColonToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.DotToken:
                case SyntaxKind.QuestionToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.CaretToken:
                    // These tokens are from 7.5.4.2 Grammar Ambiguities
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                case SyntaxKind.AmpersandAmpersandToken: // e.g. `e is A<B> && e`
                case SyntaxKind.BarBarToken: // e.g. `e is A<B> || e`
                case SyntaxKind.AmpersandToken: // e.g. `e is A<B> & e`
                case SyntaxKind.OpenBracketToken: // e.g. `e is A<B>[]`
                case SyntaxKind.LessThanToken: // e.g. `e is A<B> < C`
                case SyntaxKind.LessThanEqualsToken: // e.g. `e is A<B> <= C`
                case SyntaxKind.GreaterThanEqualsToken: // e.g. `e is A<B> >= C`
                case SyntaxKind.IsKeyword: // e.g. `e is A<B> is bool`
                case SyntaxKind.AsKeyword: // e.g. `e is A<B> as bool`
                    // These tokens were added to 7.5.4.2 Grammar Ambiguities in C# 7.0
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                case SyntaxKind.OpenBraceToken: // e.g. `e is A<B> {}`
                    // This token was added to 7.5.4.2 Grammar Ambiguities in C# 8.0
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                case SyntaxKind.GreaterThanToken when ((options & NameOptions.AfterIs) != 0) &&
                                                      this.PeekToken(1).Kind != SyntaxKind.GreaterThanToken:
                    // This token is added to 7.5.4.2 Grammar Ambiguities in C#7 for the special case in which
                    // the possible generic is following an `is` keyword, e.g. `e is A<B> > C`.
                    // We test one further token ahead because a right-shift operator `>>` looks like a pair of greater-than
                    // tokens at this stage, but we don't intend to be handling the right-shift operator.
                    // The upshot is that we retain compatibility with the two previous behaviors:
                    // `(x is A<B>>C)` is parsed as `(x is A<B>) > C`
                    // `A<B>>C` elsewhere is parsed as `A < (B >> C)`
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                case SyntaxKind.IdentifierToken:
                    // C#7: In certain contexts, we treat *identifier* as a disambiguating token. Those
                    // contexts are where the sequence of tokens being disambiguated is immediately preceded by one
                    // of the keywords is, case, or out, or arises while parsing the first element of a tuple literal
                    // (in which case the tokens are preceded by `(` and the identifier is followed by a `,`) or a
                    // subsequent element of a tuple literal (in which case the tokens are preceded by `,` and the
                    // identifier is followed by a `,` or `)`).
                    // In C#8 (or whenever recursive patterns are introduced) we also treat an identifier as a
                    // disambiguating token if we're parsing the type of a pattern.
                    // Note that we treat query contextual keywords (which appear here as identifiers) as disambiguating tokens as well.
                    if ((options & (NameOptions.AfterIs | NameOptions.DefinitePattern | NameOptions.AfterOut)) != 0 ||
                        (options & NameOptions.AfterTupleComma) != 0 &&
                        (this.PeekToken(1).Kind == SyntaxKind.CommaToken ||
                         this.PeekToken(1).Kind == SyntaxKind.CloseParenToken) ||
                        (options & NameOptions.FirstElementOfPossibleTupleLiteral) != 0 &&
                        this.PeekToken(1).Kind == SyntaxKind.CommaToken
                       )
                    {
                        // we allow 'G<T,U> x' as a pattern-matching operation and a declaration expression in a tuple.
                        return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
                    }

                    return ScanTypeArgumentListKind.PossibleTypeArgumentList;

                case SyntaxKind.EndOfFileToken: // e.g. `e is A<B>`
                    // This is useful for parsing expressions in isolation
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                case SyntaxKind.EqualsGreaterThanToken: // e.g. `e switch { A<B> => 1 }`
                    // This token was added to 7.5.4.2 Grammar Ambiguities in C# 9.0
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                default:
                    return ScanTypeArgumentListKind.PossibleTypeArgumentList;
            }
        }

        private ScanTypeFlags ScanPossibleTypeArgumentList(
            ref SyntaxToken lastTokenOfList, out bool isDefinitelyTypeArgumentList)
        {
            isDefinitelyTypeArgumentList = false;

            if (this.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                ScanTypeFlags result = ScanTypeFlags.GenericTypeOrExpression;

                do
                {
                    lastTokenOfList = this.EatToken();

                    // Type arguments cannot contain attributes, so if this is an open square, we early out and assume it is not a type argument
                    if (this.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
                    {
                        lastTokenOfList = null;
                        return ScanTypeFlags.NotType;
                    }

                    if (this.CurrentToken.Kind == SyntaxKind.GreaterThanToken)
                    {
                        lastTokenOfList = EatToken();
                        return result;
                    }

                    switch (this.ScanType(out lastTokenOfList))
                    {
                        case ScanTypeFlags.NotType:
                            lastTokenOfList = null;
                            return ScanTypeFlags.NotType;

                        case ScanTypeFlags.MustBeType:
                            // We're currently scanning a possible type-argument list.  But we're
                            // not sure if this is actually a type argument list, or is maybe some
                            // complex relational expression with <'s and >'s.  One thing we can
                            // tell though is that if we have a predefined type (like 'int' or 'string')
                            // before a comma or > then this is definitely a type argument list. i.e.
                            // if you have:
                            // 
                            //      var v = ImmutableDictionary<int,
                            //
                            // then there's no legal interpretation of this as an expression (since a
                            // standalone predefined type is not a valid simple term.  Contrast that
                            // with :
                            //
                            //  var v = ImmutableDictionary<Int32,
                            //
                            // Here this might actually be a relational expression and the comma is meant
                            // to separate out the variable declarator 'v' from the next variable.
                            //
                            // Note: we check if we got 'MustBeType' which triggers for predefined types,
                            // (int, string, etc.), or array types (Goo[], A<T>[][] etc.), or pointer types
                            // of things that must be types (int*, void**, etc.).
                            isDefinitelyTypeArgumentList =
                                DetermineIfDefinitelyTypeArgumentList(isDefinitelyTypeArgumentList);
                            result = ScanTypeFlags.GenericTypeOrMethod;
                            break;

                        // case ScanTypeFlags.TupleType:
                        // It would be nice if we saw a tuple to state that we definitely had a 
                        // type argument list.  However, there are cases where this would not be
                        // true.  For example:
                        //
                        // public class C
                        // {
                        //     public static void Main()
                        //     {
                        //         XX X = default;
                        //         int a = 1, b = 2;
                        //         bool z = X < (a, b), w = false;
                        //     }
                        // }
                        //
                        // struct XX
                        // {
                        //     public static bool operator <(XX x, (int a, int b) arg) => true;
                        //     public static bool operator >(XX x, (int a, int b) arg) => false;
                        // }

                        case ScanTypeFlags.NullableType:
                            // See above.  If we have X<Y?,  or X<Y?>, then this is definitely a type argument list.
                            isDefinitelyTypeArgumentList =
                                DetermineIfDefinitelyTypeArgumentList(isDefinitelyTypeArgumentList);
                            if (isDefinitelyTypeArgumentList)
                            {
                                result = ScanTypeFlags.GenericTypeOrMethod;
                            }

                            // Note: we intentionally fall out without setting 'result'. 
                            // Seeing a nullable type (not followed by a , or > ) is not enough 
                            // information for us to determine what this is yet.  i.e. the user may have:
                            //
                            //      X < Y ? Z : W
                            //
                            // We'd see a nullable type here, but this is definitely not a type arg list.

                            break;

                        case ScanTypeFlags.GenericTypeOrExpression:
                            // See above.  If we have  X<Y<Z>,  then this would definitely be a type argument list.
                            // However, if we have  X<Y<Z>> then this might not be type argument list.  This could just
                            // be some sort of expression where we're comparing, and then shifting values.
                            if (!isDefinitelyTypeArgumentList)
                            {
                                isDefinitelyTypeArgumentList = this.CurrentToken.Kind == SyntaxKind.CommaToken;
                                result = ScanTypeFlags.GenericTypeOrMethod;
                            }

                            break;

                        case ScanTypeFlags.GenericTypeOrMethod:
                            result = ScanTypeFlags.GenericTypeOrMethod;
                            break;
                    }
                } while (this.CurrentToken.Kind == SyntaxKind.CommaToken);

                if (this.CurrentToken.Kind != SyntaxKind.GreaterThanToken)
                {
                    lastTokenOfList = null;
                    return ScanTypeFlags.NotType;
                }

                lastTokenOfList = this.EatToken();
                return result;
            }

            return ScanTypeFlags.NonGenericTypeOrExpression;
        }

        private bool DetermineIfDefinitelyTypeArgumentList(bool isDefinitelyTypeArgumentList)
        {
            if (!isDefinitelyTypeArgumentList)
            {
                isDefinitelyTypeArgumentList =
                    this.CurrentToken.Kind == SyntaxKind.CommaToken ||
                    this.CurrentToken.Kind == SyntaxKind.GreaterThanToken;
            }

            return isDefinitelyTypeArgumentList;
        }

        private void ParseTypeArgumentList(out SyntaxToken open, SeparatedSyntaxListBuilder<TypeEx> types,
            out SyntaxToken close)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.LessThanToken);
            open = this.EatToken(SyntaxKind.LessThanToken);
            open = CheckFeatureAvailability(open, MessageID.IDS_FeatureGenerics);

            if (this.IsOpenName())
            {
                // NOTE: trivia will be attached to comma, not omitted type argument
                // var omittedTypeArgumentInstance =
                //     _syntaxFactory.OmittedTypeArgument(SyntaxFactory.Token(SyntaxKind.OmittedTypeArgumentToken));
                // types.Add(omittedTypeArgumentInstance);
                // while (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                // {
                //     types.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                //     types.Add(omittedTypeArgumentInstance);
                // }

                close = this.EatToken(SyntaxKind.GreaterThanToken);

                return;
            }

            // first type
            types.Add(this.ParseTypeArgument());

            // remaining types & commas
            while (true)
            {
                if (this.CurrentToken.Kind == SyntaxKind.GreaterThanToken)
                {
                    break;
                }
                else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleType())
                {
                    types.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                    types.Add(this.ParseTypeArgument());
                }
                else if (this.SkipBadTypeArgumentListTokens(types, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                {
                    break;
                }
            }

            close = this.EatToken(SyntaxKind.GreaterThanToken);
        }

        private PostSkipAction SkipBadTypeArgumentListTokens(SeparatedSyntaxListBuilder<TypeEx> list,
            SyntaxKind expected)
        {
            AquilaSyntaxNode tmp = null;
            Debug.Assert(list.Count > 0);
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref tmp, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleType(),
                p => p.CurrentToken.Kind == SyntaxKind.GreaterThanToken || p.IsTerminator(),
                expected);
        }

        private TypeEx ParseTypeArgument()
        {
            var attrs = default(SyntaxList<AttributeListSyntax>);
            SyntaxToken varianceToken = null;
            if (this.CurrentToken.Kind == SyntaxKind.InKeyword || this.CurrentToken.Kind == SyntaxKind.OutKeyword)
            {
                // Recognize the variance syntax, but give an error as it's
                // only appropriate in a type parameter list.
                varianceToken = this.EatToken();
                varianceToken = CheckFeatureAvailability(varianceToken, MessageID.IDS_FeatureTypeVariance);
                varianceToken = this.AddError(varianceToken, ErrorCode.ERR_IllegalVarianceSyntax);
            }

            var result = this.ParseType();

            // Consider the case where someone supplies an invalid type argument
            // Such as Action<0> or Action<static>.  In this case we generate a missing 
            // identifier in ParseType, but if we continue as is we'll immediately start to 
            // interpret 0 as the start of a new expression when we can tell it's most likely
            // meant to be part of the type list.  
            //
            // To solve this we check if the current token is not comma or greater than and 
            // the next token is a comma or greater than. If so we assume that the found 
            // token is part of this expression and we attempt to recover. This does open 
            // the door for cases where we have an  incomplete line to be interpretted as 
            // a single expression.  For example:
            //
            // Action< // Incomplete line
            // a>b;
            //
            // However, this only happens when the following expression is of the form a>... 
            // or a,... which  means this case should happen less frequently than what we're 
            // trying to solve here so we err on the side of better error messages
            // for the majority of cases.
            SyntaxKind nextTokenKind = SyntaxKind.None;

            if (result.IsMissing &&
                (this.CurrentToken.Kind != SyntaxKind.CommaToken &&
                 this.CurrentToken.Kind != SyntaxKind.GreaterThanToken) &&
                ((nextTokenKind = this.PeekToken(1).Kind) == SyntaxKind.CommaToken ||
                 nextTokenKind == SyntaxKind.GreaterThanToken))
            {
                // Eat the current token and add it as skipped so we recover
                result = AddTrailingSkippedSyntax(result, this.EatToken());
            }

            if (varianceToken != null)
            {
                result = AddLeadingSkippedSyntax(result, varianceToken);
            }

            if (attrs.Count > 0)
            {
                result = AddLeadingSkippedSyntax(result, attrs.Node);
                result = this.AddError(result, ErrorCode.ERR_TypeExpected);
            }

            return result;
        }

        private bool IsEndOfTypeArgumentList()
            => this.CurrentToken.Kind == SyntaxKind.GreaterThanToken;

        private bool IsOpenName()
        {
            bool isOpen = true;
            int n = 0;
            while (this.PeekToken(n).Kind == SyntaxKind.CommaToken)
            {
                n++;
            }

            if (this.PeekToken(n).Kind != SyntaxKind.GreaterThanToken)
            {
                isOpen = false;
            }

            return isOpen;
        }

        private void ParseMemberName(
            out SyntaxToken identifierOrThisOpt,
            out TypeParameterListSyntax typeParameterListOpt,
            bool isEvent)
        {
            identifierOrThisOpt = null;
            typeParameterListOpt = null;

            if (!IsPossibleMemberName())
            {
                // No clue what this is.  Just bail.  Our caller will have to
                // move forward and try again.
                return;
            }

            NameEx explicitInterfaceName = null;
            SyntaxToken separator = null;

            ResetPoint beforeIdentifierPoint = default(ResetPoint);
            bool beforeIdentifierPointSet = false;

            try
            {
                while (true)
                {
                    // Check if we got 'this'.  If so, then we have an indexer.
                    // Note: we parse out type parameters here as well so that
                    // we can give a useful error about illegal generic indexers.
                    if (this.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                    {
                        beforeIdentifierPoint = GetResetPoint();
                        beforeIdentifierPointSet = true;
                        identifierOrThisOpt = this.EatToken();
                        typeParameterListOpt = this.ParseTypeParameterList();
                        break;
                    }

                    // now, scan past the next name.  if it's followed by a dot then
                    // it's part of the explicit name we're building up.  Otherwise,
                    // it's the name of the member.
                    var point = GetResetPoint();
                    bool isMemberName;
                    try
                    {
                        ScanNamedTypePart();
                        isMemberName = !IsDotOrColonColonOrDotDot();
                    }
                    finally
                    {
                        this.Reset(ref point);
                        this.Release(ref point);
                    }

                    if (isMemberName)
                    {
                        // We're past any explicit interface portion and We've 
                        // gotten to the member name.  
                        beforeIdentifierPoint = GetResetPoint();
                        beforeIdentifierPointSet = true;

                        if (separator != null && separator.Kind == SyntaxKind.ColonColonToken)
                        {
                            separator = this.AddError(separator, ErrorCode.ERR_AliasQualAsExpression);
                            separator = this.ConvertToMissingWithTrailingTrivia(separator, SyntaxKind.DotToken);
                        }

                        identifierOrThisOpt = this.ParseIdentifierToken();
                        typeParameterListOpt = this.ParseTypeParameterList();
                        break;
                    }
                }
            }
            finally
            {
                if (beforeIdentifierPointSet)
                {
                    Release(ref beforeIdentifierPoint);
                }
            }
        }

        private NameEx ParseAliasQualifiedName(NameOptions allowedParts = NameOptions.None)
        {
            NameEx name = this.ParseSimpleName(allowedParts);
            if (this.CurrentToken.Kind == SyntaxKind.ColonColonToken)
            {
                var token = this.EatToken();

                name = ParseQualifiedNameRight(allowedParts, name, token);
            }

            return name;
        }

        private NameEx ParseQualifiedName(NameOptions options = NameOptions.None)
        {
            NameEx name = this.ParseAliasQualifiedName(options);

            // Handle .. tokens for error recovery purposes.
            while (IsDotOrColonColonOrDotDot())
            {
                if (this.PeekToken(1).Kind == SyntaxKind.ThisKeyword)
                {
                    break;
                }

                var separator = this.EatToken();
                name = ParseQualifiedNameRight(options, name, separator);
            }

            return name;
        }

        private NameEx ParseQualifiedNameRight(
            NameOptions options,
            NameEx left,
            SyntaxToken separator)
        {
            Debug.Assert(
                separator.Kind == SyntaxKind.DotToken ||
                separator.Kind == SyntaxKind.DotDotToken ||
                separator.Kind == SyntaxKind.ColonColonToken);
            var right = this.ParseSimpleName(options);

            switch (separator.Kind)
            {
                case SyntaxKind.DotToken:
                    return _syntaxFactory.QualifiedNameEx(left, separator, right);
                case SyntaxKind.DotDotToken:
                    // Error recovery.  If we have `X..Y` break that into `X.<missing-id>.Y`
                    return _syntaxFactory.QualifiedNameEx(RecoverFromDotDot(left, ref separator), separator, right);

                case SyntaxKind.ColonColonToken:
                    if (left.Kind != SyntaxKind.IdentifierName)
                    {
                        separator = this.AddError(separator, ErrorCode.ERR_UnexpectedAliasedName, separator.ToString());
                    }

                    // If the left hand side is not an identifier name then the user has done
                    // something like Goo.Bar::Blah. We've already made an error node for the
                    // ::, so just pretend that they typed Goo.Bar.Blah and continue on.

                    var identifierLeft = left as IdentifierEx;
                    if (identifierLeft == null)
                    {
                        separator = this.ConvertToMissingWithTrailingTrivia(separator, SyntaxKind.DotToken);
                        return _syntaxFactory.QualifiedNameEx(left, separator, right);
                    }
                    else
                    {
                        throw new NotImplementedException("What is AliasQualifiedName?");

                        // if (identifierLeft.Identifier.ContextualKind == SyntaxKind.GlobalKeyword)
                        // {
                        //     identifierLeft = _syntaxFactory.IdentifierEx(ConvertToKeyword(identifierLeft.Identifier));
                        // }
                        //
                        // identifierLeft = CheckFeatureAvailability(identifierLeft, MessageID.IDS_FeatureGlobalNamespace);
                        //
                        // // If the name on the right had errors or warnings then we need to preserve
                        // // them in the tree.
                        // return WithAdditionalDiagnostics(
                        //     _syntaxFactory.AliasQualifiedName(identifierLeft, separator, right), left.GetDiagnostics());
                    }

                default:
                    throw ExceptionUtilities.Unreachable;
            }
        }

        private NameEx RecoverFromDotDot(NameEx left, ref SyntaxToken separator)
        {
            Debug.Assert(separator.Kind == SyntaxKind.DotDotToken);

            var leftDot = SyntaxFactory.Token(separator.LeadingTrivia.Node, SyntaxKind.DotToken, null);
            var missingName = this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_IdentifierExpected);
            separator = SyntaxFactory.Token(null, SyntaxKind.DotToken, separator.TrailingTrivia.Node);
            return _syntaxFactory.QualifiedNameEx(left, leftDot, missingName);
        }

        private bool IsDotOrColonColonOrDotDot()
        {
            return this.IsDotOrColonColon() || this.CurrentToken.Kind == SyntaxKind.DotDotToken;
        }


        private SyntaxToken ConvertToMissingWithTrailingTrivia(SyntaxToken token, SyntaxKind expectedKind)
        {
            var newToken = SyntaxFactory.MissingToken(expectedKind);
            newToken = AddTrailingSkippedSyntax(newToken, token);
            return newToken;
        }

        private enum ScanTypeFlags
        {
            /// <summary>
            /// Definitely not a type name.
            /// </summary>
            NotType,

            /// <summary>
            /// Definitely a type name: either a predefined type (int, string, etc.) or an array
            /// type (ending with a [] brackets), or a pointer type (ending with *s), or a function
            /// pointer type (ending with > in valid cases, or a *, ), or calling convention
            /// identifier, in invalid cases).
            /// </summary>
            MustBeType,

            /// <summary>
            /// Might be a generic (qualified) type name or a method name.
            /// </summary>
            GenericTypeOrMethod,

            /// <summary>
            /// Might be a generic (qualified) type name or an expression or a method name.
            /// </summary>
            GenericTypeOrExpression,

            /// <summary>
            /// Might be a non-generic (qualified) type name or an expression.
            /// </summary>
            NonGenericTypeOrExpression,

            /// <summary>
            /// A type name with alias prefix (Alias::Name).  Note that Alias::Name.X would not fall under this.  This
            /// only is returned for exactly Alias::Name.
            /// </summary>
            AliasQualifiedName,

            /// <summary>
            /// Nullable type (ending with ?).
            /// </summary>
            NullableType,

            /// <summary>
            /// Might be a pointer type or a multiplication.
            /// </summary>
            PointerOrMultiplication,

            /// <summary>
            /// Might be a tuple type.
            /// </summary>
            TupleType,

            /// <summary>
            /// Might be a union type
            /// </summary>
            UnionType
        }

        private bool IsPossibleType()
        {
            var tk = this.CurrentToken.Kind;
            return IsPredefinedType(tk) || this.IsTrueIdentifier();
        }

        private ScanTypeFlags ScanType(bool forPattern = false)
        {
            return ScanType(out _, forPattern);
        }

        private ScanTypeFlags ScanType(out SyntaxToken lastTokenOfType, bool forPattern = false)
        {
            return ScanType(forPattern ? ParseTypeMode.DefinitePattern : ParseTypeMode.Normal, out lastTokenOfType);
        }

        private void ScanNamedTypePart()
        {
            SyntaxToken lastTokenOfType;
            ScanNamedTypePart(out lastTokenOfType);
        }

        private ScanTypeFlags ScanNamedTypePart(out SyntaxToken lastTokenOfType)
        {
            if (this.CurrentToken.Kind != SyntaxKind.IdentifierToken || !this.IsTrueIdentifier())
            {
                lastTokenOfType = null;
                return ScanTypeFlags.NotType;
            }

            lastTokenOfType = this.EatToken();
            if (this.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                return this.ScanPossibleTypeArgumentList(ref lastTokenOfType, out _);
            }
            else
            {
                return ScanTypeFlags.NonGenericTypeOrExpression;
            }
        }

        private ScanTypeFlags ScanType(ParseTypeMode mode, out SyntaxToken lastTokenOfType)
        {
            Debug.Assert(mode != ParseTypeMode.NewExpression);
            ScanTypeFlags result;
            bool isFunctionPointer = false;

            if (this.CurrentToken.Kind == SyntaxKind.RefKeyword)
            {
                // in a ref local or ref return, we treat "ref" and "ref readonly" as part of the type
                this.EatToken();

                if (this.CurrentToken.Kind == SyntaxKind.ReadOnlyKeyword)
                {
                    this.EatToken();
                }
            }

            // Handle :: as well for error case of an alias used without a preceding identifier.
            if (this.CurrentToken.Kind is SyntaxKind.IdentifierToken or SyntaxKind.ColonColonToken)
            {
                bool isAlias;
                if (this.CurrentToken.Kind is SyntaxKind.ColonColonToken)
                {
                    result = ScanTypeFlags.NonGenericTypeOrExpression;

                    // Definitely seems like an alias if we're starting with a ::
                    isAlias = true;

                    // We set this to null to appease the flow checker.  It will always be the case that this will be
                    // set to an appropriate value inside the `for` loop below.  We'll consume the :: there and then
                    // call ScanNamedTypePart which will always set this to a valid value.
                    lastTokenOfType = null;
                }
                else
                {
                    Debug.Assert(this.CurrentToken.Kind is SyntaxKind.IdentifierToken);

                    // We're an alias if we start with an: id::
                    isAlias = this.PeekToken(1).Kind == SyntaxKind.ColonColonToken;

                    result = this.ScanNamedTypePart(out lastTokenOfType);
                    if (result == ScanTypeFlags.NotType)
                    {
                        return ScanTypeFlags.NotType;
                    }

                    Debug.Assert(result is ScanTypeFlags.GenericTypeOrExpression or ScanTypeFlags.GenericTypeOrMethod
                        or ScanTypeFlags.NonGenericTypeOrExpression);
                }

                // Scan a name
                for (bool firstLoop = true; IsDotOrColonColon(); firstLoop = false)
                {
                    // If we consume any more dots or colons, don't consider us an alias anymore.  For dots, we now have
                    // x::y.z (which is now back to a normal expr/type, not an alias), and for colons that means we have
                    // x::y::z or x.y::z both of which are effectively gibberish.
                    if (!firstLoop)
                    {
                        isAlias = false;
                    }

                    this.EatToken();
                    result = this.ScanNamedTypePart(out lastTokenOfType);
                    if (result == ScanTypeFlags.NotType)
                    {
                        return ScanTypeFlags.NotType;
                    }

                    Debug.Assert(result is ScanTypeFlags.GenericTypeOrExpression or ScanTypeFlags.GenericTypeOrMethod
                        or ScanTypeFlags.NonGenericTypeOrExpression);
                }

                if (isAlias)
                {
                    result = ScanTypeFlags.AliasQualifiedName;
                }
            }
            else if (IsPredefinedType(this.CurrentToken.Kind))
            {
                // Simple type...
                lastTokenOfType = this.EatToken();
                result = ScanTypeFlags.MustBeType;
            }
            else if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                lastTokenOfType = this.EatToken();

                if (IsPossibleUnionType())
                {
                    return this.ScanUnionType(out lastTokenOfType);
                }
                else
                {
                    result = this.ScanTupleType(out lastTokenOfType);
                    if (result == ScanTypeFlags.NotType || mode == ParseTypeMode.DefinitePattern &&
                        this.CurrentToken.Kind != SyntaxKind.OpenBracketToken)
                    {
                        // A tuple type can appear in a pattern only if it is the element type of an array type.
                        return ScanTypeFlags.NotType;
                    }
                }
            }

            else if (IsFunctionPointerStart())
            {
                isFunctionPointer = true;
                result = ScanFunctionPointerType(out lastTokenOfType);
            }
            else
            {
                // Can't be a type!
                lastTokenOfType = null;
                return ScanTypeFlags.NotType;
            }

            int lastTokenPosition = -1;
            while (IsMakingProgress(ref lastTokenPosition))
            {
                switch (this.CurrentToken.Kind)
                {
                    case SyntaxKind.QuestionToken
                        when lastTokenOfType.Kind != SyntaxKind.QuestionToken && // don't allow `Type??`
                             lastTokenOfType.Kind != SyntaxKind.AsteriskToken && // don't allow `Type*?`
                             !isFunctionPointer: // don't allow `delegate*<...>?`
                        lastTokenOfType = this.EatToken();
                        result = ScanTypeFlags.NullableType;
                        break;
                    case SyntaxKind.AsteriskToken
                        when lastTokenOfType.Kind != SyntaxKind.CloseBracketToken: // don't allow `Type[]*`
                        // Check for pointer type(s)
                        switch (mode)
                        {
                            case ParseTypeMode.FirstElementOfPossibleTupleLiteral:
                            case ParseTypeMode.AfterTupleComma:
                                // We are parsing the type for a declaration expression in a tuple, which does
                                // not permit pointer types except as an element type of an array type.
                                // In that context a `*` is parsed as a multiplication.
                                if (PointerTypeModsFollowedByRankAndDimensionSpecifier())
                                {
                                    goto default;
                                }

                                goto done;
                            case ParseTypeMode.DefinitePattern:
                                // pointer type syntax is not supported in patterns.
                                goto done;
                            default:
                                lastTokenOfType = this.EatToken();
                                isFunctionPointer = false;
                                if (result == ScanTypeFlags.GenericTypeOrExpression ||
                                    result == ScanTypeFlags.NonGenericTypeOrExpression)
                                {
                                    result = ScanTypeFlags.PointerOrMultiplication;
                                }
                                else if (result == ScanTypeFlags.GenericTypeOrMethod)
                                {
                                    result = ScanTypeFlags.MustBeType;
                                }

                                break;
                        }

                        break;
                    case SyntaxKind.OpenBracketToken:
                        // Check for array types.
                        this.EatToken();
                        while (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                        {
                            this.EatToken();
                        }

                        if (this.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
                        {
                            lastTokenOfType = null;
                            return ScanTypeFlags.NotType;
                        }

                        lastTokenOfType = this.EatToken();
                        isFunctionPointer = false;
                        result = ScanTypeFlags.MustBeType;
                        break;
                    default:
                        goto done;
                }
            }

            done:
            return result;
        }

        /// <summary>
        /// Returns TupleType when a possible tuple type is found.
        /// Note that this is not MustBeType, so that the caller can consider deconstruction syntaxes.
        /// The caller is expected to have consumed the opening paren.
        /// </summary>
        private ScanTypeFlags ScanTupleType(out SyntaxToken lastTokenOfType)
        {
            var tupleElementType = ScanType(out lastTokenOfType);
            if (tupleElementType != ScanTypeFlags.NotType)
            {
                if (IsTrueIdentifier())
                {
                    lastTokenOfType = this.EatToken();
                }

                if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    do
                    {
                        lastTokenOfType = this.EatToken();
                        tupleElementType = ScanType(out lastTokenOfType);

                        if (tupleElementType == ScanTypeFlags.NotType)
                        {
                            lastTokenOfType = this.EatToken();
                            return ScanTypeFlags.NotType;
                        }

                        if (IsTrueIdentifier())
                        {
                            lastTokenOfType = this.EatToken();
                        }
                    } while (this.CurrentToken.Kind == SyntaxKind.CommaToken);

                    if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken)
                    {
                        lastTokenOfType = this.EatToken();
                        return ScanTypeFlags.TupleType;
                    }
                }
            }

            // Can't be a type!
            lastTokenOfType = null;
            return ScanTypeFlags.NotType;
        }

        private ScanTypeFlags ScanUnionType(out SyntaxToken lastTokenOfType)
        {
            var unionElementType = ScanType(out lastTokenOfType);
            if (unionElementType != ScanTypeFlags.NotType)
            {
                if (IsTrueIdentifier())
                {
                    lastTokenOfType = this.EatToken();
                }

                if (this.CurrentToken.Kind == SyntaxKind.BarToken)
                {
                    do
                    {
                        lastTokenOfType = this.EatToken();
                        unionElementType = ScanType(out lastTokenOfType);

                        if (unionElementType == ScanTypeFlags.NotType)
                        {
                            lastTokenOfType = this.EatToken();
                            return ScanTypeFlags.NotType;
                        }

                        if (IsTrueIdentifier())
                        {
                            lastTokenOfType = this.EatToken();
                        }
                    } while (this.CurrentToken.Kind == SyntaxKind.BarToken);

                    if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken)
                    {
                        lastTokenOfType = this.EatToken();
                        return ScanTypeFlags.TupleType;
                    }
                }
            }

            // Can't be a type!
            lastTokenOfType = null;
            return ScanTypeFlags.NotType;
        }

#nullable enable
        private ScanTypeFlags ScanFunctionPointerType(out SyntaxToken lastTokenOfType)
        {
            Debug.Assert(IsFunctionPointerStart());
            _ = EatToken(SyntaxKind.DelegateKeyword);
            lastTokenOfType = EatToken(SyntaxKind.AsteriskToken);

            TerminatorState saveTerm;

            if (CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                var peek1 = PeekToken(1);
                switch (CurrentToken)
                {
                    case { ContextualKind: SyntaxKind.ManagedKeyword }:
                    case { ContextualKind: SyntaxKind.UnmanagedKeyword }:
                    case var _ when IsPossibleFunctionPointerParameterListStart(peek1):
                    case var _ when peek1.Kind == SyntaxKind.OpenBracketToken:
                        lastTokenOfType = EatToken();
                        break;

                    default:
                        // Whatever is next, it's probably not part of the type. We know that delegate* must be
                        // a function pointer start, however, so say the asterisk is the last element and bail
                        return ScanTypeFlags.MustBeType;
                }

                if (CurrentToken.Kind == SyntaxKind.OpenBracketToken)
                {
                    lastTokenOfType = EatToken(SyntaxKind.OpenBracketToken);
                    saveTerm = _termState;
                    _termState |= TerminatorState.IsEndOfFunctionPointerCallingConvention;

                    try
                    {
                        while (true)
                        {
                            lastTokenOfType = TryEatToken(SyntaxKind.IdentifierToken) ?? lastTokenOfType;

                            if (skipBadFunctionPointerTokens() == PostSkipAction.Abort)
                            {
                                break;
                            }

                            Debug.Assert(CurrentToken.Kind == SyntaxKind.CommaToken);
                            lastTokenOfType = EatToken();
                        }

                        lastTokenOfType = TryEatToken(SyntaxKind.CloseBracketToken) ?? lastTokenOfType;
                    }
                    finally
                    {
                        _termState = saveTerm;
                    }
                }
            }

            if (!IsPossibleFunctionPointerParameterListStart(CurrentToken))
            {
                // Even though this function pointer type is incomplete, we know that it
                // must be the start of a type, as there is no other possible interpretation
                // of delegate*. By always treating it as a type, we ensure that any disambiguation
                // done in later parsing treats this as a type, which will produce better
                // errors at later stages.
                return ScanTypeFlags.MustBeType;
            }

            var validStartingToken = EatToken().Kind == SyntaxKind.LessThanToken;

            saveTerm = _termState;
            _termState |= validStartingToken
                ? TerminatorState.IsEndOfFunctionPointerParameterList
                : TerminatorState.IsEndOfFunctionPointerParameterListErrored;
            var ignoredModifiers = _pool.Allocate<SyntaxToken>();

            try
            {
                do
                {
                    ParseParameterModifiers(ignoredModifiers, isFunctionPointerParameter: true);
                    ignoredModifiers.Clear();

                    _ = ScanType(out _);

                    if (skipBadFunctionPointerTokens() == PostSkipAction.Abort)
                    {
                        break;
                    }

                    _ = EatToken(SyntaxKind.CommaToken);
                } while (true);
            }
            finally
            {
                _termState = saveTerm;
                _pool.Free(ignoredModifiers);
            }

            if (!validStartingToken && CurrentToken.Kind == SyntaxKind.CloseParenToken)
            {
                lastTokenOfType = EatTokenAsKind(SyntaxKind.GreaterThanToken);
            }
            else
            {
                lastTokenOfType = EatToken(SyntaxKind.GreaterThanToken);
            }

            return ScanTypeFlags.MustBeType;

            PostSkipAction skipBadFunctionPointerTokens()
            {
                return SkipBadTokensWithExpectedKind(
                    isNotExpectedFunction: p => p.CurrentToken.Kind != SyntaxKind.CommaToken,
                    abortFunction: p => p.IsTerminator(),
                    expected: SyntaxKind.CommaToken, trailingTrivia: out _);
            }
        }
#nullable disable

        private static bool IsPredefinedType(SyntaxKind keyword)
        {
            return SyntaxFacts.IsPredefinedType(keyword);
        }

        public TypeEx ParseTypeName()
        {
            return ParseType();
        }

        private TypeEx ParseTypeOrVoid()
        {
            if (this.CurrentToken.Kind == SyntaxKind.VoidKeyword && this.PeekToken(1).Kind != SyntaxKind.AsteriskToken)
            {
                // Must be 'void' type, so create such a type node and return it.
                return _syntaxFactory.PredefinedTypeEx(this.EatToken());
            }

            return this.ParseType();
        }

        private enum ParseTypeMode
        {
            Normal,
            Parameter,
            AfterIs,
            DefinitePattern,
            AfterOut,
            AfterRef,
            AfterTupleComma,
            AsExpression,
            NewExpression,
            FirstElementOfPossibleTupleLiteral,
        }

        private TypeEx ParseType(ParseTypeMode mode = ParseTypeMode.Normal)
        {
            if (this.CurrentToken.Kind == SyntaxKind.RefKeyword)
            {
                var refKeyword = this.EatToken();
                refKeyword = this.CheckFeatureAvailability(refKeyword, MessageID.IDS_FeatureRefLocalsReturns);

                SyntaxToken readonlyKeyword = null;
                if (this.CurrentToken.Kind == SyntaxKind.ReadOnlyKeyword)
                {
                    readonlyKeyword = this.EatToken();
                    readonlyKeyword =
                        this.CheckFeatureAvailability(readonlyKeyword, MessageID.IDS_FeatureReadOnlyReferences);
                }

                var type = ParseTypeCore(ParseTypeMode.AfterRef);
                return _syntaxFactory.RefTypeEx(refKeyword, readonlyKeyword, type);
            }

            return ParseTypeCore(mode);
        }

        private TypeEx ParseTypeCore(ParseTypeMode mode)
        {
            NameOptions nameOptions;
            switch (mode)
            {
                case ParseTypeMode.AfterIs:
                    nameOptions = NameOptions.InExpression | NameOptions.AfterIs | NameOptions.PossiblePattern;
                    break;
                case ParseTypeMode.DefinitePattern:
                    nameOptions = NameOptions.InExpression | NameOptions.DefinitePattern | NameOptions.PossiblePattern;
                    break;
                case ParseTypeMode.AfterOut:
                    nameOptions = NameOptions.InExpression | NameOptions.AfterOut;
                    break;
                case ParseTypeMode.AfterTupleComma:
                    nameOptions = NameOptions.InExpression | NameOptions.AfterTupleComma;
                    break;
                case ParseTypeMode.FirstElementOfPossibleTupleLiteral:
                    nameOptions = NameOptions.InExpression | NameOptions.FirstElementOfPossibleTupleLiteral;
                    break;
                case ParseTypeMode.NewExpression:
                case ParseTypeMode.AsExpression:
                case ParseTypeMode.Normal:
                case ParseTypeMode.Parameter:
                case ParseTypeMode.AfterRef:
                    nameOptions = NameOptions.None;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(mode);
            }

            var type = this.ParseUnderlyingType(mode, options: nameOptions);
            Debug.Assert(type != null);

            int lastTokenPosition = -1;
            while (IsMakingProgress(ref lastTokenPosition))
            {
                switch (this.CurrentToken.Kind)
                {
                    case SyntaxKind.OpenBracketToken:
                        // Now check for arrays.
                    {
                        var ranks = _pool.Allocate<ArrayRankSpecifierSyntax>();
                        try
                        {
                            while (this.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
                            {
                                var rank = this.ParseArrayRankSpecifier(out _);
                                ranks.Add(rank);
                            }

                            type = _syntaxFactory.ArrayTypeEx(type, ranks);
                        }
                        finally
                        {
                            _pool.Free(ranks);
                        }

                        continue;
                    }
                    default:
                        goto done; // token not consumed
                }
            }

            done: ;

            Debug.Assert(type != null);
            return type;
        }

        private PostSkipAction SkipBadArrayRankSpecifierTokens(ref SyntaxToken openBracket,
            SeparatedSyntaxListBuilder<ExprSyntax> list, SyntaxKind expected)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref openBracket, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleExpression(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseBracketToken || p.IsTerminator(),
                expected);
        }

        private bool IsPossibleUnionType()
        {
            return (IsPredefinedType(CurrentToken.Kind) || CurrentToken.Kind == SyntaxKind.IdentifierEx)
                   && PeekToken(1).Kind == SyntaxKind.BarToken;
        }

        private TypeEx ParseUnionType(ParseTypeMode mode, NameOptions options = NameOptions.None)
        {
            var types = _pool.AllocateSeparated<TypeEx>();

            var openParen = EatToken(SyntaxKind.OpenParenToken);

            while (CurrentToken.Kind != SyntaxKind.CloseParenToken)
            {
                types.Add(ParseType());

                if (CurrentToken.Kind == SyntaxKind.BarToken)
                {
                    types.AddSeparator(this.EatToken(SyntaxKind.BarToken));
                }
                else
                {
                    break;
                }
            }

            var closeParen = EatToken(SyntaxKind.CloseParenToken);

            return _syntaxFactory.UnionTypeEx(openParen, types, closeParen);
        }

        private TypeEx ParseUnderlyingType(ParseTypeMode mode, NameOptions options = NameOptions.None)
        {
            if (IsPredefinedType(this.CurrentToken.Kind))
            {
                // This is a predefined type
                var token = this.EatToken();
                if (token.Kind == SyntaxKind.VoidKeyword && this.CurrentToken.Kind != SyntaxKind.AsteriskToken)
                {
                    token = this.AddError(token,
                        mode == ParseTypeMode.Parameter ? ErrorCode.ERR_NoVoidParameter : ErrorCode.ERR_NoVoidHere);
                }

                return _syntaxFactory.PredefinedTypeEx(token);
            }

            // The :: case is for error recovery.
            if (IsTrueIdentifier() || this.CurrentToken.Kind == SyntaxKind.ColonColonToken)
            {
                return this.ParseQualifiedName(options);
            }

            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                return ParseUnionType(mode, options | NameOptions.InUnionType);
            }

            return this.AddError(
                this.CreateMissingIdentifierName(),
                mode == ParseTypeMode.NewExpression ? ErrorCode.ERR_BadNewExpr : ErrorCode.ERR_TypeExpected);
        }

        private SyntaxToken EatNullableQualifierIfApplicable(ParseTypeMode mode)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.QuestionToken);
            var resetPoint = this.GetResetPoint();
            try
            {
                var questionToken = this.EatToken();
                if (!canFollowNullableType())
                {
                    // Restore current token index
                    this.Reset(ref resetPoint);
                    return null;
                }

                return CheckFeatureAvailability(questionToken, MessageID.IDS_FeatureNullable);

                bool canFollowNullableType()
                {
                    switch (mode)
                    {
                        case ParseTypeMode.AfterIs:
                        case ParseTypeMode.DefinitePattern:
                        case ParseTypeMode.AsExpression:
                            // These contexts might be a type that is at the end of an expression.
                            // In these contexts we only permit the nullable qualifier if it is followed
                            // by a token that could not start an expression, because for backward
                            // compatibility we want to consider a `?` token as part of the `?:`
                            // operator if possible.
                            return !CanStartExpression();
                        case ParseTypeMode.NewExpression:
                            // A nullable qualifier is permitted as part of the type in a `new` expression.
                            // e.g. `new int?()` is allowed.  It creates a null value of type `Nullable<int>`.
                            // Similarly `new int? {}` is allowed.
                            return
                                this.CurrentToken.Kind == SyntaxKind.OpenParenToken || // ctor parameters
                                this.CurrentToken.Kind == SyntaxKind.OpenBracketToken || // array type
                                this.CurrentToken.Kind == SyntaxKind.OpenBraceToken; // object initializer
                        default:
                            return true;
                    }
                }
            }
            finally
            {
                this.Release(ref resetPoint);
            }
        }

        private bool PointerTypeModsFollowedByRankAndDimensionSpecifier()
        {
            // Are pointer specifiers (if any) followed by an array specifier?
            for (int i = 0;; i++)
            {
                switch (this.PeekToken(i).Kind)
                {
                    case SyntaxKind.AsteriskToken:
                        continue;
                    case SyntaxKind.OpenBracketToken:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private ArrayRankSpecifierSyntax ParseArrayRankSpecifier(out bool sawNonOmittedSize)
        {
            sawNonOmittedSize = false;
            bool sawOmittedSize = false;
            var open = this.EatToken(SyntaxKind.OpenBracketToken);
            var list = _pool.AllocateSeparated<ExprSyntax>();
            try
            {
                var omittedArraySizeExpressionInstance =
                    _syntaxFactory.OmittedArraySizeEx(SyntaxFactory.Token(SyntaxKind.OmittedArraySizeExpressionToken));
                int lastTokenPosition = -1;
                while (IsMakingProgress(ref lastTokenPosition) &&
                       this.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
                {
                    if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        // NOTE: trivia will be attached to comma, not omitted array size
                        sawOmittedSize = true;
                        list.Add(omittedArraySizeExpressionInstance);
                        list.AddSeparator(this.EatToken());
                    }
                    else if (this.IsPossibleExpression())
                    {
                        var size = this.ParseExpressionCore();
                        sawNonOmittedSize = true;
                        list.Add(size);

                        if (this.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
                        {
                            list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                        }
                    }
                    else if (this.SkipBadArrayRankSpecifierTokens(ref open, list, SyntaxKind.CommaToken) ==
                             PostSkipAction.Abort)
                    {
                        break;
                    }
                }

                // Don't end on a comma.
                // If the omitted size would be the only element, then skip it unless sizes were expected.
                if (((list.Count & 1) == 0))
                {
                    sawOmittedSize = true;
                    list.Add(omittedArraySizeExpressionInstance);
                }

                // Never mix omitted and non-omitted array sizes.  If there were non-omitted array sizes,
                // then convert all of the omitted array sizes to missing identifiers.
                if (sawOmittedSize && sawNonOmittedSize)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].RawKind == (int)SyntaxKind.OmittedArraySizeExpression)
                        {
                            int width = list[i].Width;
                            int offset = list[i].GetLeadingTriviaWidth();
                            list[i] = this.AddError(this.CreateMissingIdentifierName(), offset, width,
                                ErrorCode.ERR_ValueExpected);
                        }
                    }
                }

                // Eat the close brace and we're done.
                var close = this.EatToken(SyntaxKind.CloseBracketToken);
                return _syntaxFactory.ArrayRankSpecifier(open, list, close);
            }
            finally
            {
                _pool.Free(list);
            }
        }

        private bool IsFunctionPointerStart()
            => CurrentToken.Kind == SyntaxKind.DelegateKeyword && PeekToken(1).Kind == SyntaxKind.AsteriskToken;

        private static bool IsPossibleFunctionPointerParameterListStart(SyntaxToken token)
            // We consider both ( and < to be possible starts, in order to make error recovery more graceful
            // in the scenario where a user accidentally surrounds their function pointer type list with parens.
            => token.Kind == SyntaxKind.LessThanToken || token.Kind == SyntaxKind.OpenParenToken;
#nullable disable


        private StmtSyntax ParsePossiblyAttributedStatement()
            => ParseStatementCore(ParseAttributeDeclarations(), isGlobal: false);

        /// <param name="isGlobal">If we're being called while parsing a C# top-level statements (Script or Simple Program).
        /// At the top level in Script, we allow most statements *except* for local-decls/local-funcs.
        /// Those will instead be parsed out as script-fields/methods.</param>
        private StmtSyntax ParseStatementCore(SyntaxList<AttributeListSyntax> attributes, bool isGlobal)
        {
            if (canReuseStatement(attributes, isGlobal))
            {
                return (StmtSyntax)this.EatNode();
            }

            ResetPoint resetPointBeforeStatement = this.GetResetPoint();
            try
            {
                _recursionDepth++;
                StackGuard.EnsureSufficientExecutionStack(_recursionDepth);

                StmtSyntax result;

                // Main switch to handle processing almost any statement.
                switch (this.CurrentToken.Kind)
                {
                    // case SyntaxKind.FixedKeyword:
                    //     return this.ParseFixedStatement(attributes);
                    case SyntaxKind.BreakKeyword:
                        return this.ParseBreakStatement(attributes);
                    case SyntaxKind.ContinueKeyword:
                        return this.ParseContinueStatement(attributes);
                    case SyntaxKind.TryKeyword:
                    case SyntaxKind.CatchKeyword:
                    case SyntaxKind.FinallyKeyword:
                        return this.ParseTryStatement(attributes);
                    // case SyntaxKind.CheckedKeyword:
                    // case SyntaxKind.UncheckedKeyword:
                    //     return this.ParseCheckedStatement(attributes);
                    // case SyntaxKind.DoKeyword:
                    //     return this.ParseDoStatement(attributes);
                    case SyntaxKind.ForKeyword:
                        return this.ParseForOrForEachStatement(attributes);
                    // case SyntaxKind.ForEachKeyword:
                    //     return this.ParseForEachStatement(attributes, awaitTokenOpt: null);
                    // case SyntaxKind.GotoKeyword:
                    //     return this.ParseGotoStatement(attributes);
                    case SyntaxKind.IfKeyword:
                        return this.ParseIfStatement(attributes);
                    case SyntaxKind.ElseKeyword:
                        // Including 'else' keyword to handle 'else without if' error cases 
                        return this.ParseMisplacedElse(attributes);
                    // case SyntaxKind.LockKeyword:
                    //     return this.ParseLockStatement(attributes);
                    case SyntaxKind.ReturnKeyword:
                        return this.ParseReturnStatement(attributes);
                    // case SyntaxKind.SwitchKeyword:
                    //     return this.ParseSwitchStatement(attributes);
                    // case SyntaxKind.ThrowKeyword:
                    //     return this.ParseThrowStatement(attributes);
                    // case SyntaxKind.UnsafeKeyword:
                    //     result = TryParseStatementStartingWithUnsafe(attributes);
                    //     if (result != null)
                    //         return result;
                    //     break;
                    // case SyntaxKind.UsingKeyword:
                    //     return ParseStatementStartingWithUsing(attributes);
                    // case SyntaxKind.WhileKeyword:
                    //     return this.ParseWhileStatement(attributes);
                    case SyntaxKind.OpenBraceToken:
                        return this.ParseBlock(attributes);
                    case SyntaxKind.SemicolonToken:
                        return _syntaxFactory.EmptyStmt(this.EatToken());
                    case SyntaxKind.IdentifierToken:
                        result = TryParseStatementStartingWithIdentifier(attributes, isGlobal);
                        if (result != null)
                            return result;
                        break;
                    // default:
                    //     throw new Exception();
                }


                return ParseStatementCoreRest(attributes, isGlobal, ref resetPointBeforeStatement);
            }
            finally
            {
                _recursionDepth--;
                this.Release(ref resetPointBeforeStatement);
            }

            bool canReuseStatement(SyntaxList<AttributeListSyntax> attributes, bool isGlobal)
            {
                return this.IsIncrementalAndFactoryContextMatches &&
                       this.CurrentNode is Syntax.StmtSyntax &&
                       !isGlobal && // Top-level statements are reused by ParseMemberDeclarationOrStatementCore when possible.
                       attributes.Count == 0;
            }
        }


        private StmtSyntax ParseStatementCoreRest(SyntaxList<AttributeListSyntax> attributes, bool isGlobal,
            ref ResetPoint resetPointBeforeStatement)
        {
            isGlobal = isGlobal && IsScript;

            if (!this.IsPossibleLocalDeclarationStatement(isGlobal))
            {
                return this.ParseExpressionStatement(attributes);
            }

            if (isGlobal)
            {
                // if we're at the global script level, then we don't support local-decls or
                // local-funcs. The caller instead will look for those and parse them as
                // fields/methods in the global script scope.
                return null;
            }

            bool beginsWithAwait = this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword;
            var result = ParseLocalDeclarationStatement(attributes);

            // didn't get any sort of statement.  This was something else entirely
            // (like just a `}`).  No need to retry anything here.  Just reset back
            // to where we started from and bail entirely from parsing a statement.
            if (result == null)
            {
                this.Reset(ref resetPointBeforeStatement);
                return null;
            }

            if (result.ContainsDiagnostics &&
                beginsWithAwait &&
                !IsInAsync)
            {
                // Local decl had issues.  We were also starting with 'await' in a non-async
                // context. Retry parsing this as if we were in an 'async' context as it's much
                // more likely that this was a misplace await-expr' than a local decl.
                //
                // The user will still get a later binding error about an await-expr in a non-async
                // context.
                this.Reset(ref resetPointBeforeStatement);

                IsInAsync = true;
                result = ParseExpressionStatement(attributes);
                IsInAsync = false;
            }

            // Didn't want to retry as an `await expr`.  Just return what we actually
            // produced.
            return result;
        }

        private StmtSyntax TryParseStatementStartingWithIdentifier(SyntaxList<AttributeListSyntax> attributes,
            bool isGlobal)
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword &&
                this.PeekToken(1).Kind == SyntaxKind.ForEachKeyword)
            {
                //  return this.ParseForEachStatement(attributes, ParseAwaitKeyword(MessageID.IDS_FeatureAsyncStreams));
            }
            else if (IsPossibleAwaitUsing())
            {
                // if (PeekToken(2).Kind == SyntaxKind.OpenParenToken)
                // {
                //     // `await using Type ...` is handled below in ParseLocalDeclarationStatement
                //     return this.ParseUsingStatement(attributes, ParseAwaitKeyword(MessageID.IDS_FeatureAsyncUsing));
                // }
            }
            // else if (this.IsPossibleLabeledStatement())
            // {
            //     return this.ParseLabeledStatement(attributes);
            // }
            // else if (this.IsPossibleYieldStatement())
            // {
            //     return this.ParseYieldStatement(attributes);
            // }
            else if (this.IsPossibleAwaitExpressionStatement())
            {
                return this.ParseExpressionStatement(attributes);
            }
            // else if (this.IsQueryExpression(mayBeVariableDeclaration: true, mayBeMemberDeclaration: isGlobal && IsScript))
            // {
            //     return this.ParseExpressionStatement(attributes, this.ParseQueryExpression(0));
            // }

            return null;
        }

        private SyntaxToken ParseAwaitKeyword(MessageID feature)
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword);
            SyntaxToken awaitToken = this.EatContextualToken(SyntaxKind.AwaitKeyword);
            return feature != MessageID.None ? CheckFeatureAvailability(awaitToken, feature) : awaitToken;
        }

        private bool IsPossibleAwaitUsing()
            => CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword && PeekToken(1).Kind == SyntaxKind.UsingKeyword;

        private bool IsPossibleLocalDeclarationStatement(bool isGlobalScriptLevel)
        {
            // This method decides whether to parse a statement as a
            // declaration or as an expression statement. In the old
            // compiler it would simply call IsLocalDeclaration.

            var tk = this.CurrentToken.Kind;
            if (tk == SyntaxKind.RefKeyword ||
                IsDeclarationModifier(tk) || // treat `static int x = 2;` as a local variable declaration
                (SyntaxFacts.IsPredefinedType(tk) &&
                 this.PeekToken(1).Kind != SyntaxKind.DotToken && // e.g. `int.Parse()` is an expression
                 this.PeekToken(1).Kind != SyntaxKind.OpenParenToken)) // e.g. `int (x, y)` is an error decl expression
            {
                return true;
            }

            // note: `using (` and `await using (` are already handled in ParseStatementCore.
            if (tk == SyntaxKind.UsingKeyword)
            {
                Debug.Assert(PeekToken(1).Kind != SyntaxKind.OpenParenToken);
                return true;
            }

            if (IsPossibleAwaitUsing())
            {
                Debug.Assert(PeekToken(2).Kind != SyntaxKind.OpenParenToken);
                return true;
            }

            tk = this.CurrentToken.ContextualKind;

            var isPossibleAttributeOrModifier =
                (IsAdditionalLocalFunctionModifier(tk) || tk == SyntaxKind.OpenBracketToken)
                && (tk != SyntaxKind.AsyncKeyword ||
                    ShouldAsyncBeTreatedAsModifier(parsingStatementNotDeclaration: true));
            if (isPossibleAttributeOrModifier)
            {
                return true;
            }

            return IsPossibleFirstTypedIdentifierInLocaDeclarationStatement(isGlobalScriptLevel);
        }

        private bool IsPossibleFirstTypedIdentifierInLocaDeclarationStatement(bool isGlobalScriptLevel)
        {
            bool? typedIdentifier =
                IsPossibleTypedIdentifierStart(this.CurrentToken, this.PeekToken(1), allowThisKeyword: false);
            if (typedIdentifier != null)
            {
                return typedIdentifier.Value;
            }

            // It's common to have code like the following:
            // 
            //      Task.
            //      await Task.Delay()
            //
            // In this case we don't want to parse this as a local declaration like:
            //
            //      Task.await Task
            //
            // This does not represent user intent, and it causes all sorts of problems to higher 
            // layers.  This is because both the parse tree is strange, and the symbol tables have
            // entries that throw things off (like a bogus 'Task' local).
            //
            // Note that we explicitly do this check when we see that the code spreads over multiple 
            // lines.  We don't want this if the user has actually written "X.Y z"
            var tk = this.CurrentToken.ContextualKind;

            if (tk == SyntaxKind.IdentifierToken)
            {
                var token1 = PeekToken(1);
                if (token1.Kind == SyntaxKind.DotToken &&
                    token1.TrailingTrivia.Any((int)SyntaxKind.EndOfLineTrivia))
                {
                    if (PeekToken(2).Kind == SyntaxKind.IdentifierToken &&
                        PeekToken(3).Kind == SyntaxKind.IdentifierToken)
                    {
                        // We have something like:
                        //
                        //      X.
                        //      Y z
                        //
                        // This is only a local declaration if we have:
                        //
                        //      X.Y z;
                        //      X.Y z = ...
                        //      X.Y z, ...  
                        //      X.Y z( ...      (local function) 
                        //      X.Y z<W...      (local function)
                        //
                        var token4Kind = PeekToken(4).Kind;
                        if (token4Kind != SyntaxKind.SemicolonToken &&
                            token4Kind != SyntaxKind.EqualsToken &&
                            token4Kind != SyntaxKind.CommaToken &&
                            token4Kind != SyntaxKind.OpenParenToken &&
                            token4Kind != SyntaxKind.LessThanToken)
                        {
                            return false;
                        }
                    }
                }
            }

            var resetPoint = this.GetResetPoint();
            try
            {
                ScanTypeFlags st = this.ScanType();

                // We could always return true for st == AliasQualName in addition to MustBeType on the first line, however, we want it to return false in the case where
                // CurrentToken.Kind != SyntaxKind.Identifier so that error cases, like: A::N(), are not parsed as variable declarations and instead are parsed as A.N() where we can give
                // a better error message saying "did you meant to use a '.'?"
                if (st == ScanTypeFlags.MustBeType && this.CurrentToken.Kind != SyntaxKind.DotToken &&
                    this.CurrentToken.Kind != SyntaxKind.OpenParenToken)
                {
                    return true;
                }

                if (st == ScanTypeFlags.NotType || this.CurrentToken.Kind != SyntaxKind.IdentifierToken)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }
        }

        private bool IsPossibleNewExpression()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.NewKeyword);

            // skip new
            SyntaxToken nextToken = PeekToken(1);

            // new { }
            // new [ ]
            switch (nextToken.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.OpenBracketToken:
                    return true;
            }

            //
            // Declaration with new modifier vs. new expression
            // Parse it as an expression if the type is not followed by an identifier or this keyword.
            //
            // Member declarations:
            //   new T Idf ...
            //   new T this ...
            //   new partial Idf    ("partial" as a type name)
            //   new partial this   ("partial" as a type name)
            //   new partial T Idf
            //   new partial T this
            //   new <modifier>
            //   new <class|interface|struct|enum>
            //   new partial <class|interface|struct|enum>
            //
            // New expressions:
            //   new T []
            //   new T { }
            //   new <non-type>
            //
            if (SyntaxFacts.GetBaseTypeDeclarationKind(nextToken.Kind) != SyntaxKind.None)
            {
                return false;
            }

            DeclarationModifiers modifier = GetModifier(nextToken);
            if (modifier == DeclarationModifiers.Partial)
            {
                if (SyntaxFacts.IsPredefinedType(PeekToken(2).Kind))
                {
                    return false;
                }

                // class, struct, enum, interface keywords, but also other modifiers that are not allowed after 
                // partial keyword but start class declaration, so we can assume the user just swapped them.
                if (IsPossibleStartOfTypeDeclaration(PeekToken(2).Kind))
                {
                    return false;
                }
            }
            else if (modifier != DeclarationModifiers.None)
            {
                return false;
            }

            bool? typedIdentifier = IsPossibleTypedIdentifierStart(nextToken, PeekToken(2), allowThisKeyword: true);
            if (typedIdentifier != null)
            {
                // new Idf Idf
                // new Idf .
                // new partial T
                // new partial .
                return !typedIdentifier.Value;
            }

            var resetPoint = this.GetResetPoint();
            try
            {
                // skips new keyword
                EatToken();

                ScanTypeFlags st = this.ScanType();

                return !IsPossibleMemberName() || st == ScanTypeFlags.NotType;
            }
            finally
            {
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }
        }

        /// <returns>
        /// true if the current token can be the first token of a typed identifier (a type name followed by an identifier),
        /// false if it definitely can't be,
        /// null if we need to scan further to find out.
        /// </returns>
        private bool? IsPossibleTypedIdentifierStart(SyntaxToken current, SyntaxToken next, bool allowThisKeyword)
        {
            if (IsTrueIdentifier(current))
            {
                switch (next.Kind)
                {
                    // tokens that can be in type names...
                    case SyntaxKind.DotToken:
                    case SyntaxKind.AsteriskToken:
                    case SyntaxKind.QuestionToken:
                    case SyntaxKind.OpenBracketToken:
                    case SyntaxKind.LessThanToken:
                    case SyntaxKind.ColonColonToken:
                        return null;

                    case SyntaxKind.OpenParenToken:
                        if (current.IsIdentifierVar())
                        {
                            // potentially either a tuple type in a local declaration (true), or
                            // a tuple lvalue in a deconstruction assignment (false).
                            return null;
                        }
                        else
                        {
                            return false;
                        }

                    case SyntaxKind.IdentifierToken:
                        return IsTrueIdentifier(next);

                    case SyntaxKind.ThisKeyword:
                        return allowThisKeyword;

                    default:
                        return false;
                }
            }

            return null;
        }


        // <summary>
        /// Used to parse the block-body for a method or accessor.  For blocks that appear *inside*
        /// method bodies, call <see cref="ParseBlock"/>.
        /// </summary>
        /// <param name="isAccessorBody">If is true, then we produce a special diagnostic if the
        /// open brace is missing.</param>
        private BlockStmt ParseMethodOrAccessorBodyBlock(SyntaxList<AttributeListSyntax> attributes,
            bool isAccessorBody)
        {
            // Check again for incremental re-use.  This way if a method signature is edited we can
            // still quickly re-sync on the body.
            if (this.IsIncrementalAndFactoryContextMatches &&
                this.CurrentNodeKind == SyntaxKind.Block &&
                attributes.Count == 0)
                return (BlockStmt)this.EatNode();

            // There's a special error code for a missing token after an accessor keyword
            AquilaSyntaxNode openBrace = isAccessorBody && this.CurrentToken.Kind != SyntaxKind.OpenBraceToken
                ? this.AddError(
                    SyntaxFactory.MissingToken(SyntaxKind.OpenBraceToken),
                    IsFeatureEnabled(MessageID.IDS_FeatureExpressionBodiedAccessor)
                        ? ErrorCode.ERR_SemiOrLBraceOrArrowExpected
                        : ErrorCode.ERR_SemiOrLBraceExpected)
                : this.EatToken(SyntaxKind.OpenBraceToken);

            var statements = _pool.Allocate<StmtSyntax>();
            this.ParseStatements(ref openBrace, statements, stopOnSwitchSections: false);

            var block = _syntaxFactory.BlockStmt(
                (SyntaxToken)openBrace,
                // Force creation a many-children list, even if only 1, 2, or 3 elements in the statement list.
                IsLargeEnoughNonEmptyStatementList(statements)
                    ? new SyntaxList<StmtSyntax>(SyntaxList.List(((SyntaxListBuilder)statements).ToArray()))
                    : statements,
                this.EatToken(SyntaxKind.CloseBraceToken));

            _pool.Free(statements);
            return block;
        }

        /// <summary>
        /// Used to parse normal blocks that appear inside method bodies.  For the top level block
        /// of a method/accessor use <see cref="ParseMethodOrAccessorBodyBlock"/>.
        /// </summary>
        private BlockStmt ParseBlock(SyntaxList<AttributeListSyntax> attributes)
        {
            // Check again for incremental re-use, since ParseBlock is called from a bunch of places
            // other than ParseStatementCore()
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.Block)
                return (BlockStmt)this.EatNode();

            AquilaSyntaxNode openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

            var statements = _pool.Allocate<StmtSyntax>();
            this.ParseStatements(ref openBrace, statements, stopOnSwitchSections: false);

            var block = _syntaxFactory.BlockStmt(
                (SyntaxToken)openBrace,
                statements,
                this.EatToken(SyntaxKind.CloseBraceToken));

            _pool.Free(statements);
            return block;
        }

        // Is this statement list non-empty, and large enough to make using weak children beneficial?
        private static bool IsLargeEnoughNonEmptyStatementList(SyntaxListBuilder<StmtSyntax> statements)
        {
            if (statements.Count == 0)
            {
                return false;
            }
            else if (statements.Count == 1)
            {
                // If we have a single statement, it might be small, like "return null", or large,
                // like a loop or if or switch with many statements inside. Use the width as a proxy for
                // how big it is. If it's small, its better to forgo a many children list anyway, since the
                // weak reference would consume as much memory as is saved.
                return statements[0].Width > 60;
            }
            else
            {
                // For 2 or more statements, go ahead and create a many-children lists.
                return true;
            }
        }

        private void ParseStatements(ref AquilaSyntaxNode previousNode, SyntaxListBuilder<StmtSyntax> statements,
            bool stopOnSwitchSections)
        {
            var saveTerm = _termState;
            _termState |=
                TerminatorState
                    .IsPossibleStatementStartOrStop; // partial statements can abort if a new statement starts
            if (stopOnSwitchSections)
            {
                _termState |= TerminatorState.IsSwitchSectionStart;
            }

            int lastTokenPosition = -1;
            while (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken
                   && this.CurrentToken.Kind != SyntaxKind.EndOfFileToken
                   && !(stopOnSwitchSections && this.IsPossibleSwitchSection())
                   && IsMakingProgress(ref lastTokenPosition))
            {
                if (this.IsPossibleStatement(acceptAccessibilityMods: true))
                {
                    var statement = this.ParsePossiblyAttributedStatement();
                    if (statement != null)
                    {
                        statements.Add(statement);
                        continue;
                    }
                }

                GreenNode trailingTrivia;
                var action =
                    this.SkipBadStatementListTokens(statements, SyntaxKind.CloseBraceToken, out trailingTrivia);
                if (trailingTrivia != null)
                {
                    previousNode = AddTrailingSkippedSyntax(previousNode, trailingTrivia);
                }

                if (action == PostSkipAction.Abort)
                {
                    break;
                }
            }

            _termState = saveTerm;
        }

        private bool IsPossibleStatementStartOrStop()
        {
            return this.CurrentToken.Kind == SyntaxKind.SemicolonToken
                   || this.IsPossibleStatement(acceptAccessibilityMods: true);
        }

        private PostSkipAction SkipBadStatementListTokens(SyntaxListBuilder<StmtSyntax> statements,
            SyntaxKind expected, out GreenNode trailingTrivia)
        {
            return this.SkipBadListTokensWithExpectedKindHelper(
                statements,
                // We know we have a bad statement, so it can't be a local
                // function, meaning we shouldn't consider accessibility
                // modifiers to be the start of a statement
                p => !p.IsPossibleStatement(acceptAccessibilityMods: false),
                p => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(),
                expected,
                out trailingTrivia
            );
        }

        private bool IsPossibleStatement(bool acceptAccessibilityMods)
        {
            var tk = this.CurrentToken.Kind;
            switch (tk)
            {
                case SyntaxKind.FixedKeyword:
                case SyntaxKind.BreakKeyword:
                case SyntaxKind.ContinueKeyword:
                case SyntaxKind.TryKeyword:
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.DoKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.ForEachKeyword:
                case SyntaxKind.GotoKeyword:
                case SyntaxKind.IfKeyword:
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.LockKeyword:
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.ThrowKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.WhileKeyword:
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.VolatileKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.OpenBracketToken:
                    return true;

                case SyntaxKind.IdentifierToken:
                    return IsTrueIdentifier();

                case SyntaxKind.CatchKeyword:
                case SyntaxKind.FinallyKeyword:
                    return !_isInTry;

                // Accessibility modifiers are not legal in a statement,
                // but a common mistake for local functions. Parse to give a
                // better error message.
                case SyntaxKind.PubKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return acceptAccessibilityMods;
                default:
                    return IsPredefinedType(tk)
                           || IsPossibleExpression();
            }
        }


        private bool IsEndOfFixedStatement()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                   || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                   || this.CurrentToken.Kind == SyntaxKind.SemicolonToken;
        }

        private StmtSyntax ParseEmbeddedStatement()
        {
            // ParseEmbeddedStatement is called through many recursive statement parsing cases. We
            // keep the body exceptionally simple, and we optimize for the common case, to ensure it
            // is inlined into the callers.  Otherwise the overhead of this single method can have a
            // deep impact on the number of recursive calls we can make (more than a hundred during
            // empirical testing).

            return parseEmbeddedStatementRest(this.ParsePossiblyAttributedStatement());

            StmtSyntax parseEmbeddedStatementRest(StmtSyntax statement)
            {
                if (statement == null)
                {
                    // The consumers of embedded statements are expecting to receive a non-null statement 
                    // yet there are several error conditions that can lead ParseStatementCore to return 
                    // null.  When that occurs create an error empty Statement and return it to the caller.
                    return SyntaxFactory.EmptyStmt(EatToken(SyntaxKind.SemicolonToken));
                }

                // In scripts, stand-alone expression statements may not be followed by semicolons.
                // ParseExpressionStatement hides the error.
                // However, embedded expression statements are required to be followed by semicolon. 
                if (statement.Kind == SyntaxKind.ExpressionStatement &&
                    IsScript)
                {
                    var expressionStatementSyntax = (ExpressionStmt)statement;
                    var semicolonToken = expressionStatementSyntax.SemicolonToken;

                    // Do not add a new error if the same error was already added.
                    if (semicolonToken.IsMissing &&
                        !semicolonToken.GetDiagnostics().Contains(diagnosticInfo =>
                            (ErrorCode)diagnosticInfo.Code == ErrorCode.ERR_SemicolonExpected))
                    {
                        semicolonToken = this.AddError(semicolonToken, ErrorCode.ERR_SemicolonExpected);
                        return expressionStatementSyntax.Update(expressionStatementSyntax.Expression, semicolonToken);
                    }
                }

                return statement;
            }
        }

        private BreakStmt ParseBreakStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            var breakKeyword = this.EatToken(SyntaxKind.BreakKeyword);
            var semicolon = this.EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.BreakStmt(breakKeyword, semicolon);
        }

        private ContinueStmt ParseContinueStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            var continueKeyword = this.EatToken(SyntaxKind.ContinueKeyword);
            var semicolon = this.EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.ContinueStmt(continueKeyword, semicolon);
        }

        private TryStmt ParseTryStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            var isInTry = _isInTry;
            _isInTry = true;

            var @try = this.EatToken(SyntaxKind.TryKeyword);

            BlockStmt block;
            if (@try.IsMissing)
            {
                block = _syntaxFactory.BlockStmt(this.EatToken(SyntaxKind.OpenBraceToken),
                    default(SyntaxList<StmtSyntax>), this.EatToken(SyntaxKind.CloseBraceToken));
            }
            else
            {
                var saveTerm = _termState;
                _termState |= TerminatorState.IsEndOfTryBlock;
                block = this.ParseBlock(new SyntaxList<AttributeListSyntax>());
                _termState = saveTerm;
            }

            var catches = default(SyntaxListBuilder<CatchClauseSyntax>);
            FinallyClauseSyntax @finally = null;
            try
            {
                bool hasEnd = false;

                if (this.CurrentToken.Kind == SyntaxKind.CatchKeyword)
                {
                    hasEnd = true;
                    catches = _pool.Allocate<CatchClauseSyntax>();
                    while (this.CurrentToken.Kind == SyntaxKind.CatchKeyword)
                    {
                        catches.Add(this.ParseCatchClause());
                    }
                }

                if (this.CurrentToken.Kind == SyntaxKind.FinallyKeyword)
                {
                    hasEnd = true;
                    var fin = this.EatToken();
                    var finBlock = this.ParseBlock(new SyntaxList<AttributeListSyntax>());
                    @finally = _syntaxFactory.FinallyClause(fin, finBlock);
                }

                if (!hasEnd)
                {
                    block = this.AddErrorToLastToken(block, ErrorCode.ERR_ExpectedEndTry);

                    // synthesize missing tokens for "finally { }":
                    @finally = _syntaxFactory.FinallyClause(
                        SyntaxToken.CreateMissing(SyntaxKind.FinallyKeyword, null, null),
                        _syntaxFactory.BlockStmt(
                            SyntaxToken.CreateMissing(SyntaxKind.OpenBraceToken, null, null),
                            default(SyntaxList<StmtSyntax>),
                            SyntaxToken.CreateMissing(SyntaxKind.CloseBraceToken, null, null)));
                }

                _isInTry = isInTry;

                return _syntaxFactory.TryStmt(@try, block, catches, @finally);
            }
            finally
            {
                if (!catches.IsNull)
                {
                    _pool.Free(catches);
                }
            }
        }

        private bool IsEndOfTryBlock()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                   || this.CurrentToken.Kind == SyntaxKind.CatchKeyword
                   || this.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
        }


        private CatchClauseSyntax ParseCatchClause()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.CatchKeyword);

            var @catch = this.EatToken();

            CatchDeclarationSyntax decl = null;
            var saveTerm = _termState;

            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                var openParen = this.EatToken();

                _termState |= TerminatorState.IsEndOfCatchClause;
                var type = this.ParseType();
                SyntaxToken name = null;
                if (this.IsTrueIdentifier())
                {
                    name = this.ParseIdentifierToken();
                }

                _termState = saveTerm;

                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                decl = _syntaxFactory.CatchDeclaration(openParen, type, name, closeParen);
            }

            CatchFilterClauseSyntax filter = null;

            var keywordKind = this.CurrentToken.ContextualKind;
            if (keywordKind == SyntaxKind.WhenKeyword || keywordKind == SyntaxKind.IfKeyword)
            {
                var whenKeyword = this.EatContextualToken(SyntaxKind.WhenKeyword);
                if (keywordKind == SyntaxKind.IfKeyword)
                {
                    // The initial design of C# exception filters called for the use of the
                    // "if" keyword in this position.  We've since changed to "when", but 
                    // the error recovery experience for early adopters (and for old source
                    // stored in the symbol server) will be better if we consume "if" as
                    // though it were "when".
                    whenKeyword = AddTrailingSkippedSyntax(whenKeyword, EatToken());
                }

                whenKeyword = CheckFeatureAvailability(whenKeyword, MessageID.IDS_FeatureExceptionFilter);
                _termState |= TerminatorState.IsEndOfFilterClause;
                var openParen = this.EatToken(SyntaxKind.OpenParenToken);
                var filterExpression = this.ParseExpressionCore();

                _termState = saveTerm;
                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                filter = _syntaxFactory.CatchFilterClause(whenKeyword, openParen, filterExpression, closeParen);
            }

            _termState |= TerminatorState.IsEndOfCatchBlock;
            var block = this.ParseBlock(new SyntaxList<AttributeListSyntax>());
            _termState = saveTerm;

            return _syntaxFactory.CatchClause(@catch, decl, filter, block);
        }

        private bool IsEndOfCatchClause()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                   || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                   || this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                   || this.CurrentToken.Kind == SyntaxKind.CatchKeyword
                   || this.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
        }

        private bool IsEndOfFilterClause()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                   || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                   || this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                   || this.CurrentToken.Kind == SyntaxKind.CatchKeyword
                   || this.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
        }

        private bool IsEndOfCatchBlock()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                   || this.CurrentToken.Kind == SyntaxKind.CatchKeyword
                   || this.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
        }

        private StmtSyntax ParseForOrForEachStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            // Check if the user wrote the following accidentally:
            //
            // for (SomeType t in
            //
            // instead of
            //
            // foreach (SomeType t in
            //
            // In that case, parse it as a foreach, but given the appropriate message that a
            // 'foreach' keyword was expected.
            var resetPoint = this.GetResetPoint();
            try
            {
                Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ForKeyword);
                this.EatToken();
                if (this.EatToken().Kind == SyntaxKind.OpenParenToken &&
                    this.EatToken().Kind == SyntaxKind.IdentifierToken &&
                    this.EatToken().Kind == SyntaxKind.InKeyword)
                {
                    // Looks like a foreach statement.  Parse it that way instead
                    this.Reset(ref resetPoint);
                    return this.ParseForEachStatement(attributes, awaitTokenOpt: null);
                }
                else
                {
                    // Normal for statement.
                    this.Reset(ref resetPoint);
                    return this.ParseForStatement(attributes);
                }
            }
            finally
            {
                this.Release(ref resetPoint);
            }
        }

        private ForStmt ParseForStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ForKeyword);

            var forToken = this.EatToken(SyntaxKind.ForKeyword);
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfForStatementArgument;

            var resetPoint = this.GetResetPoint();
            var initializers = _pool.AllocateSeparated<ExprSyntax>();
            var incrementors = _pool.AllocateSeparated<ExprSyntax>();
            try
            {
                // Here can be either a declaration or an expression statement list.  Scan
                // for a declaration first.
                VariableDecl decl = null;
                bool isDeclaration = false;
                if (this.CurrentToken.Kind == SyntaxKind.RefKeyword)
                {
                    isDeclaration = true;
                }
                else
                {
                    isDeclaration =
                        !this.IsQueryExpression(mayBeVariableDeclaration: true, mayBeMemberDeclaration: false) &&
                        this.ScanType() != ScanTypeFlags.NotType &&
                        this.IsTrueIdentifier();

                    this.Reset(ref resetPoint);
                }

                if (isDeclaration)
                {
                    decl = ParseVariableDeclaration();
                    if (decl.Type.Kind == SyntaxKind.RefType)
                    {
                        decl = decl.Update(
                            CheckFeatureAvailability(decl.Type, MessageID.IDS_FeatureRefFor),
                            decl.Variables);
                    }
                }
                else if (this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    // Not a type followed by an identifier, so it must be an expression list.
                    this.ParseForStatementExpressionList(ref openParen, initializers);
                }

                var semi = this.EatToken(SyntaxKind.SemicolonToken);
                ExprSyntax condition = null;
                if (this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    condition = this.ParseExpressionCore();
                }

                var semi2 = this.EatToken(SyntaxKind.SemicolonToken);
                if (this.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                {
                    this.ParseForStatementExpressionList(ref semi2, incrementors);
                }

                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                var statement = ParseEmbeddedStatement();
                return _syntaxFactory.ForStmt(forToken, openParen, decl, initializers, semi, condition,
                    semi2, incrementors, closeParen, statement);
            }
            finally
            {
                _termState = saveTerm;
                this.Release(ref resetPoint);
                _pool.Free(incrementors);
                _pool.Free(initializers);
            }
        }

        private bool IsEndOfForStatementArgument()
        {
            return this.CurrentToken.Kind == SyntaxKind.SemicolonToken
                   || this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                   || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private void ParseForStatementExpressionList(ref SyntaxToken startToken,
            SeparatedSyntaxListBuilder<ExprSyntax> list)
        {
            if (this.CurrentToken.Kind != SyntaxKind.CloseParenToken &&
                this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                tryAgain:
                if (this.IsPossibleExpression() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // first argument
                    list.Add(this.ParseExpressionCore());

                    // additional arguments
                    int lastTokenPosition = -1;
                    while (IsMakingProgress(ref lastTokenPosition))
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken ||
                            this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleExpression())
                        {
                            list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                            list.Add(this.ParseExpressionCore());
                            continue;
                        }
                        else if (this.SkipBadForStatementExpressionListTokens(ref startToken, list,
                                     SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadForStatementExpressionListTokens(ref startToken, list,
                             SyntaxKind.IdentifierToken) == PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }
        }

        private PostSkipAction SkipBadForStatementExpressionListTokens(ref SyntaxToken startToken,
            SeparatedSyntaxListBuilder<ExprSyntax> list, SyntaxKind expected)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleExpression(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseParenToken ||
                     p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsTerminator(),
                expected);
        }

        private ForEachStmt ParseForEachStatement(
            SyntaxList<AttributeListSyntax> attributes, SyntaxToken awaitTokenOpt)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ForKeyword);

            SyntaxToken @foreach = EatToken(SyntaxKind.ForKeyword);

            var openParen = this.EatToken(SyntaxKind.OpenParenToken);

            var identifier = ParseIdentifierToken();
            var @in = this.EatToken(SyntaxKind.InKeyword, ErrorCode.ERR_InExpected);

            var expression = this.ParseExpressionCore();
            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
            var statement = this.ParseEmbeddedStatement();

            return _syntaxFactory.ForEachStmt(@foreach, openParen, identifier,
                @in, expression, closeParen, statement);
        }


        /// <summary>
        /// Is the following set of tokens, interpreted as a type, the type <c>var</c>?
        /// </summary>
        private bool IsVarType()
        {
            if (!this.CurrentToken.IsIdentifierVar())
            {
                return false;
            }

            switch (this.PeekToken(1).Kind)
            {
                case SyntaxKind.DotToken:
                case SyntaxKind.ColonColonToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.QuestionToken:
                case SyntaxKind.LessThanToken:
                    return false;
                default:
                    return true;
            }
        }

        private ReturnStmt ParseReturnStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ReturnKeyword);
            var @return = this.EatToken(SyntaxKind.ReturnKeyword);
            ExprSyntax arg = null;
            if (this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                arg = this.ParsePossibleRefExpression();
            }

            var semicolon = this.EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.ReturnStmt(@return, arg, semicolon);
        }

        private IfStmt ParseIfStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.IfKeyword);

            return _syntaxFactory.IfStmt(
                this.EatToken(SyntaxKind.IfKeyword),
                this.EatToken(SyntaxKind.OpenParenToken),
                this.ParseExpressionCore(),
                this.EatToken(SyntaxKind.CloseParenToken),
                this.ParseEmbeddedStatement(),
                this.ParseElseClauseOpt());
        }

        private IfStmt ParseMisplacedElse(SyntaxList<AttributeListSyntax> attributes)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ElseKeyword);

            return _syntaxFactory.IfStmt(
                this.EatToken(SyntaxKind.IfKeyword, ErrorCode.ERR_ElseCannotStartStatement),
                this.EatToken(SyntaxKind.OpenParenToken),
                this.ParseExpressionCore(),
                this.EatToken(SyntaxKind.CloseParenToken),
                this.ParseExpressionStatement(attributes: default),
                this.ParseElseClauseOpt());
        }

        private ElseClauseSyntax ParseElseClauseOpt()
        {
            if (this.CurrentToken.Kind != SyntaxKind.ElseKeyword)
            {
                return null;
            }

            var elseToken = this.EatToken(SyntaxKind.ElseKeyword);
            var elseStatement = this.ParseEmbeddedStatement();
            return _syntaxFactory.ElseClause(elseToken, elseStatement);
        }

        private bool IsPossibleSwitchSection()
        {
            return (this.CurrentToken.Kind == SyntaxKind.CaseKeyword) ||
                   (this.CurrentToken.Kind == SyntaxKind.DefaultKeyword &&
                    this.PeekToken(1).Kind != SyntaxKind.OpenParenToken);
        }

        private bool IsUsingStatementVariableDeclaration(ScanTypeFlags st)
        {
            Debug.Assert(st != ScanTypeFlags.NullableType);

            bool condition1 = st == ScanTypeFlags.MustBeType && this.CurrentToken.Kind != SyntaxKind.DotToken;
            bool condition2 = st != ScanTypeFlags.NotType && this.CurrentToken.Kind == SyntaxKind.IdentifierToken;
            bool condition3 = st == ScanTypeFlags.NonGenericTypeOrExpression ||
                              this.PeekToken(1).Kind == SyntaxKind.EqualsToken;

            return condition1 || (condition2 && condition3);
        }


        /// <summary>
        /// Parses any kind of local declaration statement: local variable or local function.
        /// </summary>
        private StmtSyntax ParseLocalDeclarationStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken awaitKeyword, usingKeyword;
            bool canParseAsLocalFunction = false;
            if (IsPossibleAwaitUsing())
            {
                awaitKeyword = ParseAwaitKeyword(MessageID.None);
                usingKeyword = EatToken();
            }
            else if (this.CurrentToken.Kind == SyntaxKind.UsingKeyword)
            {
                awaitKeyword = null;
                usingKeyword = EatToken();
            }
            else
            {
                awaitKeyword = null;
                usingKeyword = null;
                canParseAsLocalFunction = true;
            }

            if (usingKeyword != null)
            {
                usingKeyword = CheckFeatureAvailability(usingKeyword, MessageID.IDS_FeatureUsingDeclarations);
            }

            var mods = _pool.Allocate();
            this.ParseDeclarationModifiers(mods);

            var variables = _pool.AllocateSeparated<VariableInit>();
            try
            {
                this.ParseLocalDeclaration(variables,
                    allowLocalFunctions: canParseAsLocalFunction,
                    attributes: attributes,
                    mods: mods.ToList(),
                    type: out var type);


                if (canParseAsLocalFunction)
                {
                    // If we find an accessibility modifier but no local function it's likely
                    // the user forgot a closing brace. Let's back out of statement parsing.
                    // We check just for a leading accessibility modifier in the syntax because
                    // SkipBadStatementListTokens will not skip attribute lists.
                    if (attributes.Count == 0 && mods.Count > 0 &&
                        IsAccessibilityModifier(((SyntaxToken)mods[0]).ContextualKind))
                    {
                        return null;
                    }
                }

                for (int i = 0; i < mods.Count; i++)
                {
                    var mod = (SyntaxToken)mods[i];

                    if (IsAdditionalLocalFunctionModifier(mod.ContextualKind))
                    {
                        mods[i] = this.AddError(mod, ErrorCode.ERR_BadMemberFlag, mod.Text);
                    }
                }

                var semicolon = this.EatToken(SyntaxKind.SemicolonToken);
                return _syntaxFactory.LocalDeclStmt(
                    attributes,
                    mods.ToList(),
                    _syntaxFactory.VariableDecl(type, variables),
                    semicolon);
            }
            finally
            {
                _pool.Free(variables);
                _pool.Free(mods);
            }
        }

        /// <summary>
        /// Parse a local variable declaration.
        /// </summary>
        /// <returns></returns>
        private VariableDecl ParseVariableDeclaration()
        {
            var variables = _pool.AllocateSeparated<VariableInit>();
            TypeEx type;
            ParseLocalDeclaration(variables, false, attributes: default, mods: default, out type);
            var result = _syntaxFactory.VariableDecl(type, variables);
            _pool.Free(variables);
            return result;
        }

        private void ParseLocalDeclaration(
            SeparatedSyntaxListBuilder<VariableInit> variables,
            bool allowLocalFunctions,
            SyntaxList<AttributeListSyntax> attributes,
            SyntaxList<SyntaxToken> mods,
            out TypeEx type)
        {
            type = allowLocalFunctions ? ParseReturnType() : this.ParseType();

            VariableFlags flags = VariableFlags.Local;
            if (mods.Any((int)SyntaxKind.ConstKeyword))
            {
                flags |= VariableFlags.Const;
            }

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfDeclarationClause;
            this.ParseVariableDeclarators(
                type,
                flags,
                variables,
                variableDeclarationsExpected: true,
                allowLocalFunctions: allowLocalFunctions,
                attributes: attributes,
                mods: mods);
            _termState = saveTerm;

            if (allowLocalFunctions && (type as PredefinedTypeEx)?.Keyword.Kind == SyntaxKind.VoidKeyword)
            {
                type = this.AddError(type, ErrorCode.ERR_NoVoidHere);
            }
        }


        private bool IsEndOfDeclarationClause()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.ColonToken:
                    return true;
                default:
                    return false;
            }
        }

        private void ParseDeclarationModifiers(SyntaxListBuilder list)
        {
            SyntaxKind k;
            while (IsDeclarationModifier(k = this.CurrentToken.ContextualKind) || IsAdditionalLocalFunctionModifier(k))
            {
                SyntaxToken mod;
                if (k == SyntaxKind.AsyncKeyword)
                {
                    // check for things like "async async()" where async is the type and/or the function name
                    {
                        var resetPoint = this.GetResetPoint();

                        var invalid = !IsPossibleStartOfTypeDeclaration(this.EatToken().Kind) &&
                                      !IsDeclarationModifier(this.CurrentToken.Kind) &&
                                      !IsAdditionalLocalFunctionModifier(this.CurrentToken.Kind) &&
                                      (ScanType() == ScanTypeFlags.NotType ||
                                       this.CurrentToken.Kind != SyntaxKind.IdentifierToken);

                        this.Reset(ref resetPoint);
                        this.Release(ref resetPoint);

                        if (invalid)
                        {
                            break;
                        }
                    }

                    mod = this.EatContextualToken(k);
                    if (k == SyntaxKind.AsyncKeyword)
                    {
                        mod = CheckFeatureAvailability(mod, MessageID.IDS_FeatureAsync);
                    }
                }
                else
                {
                    mod = this.EatToken();
                }

                if (k == SyntaxKind.ReadOnlyKeyword || k == SyntaxKind.VolatileKeyword)
                {
                    mod = this.AddError(mod, ErrorCode.ERR_BadMemberFlag, mod.Text);
                }
                else if (list.Any(mod.RawKind))
                {
                    // check for duplicates, can only be const
                    mod = this.AddError(mod, ErrorCode.ERR_TypeExpected, mod.Text);
                }

                list.Add(mod);
            }
        }

        private static bool IsDeclarationModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.VolatileKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsAdditionalLocalFunctionModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.AsyncKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.ExternKeyword:
                // Not a valid modifier, but we should parse to give a good
                // error message
                case SyntaxKind.PubKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsAccessibilityModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                // Accessibility modifiers aren't legal in a local function,
                // but a common mistake. Parse to give a better error message.
                case SyntaxKind.PubKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return true;

                default:
                    return false;
            }
        }

        private ExpressionStmt ParseExpressionStatement(SyntaxList<AttributeListSyntax> attributes)
        {
            return ParseExpressionStatement(attributes, this.ParseExpressionCore());
        }

        private ExpressionStmt ParseExpressionStatement(SyntaxList<AttributeListSyntax> attributes,
            ExprSyntax expression)
        {
            SyntaxToken semicolon;
            if (IsScript && this.CurrentToken.Kind == SyntaxKind.EndOfFileToken)
            {
                semicolon = SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken);
            }
            else
            {
                // Do not report an error if the expression is not a statement expression.
                // The error is reported in semantic analysis.
                semicolon = this.EatToken(SyntaxKind.SemicolonToken);
            }

            return _syntaxFactory.ExpressionStmt(expression, semicolon);
        }


        public ExprSyntax ParseExpression()
        {
            return ParseWithStackGuard(
                this.ParseExpressionCore,
                this.CreateMissingIdentifierName);
        }

        private ExprSyntax ParseExpressionCore()
        {
            return this.ParseSubExpression(Precedence.Expression);
        }

        /// <summary>
        /// Is the current token one that could start an expression?
        /// </summary>
        private bool CanStartExpression()
        {
            return IsPossibleExpression(allowBinaryExpressions: false, allowAssignmentExpressions: false,
                allowAttributes: false);
        }

        /// <summary>
        /// Is the current token one that could be in an expression?
        /// </summary>
        private bool IsPossibleExpression()
        {
            return IsPossibleExpression(allowBinaryExpressions: true, allowAssignmentExpressions: true,
                allowAttributes: true);
        }

        private bool IsPossibleExpression(bool allowBinaryExpressions, bool allowAssignmentExpressions,
            bool allowAttributes)
        {
            SyntaxKind tk = this.CurrentToken.Kind;
            switch (tk)
            {
                case SyntaxKind.TypeOfKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.SizeOfKeyword:
                case SyntaxKind.MakeRefKeyword:
                case SyntaxKind.RefTypeKeyword:
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                case SyntaxKind.RefValueKeyword:
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.BaseKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.ThisKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.NullKeyword:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.InterpolatedStringStartToken:
                case SyntaxKind.InterpolatedStringToken:
                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.ColonColonToken: // bad aliased name
                case SyntaxKind.ThrowKeyword:
                case SyntaxKind.StackAllocKeyword:
                case SyntaxKind.DotDotToken:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.MatchKeyword:
                    return true;
                case SyntaxKind.StaticKeyword:
                    return IsPossibleAnonymousMethodExpression();
                case SyntaxKind.OpenBracketToken:
                    return allowAttributes;
                case SyntaxKind.IdentifierToken:
                    // Specifically allow the from contextual keyword, because it can always be the start of an
                    // expression (whether it is used as an identifier or a keyword).
                    return this.IsTrueIdentifier() || (this.CurrentToken.ContextualKind == SyntaxKind.FromKeyword);
                default:
                    return IsPredefinedType(tk)
                           || SyntaxFacts.IsAnyUnaryExpression(tk)
                           || (allowBinaryExpressions && SyntaxFacts.IsBinaryExpression(tk))
                           || (allowAssignmentExpressions && SyntaxFacts.IsAssignmentExpressionOperatorToken(tk));
            }
        }

        private static bool IsInvalidSubExpression(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.BreakKeyword:
                case SyntaxKind.CaseKeyword:
                case SyntaxKind.CatchKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.ContinueKeyword:
                case SyntaxKind.DoKeyword:
                case SyntaxKind.FinallyKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.ForEachKeyword:
                case SyntaxKind.GotoKeyword:
                case SyntaxKind.IfKeyword:
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.LockKeyword:
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.TryKeyword:
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.WhileKeyword:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsRightAssociative(SyntaxKind op)
        {
            switch (op)
            {
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression:
                case SyntaxKind.CoalesceExpression:
                    return true;
                default:
                    return false;
            }
        }

        private enum Precedence : uint
        {
            Expression = 0, // Loosest possible precedence, used to accept all expressions
            Assignment = Expression,

            Lambda =
                Assignment, // "The => operator has the same precedence as assignment (=) and is right-associative."
            Conditional,
            Coalescing,
            ConditionalOr,
            ConditionalAnd,
            LogicalOr,
            LogicalXor,
            LogicalAnd,
            Equality,
            Relational,
            Shift,
            Additive,
            Multiplicative,
            Switch,
            Range,
            Unary,
            Cast,
            PointerIndirection,
            AddressOf,
            Primary,
        }

        private static Precedence GetPrecedence(SyntaxKind op)
        {
            switch (op)
            {
                case SyntaxKind.QueryExpression:
                    return Precedence.Expression;
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.AnonymousMethodExpression:
                    return Precedence.Lambda;
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression:
                    return Precedence.Assignment;
                case SyntaxKind.CoalesceExpression:
                case SyntaxKind.ThrowExpression:
                    return Precedence.Coalescing;
                case SyntaxKind.LogicalOrExpression:
                    return Precedence.ConditionalOr;
                case SyntaxKind.LogicalAndExpression:
                    return Precedence.ConditionalAnd;
                case SyntaxKind.BitwiseOrExpression:
                    return Precedence.LogicalOr;
                case SyntaxKind.ExclusiveOrExpression:
                    return Precedence.LogicalXor;
                case SyntaxKind.BitwiseAndExpression:
                    return Precedence.LogicalAnd;
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    return Precedence.Equality;
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.IsPatternExpression:
                    return Precedence.Relational;
                case SyntaxKind.MatchExpression:
                case SyntaxKind.SwitchExpression:
                case SyntaxKind.WithExpression:
                    return Precedence.Switch;
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return Precedence.Shift;
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                    return Precedence.Additive;
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                    return Precedence.Multiplicative;
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.TypeOfExpression:
                case SyntaxKind.SizeOfExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression:
                case SyntaxKind.MakeRefExpression:
                case SyntaxKind.RefValueExpression:
                case SyntaxKind.RefTypeExpression:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.IndexExpression:
                    return Precedence.Unary;
                case SyntaxKind.CastExpression:
                    return Precedence.Cast;
                case SyntaxKind.PointerIndirectionExpression:
                    return Precedence.PointerIndirection;
                case SyntaxKind.AddressOfExpression:
                    return Precedence.AddressOf;
                case SyntaxKind.RangeExpression:
                    return Precedence.Range;
                case SyntaxKind.ConditionalExpression:
                    return Precedence.Expression;
                case SyntaxKind.AliasQualifiedName:
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.ArgListExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.BaseExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.ConditionalAccessExpression:
                case SyntaxKind.DeclarationExpression:
                case SyntaxKind.DefaultExpression:
                case SyntaxKind.DefaultLiteralExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.GenericName:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.IdentifierEx:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.ImplicitStackAllocArrayCreationExpression:
                case SyntaxKind.ImplicitObjectCreationExpression:
                case SyntaxKind.InterpolatedStringExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.AllocExpression:
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.PointerMemberAccessExpression:
                case SyntaxKind.PostDecrementExpression:
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.RefExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.StackAllocArrayCreationExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.SuppressNullableWarningExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.TupleExpression:
                    return Precedence.Primary;
                default:
                    throw ExceptionUtilities.UnexpectedValue(op);
            }
        }

        private static bool IsExpectedPrefixUnaryOperator(SyntaxKind kind)
        {
            return SyntaxFacts.IsPrefixUnaryExpression(kind) && kind != SyntaxKind.RefKeyword &&
                   kind != SyntaxKind.OutKeyword;
        }

        private static bool IsExpectedBinaryOperator(SyntaxKind kind)
        {
            return SyntaxFacts.IsBinaryExpression(kind);
        }

        private static bool IsExpectedAssignmentOperator(SyntaxKind kind)
        {
            return SyntaxFacts.IsAssignmentExpressionOperatorToken(kind);
        }

        private bool IsPossibleAwaitExpressionStatement()
        {
            return (this.IsScript || this.IsInAsync) && this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword;
        }

        private bool IsAwaitExpression()
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword)
            {
                if (this.IsInAsync)
                {
                    // If we see an await in an async function, parse it as an unop.
                    return true;
                }

                // If we see an await followed by a token that cannot follow an identifier, parse await as a unop.
                // BindAwait() catches the cases where await successfully parses as a unop but is not in an async
                // function, and reports an appropriate ERR_BadAwaitWithoutAsync* error.
                var next = PeekToken(1);
                switch (next.Kind)
                {
                    case SyntaxKind.IdentifierToken:
                        return next.ContextualKind != SyntaxKind.WithKeyword;

                    // Keywords
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.ThisKeyword:
                    case SyntaxKind.BaseKeyword:
                    case SyntaxKind.DelegateKeyword:
                    case SyntaxKind.TypeOfKeyword:
                    case SyntaxKind.CheckedKeyword:
                    case SyntaxKind.UncheckedKeyword:
                    case SyntaxKind.DefaultKeyword:

                    // Literals
                    case SyntaxKind.TrueKeyword:
                    case SyntaxKind.FalseKeyword:
                    case SyntaxKind.StringLiteralToken:
                    case SyntaxKind.InterpolatedStringStartToken:
                    case SyntaxKind.InterpolatedStringToken:
                    case SyntaxKind.NumericLiteralToken:
                    case SyntaxKind.NullKeyword:
                    case SyntaxKind.CharacterLiteralToken:
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parse a subexpression of the enclosing operator of the given precedence.
        /// </summary>
        private ExprSyntax ParseSubExpression(Precedence precedence)
        {
            _recursionDepth++;

            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);

            var result = ParseSubExpressionCore(precedence);
#if DEBUG
            // Ensure every expression kind is handled in GetPrecedence
            _ = GetPrecedence(result.Kind);
#endif
            _recursionDepth--;
            return result;
        }

        private ExprSyntax ParseSubExpressionCore(Precedence precedence)
        {
            ExprSyntax leftOperand;
            Precedence newPrecedence = 0;

            // all of these are tokens that start statements and are invalid
            // to start a expression with. if we see one, then we must have
            // something like:
            //
            // return
            // if (...
            // parse out a missing name node for the expression, and keep on going
            var tk = this.CurrentToken.Kind;
            if (IsInvalidSubExpression(tk))
            {
                return this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_InvalidExprTerm,
                    SyntaxFacts.GetText(tk));
            }

            // Parse a left operand -- possibly preceded by a unary operator.
            if (IsExpectedPrefixUnaryOperator(tk))
            {
                var opKind = SyntaxFacts.GetPrefixUnaryExpression(tk);
                newPrecedence = GetPrecedence(opKind);
                var opToken = this.EatToken();
                var operand = this.ParseSubExpression(newPrecedence);
                leftOperand = _syntaxFactory.PrefixUnaryEx(opKind, opToken, operand);
            }
            else if (tk == SyntaxKind.DotDotToken)
            {
                // Operator ".." here can either be a prefix unary operator or a stand alone empty range:
                var opToken = this.EatToken();
                newPrecedence = GetPrecedence(SyntaxKind.RangeExpression);

                ExprSyntax rightOperand;
                if (CanStartExpression())
                {
                    rightOperand = this.ParseSubExpression(newPrecedence);
                }
                else
                {
                    rightOperand = null;
                }

                leftOperand = _syntaxFactory.RangeEx(leftOperand: null, opToken, rightOperand);
            }

            else if (this.IsQueryExpression(mayBeVariableDeclaration: false, mayBeMemberDeclaration: false))
            {
                leftOperand = null; //this.ParseQueryExpression(precedence);
            }
            else if (this.CurrentToken.ContextualKind == SyntaxKind.FromKeyword && IsInQuery)
            {
                // If this "from" token wasn't the start of a query then it's not really an expression.
                // Consume it so that we don't try to parse it again as the next argument in an
                // argument list.
                SyntaxToken skipped = this.EatToken(); // consume but skip "from"
                skipped = this.AddError(skipped, ErrorCode.ERR_InvalidExprTerm, this.CurrentToken.Text);
                leftOperand = AddTrailingSkippedSyntax(this.CreateMissingIdentifierName(), skipped);
            }
            else if (tk == SyntaxKind.ThrowKeyword)
            {
                var result = ParseThrowExpression();
                // we parse a throw expression even at the wrong precedence for better recovery
                return (precedence <= Precedence.Coalescing)
                    ? result
                    : this.AddError(result, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(tk));
            }
            else if (this.IsPossibleDeconstructionLeft(precedence))
            {
                throw new NotImplementedException();
                // leftOperand = ParseDeclarationExpression(ParseTypeMode.Normal, MessageID.IDS_FeatureTuples);
            }
            else
            {
                // Not a unary operator - get a primary expression.
                leftOperand = this.ParseTerm(precedence);
            }

            return ParseExpressionContinued(leftOperand, precedence);
        }

        private ExprSyntax ParseExpressionContinued(ExprSyntax leftOperand, Precedence precedence)
        {
            while (true)
            {
                // We either have a binary or assignment operator here, or we're finished.
                var tk = this.CurrentToken.ContextualKind;

                bool isAssignmentOperator = false;
                SyntaxKind opKind;
                if (IsExpectedBinaryOperator(tk))
                {
                    opKind = SyntaxFacts.GetBinaryExpression(tk);
                }
                else if (IsExpectedAssignmentOperator(tk))
                {
                    opKind = SyntaxFacts.GetAssignmentExpression(tk);
                    isAssignmentOperator = true;
                }
                else if (tk == SyntaxKind.DotDotToken)
                {
                    opKind = SyntaxKind.RangeExpression;
                }
                else if (tk == SyntaxKind.SwitchKeyword && this.PeekToken(1).Kind == SyntaxKind.OpenBraceToken)
                {
                    opKind = SyntaxKind.SwitchExpression;
                }
                else if (tk == SyntaxKind.WithKeyword && this.PeekToken(1).Kind == SyntaxKind.OpenBraceToken)
                {
                    opKind = SyntaxKind.WithExpression;
                }
                else
                {
                    break;
                }

                var newPrecedence = GetPrecedence(opKind);

                // check for >> or >>=
                bool doubleOp = false;
                if (tk == SyntaxKind.GreaterThanToken
                    && (this.PeekToken(1).Kind == SyntaxKind.GreaterThanToken ||
                        this.PeekToken(1).Kind == SyntaxKind.GreaterThanEqualsToken))
                {
                    // check to see if they really are adjacent
                    if (this.CurrentToken.GetTrailingTriviaWidth() == 0 &&
                        this.PeekToken(1).GetLeadingTriviaWidth() == 0)
                    {
                        if (this.PeekToken(1).Kind == SyntaxKind.GreaterThanToken)
                        {
                            opKind = SyntaxFacts.GetBinaryExpression(SyntaxKind.GreaterThanGreaterThanToken);
                        }
                        else
                        {
                            opKind = SyntaxFacts.GetAssignmentExpression(SyntaxKind.GreaterThanGreaterThanEqualsToken);
                            isAssignmentOperator = true;
                        }

                        newPrecedence = GetPrecedence(opKind);
                        doubleOp = true;
                    }
                }

                // Check the precedence to see if we should "take" this operator
                if (newPrecedence < precedence)
                {
                    break;
                }

                // Same precedence, but not right-associative -- deal with this "later"
                if ((newPrecedence == precedence) && !IsRightAssociative(opKind))
                {
                    break;
                }

                // We'll "take" this operator, as precedence is tentatively OK.
                var opToken = this.EatContextualToken(tk);

                var leftPrecedence = GetPrecedence(leftOperand.Kind);
                if (newPrecedence > leftPrecedence)
                {
                    // Normally, a left operand with a looser precedence will consume all right operands that
                    // have a tighter precedence.  For example, in the expression `a + b * c`, the `* c` part
                    // will be consumed as part of the right operand of the addition.  However, there are a
                    // few circumstances in which a tighter precedence is not consumed: that occurs when the
                    // left hand operator does not have an expression as its right operand.  This occurs for
                    // the is-type operator and the is-pattern operator.  Source text such as
                    // `a is {} + b` should produce a syntax error, as parsing the `+` with an `is`
                    // expression as its left operand would be a precedence inversion.  Similarly, it occurs
                    // with an anonymous method expression or a lambda expression with a block body.  No
                    // further parsing will find a way to fix things up, so we accept the operator but issue
                    // a diagnostic.
                    ErrorCode errorCode = leftOperand.Kind == SyntaxKind.IsPatternExpression
                        ? ErrorCode.ERR_UnexpectedToken
                        : ErrorCode.WRN_PrecedenceInversion;
                    opToken = this.AddError(opToken, errorCode, opToken.Text);
                }

                if (doubleOp)
                {
                    // combine tokens into a single token
                    var opToken2 = this.EatToken();
                    var kind = opToken2.Kind == SyntaxKind.GreaterThanToken
                        ? SyntaxKind.GreaterThanGreaterThanToken
                        : SyntaxKind.GreaterThanGreaterThanEqualsToken;
                    opToken = SyntaxFactory.Token(opToken.GetLeadingTrivia(), kind, opToken2.GetTrailingTrivia());
                }

                if (opKind == SyntaxKind.AsExpression)
                {
                    var type = this.ParseType(ParseTypeMode.AsExpression);
                    leftOperand = _syntaxFactory.BinaryEx(opKind, leftOperand, opToken, type);
                }
                else if (isAssignmentOperator)
                {
                    ExprSyntax rhs = this.ParseSubExpression(newPrecedence);

                    if (opKind == SyntaxKind.CoalesceAssignmentExpression)
                    {
                        opToken = CheckFeatureAvailability(opToken, MessageID.IDS_FeatureCoalesceAssignmentExpression);
                    }

                    leftOperand = _syntaxFactory.AssignEx(opKind, leftOperand, opToken, rhs);
                }
                else if (tk == SyntaxKind.DotDotToken)
                {
                    // Operator ".." here can either be a binary or a postfix unary operator:
                    Debug.Assert(opKind == SyntaxKind.RangeExpression);

                    ExprSyntax rightOperand;
                    if (CanStartExpression())
                    {
                        newPrecedence = GetPrecedence(opKind);
                        rightOperand = this.ParseSubExpression(newPrecedence);
                    }
                    else
                    {
                        rightOperand = null;
                    }

                    leftOperand = _syntaxFactory.RangeEx(leftOperand, opToken, rightOperand);
                }
                else
                {
                    Debug.Assert(IsExpectedBinaryOperator(tk));
                    leftOperand = _syntaxFactory.BinaryEx(opKind, leftOperand, opToken,
                        this.ParseSubExpression(newPrecedence));
                }
            }

            // From the language spec:
            //
            // conditional-expression:
            //  null-coalescing-expression
            //  null-coalescing-expression   ?   expression   :   expression
            //
            // Only take the conditional if we're at or below its precedence.
            if (CurrentToken.Kind == SyntaxKind.QuestionToken && precedence <= Precedence.Conditional)
            {
                var questionToken = this.EatToken();
                var colonLeft = this.ParsePossibleRefExpression();
                if (this.CurrentToken.Kind == SyntaxKind.EndOfFileToken && this.lexer.InterpolationFollowedByColon)
                {
                    // We have an interpolated string with an interpolation that contains a conditional expression.
                    // Unfortunately, the precedence demands that the colon is considered to signal the start of the
                    // format string. Without this code, the compiler would complain about a missing colon, and point
                    // to the colon that is present, which would be confusing. We aim to give a better error message.
                    var colon = SyntaxFactory.MissingToken(SyntaxKind.ColonToken);
                    var colonRight =
                        _syntaxFactory.IdentifierEx(SyntaxFactory.MissingToken(SyntaxKind.IdentifierToken));
                    leftOperand =
                        _syntaxFactory.ConditionalEx(leftOperand, questionToken, colonLeft, colon, colonRight);
                    leftOperand = this.AddError(leftOperand, ErrorCode.ERR_ConditionalInInterpolation);
                }
                else
                {
                    var colon = this.EatToken(SyntaxKind.ColonToken);
                    var colonRight = this.ParsePossibleRefExpression();
                    leftOperand =
                        _syntaxFactory.ConditionalEx(leftOperand, questionToken, colonLeft, colon, colonRight);
                }
            }

            return leftOperand;
        }

        // private ExprSyntax ParseDeclarationExpression(ParseTypeMode mode, MessageID feature)
        // {
        //     TypeEx type = this.ParseType(mode);
        //     var designation = ParseDesignation(forPattern: false);
        //     if (feature != MessageID.None)
        //     {
        //         designation = CheckFeatureAvailability(designation, feature);
        //     }
        //
        //     return _syntaxFactory.DeclarationExpression(type, designation);
        // }

        private ExprSyntax ParseThrowExpression()
        {
            var throwToken = this.EatToken(SyntaxKind.ThrowKeyword);
            var thrown = this.ParseSubExpression(Precedence.Coalescing);
            var result = _syntaxFactory.ThrowEx(throwToken, thrown);
            return CheckFeatureAvailability(result, MessageID.IDS_FeatureThrowExpression);
        }

        private ExprSyntax ParseTerm(Precedence precedence)
            => this.ParsePostFixExpression(ParseTermWithoutPostfix(precedence));

        private ExprSyntax ParseTermWithoutPostfix(Precedence precedence)
        {
            var tk = this.CurrentToken.Kind;
            switch (tk)
            {
                // case SyntaxKind.TypeOfKeyword:
                //     return this.ParseTypeOfExpression();
                // case SyntaxKind.DefaultKeyword:
                //     return this.ParseDefaultExpression();
                // case SyntaxKind.SizeOfKeyword:
                //     return this.ParseSizeOfExpression();
                // case SyntaxKind.MakeRefKeyword:
                //     return this.ParseMakeRefExpression();
                // case SyntaxKind.RefTypeKeyword:
                //     return this.ParseRefTypeExpression();
                // case SyntaxKind.CheckedKeyword:
                // case SyntaxKind.UncheckedKeyword:
                //     return this.ParseCheckedOrUncheckedExpression();
                // case SyntaxKind.RefValueKeyword:
                //     return this.ParseRefValueExpression();
                case SyntaxKind.ColonColonToken:
                    // misplaced ::
                    // Calling ParseAliasQualifiedName will cause us to create a missing identifier node that then
                    // properly consumes the :: and the reset of the alias name afterwards.
                    return this.ParseAliasQualifiedName(NameOptions.InExpression);
                // case SyntaxKind.EqualsGreaterThanToken:
                //     return this.ParseLambdaExpression();
                // case SyntaxKind.StaticKeyword:
                //     if (this.IsPossibleAnonymousMethodExpression())
                //     {
                //         return this.ParseAnonymousMethodExpression();
                //     }
                //     else if (this.IsPossibleLambdaExpression(precedence))
                //     {
                //         return this.ParseLambdaExpression();
                //     }
                //     else
                //     {
                //         return this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_InvalidExprTerm,
                //             this.CurrentToken.Text);
                //     }
                case SyntaxKind.MatchKeyword:
                    return this.ParseMatch();
                case SyntaxKind.IdentifierToken:
                    if (this.IsTrueIdentifier())
                    {
                        // if (this.IsPossibleAnonymousMethodExpression())
                        // {
                        //     return this.ParseAnonymousMethodExpression();
                        // }
                        // else if (this.IsPossibleLambdaExpression(precedence) &&
                        //          this.TryParseLambdaExpression() is { } lambda)
                        // {
                        //     return lambda;
                        // }
                        // else if (this.IsPossibleDeconstructionLeft(precedence))
                        // {
                        //     return ParseDeclarationExpression(ParseTypeMode.Normal, MessageID.IDS_FeatureTuples);
                        // }
                        // else
                        // {
                        return this.ParseAliasQualifiedName(NameOptions.InExpression);
                        // }
                    }
                    else
                    {
                        return this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_InvalidExprTerm,
                            this.CurrentToken.Text);
                    }
                // case SyntaxKind.OpenBracketToken:
                //     return this.ParseRange();
                // case SyntaxKind.ThisKeyword:
                //     return _syntaxFactory.ThisExpression(this.EatToken());
                // case SyntaxKind.BaseKeyword:
                //     return ParseBaseExpression();

                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.NullKeyword:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.CharacterLiteralToken:
                    return _syntaxFactory.LiteralEx(SyntaxFacts.GetLiteralExpression(tk), this.EatToken());
                case SyntaxKind.InterpolatedStringStartToken:
                    throw
                        new NotImplementedException(); // this should not occur because these tokens are produced and parsed immediately
                case SyntaxKind.InterpolatedStringToken:
                    return this.ParseInterpolatedStringToken();
                case SyntaxKind.OpenParenToken:
                    return this.ParseCastOrParenExpression();
                // case SyntaxKind.NewKeyword:
                //     return this.ParseNewExpression();
                // case SyntaxKind.StackAllocKeyword:
                //     return this.ParseStackAllocExpression();
                // case SyntaxKind.DelegateKeyword:
                //     // check for lambda expression with explicit function pointer return type
                //     if (this.IsPossibleLambdaExpression(precedence))
                //     {
                //         return this.ParseLambdaExpression();
                //     }
                //
                //     return this.ParseAnonymousMethodExpression();
                // case SyntaxKind.RefKeyword:
                //     // check for lambda expression with explicit ref return type: `ref int () => { ... }`
                //     if (this.IsPossibleLambdaExpression(precedence))
                //     {
                //         return this.ParseLambdaExpression();
                //     }
                //
                //     // ref is not expected to appear in this position.
                //     var refKeyword = this.EatToken();
                //     return this.AddError(_syntaxFactory.RefExpression(refKeyword, this.ParseExpressionCore()),
                //         ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(tk));
                default:
                    if (IsPredefinedType(tk))
                    {
                        // check for intrinsic type followed by '.'
                        var expr = _syntaxFactory.PredefinedTypeEx(this.EatToken());

                        if (this.CurrentToken.Kind != SyntaxKind.DotToken || tk == SyntaxKind.VoidKeyword)
                        {
                            expr = this.AddError(expr, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(tk));
                        }

                        return expr;
                    }
                    else
                    {
                        var expr = this.CreateMissingIdentifierName();

                        if (tk == SyntaxKind.EndOfFileToken)
                        {
                            expr = this.AddError(expr, ErrorCode.ERR_ExpressionExpected);
                        }
                        else
                        {
                            expr = this.AddError(expr, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(tk));
                        }

                        return expr;
                    }
            }
        }


        /// <summary>
        /// Returns true if...
        /// 1. The precedence is less than or equal to Assignment, and
        /// 2. The current token is the identifier var or a predefined type, and
        /// 3. it is followed by (, and
        /// 4. that ( begins a valid parenthesized designation, and
        /// 5. the token following that designation is =
        /// </summary>
        private bool IsPossibleDeconstructionLeft(Precedence precedence)
        {
            if (precedence > Precedence.Assignment ||
                !(this.CurrentToken.IsIdentifierVar() || IsPredefinedType(this.CurrentToken.Kind)))
            {
                return false;
            }

            var resetPoint = this.GetResetPoint();
            try
            {
                this.EatToken(); // `var`
                return
                    this.CurrentToken.Kind == SyntaxKind.OpenParenToken && ScanDesignator() &&
                    this.CurrentToken.Kind == SyntaxKind.EqualsToken;
            }
            finally
            {
                // Restore current token index
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }
        }

        private bool ScanDesignator()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.IdentifierToken:
                    if (!IsTrueIdentifier())
                    {
                        goto default;
                    }

                    this.EatToken(); // eat the identifier
                    return true;
                case SyntaxKind.OpenParenToken:
                    while (true)
                    {
                        this.EatToken(); // eat the open paren or comma
                        if (!ScanDesignator())
                        {
                            return false;
                        }

                        switch (this.CurrentToken.Kind)
                        {
                            case SyntaxKind.CommaToken:
                                continue;
                            case SyntaxKind.CloseParenToken:
                                this.EatToken(); // eat the close paren
                                return true;
                            default:
                                return false;
                        }
                    }
                default:
                    return false;
            }
        }

        private bool IsPossibleAnonymousMethodExpression()
        {
            // Skip past any static/async keywords.
            var tokenIndex = 0;
            while (this.PeekToken(tokenIndex).Kind == SyntaxKind.StaticKeyword ||
                   this.PeekToken(tokenIndex).ContextualKind == SyntaxKind.AsyncKeyword)
            {
                tokenIndex++;
            }

            return this.PeekToken(tokenIndex).Kind == SyntaxKind.DelegateKeyword &&
                   this.PeekToken(tokenIndex + 1).Kind != SyntaxKind.AsteriskToken;
        }

        private ExprSyntax ParsePostFixExpression(ExprSyntax expr)
        {
            Debug.Assert(expr != null);

            while (true)
            {
                SyntaxKind tk = this.CurrentToken.Kind;
                switch (tk)
                {
                    case SyntaxKind.OpenParenToken:
                        expr = _syntaxFactory.InvocationEx(expr, this.ParseParenthesizedArgumentList());
                        break;

                    case SyntaxKind.OpenBracketToken:
                        expr = _syntaxFactory.ElementAccessEx(expr, this.ParseBracketedArgumentList());
                        break;

                    case SyntaxKind.PlusPlusToken:
                    case SyntaxKind.MinusMinusToken:
                        expr = _syntaxFactory.PostfixUnaryEx(SyntaxFacts.GetPostfixUnaryExpression(tk), expr,
                            this.EatToken());
                        break;

                    case SyntaxKind.ColonColonToken:
                        if (this.PeekToken(1).Kind == SyntaxKind.IdentifierToken)
                        {
                            // replace :: with missing dot and annotate with skipped text "::" and error
                            var ccToken = this.EatToken();
                            ccToken = this.AddError(ccToken, ErrorCode.ERR_UnexpectedAliasedName);
                            var dotToken = this.ConvertToMissingWithTrailingTrivia(ccToken, SyntaxKind.DotToken);
                            expr = _syntaxFactory.MemberAccessEx(SyntaxKind.SimpleMemberAccessExpression, expr,
                                dotToken, this.ParseSimpleName(NameOptions.InExpression));
                        }
                        else
                        {
                            // just some random trailing :: ?
                            expr = AddTrailingSkippedSyntax(expr, this.EatTokenWithPrejudice(SyntaxKind.DotToken));
                        }

                        break;

                    case SyntaxKind.MinusGreaterThanToken:
                        expr = _syntaxFactory.MemberAccessEx(SyntaxKind.PointerMemberAccessExpression, expr,
                            this.EatToken(), this.ParseSimpleName(NameOptions.InExpression));
                        break;
                    case SyntaxKind.DotToken:
                        // if we have the error situation:
                        //
                        //      expr.
                        //      X Y
                        //
                        // Then we don't want to parse this out as "Expr.X"
                        //
                        // It's far more likely the member access expression is simply incomplete and
                        // there is a new declaration on the next line.
                        if (this.CurrentToken.TrailingTrivia.Any((int)SyntaxKind.EndOfLineTrivia) &&
                            this.PeekToken(1).Kind == SyntaxKind.IdentifierToken &&
                            this.PeekToken(2).ContextualKind == SyntaxKind.IdentifierToken)
                        {
                            expr = _syntaxFactory.MemberAccessEx(
                                SyntaxKind.SimpleMemberAccessExpression, expr, this.EatToken(),
                                this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_IdentifierExpected));

                            return expr;
                        }

                        expr = _syntaxFactory.MemberAccessEx(SyntaxKind.SimpleMemberAccessExpression, expr,
                            this.EatToken(), this.ParseSimpleName(NameOptions.InExpression));
                        break;

                    // case SyntaxKind.QuestionToken:
                    //     if (CanStartConsequenceExpression(this.PeekToken(1).Kind))
                    //     {
                    //         var qToken = this.EatToken();
                    //         var consequence = ParseConsequenceSyntax();
                    //         expr = _syntaxFactory.ConditionalAccessExpression(expr, qToken, consequence);
                    //         expr = CheckFeatureAvailability(expr, MessageID.IDS_FeatureNullPropagatingOperator);
                    //         break;
                    //     }
                    //
                    //     goto default;

                    case SyntaxKind.ExclamationToken:
                        expr = _syntaxFactory.PostfixUnaryEx(SyntaxFacts.GetPostfixUnaryExpression(tk), expr,
                            this.EatToken());
                        expr = CheckFeatureAvailability(expr, MessageID.IDS_FeatureNullableReferenceTypes);
                        break;

                    case SyntaxKind.OpenBraceToken when expr is TypeEx typeEx:
                        expr = _syntaxFactory.AllocEx(typeEx, this.ParseObjectOrCollectionInitializer());
                        break;


                    default:
                        return expr;
                }
            }
        }


        private static bool CanStartConsequenceExpression(SyntaxKind kind)
        {
            return kind == SyntaxKind.DotToken ||
                   kind == SyntaxKind.OpenBracketToken;
        }

        internal ArgumentListSyntax ParseParenthesizedArgumentList()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.ArgumentList)
            {
                return (ArgumentListSyntax)this.EatNode();
            }

            ParseArgumentList(
                openToken: out SyntaxToken openToken,
                arguments: out SeparatedSyntaxList<ArgumentSyntax> arguments,
                closeToken: out SyntaxToken closeToken,
                openKind: SyntaxKind.OpenParenToken,
                closeKind: SyntaxKind.CloseParenToken);
            return _syntaxFactory.ArgumentList(openToken, arguments, closeToken);
        }

        internal BracketedArgumentListSyntax ParseBracketedArgumentList()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.BracketedArgumentList)
            {
                return (BracketedArgumentListSyntax)this.EatNode();
            }

            ParseArgumentList(
                openToken: out SyntaxToken openToken,
                arguments: out SeparatedSyntaxList<ArgumentSyntax> arguments,
                closeToken: out SyntaxToken closeToken,
                openKind: SyntaxKind.OpenBracketToken,
                closeKind: SyntaxKind.CloseBracketToken);
            return _syntaxFactory.BracketedArgumentList(openToken, arguments, closeToken);
        }

        private void ParseArgumentList(
            out SyntaxToken openToken,
            out SeparatedSyntaxList<ArgumentSyntax> arguments,
            out SyntaxToken closeToken,
            SyntaxKind openKind,
            SyntaxKind closeKind)
        {
            Debug.Assert(openKind == SyntaxKind.OpenParenToken || openKind == SyntaxKind.OpenBracketToken);
            Debug.Assert(closeKind == SyntaxKind.CloseParenToken || closeKind == SyntaxKind.CloseBracketToken);
            Debug.Assert((openKind == SyntaxKind.OpenParenToken) == (closeKind == SyntaxKind.CloseParenToken));
            bool isIndexer = openKind == SyntaxKind.OpenBracketToken;

            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken ||
                this.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
            {
                // convert `[` into `(` or vice versa for error recovery
                openToken = this.EatTokenAsKind(openKind);
            }
            else
            {
                openToken = this.EatToken(openKind);
            }

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfArgumentList;

            SeparatedSyntaxListBuilder<ArgumentSyntax> list = default(SeparatedSyntaxListBuilder<ArgumentSyntax>);
            try
            {
                if (this.CurrentToken.Kind != closeKind && this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    tryAgain:
                    if (list.IsNull)
                    {
                        list = _pool.AllocateSeparated<ArgumentSyntax>();
                    }

                    if (this.IsPossibleArgumentExpression() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        // first argument
                        list.Add(this.ParseArgumentExpression(isIndexer));

                        // additional arguments
                        var lastTokenPosition = -1;
                        while (IsMakingProgress(ref lastTokenPosition))
                        {
                            if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken ||
                                this.CurrentToken.Kind == SyntaxKind.CloseBracketToken ||
                                this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                            {
                                break;
                            }
                            else if (this.CurrentToken.Kind == SyntaxKind.CommaToken ||
                                     this.IsPossibleArgumentExpression())
                            {
                                list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                                list.Add(this.ParseArgumentExpression(isIndexer));
                                continue;
                            }
                            else if (this.SkipBadArgumentListTokens(ref openToken, list, SyntaxKind.CommaToken,
                                         closeKind) == PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                    }
                    else if (this.SkipBadArgumentListTokens(ref openToken, list, SyntaxKind.IdentifierToken,
                                 closeKind) == PostSkipAction.Continue)
                    {
                        goto tryAgain;
                    }
                }
                else if (isIndexer && this.CurrentToken.Kind == closeKind)
                {
                    // An indexer always expects at least one value. And so we need to give an error
                    // for the case where we see only "[]". ParseArgumentExpression gives it.

                    if (list.IsNull)
                    {
                        list = _pool.AllocateSeparated<ArgumentSyntax>();
                    }

                    list.Add(this.ParseArgumentExpression(isIndexer));
                }

                _termState = saveTerm;

                if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken ||
                    this.CurrentToken.Kind == SyntaxKind.CloseBracketToken)
                {
                    // convert `]` into `)` or vice versa for error recovery
                    closeToken = this.EatTokenAsKind(closeKind);
                }
                else
                {
                    closeToken = this.EatToken(closeKind);
                }

                arguments = list.ToList();
            }
            finally
            {
                if (!list.IsNull)
                {
                    _pool.Free(list);
                }
            }
        }


        private PostSkipAction SkipBadArgumentListTokens(ref SyntaxToken open,
            SeparatedSyntaxListBuilder<ArgumentSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref open, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleArgumentExpression(),
                p => p.CurrentToken.Kind == closeKind || p.CurrentToken.Kind == SyntaxKind.SemicolonToken ||
                     p.IsTerminator(),
                expected);
        }

        private bool IsEndOfArgumentList()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                   || this.CurrentToken.Kind == SyntaxKind.CloseBracketToken;
        }

        private bool IsPossibleArgumentExpression()
        {
            return IsValidArgumentRefKindKeyword(this.CurrentToken.Kind) || this.IsPossibleExpression();
        }

        private static bool IsValidArgumentRefKindKeyword(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private ArgumentSyntax ParseArgumentExpression(bool isIndexer)
        {
            NameColonSyntax nameColon = null;
            if (this.CurrentToken.Kind == SyntaxKind.IdentifierToken && this.PeekToken(1).Kind == SyntaxKind.ColonToken)
            {
                var name = this.ParseIdentifierName();
                var colon = this.EatToken(SyntaxKind.ColonToken);
                nameColon = _syntaxFactory.NameColon(name, colon);
                nameColon = CheckFeatureAvailability(nameColon, MessageID.IDS_FeatureNamedArgument);
            }

            SyntaxToken refKindKeyword = null;

            ExprSyntax expression;

            if (isIndexer && (this.CurrentToken.Kind == SyntaxKind.CommaToken ||
                              this.CurrentToken.Kind == SyntaxKind.CloseBracketToken))
            {
                expression = this.ParseIdentifierName(ErrorCode.ERR_ValueExpected);
            }
            else if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
            {
                expression = this.ParseIdentifierName(ErrorCode.ERR_MissingArgument);
            }
            else
            {
                if (refKindKeyword?.Kind == SyntaxKind.InKeyword)
                {
                    refKindKeyword =
                        this.CheckFeatureAvailability(refKindKeyword, MessageID.IDS_FeatureReadOnlyReferences);
                }

                // According to Language Specification, section 7.6.7 Element access
                //      The argument-list of an element-access is not allowed to contain ref or out arguments.
                // However, we actually do support ref indexing of indexed properties in COM interop
                // scenarios, and when indexing an object of static type "dynamic". So we enforce
                // that the ref/out of the argument must match the parameter when binding the argument list.

                expression = ParseSubExpression(Precedence.Expression);
            }

            return _syntaxFactory.Argument(nameColon, refKindKeyword, expression);
        }

        private bool ScanParenthesizedImplicitlyTypedLambda(Precedence precedence)
        {
            Debug.Assert(CurrentToken.Kind == SyntaxKind.OpenParenToken);

            if (!(precedence <= Precedence.Lambda))
            {
                return false;
            }

            //  case 1:  ( x ,
            if (this.PeekToken(1).Kind == SyntaxKind.IdentifierToken
                && (!this.IsInQuery || !IsTokenQueryContextualKeyword(this.PeekToken(1)))
                && this.PeekToken(2).Kind == SyntaxKind.CommaToken)
            {
                // Make sure it really looks like a lambda, not just a tuple
                int curTk = 3;
                while (true)
                {
                    var tk = this.PeekToken(curTk++);

                    // skip  identifiers commas and predefined types in any combination for error recovery
                    if (tk.Kind != SyntaxKind.IdentifierToken
                        && !SyntaxFacts.IsPredefinedType(tk.Kind)
                        && tk.Kind != SyntaxKind.CommaToken
                        && (this.IsInQuery || !IsTokenQueryContextualKeyword(tk)))
                    {
                        break;
                    }

                    ;
                }

                // ) =>
                return this.PeekToken(curTk - 1).Kind == SyntaxKind.CloseParenToken &&
                       this.PeekToken(curTk).Kind == SyntaxKind.EqualsGreaterThanToken;
            }

            //  case 2:  ( x ) =>
            if (IsTrueIdentifier(this.PeekToken(1))
                && this.PeekToken(2).Kind == SyntaxKind.CloseParenToken
                && this.PeekToken(3).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }

            //  case 3:  ( ) =>
            if (this.PeekToken(1).Kind == SyntaxKind.CloseParenToken
                && this.PeekToken(2).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }

            // case 4:  ( params
            // This case is interesting in that it is not legal; this error could be caught at parse time but we would rather
            // recover from the error and let the semantic analyzer deal with it.
            if (this.PeekToken(1).Kind == SyntaxKind.ParamsKeyword)
            {
                return true;
            }

            return false;
        }

        private bool ScanCast(bool forPattern = false)
        {
            if (this.CurrentToken.Kind != SyntaxKind.OpenParenToken)
            {
                return false;
            }

            this.EatToken();

            var type = this.ScanType(forPattern: forPattern);
            if (type == ScanTypeFlags.NotType)
            {
                return false;
            }

            if (this.CurrentToken.Kind != SyntaxKind.CloseParenToken)
            {
                return false;
            }

            this.EatToken();

            if (forPattern && this.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                // In a pattern, an identifier can follow a cast unless it's a binary pattern token.
                return !isBinaryPattern();
            }

            switch (type)
            {
                // If we have any of the following, we know it must be a cast:
                // 1) (Goo*)bar;
                // 2) (Goo?)bar;
                // 3) "(int)bar" or "(int[])bar"
                // 4) (G::Goo)bar
                case ScanTypeFlags.PointerOrMultiplication:
                case ScanTypeFlags.NullableType:
                case ScanTypeFlags.MustBeType:
                case ScanTypeFlags.AliasQualifiedName:
                    // The thing between parens is unambiguously a type.
                    // In a pattern, we need more lookahead to confirm it is a cast and not
                    // a parenthesized type pattern.  In this case the tokens that
                    // have both unary and binary operator forms may appear in their unary form
                    // following a cast.
                    return !forPattern || this.CurrentToken.Kind switch
                    {
                        SyntaxKind.PlusToken or
                            SyntaxKind.MinusToken or
                            SyntaxKind.AmpersandToken or
                            SyntaxKind.AsteriskToken or
                            SyntaxKind.DotDotToken => true,
                        var tk => CanFollowCast(tk)
                    };

                case ScanTypeFlags.GenericTypeOrMethod:
                case ScanTypeFlags.GenericTypeOrExpression:
                case ScanTypeFlags.NonGenericTypeOrExpression:
                case ScanTypeFlags.TupleType:
                    // check for ambiguous type or expression followed by disambiguating token.  i.e.
                    //
                    // "(A)b" is a cast.  But "(A)+b" is not a cast.  
                    return CanFollowCast(this.CurrentToken.Kind);

                default:
                    throw ExceptionUtilities.UnexpectedValue(type);
            }

            bool isBinaryPattern()
            {
                if (!isBinaryPatternKeyword())
                {
                    return false;
                }

                bool lastTokenIsBinaryOperator = true;

                EatToken();
                while (isBinaryPatternKeyword())
                {
                    // If we see a subsequent binary pattern token, it can't be an operator.
                    // Later, it will be parsed as an identifier.
                    lastTokenIsBinaryOperator = !lastTokenIsBinaryOperator;
                    EatToken();
                }

                // In case a combinator token is used as a constant, we explicitly check that a pattern is NOT followed.
                // Such as `(e is (int)or or >= 0)` versus `(e is (int) or or)`
                return lastTokenIsBinaryOperator == false;
            }

            bool isBinaryPatternKeyword()
            {
                return this.CurrentToken.ContextualKind is SyntaxKind.OrKeyword or SyntaxKind.AndKeyword;
            }
        }

        private static bool CanFollowCast(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.AsKeyword:
                case SyntaxKind.IsKeyword:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.EqualsToken:
                case SyntaxKind.PlusEqualsToken:
                case SyntaxKind.MinusEqualsToken:
                case SyntaxKind.AsteriskEqualsToken:
                case SyntaxKind.SlashEqualsToken:
                case SyntaxKind.PercentEqualsToken:
                case SyntaxKind.AmpersandEqualsToken:
                case SyntaxKind.CaretEqualsToken:
                case SyntaxKind.BarEqualsToken:
                case SyntaxKind.LessThanLessThanEqualsToken:
                case SyntaxKind.GreaterThanGreaterThanEqualsToken:
                case SyntaxKind.QuestionToken:
                case SyntaxKind.ColonToken:
                case SyntaxKind.BarBarToken:
                case SyntaxKind.AmpersandAmpersandToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.QuestionQuestionEqualsToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.PercentToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.DotToken:
                case SyntaxKind.MinusGreaterThanToken:
                case SyntaxKind.QuestionQuestionToken:
                case SyntaxKind.EndOfFileToken:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.EqualsGreaterThanToken:
                case SyntaxKind.DotDotToken:
                    return false;
                default:
                    return true;
            }
        }

        private bool IsAnonymousType()
        {
            return this.CurrentToken.Kind == SyntaxKind.NewKeyword &&
                   this.PeekToken(1).Kind == SyntaxKind.OpenBraceToken;
        }

        private bool IsInitializerMember()
        {
            return this.IsComplexElementInitializer() ||
                   this.IsNamedAssignment() ||
                   this.IsDictionaryInitializer() ||
                   this.IsPossibleExpression();
        }

        private bool IsComplexElementInitializer()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private bool IsNamedAssignment()
        {
            return IsTrueIdentifier() && this.PeekToken(1).Kind == SyntaxKind.EqualsToken;
        }

        private bool IsDictionaryInitializer()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBracketToken;
        }


        private bool IsImplicitObjectCreation()
        {
            // The caller is expected to have consumed the new keyword.
            if (this.CurrentToken.Kind != SyntaxKind.OpenParenToken)
            {
                return false;
            }

            var point = this.GetResetPoint();
            try
            {
                this.EatToken(); // open paren
                ScanTypeFlags scanTypeFlags = ScanTupleType(out _);
                if (scanTypeFlags != ScanTypeFlags.NotType)
                {
                    switch (this.CurrentToken.Kind)
                    {
                        case SyntaxKind.QuestionToken: // e.g. `new(a, b)?()`
                        case SyntaxKind.OpenBracketToken: // e.g. `new(a, b)[]`
                        case SyntaxKind.OpenParenToken: // e.g. `new(a, b)()` for better error recovery
                            return false;
                    }
                }

                return true;
            }
            finally
            {
                this.Reset(ref point);
                this.Release(ref point);
            }
        }


        private InitializerEx ParseObjectOrCollectionInitializer()
        {
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

            var initializers = _pool.AllocateSeparated<ExprSyntax>();
            try
            {
                bool isObjectInitializer;
                this.ParseObjectOrCollectionInitializerMembers(ref openBrace, initializers, out isObjectInitializer);
                Debug.Assert(initializers.Count > 0 || isObjectInitializer);

                openBrace = CheckFeatureAvailability(openBrace,
                    isObjectInitializer
                        ? MessageID.IDS_FeatureObjectInitializer
                        : MessageID.IDS_FeatureCollectionInitializer);

                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
                return _syntaxFactory.InitializerEx(
                    isObjectInitializer
                        ? SyntaxKind.ObjectInitializerExpression
                        : SyntaxKind.CollectionInitializerExpression,
                    openBrace,
                    initializers,
                    closeBrace);
            }
            finally
            {
                _pool.Free(initializers);
            }
        }

        private void ParseObjectOrCollectionInitializerMembers(ref SyntaxToken startToken,
            SeparatedSyntaxListBuilder<ExprSyntax> list, out bool isObjectInitializer)
        {
            // Empty initializer list must be parsed as an object initializer.
            isObjectInitializer = true;

            if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
                tryAgain:
                if (this.IsInitializerMember() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // We have at least one initializer expression.
                    // If at least one initializer expression is a named assignment, this is an object initializer.
                    // Otherwise, this is a collection initializer.
                    isObjectInitializer = false;

                    // first argument
                    list.Add(this.ParseObjectOrCollectionInitializerMember(ref isObjectInitializer));

                    // additional arguments
                    int lastTokenPosition = -1;
                    while (IsMakingProgress(ref lastTokenPosition))
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsInitializerMember())
                        {
                            list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));

                            // check for exit case after legal trailing comma
                            if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                break;
                            }

                            list.Add(this.ParseObjectOrCollectionInitializerMember(ref isObjectInitializer));
                            continue;
                        }
                        else if (this.SkipBadInitializerListTokens(ref startToken, list, SyntaxKind.CommaToken) ==
                                 PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadInitializerListTokens(ref startToken, list, SyntaxKind.IdentifierToken) ==
                         PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }

            // We may have invalid initializer elements. These will be reported during binding.
        }


        private ExprSyntax ParseObjectOrCollectionInitializerMember(ref bool isObjectInitializer)
        {
            if (this.IsComplexElementInitializer())
            {
                return this.ParseComplexElementInitializer();
            }
            else if (IsDictionaryInitializer())
            {
                isObjectInitializer = true;
                var initializer = this.ParseDictionaryInitializer();
                initializer = CheckFeatureAvailability(initializer, MessageID.IDS_FeatureDictionaryInitializer);
                return initializer;
            }
            else if (this.IsNamedAssignment())
            {
                isObjectInitializer = true;
                return this.ParseObjectInitializerNamedAssignment();
            }
            else
            {
                return this.ParseExpressionCore();
            }
        }

        private PostSkipAction SkipBadInitializerListTokens<T>(ref SyntaxToken startToken,
            SeparatedSyntaxListBuilder<T> list, SyntaxKind expected)
            where T : AquilaSyntaxNode
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleExpression(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(),
                expected);
        }


        private ExprSyntax ParseObjectInitializerNamedAssignment()
        {
            var identifier = this.ParseIdentifierName();
            var equal = this.EatToken(SyntaxKind.EqualsToken);
            ExprSyntax expression;
            if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                expression = this.ParseObjectOrCollectionInitializer();
            }
            else
            {
                expression = this.ParseExpressionCore();
            }

            return _syntaxFactory.AssignEx(SyntaxKind.SimpleAssignmentExpression, identifier, equal, expression);
        }

        private ExprSyntax ParseDictionaryInitializer()
        {
            throw new NotImplementedException();
            // var arguments = this.ParseBracketedArgumentList();
            // var equal = this.EatToken(SyntaxKind.EqualsToken);
            // var expression = this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
            //     ? this.ParseObjectOrCollectionInitializer()
            //     : this.ParseExpressionCore();
            //
            // var elementAccess = _syntaxFactory.ImplicitElementAccess(arguments);
            // return _syntaxFactory.AssignEx(
            //     SyntaxKind.SimpleAssignmentExpression, elementAccess, equal, expression);
        }

        private InitializerEx ParseComplexElementInitializer()
        {
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);
            var initializers = _pool.AllocateSeparated<ExprSyntax>();
            try
            {
                DiagnosticInfo closeBraceError;
                this.ParseExpressionsForComplexElementInitializer(ref openBrace, initializers, out closeBraceError);
                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
                if (closeBraceError != null)
                {
                    closeBrace = WithAdditionalDiagnostics(closeBrace, closeBraceError);
                }

                return _syntaxFactory.InitializerEx(SyntaxKind.ComplexElementInitializerExpression, openBrace,
                    initializers, closeBrace);
            }
            finally
            {
                _pool.Free(initializers);
            }
        }

        private void ParseExpressionsForComplexElementInitializer(ref SyntaxToken openBrace,
            SeparatedSyntaxListBuilder<ExprSyntax> list, out DiagnosticInfo closeBraceError)
        {
            closeBraceError = null;

            if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
                tryAgain:
                if (this.IsPossibleExpression() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // first argument
                    list.Add(this.ParseExpressionCore());

                    // additional arguments
                    int lastTokenPosition = -1;
                    while (IsMakingProgress(ref lastTokenPosition))
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleExpression())
                        {
                            list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                            if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                closeBraceError = MakeError(this.CurrentToken, ErrorCode.ERR_ExpressionExpected);
                                break;
                            }

                            list.Add(this.ParseExpressionCore());
                            continue;
                        }
                        else if (this.SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.CommaToken) ==
                                 PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.IdentifierToken) ==
                         PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }
        }

        private bool IsImplicitlyTypedArray()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.NewKeyword ||
                         this.CurrentToken.Kind == SyntaxKind.StackAllocKeyword);
            return this.PeekToken(1).Kind == SyntaxKind.OpenBracketToken;
        }


        private SyntaxList<SyntaxToken> ParseAnonymousFunctionModifiers()
        {
            var modifiers = _pool.Allocate();

            while (true)
            {
                if (this.CurrentToken.Kind == SyntaxKind.StaticKeyword)
                {
                    var staticKeyword = this.EatToken(SyntaxKind.StaticKeyword);
                    staticKeyword =
                        CheckFeatureAvailability(staticKeyword, MessageID.IDS_FeatureStaticAnonymousFunction);
                    modifiers.Add(staticKeyword);
                    continue;
                }

                if (this.CurrentToken.ContextualKind == SyntaxKind.AsyncKeyword &&
                    IsAnonymousFunctionAsyncModifier())
                {
                    var asyncToken = this.EatContextualToken(SyntaxKind.AsyncKeyword);
                    asyncToken = CheckFeatureAvailability(asyncToken, MessageID.IDS_FeatureAsync);
                    modifiers.Add(asyncToken);
                    continue;
                }

                break;
            }

            var result = modifiers.ToList();
            _pool.Free(modifiers);
            return result;
        }

        private InitializerEx ParseArrayInitializer()
        {
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

            // NOTE:  This loop allows " { <initexpr>, } " but not " { , } "
            var list = _pool.AllocateSeparated<ExprSyntax>();
            try
            {
                if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    tryAgain:
                    if (this.IsPossibleVariableInitializer() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        list.Add(this.ParseVariableInitializer());

                        int lastTokenPosition = -1;
                        while (IsMakingProgress(ref lastTokenPosition))
                        {
                            if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                break;
                            }
                            else if (this.IsPossibleVariableInitializer() ||
                                     this.CurrentToken.Kind == SyntaxKind.CommaToken)
                            {
                                list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));

                                // check for exit case after legal trailing comma
                                if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                                {
                                    break;
                                }
                                else if (!this.IsPossibleVariableInitializer())
                                {
                                    goto tryAgain;
                                }

                                list.Add(this.ParseVariableInitializer());
                                continue;
                            }
                            else if (SkipBadArrayInitializerTokens(ref openBrace, list, SyntaxKind.CommaToken) ==
                                     PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                    }
                    else if (SkipBadArrayInitializerTokens(ref openBrace, list, SyntaxKind.CommaToken) ==
                             PostSkipAction.Continue)
                    {
                        goto tryAgain;
                    }
                }

                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);

                return _syntaxFactory.InitializerEx(SyntaxKind.ArrayInitializerExpression, openBrace, list,
                    closeBrace);
            }
            finally
            {
                _pool.Free(list);
            }
        }

        private PostSkipAction SkipBadArrayInitializerTokens(ref SyntaxToken openBrace,
            SeparatedSyntaxListBuilder<ExprSyntax> list, SyntaxKind expected)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref openBrace, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleVariableInitializer(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(),
                expected);
        }


        private bool IsAnonymousFunctionAsyncModifier()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.AsyncKeyword);

            switch (this.PeekToken(1).Kind)
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.DelegateKeyword:
                    return true;
                case var kind:
                    return IsPredefinedType(kind);
            }

            ;
        }

        /// <summary>
        /// Parse expected lambda expression but assume `x ? () => y :` is a conditional
        /// expression rather than a lambda expression with an explicit return type and
        /// return null in that case only.
        /// </summary>
        private bool IsPossibleLambdaParameter()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.ParamsKeyword:
                // params is not actually legal in a lambda, but we allow it for error
                // recovery purposes and then give an error during semantic analysis.
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.OpenParenToken: // tuple
                case SyntaxKind.OpenBracketToken: // attribute
                    return true;

                case SyntaxKind.IdentifierToken:
                    return this.IsTrueIdentifier();

                case SyntaxKind.DelegateKeyword:
                    return this.IsFunctionPointerStart();

                default:
                    return IsPredefinedType(this.CurrentToken.Kind);
            }
        }

        private PostSkipAction SkipBadLambdaParameterListTokens(ref SyntaxToken openParen,
            SeparatedSyntaxListBuilder<ParameterSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref openParen, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleLambdaParameter(),
                p => p.CurrentToken.Kind == closeKind || p.IsTerminator(),
                expected);
        }


        private bool ShouldParseLambdaParameterType(bool hasModifier)
        {
            // If we have "ref/out/in/params" always try to parse out a type.
            if (hasModifier)
            {
                return true;
            }

            // If we have "int/string/etc." always parse out a type.
            if (IsPredefinedType(this.CurrentToken.Kind))
            {
                return true;
            }

            // if we have a tuple type in a lambda.
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                return true;
            }

            if (this.IsFunctionPointerStart())
            {
                return true;
            }

            if (this.IsTrueIdentifier(this.CurrentToken))
            {
                // Don't parse out a type if we see:
                //
                //      (a,
                //      (a)
                //      (a =>
                //      (a {
                //
                // In all other cases, parse out a type.
                var peek1 = this.PeekToken(1);
                if (peek1.Kind != SyntaxKind.CommaToken &&
                    peek1.Kind != SyntaxKind.CloseParenToken &&
                    peek1.Kind != SyntaxKind.EqualsGreaterThanToken &&
                    peek1.Kind != SyntaxKind.OpenBraceToken)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsCurrentTokenQueryContextualKeyword
        {
            get { return IsTokenQueryContextualKeyword(this.CurrentToken); }
        }

        private static bool IsTokenQueryContextualKeyword(SyntaxToken token)
        {
            if (IsTokenStartOfNewQueryClause(token))
            {
                return true;
            }

            switch (token.ContextualKind)
            {
                case SyntaxKind.OnKeyword:
                case SyntaxKind.EqualsKeyword:
                case SyntaxKind.AscendingKeyword:
                case SyntaxKind.DescendingKeyword:
                case SyntaxKind.ByKeyword:
                    return true;
            }

            return false;
        }

        private static bool IsTokenStartOfNewQueryClause(SyntaxToken token)
        {
            switch (token.ContextualKind)
            {
                case SyntaxKind.FromKeyword:
                case SyntaxKind.JoinKeyword:
                case SyntaxKind.IntoKeyword:
                case SyntaxKind.WhereKeyword:
                case SyntaxKind.OrderByKeyword:
                case SyntaxKind.GroupKeyword:
                case SyntaxKind.SelectKeyword:
                case SyntaxKind.LetKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsQueryExpression(bool mayBeVariableDeclaration, bool mayBeMemberDeclaration)
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.FromKeyword)
            {
                return this.IsQueryExpressionAfterFrom(mayBeVariableDeclaration, mayBeMemberDeclaration);
            }

            return false;
        }

        // from_clause ::= from <type>? <identifier> in expression
        private bool IsQueryExpressionAfterFrom(bool mayBeVariableDeclaration, bool mayBeMemberDeclaration)
        {
            // from x ...
            var pk1 = this.PeekToken(1).Kind;
            if (IsPredefinedType(pk1))
            {
                return true;
            }

            if (pk1 == SyntaxKind.IdentifierToken)
            {
                var pk2 = this.PeekToken(2).Kind;
                if (pk2 == SyntaxKind.InKeyword)
                {
                    return true;
                }

                if (mayBeVariableDeclaration)
                {
                    if (pk2 == SyntaxKind.SemicolonToken || // from x;
                        pk2 == SyntaxKind.CommaToken || // from x, y;
                        pk2 == SyntaxKind.EqualsToken) // from x = null;
                    {
                        return false;
                    }
                }

                if (mayBeMemberDeclaration)
                {
                    // from idf { ...   property decl
                    // from idf(...     method decl
                    if (pk2 == SyntaxKind.OpenParenToken ||
                        pk2 == SyntaxKind.OpenBraceToken)
                    {
                        return false;
                    }

                    // otherwise we need to scan a type
                }
                else
                {
                    return true;
                }
            }

            // from T x ...
            var resetPoint = this.GetResetPoint();
            try
            {
                this.EatToken();

                ScanTypeFlags isType = this.ScanType();
                if (isType != ScanTypeFlags.NotType && (this.CurrentToken.Kind == SyntaxKind.IdentifierToken ||
                                                        this.CurrentToken.Kind == SyntaxKind.InKeyword))
                {
                    return true;
                }
            }
            finally
            {
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }

            return false;
        }

        [Obsolete("Use IsIncrementalAndFactoryContextMatches")]
        private new bool IsIncremental
        {
            get { throw new Exception("Use IsIncrementalAndFactoryContextMatches"); }
        }

        private bool IsIncrementalAndFactoryContextMatches
        {
            get
            {
                if (!base.IsIncremental)
                {
                    return false;
                }

                Aquila.CodeAnalysis.AquilaSyntaxNode current = this.CurrentNode;
                return current != null && MatchesFactoryContext(current.Green, _syntaxFactoryContext);
            }
        }

        internal static bool MatchesFactoryContext(GreenNode green, SyntaxFactoryContext context)
        {
            return context.IsInAsync == green.ParsedInAsync &&
                   context.IsInQuery == green.ParsedInQuery;
        }

        private bool IsInAsync
        {
            get { return _syntaxFactoryContext.IsInAsync; }
            set { _syntaxFactoryContext.IsInAsync = value; }
        }

        private bool IsInQuery
        {
            get { return _syntaxFactoryContext.IsInQuery; }
        }

        private void EnterQuery()
        {
            _syntaxFactoryContext.QueryDepth++;
        }

        private void LeaveQuery()
        {
            Debug.Assert(_syntaxFactoryContext.QueryDepth > 0);
            _syntaxFactoryContext.QueryDepth--;
        }

        private void EnterHtml()
        {
            _syntaxFactoryContext.HtmlDepth++;
        }

        private void LeaveHtml()
        {
            Debug.Assert(_syntaxFactoryContext.HtmlDepth > 0);
            _syntaxFactoryContext.HtmlDepth--;
        }

        private new ResetPoint GetResetPoint()
        {
            return new ResetPoint(
                base.GetResetPoint(),
                _termState,
                _isInTry,
                _syntaxFactoryContext.IsInAsync,
                _syntaxFactoryContext.QueryDepth,
                _syntaxFactoryContext.HtmlDepth);
        }

        private void Reset(ref ResetPoint state)
        {
            _termState = state.TerminatorState;
            _isInTry = state.IsInTry;
            _syntaxFactoryContext.IsInAsync = state.IsInAsync;
            _syntaxFactoryContext.QueryDepth = state.QueryDepth;
            base.Reset(ref state.BaseResetPoint);
        }

        private void Release(ref ResetPoint state)
        {
            base.Release(ref state.BaseResetPoint);
        }

        private new struct ResetPoint
        {
            internal SyntaxParser.ResetPoint BaseResetPoint;
            internal readonly TerminatorState TerminatorState;
            internal readonly bool IsInTry;
            internal readonly bool IsInAsync;
            internal readonly int QueryDepth;
            internal readonly int HtmlDepth;


            internal ResetPoint(
                SyntaxParser.ResetPoint resetPoint,
                TerminatorState terminatorState,
                bool isInTry,
                bool isInAsync,
                int queryDepth,
                int htmlDepth)
            {
                this.BaseResetPoint = resetPoint;
                this.TerminatorState = terminatorState;
                this.IsInTry = isInTry;
                this.IsInAsync = isInAsync;
                this.QueryDepth = queryDepth;
                this.HtmlDepth = htmlDepth;
            }
        }

        internal TNode ConsumeUnexpectedTokens<TNode>(TNode node) where TNode : AquilaSyntaxNode
        {
            if (this.CurrentToken.Kind == SyntaxKind.EndOfFileToken) return node;
            SyntaxListBuilder<SyntaxToken> b = _pool.Allocate<SyntaxToken>();
            while (this.CurrentToken.Kind != SyntaxKind.EndOfFileToken)
            {
                b.Add(this.EatToken());
            }

            var trailingTrash = b.ToList();
            _pool.Free(b);

            node = this.AddError(node, ErrorCode.ERR_UnexpectedToken, trailingTrash[0].ToString());
            node = this.AddTrailingSkippedSyntax(node, trailingTrash.Node);
            return node;
        }
    }
}