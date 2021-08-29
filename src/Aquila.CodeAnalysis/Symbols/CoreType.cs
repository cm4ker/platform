using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Descriptor of a well-known type.
    /// </summary>
    [DebuggerDisplay("CoreType {FullName}")]
    sealed class CoreType : IEquatable<CoreType>, IEquatable<TypeSymbol>
    {
        /// <summary>
        /// Gets full type name.
        /// </summary>
        public readonly string FullName;

        /// <summary>
        /// Gets associated symbol.
        /// </summary>
        /// <remarks>Assuming single singleton instance of library.</remarks>
        public NamedTypeSymbol Symbol { get; private set; }

        public CoreType(string fullName)
        {
            Debug.Assert(!string.IsNullOrEmpty(fullName));
            this.FullName = fullName;
        }

        internal void Update(NamedTypeSymbol symbol)
        {
            Contract.ThrowIfNull(symbol);
            Debug.Assert(this.Symbol == null);
            this.Symbol = symbol;
        }

        /// <summary>
        /// Implicit cast to type symbol.
        /// </summary>
        public static implicit operator NamedTypeSymbol(CoreType t) => t.Symbol;

        /// <summary>
        /// Implicit cast to type symbol.
        /// </summary>
        public static explicit operator TypeSymbol(CoreType t) => t.Symbol;

        #region IEquatable

        //public override bool Equals(object obj)
        //{
        //    return base.Equals(obj);
        //}

        bool IEquatable<CoreType>.Equals(CoreType other)
        {
            return object.ReferenceEquals(this, other);
        }

        bool IEquatable<TypeSymbol>.Equals(TypeSymbol other)
        {
            return this.Symbol == other;
        }

        //public static bool operator ==(TypeSymbol s, CoreType t)
        //{
        //    return ((IEquatable<TypeSymbol>)t).Equals(s);
        //}

        //public static bool operator !=(TypeSymbol s, CoreType t)
        //{
        //    return !((IEquatable<TypeSymbol>)t).Equals(s);
        //}

        #endregion
    }

    static class CoreTypeExtensions
    {
        public static CoreMethod Method(this CoreType type, string name, params CoreType[] ptypes) =>
            new CoreMethod(type, name, ptypes);

        public static CoreProperty Property(this CoreType type, string name) => new CoreProperty(type, name);
        public static CoreField Field(this CoreType type, string name) => new CoreField(type, name);

        public static CoreOperator Operator(this CoreType type, string name, params CoreType[] ptypes) =>
            new CoreOperator(type, name, ptypes);

        public static CoreConstructor Ctor(this CoreType type, params CoreType[] ptypes) =>
            new CoreConstructor(type, ptypes);

        public static CoreCast CastImplicit(this CoreType type, CoreType target) => new CoreCast(type, target, false);
    }

    /// <summary>
    /// Set of well-known types declared in core libraries.
    /// </summary>
    class CoreTypes
    {
        readonly AquilaCompilation _compilation;

        /// <summary>
        /// Root namespace for Aquila Runtime types.
        /// </summary>
        public const string AquilaRuntimeNamespace = "Aquila.Core";


        public const string AquilaQueryAttributeFullName = AquilaRuntimeNamespace + ".QueryAttribute";

        public const string AquilaExtensionAqAttributeFullName =
            AquilaRuntimeNamespace + ".ExtensionAqAttribute";

        public const string AquilaEntityAttributeFullName = AquilaRuntimeNamespace + ".EntityAttribute";

        public const string AquilaLinkAttributeFullName = AquilaRuntimeNamespace + ".LinkAttribute";

        public const string AquilaTargetPlatformAttributeFullName = AquilaRuntimeNamespace + ".TargetPlatform";

        public const string AquilaPlatformQueryFullName = AquilaRuntimeNamespace + ".AqQuery";

        public const string AquilaPlatformContextFullName = AquilaRuntimeNamespace + ".AqContext";


        //System.Data

        /// <summary>
        /// Root namespace for  System.Data.Common
        /// </summary>
        public const string SystemDataCommonNamespace = "System.Data.Common";

        public static readonly string SDCDBCommandFullName = $"{SystemDataCommonNamespace}.DbCommand";
        public static readonly string SDCDBParameterFullName = $"{SystemDataCommonNamespace}.DbParameter";

        /// <summary>
        /// Full name of Context+DllLoader&lt;&gt;.
        /// </summary>
        public const string Context_DllLoader_T = AquilaRuntimeNamespace + ".Context+DllLoader`1";

        public readonly CoreType
            Void,
            Object,
            Byte,
            Int32,
            Int64,
            Double,
            Char,
            Boolean,
            Guid,
            String,
            DateTime,
            Decimal,
            IntPtr,
            Array,
            Exception,
            RuntimeTypeHandle,
            RuntimeMethodHandle,

            //Attributes
            QueryAttribute,
            EntityAttribute,
            LinkAttribute,
            ExtensionMethodAttribute,

            //Aq Runtime Types
            AqQuery,
            AqContext,

            //System.Data.Common Types
            DbCommand,
            DbParameter;

        public CoreTypes(AquilaCompilation compilation)
        {
            Contract.ThrowIfNull(compilation);
            _compilation = compilation;
            _table = new Dictionary<string, CoreType>();

            #region BCL

            Void = Create(SpecialType.System_Void);
            Object = Create(SpecialType.System_Object);
            Byte = Create(SpecialType.System_Byte);
            Int32 = Create(SpecialType.System_Int32);
            Int64 = Create(SpecialType.System_Int64);
            Double = Create(SpecialType.System_Double);
            Decimal = Create(SpecialType.System_Decimal);
            Char = Create(SpecialType.System_Char);
            Boolean = Create(SpecialType.System_Boolean);
            String = Create(SpecialType.System_String);
            DateTime = Create(SpecialType.System_DateTime);
            Guid = CreateFromFullName("System.Guid");
            IntPtr = Create(SpecialType.System_IntPtr);
            Array = Create(SpecialType.System_Array);
            Exception = CreateFromFullName(WellKnownTypes.GetMetadataName(WellKnownType.System_Exception));
            RuntimeTypeHandle = Create(SpecialType.System_RuntimeTypeHandle);
            RuntimeMethodHandle = Create(SpecialType.System_RuntimeMethodHandle);

            #endregion

            #region Attributes

            QueryAttribute = CreateFromFullName(AquilaQueryAttributeFullName);
            EntityAttribute = CreateFromFullName(AquilaEntityAttributeFullName);
            LinkAttribute = CreateFromFullName(AquilaLinkAttributeFullName);
            ExtensionMethodAttribute = CreateFromFullName(AquilaExtensionAqAttributeFullName);

            #endregion

            #region Types

            AqQuery = CreateFromFullName(AquilaPlatformQueryFullName);
            AqContext = CreateFromFullName(AquilaPlatformContextFullName);

            #endregion

            #region System.Data.Common

            DbCommand = CreateFromFullName(SDCDBCommandFullName);
            DbParameter = CreateFromFullName(SDCDBParameterFullName);

            #endregion

            ;
        }

        #region Table of types

        readonly Dictionary<string, CoreType> _table;

        readonly Dictionary<TypeSymbol, CoreType> _typetable = new Dictionary<TypeSymbol, CoreType>();
        //readonly Dictionary<SpecialType, CoreType> _specialTypes = new Dictionary<SpecialType, CoreType>();

        CoreType Create(string name) => CreateFromFullName(AquilaRuntimeNamespace + "." + name);

        CoreType Create(SpecialType type) => CreateFromFullName(SpecialTypes.GetMetadataName(type));

        CoreType CreateFromFullName(string fullName)
        {
            var type = new CoreType(fullName);

            _table.Add(fullName, type);

            return type;
        }

        /// <summary>
        /// Gets well-known core type by its full CLR name.
        /// </summary>
        public CoreType GetTypeFromMetadataName(string fullName)
        {
            CoreType t;
            _table.TryGetValue(fullName, out t);
            return t;
        }

        /// <summary>
        /// Gets well-known core type by associated symbol.
        /// </summary>
        public CoreType GetTypeFromSymbol(TypeSymbol symbol)
        {
            CoreType t;
            _typetable.TryGetValue(symbol, out t);
            return t;
        }

        ///// <summary>
        ///// Gets special core type.
        ///// </summary>
        //public CoreType GetSpecialType(SpecialType type)
        //{
        //    CoreType t;
        //    _specialTypes.TryGetValue(type, out t);
        //    return t;
        //}

        internal void Update(AssemblySymbol coreass)
        {
            Contract.ThrowIfNull(coreass);

            foreach (var t in _table.Values)
            {
                if (t.Symbol == null)
                {
                    var fullname = t.FullName;

                    // nested types: todo: in Lookup
                    string nested = null;
                    int plus = fullname.IndexOf('+');
                    if (plus > 0)
                    {
                        nested = fullname.Substring(plus + 1);
                        fullname = fullname.Remove(plus);
                    }

                    var mdname = MetadataTypeName.FromFullName(fullname, false);
                    var symbol = coreass.LookupTopLevelMetadataType(ref mdname, true);
                    if (symbol.IsValidType())
                    {
                        if (nested != null)
                        {
                            symbol = symbol
                                .GetTypeMembers(nested)
                                .SingleOrDefault();

                            if (symbol == null)
                            {
                                continue;
                            }
                        }

                        _typetable[symbol] = t;
                        t.Update(symbol);

                        //if (symbol.SpecialType != SpecialType.None)
                        //    _specialTypes[symbol.SpecialType] = t;
                    }
                }
            }
        }

        #endregion
    }
}