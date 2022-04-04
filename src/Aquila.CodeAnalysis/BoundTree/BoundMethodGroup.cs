// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Syntax;
using Auila.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    internal sealed partial class BoundMethodGroup : BoundMethodOrPropertyGroup
    {
        public BoundMethodGroup(
            SyntaxNode syntax,
            ImmutableArray<TypeWithAnnotations> typeArgumentsOpt,
            BoundExpression receiverOpt,
            string name,
            ImmutableArray<MethodSymbol> methods,
            LookupResult lookupResult,
            BoundMethodGroupFlags flags,
            Binder binder,
            bool hasErrors = false)
            : this(syntax, typeArgumentsOpt, name, methods, lookupResult.SingleSymbolOrDefault, lookupResult.Error,
                flags, functionType: GetLazyFunctionType(binder, syntax), receiverOpt, lookupResult.Kind, hasErrors)
        {
            FunctionType?.SetExpression(this);
        }

        private static FunctionTypeSymbol.Lazy? GetLazyFunctionType(Binder binder, SyntaxNode syntax)
        {
            throw new NotImplementedException();
            //return FunctionTypeSymbol.Lazy.CreateIfFeatureEnabled(syntax, binder, static (binder, expr) => binder.GetMethodGroupDelegateType((BoundMethodGroup)expr));
        }

        public MemberAccessEx? MemberAccessExpressionSyntax
        {
            get { return this.Syntax as MemberAccessEx; }
        }

        public SyntaxNode NameSyntax
        {
            get
            {
                var memberAccess = this.MemberAccessExpressionSyntax;
                if (memberAccess != null)
                {
                    return memberAccess.Name;
                }
                else
                {
                    return this.Syntax;
                }
            }
        }

        public BoundExpression? InstanceOpt
        {
            get
            {
                if (this.ReceiverOpt == null || this.ReceiverOpt.Kind == BoundKind.TypeExpression)
                {
                    return null;
                }
                else
                {
                    return this.ReceiverOpt;
                }
            }
        }

        public bool SearchExtensionMethods
        {
            get { return (this.Flags & BoundMethodGroupFlags.SearchExtensionMethods) != 0; }
        }
    }
}