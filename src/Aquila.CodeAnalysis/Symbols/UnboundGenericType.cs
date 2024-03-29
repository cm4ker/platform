﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    internal static partial class TypeSymbolExtensions
    {
        static public NamedTypeSymbol AsUnboundGenericType(this NamedTypeSymbol type)
        {
            if (!type.IsGenericType)
            {
                // This exception is part of the public contract of NamedTypeSymbol.ConstructUnboundGenericType
                throw new InvalidOperationException();
            }

            var original = type.OriginalDefinition;
            int n = original.Arity;
            NamedTypeSymbol originalContainingType = original.ContainingType;

            var constructedFrom = ((object)originalContainingType == null) ?
                original :
                original.AsMember(originalContainingType.IsGenericType ? originalContainingType.AsUnboundGenericType() : originalContainingType);
            if (n == 0)
            {
                return constructedFrom;
            }

            var typeArguments = UnboundArgumentErrorTypeSymbol.CreateTypeArguments(
                constructedFrom.TypeParameters,
                n,
                null);
            return constructedFrom.Construct(typeArguments, unbound: true);
        }
    }

    internal sealed class UnboundArgumentErrorTypeSymbol : ErrorTypeSymbol
    {
        public static ImmutableArray<TypeWithModifiers> CreateTypeArguments(ImmutableArray<TypeParameterSymbol> typeParameters, int n, DiagnosticInfo errorInfo)
        {
            var result = ArrayBuilder<TypeWithModifiers>.GetInstance();
            for (int i = 0; i < n; i++)
            {
                string name = (i < typeParameters.Length) ? typeParameters[i].Name : string.Empty;
                result.Add(new TypeWithModifiers(new UnboundArgumentErrorTypeSymbol(name, errorInfo)));
            }
            return result.ToImmutableAndFree();
        }

        public static readonly ErrorTypeSymbol Instance = new UnboundArgumentErrorTypeSymbol(string.Empty, null/*new CSDiagnosticInfo(ErrorCode.ERR_UnexpectedUnboundGenericName)*/);

        private readonly string _name;
        private readonly DiagnosticInfo _errorInfo;

        public override CandidateReason CandidateReason => CandidateReason.None;

        private UnboundArgumentErrorTypeSymbol(string name, DiagnosticInfo errorInfo)
        {
            _name = name;
            _errorInfo = errorInfo;
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        internal override bool MangleName
        {
            get
            {
                Debug.Assert(Arity == 0);
                return false;
            }
        }

        internal override bool Equals(TypeSymbol t2, bool ignoreCustomModifiersAndArraySizesAndLowerBounds, bool ignoreDynamic)
        {
            if ((object)t2 == (object)this)
            {
                return true;
            }

            UnboundArgumentErrorTypeSymbol other = t2 as UnboundArgumentErrorTypeSymbol;
            return (object)other != null && string.Equals(other._name, _name, StringComparison.Ordinal) && object.Equals(other._errorInfo, _errorInfo);
        }

        public override int GetHashCode()
        {
            return _errorInfo == null
                ? _name.GetHashCode()
                : Hash.Combine(_name, _errorInfo.Code);
        }
    }
}
