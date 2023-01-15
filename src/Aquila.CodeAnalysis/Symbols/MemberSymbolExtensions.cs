using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// SymbolExtensions for member symbols.
    /// </summary>
    internal static partial class SymbolExtensions
    {
        internal static bool HasParamsParameter(this Symbol member)
        {
            var @params = member.GetParameters();
            return !@params.IsEmpty && @params.Last().IsParams;
        }

        /// <summary>
        /// Get the parameters of a member symbol.  Should be a method, property, or event.
        /// </summary>
        internal static ImmutableArray<ParameterSymbol> GetParameters(this Symbol member)
        {
            switch (member.Kind)
            {
                case SymbolKind.Method:
                    return ((MethodSymbol)member).Parameters;
                case SymbolKind.Property:
                    return ((PropertySymbol)member).Parameters;
                case SymbolKind.Event:
                    return ImmutableArray<ParameterSymbol>.Empty;
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.Kind);
            }
        }

        internal static bool GetIsVararg(this Symbol member)
        {
            switch (member.Kind)
            {
                case SymbolKind.Method:
                    return ((MethodSymbol)member).IsVararg;
                case SymbolKind.Property:
                case SymbolKind.Event:
                    return false;
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.Kind);
            }
        }

        internal static int GetParameterCount(this Symbol member)
        {
            switch (member.Kind)
            {
                case SymbolKind.Method:
                    return ((MethodSymbol)member).ParameterCount;
                case SymbolKind.Property:
                    return ((PropertySymbol)member).ParameterCount;
                case SymbolKind.Event:
                    return 0;
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.Kind);
            }
        }


        public static bool IsAccessor(this MethodSymbol methodSymbol)
        {
            return (object)methodSymbol.AssociatedSymbol != null;
        }

        public static bool IsAccessor(this Symbol symbol)
        {
            return symbol.Kind == SymbolKind.Method && IsAccessor((MethodSymbol)symbol);
        }

        public static bool IsOperator(this MethodSymbol methodSymbol)
        {
            return methodSymbol.MethodKind == MethodKind.UserDefinedOperator ||
                   methodSymbol.MethodKind == MethodKind.Conversion;
        }

        public static bool IsOperator(this Symbol symbol)
        {
            return symbol.Kind == SymbolKind.Method && IsOperator((MethodSymbol)symbol);
        }

        public static bool IsIndexer(this Symbol symbol)
        {
            return symbol.Kind == SymbolKind.Property && ((PropertySymbol)symbol).IsIndexer;
        }

        public static bool IsIndexedProperty(this Symbol symbol)
        {
            return symbol.Kind == SymbolKind.Property && ((PropertySymbol)symbol).IsIndexedProperty;
        }

        public static bool IsUserDefinedConversion(this Symbol symbol)
        {
            return symbol.Kind == SymbolKind.Method && ((MethodSymbol)symbol).MethodKind == MethodKind.Conversion;
        }

        internal static Symbol SymbolAsMember(this Symbol s, NamedTypeSymbol newOwner)
        {
            switch (s.Kind)
            {
                case SymbolKind.Field:
                    return ((FieldSymbol)s).AsMember(newOwner);
                case SymbolKind.Method:
                    return ((MethodSymbol)s).AsMember(newOwner);
                case SymbolKind.NamedType:
                    return ((NamedTypeSymbol)s).AsMember(newOwner);
                case SymbolKind.Property:
                    return ((PropertySymbol)s).AsMember(newOwner);
                default:
                    throw ExceptionUtilities.UnexpectedValue(s.Kind);
            }
        }

        /// <summary>
        /// Return the arity of a member.
        /// </summary>
        internal static int GetMemberArity(this Symbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    return ((MethodSymbol)symbol).Arity;

                case SymbolKind.NamedType:
                case SymbolKind.ErrorType:
                    return ((NamedTypeSymbol)symbol).Arity;

                default:
                    return 0;
            }
        }

        internal static NamespaceOrTypeSymbol OfMinimalArity(this IEnumerable<NamespaceOrTypeSymbol> symbols)
        {
            NamespaceOrTypeSymbol minAritySymbol = null;
            int minArity = Int32.MaxValue;
            foreach (var symbol in symbols)
            {
                int arity = GetMemberArity(symbol);
                if (arity < minArity)
                {
                    minArity = arity;
                    minAritySymbol = symbol;
                }
            }

            return minAritySymbol;
        }

        /// <summary>
        /// NOTE: every struct has a public parameterless constructor either used-defined or default one
        /// </summary>
        internal static bool IsParameterlessConstructor(this MethodSymbol method)
        {
            return method.MethodKind == MethodKind.Constructor && method.ParameterCount == 0;
        }

        /// <summary>
        /// default zero-init constructor symbol is added to a struct when it does not define 
        /// its own parameterless public constructor.
        /// We do not emit this constructor and do not call it 
        /// </summary>
        internal static bool IsDefaultValueTypeConstructor(this MethodSymbol method)
        {
            if (!method.ContainingType.IsValueType)
            {
                return false;
            }

            if (!method.IsParameterlessConstructor() || !method.IsImplicitlyDeclared)
            {
                return false;
            }

            throw new NotImplementedException();
        }

        internal static TypeSymbol GetTypeOrReturnType(this Symbol member)
        {
            TypeSymbol returnType;
            ImmutableArray<CustomModifier> returnTypeCustomModifiers;
            GetTypeOrReturnType(member, out returnType, out returnTypeCustomModifiers);
            return returnType;
        }

        internal static void GetTypeOrReturnType(this Symbol member, out TypeSymbol returnType,
            out ImmutableArray<CustomModifier> returnTypeCustomModifiers)
        {
            switch (member.Kind)
            {
                case SymbolKind.Local:
                    returnType = (TypeSymbol)((ILocalSymbol)member).Type;
                    returnTypeCustomModifiers = ImmutableArray<CustomModifier>.Empty;
                    break;
                case SymbolKind.Field:
                    FieldSymbol field = (FieldSymbol)member;
                    returnType = field.Type;
                    returnTypeCustomModifiers = ImmutableArray<CustomModifier>.Empty;
                    break;
                case SymbolKind.Method:
                    MethodSymbol method = (MethodSymbol)member;
                    returnType = method.ReturnType;
                    returnTypeCustomModifiers = method.ReturnTypeCustomModifiers;
                    break;
                case SymbolKind.Property:
                    PropertySymbol property = (PropertySymbol)member;
                    returnType = property.Type;
                    returnTypeCustomModifiers = property.TypeCustomModifiers;
                    break;
                case SymbolKind.Event:
                    throw new NotImplementedException();
                case SymbolKind.Parameter:
                    var p = (ParameterSymbol)member;
                    returnType = p.Type;
                    returnTypeCustomModifiers = p.CustomModifiers;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.Kind);
            }
        }

        internal static bool IsFieldOrFieldLikeEvent(this Symbol member, out FieldSymbol field)
        {
            switch (member.Kind)
            {
                case SymbolKind.Field:
                    field = (FieldSymbol)member;
                    return true;
                case SymbolKind.Event:
                    throw new NotImplementedException();
                default:
                    field = null;
                    return false;
            }
        }

        internal static string GetMemberCallerName(this Symbol member)
        {
            if (member.Kind == SymbolKind.Method)
            {
                member = (Symbol)((MethodSymbol)member).AssociatedSymbol ?? member;
            }

            throw new NotImplementedException();
        }
    }
}