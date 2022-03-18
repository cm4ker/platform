using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols.PE;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    internal static partial class SymbolExtensions
    {
        public static readonly Func<Symbol, bool> s_IsReachable = new Func<Symbol, bool>(t => !t.IsUnreachable);

        public static IEnumerable<T> WhereReachable<T>(this IEnumerable<T> symbols) where T : Symbol =>
            symbols.Where<T>(s_IsReachable);

        /// <summary>
        /// Returns a constructed named type symbol if 'type' is generic, otherwise just returns 'type'
        /// </summary>
        public static NamedTypeSymbol ConstructIfGeneric(this NamedTypeSymbol type,
            ImmutableArray<TypeWithModifiers> typeArguments)
        {
            Debug.Assert(type.TypeParameters.IsEmpty == (typeArguments.Length == 0));
            return type.TypeParameters.IsEmpty ? type : type.Construct(typeArguments, unbound: false);
        }

        public static bool IsAquilaHidden(this Symbol s, AquilaCompilation compilation = null)
        {
            if (s is MethodSymbol m)
            {
                return m.IsAquilaHidden;
            }

            var attrs = s.GetAttributes();
            if (attrs.Length != 0)
            {
                bool hascond = false;
                bool hasmatch = false;

                foreach (var attr in attrs)
                {
                }

                if (hascond && !hasmatch) return true; // conditions defined but not satisfied => hide
            }

            //
            return false;
        }

        /// <summary>
        /// Determines if
        /// - method symbol cannot return NULL. E.g. has return attribute [return: NotNullAttribute] or [return: CastToFalse]
        /// - parameter symbol cannot be NULL (has attribute [NotNullAttribute])
        /// - field symbol cannot be NULL (has [NotNullAttribute])
        /// - property cannot be NULL (has [NotNullAttribute])
        /// </summary>
        public static bool IsNotNull(this Symbol symbol)
        {
            return false;
        }

        /// <summary>
        /// Determines Aquila type name of an exported Aquila type.
        /// Gets default&lt;QualifiedName&gt; if type is not exported Aquila type.
        /// </summary>
        public static QualifiedName GetAquilaTypeNameOrNull(this PENamedTypeSymbol s)
        {
            return default;
        }

        /// <summary>
        /// Gets type full qualified name.
        /// </summary>
        public static QualifiedName MakeQualifiedName(this NamedTypeSymbol type)
        {
            return NameUtils.MakeQualifiedName(type.Name, type.OriginalDefinition.NamespaceName, true);
        }

        /// <summary>
        /// Gets the symbol name as it appears in Aquila context.
        /// </summary>
        public static string AquilaName(this Symbol s)
        {
            return s.Name;
        }

        /// <summary>
        /// Gets type qualified name.
        /// </summary>
        public static QualifiedName AquilaQualifiedName(this NamedTypeSymbol t) => MakeQualifiedName(t);

        public static bool IsAccessible(this Symbol symbol, TypeSymbol classCtx)
        {
            if (symbol.DeclaredAccessibility == Accessibility.Private)
            {
                return (symbol.ContainingType == classCtx);
            }
            else if (symbol.DeclaredAccessibility == Accessibility.Protected)
            {
                return classCtx != null && (
                    symbol.ContainingType.IsEqualToOrDerivedFrom(classCtx) ||
                    classCtx.IsEqualToOrDerivedFrom(symbol.ContainingType));
            }
            else if (
                symbol.DeclaredAccessibility == Accessibility.ProtectedAndInternal ||
                symbol.DeclaredAccessibility == Accessibility.Internal)
            {
                return classCtx.ContainingAssembly == symbol.ContainingAssembly; // TODO
            }

            return true;
        }


        public static AttributeData GetAttribute(this Symbol symbol, string clrname)
        {
            var attrs = symbol.GetAttributes();
            for (int i = 0; i < attrs.Length; i++)
            {
                var a = attrs[i];

                var fullname = MetadataHelpers.BuildQualifiedName((a.AttributeClass as NamedTypeSymbol)?.NamespaceName,
                    a.AttributeClass.Name);
                if (fullname == clrname)
                {
                    return a;
                }
            }

            return null;
        }
    }
}