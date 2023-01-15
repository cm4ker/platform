using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    internal static partial class TypeSymbolExtensions
    {
        /// <summary>
        /// Throws exception in type is not as expected.
        /// </summary>
        public static TypeSymbol Expect(this TypeSymbol actual, TypeSymbol expecting)
        {
            Debug.Assert(actual == expecting);

            return actual;
        }

        /// <summary>
        /// Throws exception in type is not as expected.
        /// </summary>
        public static TypeSymbol Expect(this TypeSymbol actual, SpecialType expecting)
        {
            Debug.Assert(actual != null);
            Debug.Assert(actual.SpecialType == expecting && expecting != SpecialType.None);

            return actual;
        }

        /// <summary>Assertion that type is valid.</summary>
        public static TypeSymbol ExpectValid(this TypeSymbol actual)
        {
            Debug.Assert(actual.IsValidType());
            return actual;
        }

        /// <summary>Gets value indicating the type is not null, not ambiguous and not error type.</summary>
        public static bool IsValidType(this TypeSymbol type) => !IsErrorTypeOrNull(type);

        public static bool ImplementsInterface(this TypeSymbol subType, TypeSymbol superInterface)
        {
            if (subType == superInterface)
            {
                return true;
            }

            foreach (var @interface in subType.AllInterfaces)
            {
                if (@interface.IsInterface && @interface == superInterface)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanBeAssignedNull(this TypeSymbol type)
        {
            return type.IsReferenceType || type.IsNullableType();
        }

        public static bool CanBeConst(this TypeSymbol typeSymbol)
        {
            Debug.Assert((object)typeSymbol != null);

            return typeSymbol.IsReferenceType || typeSymbol.IsEnumType() || typeSymbol.SpecialType.CanBeConst();
        }

        public static bool IsOfType(this TypeSymbol t, TypeSymbol oftype)
        {
            if (oftype != null)
            {
                if (t.Equals(oftype, ignoreDynamic: true))
                {
                    return true;
                }

                if (oftype.IsClassType())
                {
                    HashSet<DiagnosticInfo> set = null;
                    return t.IsDerivedFrom(oftype, true, ref set);
                }
                else if (oftype.IsInterfaceType())
                {
                    return t.ImplementsInterface(oftype);
                }
            }

            //
            return false;
        }

        public static bool IsEqualOrDerivedFrom(this ITypeSymbol t, ITypeSymbol ofType)
        {
            var tx = (TypeSymbol)t;
            var ty = (TypeSymbol)ofType;

            return tx.IsEqualToOrDerivedFrom(ty);
        }

        public static bool IsAssignableFrom(this TypeSymbol t, TypeSymbol fromtype)
        {
            return fromtype.IsOfType(t) || (fromtype.IsInterfaceType() && t.IsObjectType());
        }

        public static bool IsNullableType(this TypeSymbol type)
        {
            var original = (TypeSymbol)type.OriginalDefinition;
            return original.SpecialType == SpecialType.System_Nullable_T;
        }

        public static bool IsNullableType(this TypeSymbol type, out TypeSymbol tType)
        {
            if (IsNullableType(type))
            {
                tType = ((NamedTypeSymbol)type).TypeArguments[0];
                return true;
            }
            else
            {
                tType = null;
                return false;
            }
        }

        public static TypeSymbol GetNullableUnderlyingType(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            Debug.Assert(IsNullableType(type));
            Debug.Assert(type is NamedTypeSymbol); //not testing Kind because it may be an ErrorType

            return ((NamedTypeSymbol)type).TypeArgumentsNoUseSiteDiagnostics[0];
        }

        public static bool IsObjectType(this TypeSymbol type)
        {
            return type.SpecialType == SpecialType.System_Object;
        }

        public static bool IsStringType(this TypeSymbol type)
        {
            return type.SpecialType == SpecialType.System_String;
        }

        public static bool IsCharType(this TypeSymbol type)
        {
            return type.SpecialType == SpecialType.System_Char;
        }

        public static bool IsIntegralType(this TypeSymbol type)
        {
            return type.SpecialType.IsIntegralType();
        }

        public static NamedTypeSymbol GetEnumUnderlyingType(this TypeSymbol type)
        {
            var namedType = type as NamedTypeSymbol;
            return ((object)namedType != null) ? namedType.EnumUnderlyingType : null;
        }

        public static bool IsEnumType(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.TypeKind == TypeKind.Enum;
        }

        public static bool IsValidExtensionParameterType(this TypeSymbol type)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Pointer:
                case TypeKind.Dynamic:
                    return false;
                default:
                    return true;
            }
        }

        public static bool IsInterfaceType(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.Kind == SymbolKind.NamedType && ((NamedTypeSymbol)type).TypeKind == TypeKind.Interface;
        }

        public static bool IsClassType(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.TypeKind == TypeKind.Class;
        }

        public static bool IsStructType(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.TypeKind == TypeKind.Struct;
        }

        public static bool IsErrorType(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.Kind == SymbolKind.ErrorType;
        }

        public static bool IsErrorTypeOrNull(this TypeSymbol type)
        {
            return type == null || type.Kind == SymbolKind.ErrorType;
        }

        public static bool IsDynamic(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.Dynamic;
        }

        public static bool IsTypeParameter(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.TypeKind == TypeKind.TypeParameter;
        }

        public static bool IsArray(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.TypeKind == TypeKind.Array;
        }

        public static bool IsSZArray(this ITypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.TypeKind == TypeKind.Array && ((ArrayTypeSymbol)type).IsSZArray;
        }

        // If the type is a delegate type, it returns it. If the type is an
        // expression tree type associated with a delegate type, it returns
        // the delegate type. Otherwise, null.
        public static NamedTypeSymbol GetDelegateType(this TypeSymbol type)
        {
            if ((object)type == null) return null;
            if (type.IsExpressionTree())
            {
                type = ((NamedTypeSymbol)type).TypeArguments[0];
            }

            return type.IsDelegateType() ? (NamedTypeSymbol)type : null;
        }

        /// <summary>
        /// return true if the type is constructed from System.Linq.Expressions.Expression`1
        /// </summary>
        public static bool IsExpressionTree(this TypeSymbol _type)
        {
            // TODO: there must be a better way!
            var type = _type.OriginalDefinition as NamedTypeSymbol;
            return
                (object)type != null &&
                type.Arity == 1 &&
                type.MangleName &&
                type.Name == "Expression" &&
                CheckFullName(type.ContainingSymbol, s_expressionsNamespaceName);
        }

        /// <summary>
        /// return true if the type is constructed from a generic interface that 
        /// might be implemented by an array.
        /// </summary>
        public static bool IsPossibleArrayGenericInterface(this TypeSymbol _type)
        {
            NamedTypeSymbol t = _type as NamedTypeSymbol;
            if ((object)t == null)
            {
                return false;
            }

            t = (NamedTypeSymbol)t.OriginalDefinition;

            SpecialType st = t.SpecialType;

            if (st == SpecialType.System_Collections_Generic_IList_T ||
                st == SpecialType.System_Collections_Generic_ICollection_T ||
                st == SpecialType.System_Collections_Generic_IEnumerable_T ||
                st == SpecialType.System_Collections_Generic_IReadOnlyList_T ||
                st == SpecialType.System_Collections_Generic_IReadOnlyCollection_T)
            {
                return true;
            }

            return false;
        }

        private static readonly string[] s_expressionsNamespaceName = { "Expressions", "Linq", "System", "" };

        private static bool CheckFullName(Symbol symbol, string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                if ((object)symbol == null || symbol.Name != names[i]) return false;
                symbol = symbol.ContainingSymbol;
            }

            return true;
        }

        public static bool IsDelegateType(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            return type.TypeKind == TypeKind.Delegate;
        }

        public static ImmutableArray<ParameterSymbol> DelegateParameters(this TypeSymbol type)
        {
            Debug.Assert(
                (object)type.DelegateInvokeMethod() != null, // && !type.DelegateInvokeMethod().HasUseSiteError,
                "This method should only be called on valid delegate types.");
            return type.DelegateInvokeMethod().Parameters;
        }

        public static MethodSymbol DelegateInvokeMethod(this TypeSymbol type)
        {
            Debug.Assert((object)type != null);
            Debug.Assert(type.IsDelegateType() || type.IsExpressionTree());
            return (MethodSymbol)type.GetDelegateType().DelegateInvokeMethod;
        }

        /// <summary>
        /// Return the default value constant for the given type,
        /// or null if the default value is not a constant.
        /// </summary>
        public static ConstantValue GetDefaultValue(this TypeSymbol type)
        {
            // SPEC:    A default-value-expression is a constant expression (§7.19) if the type
            // SPEC:    is a reference type or a type parameter that is known to be a reference type (§10.1.5). 
            // SPEC:    In addition, a default-value-expression is a constant expression if the type is
            // SPEC:    one of the following value types:
            // SPEC:    sbyte, byte, short, ushort, int, uint, long, ulong, char, float, double, decimal, bool, or any enumeration type.

            Debug.Assert((object)type != null);

            if (type.IsErrorType())
            {
                return null;
            }

            if (type.IsReferenceType)
            {
                return ConstantValue.Null;
            }

            if (type.IsValueType)
            {
                if (type.IsEnumType())
                {
                    throw new NotImplementedException();
                }

                switch (type.SpecialType)
                {
                    case SpecialType.System_SByte:
                    case SpecialType.System_Byte:
                    case SpecialType.System_Int16:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                    case SpecialType.System_Char:
                    case SpecialType.System_Boolean:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        return ConstantValue.Default(type.SpecialType);
                }
            }

            return null;
        }

        public static SpecialType GetSpecialTypeSafe(this TypeSymbol type)
        {
            return (object)type != null ? type.SpecialType : SpecialType.None;
        }

        /// <summary>
        /// Visit the given type and, in the case of compound types, visit all "sub type"
        /// (such as A in A[], or { A&lt;T&gt;, T, U } in A&lt;T&gt;.B&lt;U&gt;) invoking 'predicate'
        /// with the type and 'arg' at each sub type. If the predicate returns true for any type,
        /// traversal stops and that type is returned from this method. Otherwise if traversal
        /// completes without the predicate returning true for any type, this method returns null.
        /// </summary>
        public static TypeSymbol VisitType<T>(this TypeSymbol type, Func<TypeSymbol, T, bool, bool> predicate, T arg)
        {
            // In order to handle extremely "deep" types like "int[][][][][][][][][]...[]"
            // or int*****************...* we implement manual tail recursion rather than 
            // doing the natural recursion.

            TypeSymbol current = type;

            while (true)
            {
                bool isNestedNamedType = false;

                // Visit containing types from outer-most to inner-most.
                switch (current.TypeKind)
                {
                    case TypeKind.Class:
                    case TypeKind.Struct:
                    case TypeKind.Interface:
                    case TypeKind.Enum:
                    case TypeKind.Delegate:
                    {
                        var containingType = (TypeSymbol)current.ContainingType;
                        if ((object)containingType != null)
                        {
                            isNestedNamedType = true;
                            var result = containingType.VisitType(predicate, arg);
                            if ((object)result != null)
                            {
                                return result;
                            }
                        }
                    }
                        break;

                    case TypeKind.Submission:
                        Debug.Assert((object)current.ContainingType == null);
                        break;
                }

                if (predicate(current, arg, isNestedNamedType))
                {
                    return current;
                }

                switch (current.TypeKind)
                {
                    case TypeKind.Error:
                    case TypeKind.Dynamic:
                    case TypeKind.TypeParameter:
                    case TypeKind.Submission:
                        return null;

                    case TypeKind.Class:
                    case TypeKind.Struct:
                    case TypeKind.Interface:
                    case TypeKind.Enum:
                    case TypeKind.Delegate:
                        throw new NotImplementedException();

                    case TypeKind.Array:
                        current = ((ArrayTypeSymbol)current).ElementType;
                        continue;

                    case TypeKind.Pointer:
                        throw new NotImplementedException();
                    default:
                        throw ExceptionUtilities.UnexpectedValue(current.TypeKind);
                }
            }
        }

        public static bool IsUnboundGenericType(this TypeSymbol type)
        {
            var namedType = type as NamedTypeSymbol;
            return (object)namedType != null && namedType.IsUnboundGenericType;
        }

        public static bool IsTopLevelType(this NamedTypeSymbol type)
        {
            return (object)type.ContainingType == null;
        }

        public static bool ContainsTypeParameter(this TypeSymbol type, MethodSymbol parameterContainer)
        {
            Debug.Assert((object)parameterContainer != null);

            var result = type.VisitType(s_isTypeParameterWithSpecificContainerPredicate, parameterContainer);
            return (object)result != null;
        }

        private static readonly Func<TypeSymbol, Symbol, bool, bool> s_isTypeParameterWithSpecificContainerPredicate =
            (type, parameterContainer, unused) => type.TypeKind == TypeKind.TypeParameter &&
                                                  (object)type.ContainingSymbol == (object)parameterContainer;

        /// <summary>
        /// Return true if the type contains any dynamic type reference.
        /// </summary>
        public static bool ContainsDynamic(this TypeSymbol type)
        {
            var result = type.VisitType(s_containsDynamicPredicate, null);
            return (object)result != null;
        }

        private static readonly Func<TypeSymbol, object, bool, bool> s_containsDynamicPredicate =
            (type, unused1, unused2) => type.TypeKind == TypeKind.Dynamic;

