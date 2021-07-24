using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Symbols;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics.Model;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Roslyn.Utilities;
using System.Collections.Immutable;
using System.Threading;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Operations;
using Aquila.CodeAnalysis;
using Microsoft.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis
{
    partial class AquilaCompilation
    {
        private readonly WellKnownMembersSignatureComparer _wellKnownMemberSignatureComparer;

        /// <summary>
        /// An array of cached well known types available for use in this Compilation.
        /// Lazily filled by GetWellKnownType method.
        /// </summary>
        private NamedTypeSymbol[] _lazyWellKnownTypes;

        /// <summary>
        /// Lazy cache of well known members.
        /// Not yet known value is represented by ErrorTypeSymbol.UnknownResultType
        /// </summary>
        private Aquila.CodeAnalysis.Symbols.Symbol[] _lazyWellKnownTypeMembers;

        internal Conversions Conversions { get; }

        /// <summary>
        /// Gets factory object for constructing <see cref="BoundTypeRef"/>.
        /// </summary>
        internal BoundTypeRefFactory TypeRefFactory { get; }

        #region CoreTypes, CoreMethods

        /// <summary>
        /// Well known types associated with this compilation.
        /// </summary>
        internal CoreTypes CoreTypes => _coreTypes;

        readonly CoreTypes _coreTypes;

        /// <summary>
        /// Well known methods associated with this compilation.
        /// </summary>
        internal CoreMethods CoreMethods => _coreMethods;

        readonly CoreMethods _coreMethods;

        #endregion

        #region Aquila Type Hierarchy

        GlobalSymbolProvider _model;

        /// <summary>
        /// Gets global semantics. To be replaced once we implement SyntaxNode (<see cref="CommonGetSemanticModel"/>).
        /// </summary>
        internal GlobalSymbolProvider GlobalSemantics => _model ?? (_model = new GlobalSymbolProvider(this));

        /// <summary>
        /// Merges two CLR types into one
        /// </summary>
        /// <param name="first">First type.</param>
        /// <param name="second">Second type.</param>
        /// <returns>One type convering both <paramref name="first"/> and <paramref name="second"/> types.</returns>
        internal TypeSymbol Merge(TypeSymbol first, TypeSymbol second)
        {
            Contract.ThrowIfNull(first);
            Contract.ThrowIfNull(second);

            // merge is not needed:
            if (first == second)
                return first;


            // an integer (int | long)
            if (IsIntegerNumber(first) && IsIntegerNumber(second))
                return CoreTypes.Int64;

            // float|double
            if (IsFloatNumber(first) && IsFloatNumber(second))
                return CoreTypes.Double;

            return null;
        }

        internal TypeSymbol FindCommonBase(ImmutableArray<NamedTypeSymbol> types)
        {
            if (types.Length == 0)
            {
                return null;
            }
            else if (types.Length == 1)
            {
                return types[0];
            }
            else if (types.Length == 2)
            {
                return FindCommonBase(types[0], types[1]);
            }
            else
            {
                var t = (TypeSymbol)types[0];
                for (int i = 1; i < types.Length && t != null && t.SpecialType != SpecialType.System_Object; i++)
                {
                    t = FindCommonBase(t, types[i]);
                }

                return t;
            }
        }

        /// <summary>
        /// Resolves a <see cref="TypeSymbol"/> that both given types share.
        /// Gets <c>System.Object</c> in worst case.
        /// </summary>
        internal TypeSymbol FindCommonBase(TypeSymbol a, TypeSymbol b)
        {
            Debug.Assert(a != null && b != null);

            if (a.IsReferenceType && b.IsReferenceType)
            {
                if (a.SpecialType != SpecialType.System_Object &&
                    b.SpecialType != SpecialType.System_Object)
                {
                    if (a.IsOfType(b)) return b; // A >> B -> B
                    if (b.IsOfType(a)) return a; // A << B -> A

                    // find common base
                    // find a common interface

                    var set = new HashSet<TypeSymbol>();

                    // walk through "a" and remember all the base types
                    for (var ax = a.BaseType;
                        ax != null && ax.SpecialType != SpecialType.System_Object;
                        ax = ax.BaseType)
                        set.Add(ax);
                    foreach (var ax in a.AllInterfaces)
                        set.Add(ax);

                    // walk through "b" and find something in the hierarchy shared by "a",
                    // base types first
                    for (var ax = b.BaseType;
                        ax != null && ax.SpecialType != SpecialType.System_Object;
                        ax = ax.BaseType)
                        if (set.Contains(ax))
                            return ax; // a common base

                    foreach (var ax in b.AllInterfaces)
                        if (set.Contains(ax))
                            return ax; // a common interface
                }

                //
                return CoreTypes.Object;
            }

            // dunno
            return null;
        }

        /// <summary>
        /// Merges CLR type to be nullable.
        /// </summary>
        internal TypeSymbol MergeNull(TypeSymbol type)
        {
            Contract.ThrowIfNull(type);

            if (type.IsVoid())
            {
                return CoreTypes.Object;
            }

            if (type.IsValueType || type.IsOfType(null))
            {
                return null;
            }

            return type;
        }

        static bool IsIntegerNumber(TypeSymbol type)
        {
            Contract.ThrowIfNull(type);

            switch (type.SpecialType)
            {
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                    return true;
                default:
                    return false;
            }
        }

        static bool IsFloatNumber(TypeSymbol type)
        {
            Contract.ThrowIfNull(type);

            switch (type.SpecialType)
            {
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether given type is treated as a Aquila numeric (<c>int</c> or <c>double</c>).
        /// </summary>
        internal bool IsNumber(TypeSymbol type)
        {
            Contract.ThrowIfNull(type);

            return
                IsIntegerNumber(type) ||
                type.SpecialType == SpecialType.System_Double ||
                type.SpecialType == SpecialType.System_Single ||
                type == null;
        }

        /// <summary>
        /// Determines given type is treated as a string.
        /// </summary>
        internal bool IsAString(TypeSymbol type)
        {
            Contract.ThrowIfNull(type);

            return
                type.SpecialType == SpecialType.System_String ||
                type == null;
        }

        #endregion

        #region TypeSymbol From AST.TypeRef

        /// <summary>
        /// Binds <see cref="TypeRef"/> to a type symbol.
        /// </summary>
        /// <param name="tref">Type reference.</param>
        /// <param name="selfHint">Optional.
        /// Current type scope for better <paramref name="tref"/> resolution since <paramref name="tref"/> might be ambiguous</param>
        /// <param name="nullable">Whether the resulting type must be able to contain NULL. Default is <c>false</c>.</param>
        /// <returns>Resolved symbol.</returns>
        internal TypeSymbol GetTypeFromTypeRef(TypeRef tref, object selfHint = null,
            bool nullable = false)
        {
            if (tref == null)
            {
                return null;
            }

            var t = this.TypeRefFactory.CreateFromTypeRef(tref, null, selfHint);

            var symbol = t.ResolveRuntimeType(this);

            if (t.IsNullable || nullable)
            {
                // TODO: for value types -> Nullable<T>
                symbol = MergeNull(symbol);
            }

            return symbol;
        }

        #endregion

        /// <summary>
        /// Resolves the value's type.
        /// </summary>
        internal TypeSymbol GetConstantValueType(object value)
        {
            if (ReferenceEquals(value, null)) return CoreTypes.Object;

            if (value is bool) return CoreTypes.Boolean;
            if (value is int) return CoreTypes.Int32;
            if (value is long) return CoreTypes.Int64;
            if (value is double) return CoreTypes.Double;
            if (value is string) return CoreTypes.String;

            if (value is byte) return GetSpecialType(SpecialType.System_Byte);
            if (value is uint) return GetSpecialType(SpecialType.System_UInt32);
            if (value is ulong) return GetSpecialType(SpecialType.System_UInt64);
            if (value is float) return GetSpecialType(SpecialType.System_Single);
            if (value is char) return GetSpecialType(SpecialType.System_Char);

            //
            throw ExceptionUtilities.UnexpectedValue(value);
        }

        /// <summary>
        /// Lookup member declaration in well known type used by this Compilation.
        /// </summary>
        /// <remarks>
        /// If a well-known member of a generic type instantiation is needed use this method to get the corresponding generic definition and 
        /// <see cref="MethodSymbol.AsMember"/> to construct an instantiation.
        /// </remarks>
        internal Aquila.CodeAnalysis.Symbols.Symbol GetWellKnownTypeMember(WellKnownMember member)
        {
            Debug.Assert(member >= 0 && member < WellKnownMember.Count);

            // Test hook: if a member is marked missing, then return null.
            if (IsMemberMissing(member)) return null;

            if (_lazyWellKnownTypeMembers == null || ReferenceEquals(_lazyWellKnownTypeMembers[(int)member],
                ErrorTypeSymbol.UnknownResultType))
            {
                if (_lazyWellKnownTypeMembers == null)
                {
                    var wellKnownTypeMembers = new Aquila.CodeAnalysis.Symbols.Symbol[(int)WellKnownMember.Count];

                    for (int i = 0; i < wellKnownTypeMembers.Length; i++)
                    {
                        wellKnownTypeMembers[i] = ErrorTypeSymbol.UnknownResultType;
                    }

                    Interlocked.CompareExchange(ref _lazyWellKnownTypeMembers, wellKnownTypeMembers, null);
                }

                MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(member);
                NamedTypeSymbol type = descriptor.DeclaringTypeId <= (int)SpecialType.Count
                    ? this.GetSpecialType((SpecialType)descriptor.DeclaringTypeId)
                    : this.GetWellKnownType((WellKnownType)descriptor.DeclaringTypeId);
                Aquila.CodeAnalysis.Symbols.Symbol result = null;

                if (!type.IsErrorType())
                {
                    result = GetRuntimeMember(type, ref descriptor, _wellKnownMemberSignatureComparer,
                        accessWithinOpt: this.Assembly);
                }

                Interlocked.CompareExchange(ref _lazyWellKnownTypeMembers[(int)member], result,
                    ErrorTypeSymbol.UnknownResultType);
            }

            return _lazyWellKnownTypeMembers[(int)member];
        }

        internal NamedTypeSymbol GetWellKnownType(WellKnownType type)
        {
            Debug.Assert(type >= WellKnownType.First && type < WellKnownType.NextAvailable);

            int index = (int)type - (int)WellKnownType.First;
            if (_lazyWellKnownTypes == null || (object)_lazyWellKnownTypes[index] == null)
            {
                if (_lazyWellKnownTypes == null)
                {
                    Interlocked.CompareExchange(ref _lazyWellKnownTypes,
                        new NamedTypeSymbol[(int)WellKnownTypes.Count], null);
                }

                string mdName = type.GetMetadataName();
                var warnings = DiagnosticBag.GetInstance();
                NamedTypeSymbol result;

                if (IsTypeMissing(type))
                {
                    result = null;
                }
                else
                {
                    result = this.SourceAssembly.GetTypeByMetadataName(
                        mdName, includeReferences: true, useCLSCompliantNameArityEncoding: true, isWellKnownType: true,
                        warnings: warnings);
                }

                if ((object)result == null)
                {
                    // TODO: should GetTypeByMetadataName rather return a missing symbol?
                    //MetadataTypeName emittedName = MetadataTypeName.FromFullName(mdName, useCLSCompliantNameArityEncoding: true);
                    //result = new MissingMetadataTypeSymbol.TopLevel(this.Assembly.Modules[0], ref emittedName, type);
                    Debug.Assert(false);
                    result = new MissingMetadataTypeSymbol(mdName, 0, false);
                }

                if ((object)Interlocked.CompareExchange(ref _lazyWellKnownTypes[index], result, null) != null)
                {
                    Debug.Assert(
                        result == _lazyWellKnownTypes[index] ||
                        (_lazyWellKnownTypes[index].IsErrorType() && result.IsErrorType())
                    );
                }
                else
                {
                    // TODO //AdditionalCodegenWarnings.AddRange(warnings);
                }

                warnings.Free();
            }

            return _lazyWellKnownTypes[index];
        }

        /// <summary>
        /// Get the symbol for the predefined type from the Cor Library referenced by this
        /// compilation.
        /// </summary>
        internal new NamedTypeSymbol GetSpecialType(SpecialType specialType)
        {
            return (NamedTypeSymbol)CommonGetSpecialType(specialType);
        }

        internal override bool IsAttributeType(ITypeSymbol type)
        {
            HashSet<DiagnosticInfo> useSiteDiagnostics = null;
            return ((TypeSymbol)type).IsDerivedFrom(GetWellKnownType(WellKnownType.System_Attribute), false,
                ref useSiteDiagnostics);
        }

        internal override bool IsSystemTypeReference(ITypeSymbolInternal type)
        {
            return (TypeSymbol)type == GetWellKnownType(WellKnownType.System_Type);
        }

        private protected override INamedTypeSymbolInternal CommonGetSpecialType(SpecialType specialType)
        {
            return this.CorLibrary.GetSpecialType(specialType);
        }

        internal override ISymbolInternal CommonGetSpecialTypeMember(SpecialMember specialMember)
        {
            return this.CorLibrary.GetDeclaredSpecialTypeMember(specialMember);
        }

        /// <summary>
        /// Get the symbol for the predefined type member from the COR Library referenced by this compilation.
        /// </summary>
        internal Aquila.CodeAnalysis.Symbols.Symbol GetSpecialTypeMember(SpecialMember specialMember)
        {
            return this.CorLibrary.GetDeclaredSpecialTypeMember(specialMember);
        }

        internal static Aquila.CodeAnalysis.Symbols.Symbol GetRuntimeMember(NamedTypeSymbol declaringType,
            ref MemberDescriptor descriptor,
            SignatureComparer<MethodSymbol, FieldSymbol, PropertySymbol, TypeSymbol, ParameterSymbol> comparer,
            IAssemblySymbol accessWithinOpt)
        {
            Aquila.CodeAnalysis.Symbols.Symbol result = null;
            SymbolKind targetSymbolKind;
            MethodKind targetMethodKind = MethodKind.Ordinary;
            bool isStatic = (descriptor.Flags & MemberFlags.Static) != 0;

            switch (descriptor.Flags & MemberFlags.KindMask)
            {
                case MemberFlags.Constructor:
                    targetSymbolKind = SymbolKind.Method;
                    targetMethodKind = MethodKind.Constructor;
                    //  static constructors are never called explicitly
                    Debug.Assert(!isStatic);
                    break;

                case MemberFlags.Method:
                    targetSymbolKind = SymbolKind.Method;
                    break;

                case MemberFlags.PropertyGet:
                    targetSymbolKind = SymbolKind.Method;
                    targetMethodKind = MethodKind.PropertyGet;
                    break;

                case MemberFlags.Field:
                    targetSymbolKind = SymbolKind.Field;
                    break;

                case MemberFlags.Property:
                    targetSymbolKind = SymbolKind.Property;
                    break;

                default:
                    throw ExceptionUtilities.UnexpectedValue(descriptor.Flags);
            }

            foreach (var member in declaringType.GetMembers(descriptor.Name))
            {
                Debug.Assert(member.Name.Equals(descriptor.Name));

                if (member.Kind != targetSymbolKind || member.IsStatic != isStatic ||
                    !(member.DeclaredAccessibility == Accessibility.Public))
                {
                    continue;
                }

                switch (targetSymbolKind)
                {
                    case SymbolKind.Method:
                    {
                        MethodSymbol method = (MethodSymbol)member;
                        MethodKind methodKind = method.MethodKind;
                        // Treat user-defined conversions and operators as ordinary methods for the purpose
                        // of matching them here.
                        if (methodKind == MethodKind.Conversion || methodKind == MethodKind.UserDefinedOperator)
                        {
                            methodKind = MethodKind.Ordinary;
                        }

                        if (method.Arity != descriptor.Arity || methodKind != targetMethodKind ||
                            ((descriptor.Flags & MemberFlags.Virtual) != 0) !=
                            (method.IsVirtual || method.IsOverride || method.IsAbstract))
                        {
                            continue;
                        }

                        if (!comparer.MatchMethodSignature(method, descriptor.Signature))
                        {
                            continue;
                        }
                    }

                        break;

                    case SymbolKind.Property:
                    {
                        PropertySymbol property = (PropertySymbol)member;
                        if (((descriptor.Flags & MemberFlags.Virtual) != 0) !=
                            (property.IsVirtual || property.IsOverride || property.IsAbstract))
                        {
                            continue;
                        }

                        if (!comparer.MatchPropertySignature(property, descriptor.Signature))
                        {
                            continue;
                        }
                    }

                        break;

                    case SymbolKind.Field:
                        if (!comparer.MatchFieldSignature((FieldSymbol)member, descriptor.Signature))
                        {
                            continue;
                        }

                        break;

                    default:
                        throw ExceptionUtilities.UnexpectedValue(targetSymbolKind);
                }

                // ambiguity
                if ((object)result != null)
                {
                    result = null;
                    break;
                }

                result = member;
            }

            return result;
        }

        internal IEnumerable<IAssemblySymbol> ProbingAssemblies
        {
            get
            {
                foreach (var pair in CommonGetBoundReferenceManager().GetReferencedAssemblies())
                    yield return (IAssemblySymbol)pair.Value;

                yield return this.SourceAssembly;
            }
        }

        protected override INamedTypeSymbol CommonGetTypeByMetadataName(string metadataName)
        {
            return ProbingAssemblies
                .Select(a => a.GetTypeByMetadataName(metadataName))
                .Where(a => a != null)
                .FirstOrDefault();
        }

        /// <summary>
        /// Resolves <see cref="TypeSymbol"/> best fitting given type mask.
        /// </summary>
        internal TypeSymbol GetTypeFromTypeRef(TypeRefContext typeCtx)
        {
            return null;
        }

        /// <summary>
        /// Resolves <see cref="INamedTypeSymbol"/> best fitting given type mask.
        /// </summary>
        internal TypeSymbol GetTypeFromTypeRef(SourceMethodSymbol method)
        {
            Debug.Assert(method != null);
            return this.GetTypeFromTypeRef(method.TypeRefContext);
        }

        /// <summary>
        /// Aquila has a different semantic of explicit conversions,
        /// try to resolve explicit conversion first.
        /// </summary>
        public CommonConversion ClassifyExplicitConversion(ITypeSymbol source, ITypeSymbol destination)
        {
            var conv = Conversions.ClassifyConversion((TypeSymbol)source, (TypeSymbol)destination,
                ConversionKind.Explicit);
            if (conv.Exists == false)
            {
                // try regular implicit conversion instead
                conv = ClassifyCommonConversion(source, destination);
            }

            return conv;
        }

        public override CommonConversion ClassifyCommonConversion(ITypeSymbol source, ITypeSymbol destination)
        {
            return Conversions.ClassifyConversion((TypeSymbol)source, (TypeSymbol)destination,
                ConversionKind.Implicit | ConversionKind.Explicit);
        }

        internal override IConvertibleConversion ClassifyConvertibleConversion(IOperation source,
            ITypeSymbol destination, out ConstantValue constantValue)
        {
            //constantValue = default;

            //if (destination is null)
            //{
            //    return Conversions.NoConversion;
            //}

            //ITypeSymbol sourceType = source.Type;

            //if (sourceType is null)
            //{
            //    if (source.ConstantValue.HasValue && source.ConstantValue.Value is null && destination.IsReferenceType)
            //    {
            //        constantValue = source.ConstantValue;
            //        return Conversions.DefaultOrNullLiteral;
            //    }

            //    return Conversion.NoConversion;
            //}

            //var result = Conversions.ClassifyConversion(this, sourceType, destination);

            //if (result.IsReference && source.ConstantValue.HasValue && source.ConstantValue.Value is null)
            //{
            //    constantValue = source.ConstantValue;
            //}

            //return result;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Used to generate the dynamic attributes for the required typesymbol.
        /// </summary>
        internal static class DynamicTransformsEncoder
        {
            internal static ImmutableArray<TypedConstant> Encode(TypeSymbol type, TypeSymbol booleanType,
                int customModifiersCount, RefKind refKind)
            {
                var flagsBuilder = ArrayBuilder<bool>.GetInstance();
                EncodeInternal(type, customModifiersCount, refKind, flagsBuilder);
                Debug.Assert(flagsBuilder.Any());
                Debug.Assert(flagsBuilder.Contains(true));

                var constantsBuilder = ArrayBuilder<TypedConstant>.GetInstance(flagsBuilder.Count);
                foreach (bool flag in flagsBuilder)
                {
                    constantsBuilder.Add(new TypedConstant(booleanType, TypedConstantKind.Primitive, flag));
                }

                flagsBuilder.Free();
                return constantsBuilder.ToImmutableAndFree();
            }

            internal static ImmutableArray<bool> Encode(TypeSymbol type, int customModifiersCount, RefKind refKind)
            {
                var transformFlagsBuilder = ArrayBuilder<bool>.GetInstance();
                EncodeInternal(type, customModifiersCount, refKind, transformFlagsBuilder);
                return transformFlagsBuilder.ToImmutableAndFree();
            }

            internal static void EncodeInternal(TypeSymbol type, int customModifiersCount, RefKind refKind,
                ArrayBuilder<bool> transformFlagsBuilder)
            {
                Debug.Assert(!transformFlagsBuilder.Any());

                if (refKind != RefKind.None)
                {
                    // Native compiler encodes an extra transform flag, always false, for ref/out parameters.
                    transformFlagsBuilder.Add(false);
                }

                // Native compiler encodes an extra transform flag, always false, for each custom modifier.
                HandleCustomModifiers(customModifiersCount, transformFlagsBuilder);

                type.VisitType(s_encodeDynamicTransform, transformFlagsBuilder);
            }

            private static readonly Func<TypeSymbol, ArrayBuilder<bool>, bool, bool> s_encodeDynamicTransform =
                (type, transformFlagsBuilder, isNestedNamedType) =>
                {
                    // Encode transforms flag for this type and it's custom modifiers (if any).
                    switch (type.TypeKind)
                    {
                        case TypeKind.Dynamic:
                            transformFlagsBuilder.Add(true);
                            break;

                        case TypeKind.Array:
                            HandleCustomModifiers(((ArrayTypeSymbol)type).CustomModifiers.Length,
                                transformFlagsBuilder);
                            transformFlagsBuilder.Add(false);
                            break;

                        case TypeKind.Pointer:
                            //HandleCustomModifiers(((PointerTypeSymbol)type).CustomModifiers.Length, transformFlagsBuilder);
                            //transformFlagsBuilder.Add(false);
                            //break;
                            throw new NotImplementedException();

                        default:
                            // Encode transforms flag for this type.
                            // For nested named types, a single flag (false) is encoded for the entire type name, followed by flags for all of the type arguments.
                            // For example, for type "A<T>.B<dynamic>", encoded transform flags are:
                            //      {
                            //          false,  // Type "A.B"
                            //          false,  // Type parameter "T"
                            //          true,   // Type parameter "dynamic"
                            //      }

                            if (!isNestedNamedType)
                            {
                                transformFlagsBuilder.Add(false);
                            }

                            break;
                    }

                    // Continue walking types
                    return false;
                };

            private static void HandleCustomModifiers(int customModifiersCount,
                ArrayBuilder<bool> transformFlagsBuilder)
            {
                for (int i = 0; i < customModifiersCount; i++)
                {
                    // Native compiler encodes an extra transforms flag, always false, for each custom modifier.
                    transformFlagsBuilder.Add(false);
                }
            }
        }

        internal class SpecialMembersSignatureComparer : SignatureComparer<MethodSymbol, FieldSymbol, PropertySymbol,
            TypeSymbol, ParameterSymbol>
        {
            // Fields
            public static readonly SpecialMembersSignatureComparer Instance = new SpecialMembersSignatureComparer();

            // Methods
            protected SpecialMembersSignatureComparer()
            {
            }

            protected override TypeSymbol GetMDArrayElementType(TypeSymbol type)
            {
                if (type.Kind != SymbolKind.ArrayType)
                {
                    return null;
                }

                ArrayTypeSymbol array = (ArrayTypeSymbol)type;
                if (array.IsSZArray)
                {
                    return null;
                }

                return array.ElementType;
            }

            protected override TypeSymbol GetFieldType(FieldSymbol field)
            {
                return field.Type;
            }

            protected override TypeSymbol GetPropertyType(PropertySymbol property)
            {
                return property.Type;
            }

            protected override TypeSymbol GetGenericTypeArgument(TypeSymbol type, int argumentIndex)
            {
                if (type.Kind != SymbolKind.NamedType)
                {
                    return null;
                }

                NamedTypeSymbol named = (NamedTypeSymbol)type;
                if (named.Arity <= argumentIndex)
                {
                    return null;
                }

                if ((object)named.ContainingType != null)
                {
                    return null;
                }

                return named.TypeArguments
                    [argumentIndex]; //return named.TypeArgumentsNoUseSiteDiagnostics[argumentIndex];
            }

            protected override TypeSymbol GetGenericTypeDefinition(TypeSymbol type)
            {
                if (type.Kind != SymbolKind.NamedType)
                {
                    return null;
                }

                NamedTypeSymbol named = (NamedTypeSymbol)type;
                if ((object)named.ContainingType != null)
                {
                    return null;
                }

                if (named.Arity == 0)
                {
                    return null;
                }

                return (NamedTypeSymbol)named.OriginalDefinition;
            }

            protected override ImmutableArray<ParameterSymbol> GetParameters(MethodSymbol method)
            {
                return method.Parameters;
            }

            protected override ImmutableArray<ParameterSymbol> GetParameters(PropertySymbol property)
            {
                return property.Parameters;
            }

            protected override TypeSymbol GetParamType(ParameterSymbol parameter)
            {
                return parameter.Type;
            }

            protected override TypeSymbol GetPointedToType(TypeSymbol type)
            {
                if (type.Kind == SymbolKind.PointerType)
                    throw new NotImplementedException();

                return null;
            }

            protected override TypeSymbol GetReturnType(MethodSymbol method)
            {
                return method.ReturnType;
            }

            protected override TypeSymbol GetSZArrayElementType(TypeSymbol type)
            {
                if (type.Kind != SymbolKind.ArrayType)
                {
                    return null;
                }

                ArrayTypeSymbol array = (ArrayTypeSymbol)type;
                if (!array.IsSZArray)
                {
                    return null;
                }

                return array.ElementType;
            }

            protected override bool IsByRefParam(ParameterSymbol parameter)
            {
                return parameter.RefKind != RefKind.None;
            }

            protected override bool IsByRefMethod(MethodSymbol method)
            {
                return method.RefKind != RefKind.None;
            }

            protected override bool IsByRefProperty(PropertySymbol property)
            {
                return property.RefKind != RefKind.None;
            }


            protected override bool IsGenericMethodTypeParam(TypeSymbol type, int paramPosition)
            {
                if (type.Kind != SymbolKind.TypeParameter)
                {
                    return false;
                }

                TypeParameterSymbol typeParam = (TypeParameterSymbol)type;
                if (typeParam.ContainingSymbol.Kind != SymbolKind.Method)
                {
                    return false;
                }

                return (typeParam.Ordinal == paramPosition);
            }

            protected override bool IsGenericTypeParam(TypeSymbol type, int paramPosition)
            {
                if (type.Kind != SymbolKind.TypeParameter)
                {
                    return false;
                }

                TypeParameterSymbol typeParam = (TypeParameterSymbol)type;
                if (typeParam.ContainingSymbol.Kind != SymbolKind.NamedType)
                {
                    return false;
                }

                return (typeParam.Ordinal == paramPosition);
            }

            protected override bool MatchArrayRank(TypeSymbol type, int countOfDimensions)
            {
                if (type.Kind != SymbolKind.ArrayType)
                {
                    return false;
                }

                ArrayTypeSymbol array = (ArrayTypeSymbol)type;
                return (array.Rank == countOfDimensions);
            }

            protected override bool MatchTypeToTypeId(TypeSymbol type, int typeId)
            {
                return (int)type.SpecialType == typeId;
            }
        }

        private class WellKnownMembersSignatureComparer : SpecialMembersSignatureComparer
        {
            private readonly AquilaCompilation _compilation;

            public WellKnownMembersSignatureComparer(AquilaCompilation compilation)
            {
                _compilation = compilation;
            }

            protected override bool MatchTypeToTypeId(TypeSymbol type, int typeId)
            {
                WellKnownType wellKnownId = (WellKnownType)typeId;
                if (wellKnownId >= WellKnownType.First && wellKnownId < WellKnownType.NextAvailable)
                {
                    return (type == _compilation.GetWellKnownType(wellKnownId));
                }

                return base.MatchTypeToTypeId(type, typeId);
            }
        }
    }
}