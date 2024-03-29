﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Aquila.CodeAnalysis.Syntax;

namespace Aquila.CodeAnalysis.Syntax
{
    partial class NameColonSyntax
    {
        public override ExprSyntax Expression => Name;

        internal override BaseExpressionColonSyntax WithExpressionCore(ExprSyntax expression)
        {
            if (expression is IdentifierEx identifierName)
                return WithName(identifierName);
            return SyntaxFactory.ExprColon(expression, this.ColonToken);
        }
    }
}

namespace Aquila.CodeAnalysis
{
    public partial class SyntaxFactory
    {
        public static NameColonSyntax NameColon(IdentifierEx name)
            => NameColon(name, Token(SyntaxKind.ColonToken));

        public static NameColonSyntax NameColon(string name)
            => NameColon(IdentifierName(name));
    }
}