#pragma warning disable RS0010
        /// <summary>
        /// Returns true if the type is one of the restricted types, namely: <see cref="T:System.TypedReference"/>, 
        /// <see cref="T:System.ArgIterator"/>, or <see cref="T:System.RuntimeArgumentHandle"/>.
        /// </summary>
#pragma warning restore RS0010
        internal static bool IsRestrictedType(this TypeSymbol type)
        {
            // See Dev10 C# compiler, "type.cpp", bool Type::isSpecialByRefType() const
            Debug.Assert((object)type != null);
            switch (type.SpecialType)
            {
                case SpecialType.System_TypedReference:
                case SpecialType.System_ArgIterator:
                case SpecialType.System_RuntimeArgumentHandle:
                    return true;
            }

            return false;
        }

        public static bool IsIntrinsicType(this TypeSymbol type)
        {
            return type.SpecialType.IsIntrinsicType();
        }

        internal static int FixedBufferElementSizeInBytes(this TypeSymbol type)
        {
            return type.SpecialType.FixedBufferElementSizeInBytes();
        }

        // check that its type is allowed for Volatile
        internal static bool IsValidVolatileFieldType(this TypeSymbol type)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Struct:
                    return type.SpecialType.IsValidVolatileFieldType();

                case TypeKind.Array:
                case TypeKind.Class:
                case TypeKind.Delegate:
                case TypeKind.Dynamic:
                case TypeKind.Error:
                case TypeKind.Interface:
                case TypeKind.Pointer:
                    return true;

                case TypeKind.Enum:
                    return ((NamedTypeSymbol)type).EnumUnderlyingType.SpecialType.IsValidVolatileFieldType();

                case TypeKind.TypeParameter:
                    return type.IsReferenceType;

                case TypeKind.Submission:
                    throw ExceptionUtilities.UnexpectedValue(type.TypeKind);
            }

            return false;
        }

        /// <summary>
        /// Add this instance to the set of checked types. Returns true
        /// if this was added, false if the type was already in the set.
        /// </summary>
        public static bool MarkCheckedIfNecessary(this TypeSymbol type, ref HashSet<TypeSymbol> checkedTypes)
        {
            if (checkedTypes == null)
            {
                checkedTypes = new HashSet<TypeSymbol>();
            }

            return checkedTypes.Add(type);
        }

        /// <summary>
        /// Gets value indicating the type is <see cref="System.Void"/>.
        /// </summary>
        internal static bool IsVoid(this TypeSymbol type) => type.SpecialType == SpecialType.System_Void;

        /// <summary>
        /// These special types are structs that contain fields of the same type
        /// (e.g. <see cref="System.Int32"/> contains an instance field of type <see cref="System.Int32"/>).
        /// </summary>
        internal static bool IsPrimitiveRecursiveStruct(this TypeSymbol t)
        {
            return t.SpecialType.IsPrimitiveRecursiveStruct();
        }

        /// <summary>
        /// Compute a hash code for the constructed type. The return value will be
        /// non-zero so callers can used zero to represent an uninitialized value.
        /// </summary>
        internal static int ComputeHashCode(this NamedTypeSymbol type)
        {
            int code = type.OriginalDefinition.GetHashCode();
            code = Hash.Combine(type.ContainingType, code);

            // Unconstructed type may contain alpha-renamed type parameters while
            // may still be considered equal, we do not want to give different hashcode to such types.
            //
            // Example:
            //   Having original type A<U>.B<V> we create two _unconstructed_ types
            //    A<int>.B<V'>
            //    A<int>.B<V">     
            //  Note that V' and V" are type parameters substituted via alpha-renaming of original V
            //  These are different objects, but represent the same "type parameter at index 1"
            //
            //  In short - we are not interested in the type parameters of unconstructed types.
            if ((object)type.ConstructedFrom != (object)type)
            {
                foreach (var arg in type.TypeArguments) // .TypeArgumentsNoUseSiteDiagnostics)
                {
                    code = Hash.Combine(arg, code);
                }
            }

            // 0 may be used by the caller to indicate the hashcode is not
            // initialized. If we computed 0 for the hashcode, tweak it.
            if (code == 0)
            {
                code++;
            }

            return code;
        }

        /// <summary>
        /// Type variables are never considered reference types by the verifier.
        /// </summary>
        internal static bool IsVerifierReference(this TypeSymbol type)
        {
            return type.IsReferenceType && type.TypeKind != TypeKind.TypeParameter;
        }

        /// <summary>
        /// Type variables are never considered value types by the verifier.
        /// </summary>
        internal static bool IsVerifierValue(this TypeSymbol type)
        {
            return type.IsValueType && type.TypeKind != TypeKind.TypeParameter;
        }

        /// <summary>
        /// Return all of the type parameters in this type and enclosing types,
        /// from outer-most to inner-most type.
        /// </summary>
        internal static ImmutableArray<TypeParameterSymbol> GetAllTypeParameters(this NamedTypeSymbol type)
        {
            // Avoid allocating a builder in the common case.
            if ((object)type.ContainingType == null)
            {
                return type.TypeParameters;
            }

            var builder = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            type.GetAllTypeParameters(builder);
            return builder.ToImmutableAndFree();
        }

        /// <summary>
        /// Return all of the type parameters in this type and enclosing types,
        /// from outer-most to inner-most type.
        /// </summary>
        internal static void GetAllTypeParameters(this NamedTypeSymbol type, ArrayBuilder<TypeParameterSymbol> result)
        {
            var containingType = type.ContainingType;
            if ((object)containingType != null)
            {
                containingType.GetAllTypeParameters(result);
            }

            result.AddRange(type.TypeParameters);
        }

        /// <summary>
        /// Return true if the fully qualified name of the type's containing symbol
        /// matches the given name. This method avoids string concatenations
        /// in the common case where the type is a top-level type.
        /// </summary>
        internal static bool HasNameQualifier(this NamedTypeSymbol type, string qualifiedName)
        {
            const StringComparison comparison = StringComparison.Ordinal;

            var container = type.ContainingSymbol;
            if (container.Kind != SymbolKind.Namespace)
            {
                // Nested type. For simplicity, compare qualified name to SymbolDisplay result.
                return string.Equals(container.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat),
                    qualifiedName, comparison);
            }

            var @namespace = (NamespaceSymbol)container;
            if (@namespace.IsGlobalNamespace)
            {
                return qualifiedName.Length == 0;
            }

            return HasNamespaceName(@namespace, qualifiedName, comparison, length: qualifiedName.Length);
        }

        private static bool HasNamespaceName(NamespaceSymbol @namespace, string namespaceName,
            StringComparison comparison, int length)
        {
            if (length == 0)
            {
                return false;
            }

            var container = @namespace.ContainingNamespace;
            int separator = namespaceName.LastIndexOf('.', length - 1, length);
            int offset = 0;
            if (separator >= 0)
            {
                if (container.IsGlobalNamespace)
                {
                    return false;
                }

                if (!HasNamespaceName(container, namespaceName, comparison, length: separator))
                {
                    return false;
                }

                int n = separator + 1;
                offset = n;
                length -= n;
            }
            else if (!container.IsGlobalNamespace)
            {
                return false;
            }

            var name = @namespace.Name;
            return (name.Length == length) && (string.Compare(name, 0, namespaceName, offset, length, comparison) == 0);
        }
    }
}