// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;
using static Aquila.CodeAnalysis.SyntaxKind;

namespace Aquila.CodeAnalysis
{
    public static partial class SyntaxFacts
    {
        /// <summary>
        /// Is the node the name of a named argument of an invocation, object creation expression, 
        /// constructor initializer, or element access, but not an attribute.
        /// </summary>
        public static bool IsNamedArgumentName(SyntaxNode node)
        {
            // An argument name is an IdentifierName inside a NameColon, inside an Argument, inside an ArgumentList, inside an
            // Invocation, ObjectCreation, ObjectInitializer, ElementAccess or Subpattern.
        
            if (!node.IsKind(IdentifierName))
            {
                return false;
            }
        
            var parent1 = node.Parent;
            if (parent1 == null || !parent1.IsKind(NameColon))
            {
                return false;
            }
        
            var parent2 = parent1.Parent;
            if (parent2.IsKind(SyntaxKind.Subpattern))
            {
                return true;
            }
        
            if (parent2 == null || !(parent2.IsKind(Argument) || parent2.IsKind(AttributeArgument)))
            {
                return false;
            }
        
            var parent3 = parent2.Parent;
            if (parent3 == null)
            {
                return false;
            }
        
            if (parent3.IsKind(SyntaxKind.TupleExpression))
            {
                return true;
            }
        
            if (!(parent3 is BaseArgumentListSyntax || parent3.IsKind(AttributeArgumentList)))
            {
                return false;
            }
        
            var parent4 = parent3.Parent;
            if (parent4 == null)
            {
                return false;
            }
        
            switch (parent4.Kind())
            {
                case InvocationExpression:
                case TupleExpression:
                case ObjectCreationExpression:
                case ImplicitObjectCreationExpression:
                case ObjectInitializerExpression:
                case ElementAccessExpression:
                case Attribute:
                case BaseConstructorInitializer:
                case ThisConstructorInitializer:
                case PrimaryConstructorBaseType:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is the expression the initializer in a fixed statement?
        /// </summary>
        public static bool IsFixedStatementExpression(SyntaxNode node)
        {
            var current = node.Parent;
            // Dig through parens because dev10 does (even though the spec doesn't say so)
            // Dig through casts because there's a special error code (CS0254) for such casts.
            while (current != null && (current.IsKind(ParenthesizedExpression) || current.IsKind(CastExpression))) current = current.Parent;
            if (current == null || !current.IsKind(EqualsValueClause)) return false;
            current = current.Parent;
            if (current == null || !current.IsKind(VariableDeclarator)) return false;
            current = current.Parent;
            if (current == null || !current.IsKind(VariableDeclaration)) return false;
            current = current.Parent;
            return current != null && current.IsKind(FixedStatement);
        }

        public static string GetText(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.NotApplicable:
                    return string.Empty;
                case Accessibility.Private:
                    return SyntaxFacts.GetText(PrivateKeyword);
                case Accessibility.ProtectedAndInternal:
                    return SyntaxFacts.GetText(PrivateKeyword) + " " + SyntaxFacts.GetText(ProtectedKeyword);
                case Accessibility.Internal:
                    return SyntaxFacts.GetText(InternalKeyword);
                case Accessibility.Protected:
                    return SyntaxFacts.GetText(ProtectedKeyword);
                case Accessibility.ProtectedOrInternal:
                    return SyntaxFacts.GetText(ProtectedKeyword) + " " + SyntaxFacts.GetText(InternalKeyword);
                case Accessibility.Public:
                    return SyntaxFacts.GetText(PubKeyword);
                default:
                    throw ExceptionUtilities.UnexpectedValue(accessibility);
            }
        }

        internal static bool IsIdentifierVar(this Syntax.InternalSyntax.SyntaxToken node)
        {
            return node.ContextualKind == SyntaxKind.VarKeyword;
        }

        internal static bool IsNestedFunction(SyntaxNode child)
        {
            switch (child.Kind())
            {
                case SyntaxKind.LocalFunctionStatement:
                case SyntaxKind.AnonymousMethodExpression:
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return true;
                default:
                    return false;
            }
        }
    }
}
