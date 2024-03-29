﻿using System;
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

        private readonly CoreTypes _ct;
        private TypeSymbol _symbol;

        private bool _isArray;
        private CoreType _elementType;

        /// <summary>
        /// Gets associated symbol.
        /// </summary>
        /// <remarks>Assuming single singleton instance of library.</remarks>
        public TypeSymbol Symbol
        {
            get
            {
                if (_isArray && _symbol == null)
                {
                    var arrSymbol =
                        ArrayTypeSymbol.CreateSZArray(_ct.Array.Symbol.ContainingAssembly, _elementType._symbol);

                    _symbol = arrSymbol;
                }

                return _symbol;
            }
        }

        public NamedTypeSymbol Construct(params TypeSymbol[] types)
        {
            return ((NamedTypeSymbol)Symbol).Construct(types);
        }

        public CoreType(string fullName, CoreTypes ct)
        {
            Debug.Assert(!string.IsNullOrEmpty(fullName));
            this.FullName = fullName;
            _ct = ct;
        }

        internal void Update(NamedTypeSymbol symbol)
        {
            Contract.ThrowIfNull(symbol);
            Debug.Assert(this.Symbol == null);
            _symbol = symbol;
        }

        public CoreType AsSZArray()
        {
            return new CoreType(this.FullName + "[]", _ct) { _isArray = true, _elementType = this };
        }


        /// <summary>
        /// Implicit cast to type symbol.
        /// </summary>
        public static implicit operator NamedTypeSymbol(CoreType t) => (NamedTypeSymbol)t.Symbol;

        
        
        /// <summary>
        /// Implicit cast to type symbol.
        /// </summary>
        public static explicit operator TypeSymbol(CoreType t) => t.Symbol;


        #region IEquatable

        bool IEquatable<CoreType>.Equals(CoreType other)
        {
            return object.ReferenceEquals(this, other);
        }

        bool IEquatable<TypeSymbol>.Equals(TypeSymbol other)
        {
            return this.Symbol == other;
        }

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

        public const string AquilaUserVisibleAttributeFullName =
            AquilaRuntimeNamespace + ".AqUserVisibleAttribute";

        public const string AquilaEntityAttributeFullName = AquilaRuntimeNamespace + ".EntityAttribute";

        public const string AquilaLinkAttributeFullName = AquilaRuntimeNamespace + ".LinkAttribute";

        public const string AquilaTargetPlatformAttributeFullName = AquilaRuntimeNamespace + ".TargetPlatform";

        public const string AquilaPlatformQueryFullName = AquilaRuntimeNamespace + ".AqQuery";

        public const string AquilaPlatformContextFullName = AquilaRuntimeNamespace + ".AqContext";
        public const string AquilaPlatformHelperFullName = AquilaRuntimeNamespace + ".AqHelper";

        public const string AquilaHttpMethodHandlerAttributeFullName = AquilaRuntimeNamespace + ".HttpHandlerAttribute";

        public const string AquilaRuntimeInitAttributeFullName = AquilaRuntimeNamespace + ".RuntimeInitAttribute";

        public const string AquilaRuntimeInitKindFullName = AquilaRuntimeNamespace + ".RuntimeInitKind";

        public const string AquilaHttpMethodKindFullName = AquilaRuntimeNamespace + ".HttpMethodKind";

        public const string AquilaParamValueFullName = AquilaRuntimeNamespace + ".AqParamValue";

        public const string AquilaReadDelegateFullName = AquilaRuntimeNamespace + ".AqReadDelegate";
        //System.Data

        /// <summary>
        /// Root namespace for  System.Data.Common
        /// </summary>
        public const string SystemDataCommonNamespace = "System.Data.Common";

        public static readonly string SDCDBCommandFullName = $"{SystemDataCommonNamespace}.DbCommand";
        public static readonly string SDCDBParameterFullName = $"{SystemDataCommonNamespace}.DbParameter";
        public static readonly string SDCDBReaderFullName = $"{SystemDataCommonNamespace}.DbDataReader";

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
            @Action,
            IEnumerable,
            IEnumerable_arg1,
            Func_arg1,
            Func_arg2,
            ImmutableArray_arg1,
            List_arg1,
            System_linq_enumerable,
            Task,

            //Attributes
            RuntimeInitAttribute,
            QueryAttribute,
            EntityAttribute,
            LinkAttribute,
            ExtensionMethodAttribute,
            HttpHandlerAttribute,
            CrudHandlerAttribute,
            EndpointAttribute,

            //Enums
            RuntimeInitKind,
            HttpMethodKind,

            //Aq Runtime Types
            AqQuery,
            AqContext,
            AqException,
            AqHelper,
            AqComparison,
            AqParamValue,
            AqReadDelegate,
            AqFactoryDelegate,
            AqList,
            //AqImmutableCollection,

            //System.Data.Common Types
            DbCommand,
            DbParameter,
            DbReader,

            //WebTypes
            Web_ComponentBase,
            Web_RenderTreeBuilder,
            Web_ParameterView,
            Web_Route

            //End
            ;

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
            IEnumerable = Create(SpecialType.System_Collections_IEnumerable);
            IEnumerable_arg1 = Create(SpecialType.System_Collections_Generic_IEnumerable_T);
            Func_arg1 = CreateFromFullName(WellKnownTypes.GetMetadataName(WellKnownType.System_Func_T));
            Func_arg2 = CreateFromFullName(WellKnownTypes.GetMetadataName(WellKnownType.System_Func_T2));
            ImmutableArray_arg1 = CreateFromFullName("System.Collections.Immutable.ImmutableArray`1");
            List_arg1 = CreateFromFullName("System.Collections.Generic.List`1");
            System_linq_enumerable = CreateFromFullName("System.Linq.Enumerable");
            Task = CreateFromFullName("System.Threading.Tasks.Task");
            #endregion

            #region Linq

            #endregion


            #region Attributes

            RuntimeInitAttribute = CreateFromFullName(AquilaRuntimeInitAttributeFullName);
            QueryAttribute = CreateFromFullName(AquilaQueryAttributeFullName);
            EntityAttribute = CreateFromFullName(AquilaEntityAttributeFullName);
            LinkAttribute = CreateFromFullName(AquilaLinkAttributeFullName);
            ExtensionMethodAttribute = CreateFromFullName(AquilaExtensionAqAttributeFullName);
            HttpHandlerAttribute = CreateFromFullName(AquilaHttpMethodHandlerAttributeFullName);
            CrudHandlerAttribute = CreateFromRuntimeName("CrudHandlerAttribute");
            EndpointAttribute = CreateFromRuntimeName("endpoint");

            #endregion

            #region Enums

            RuntimeInitKind = CreateFromFullName(AquilaRuntimeInitKindFullName);
            HttpMethodKind = CreateFromFullName(AquilaHttpMethodKindFullName);

            #endregion

            #region Types

            AqQuery = CreateFromFullName(AquilaPlatformQueryFullName);
            AqContext = CreateFromFullName(AquilaPlatformContextFullName);
            AqHelper = CreateFromFullName(AquilaPlatformHelperFullName);
            AqParamValue = CreateFromFullName(AquilaParamValueFullName);
            AqReadDelegate = CreateFromRuntimeName("AqReadDelegate`1");
            AqFactoryDelegate = CreateFromRuntimeName("AqFactoryDelegate`1");
            AqComparison = CreateFromRuntimeName("AqComparison");
            AqList = CreateFromRuntimeName("AqList`1");
            AqException = CreateFromRuntimeName("AqException");

            #endregion

            #region System.Data.Common

            DbCommand = CreateFromFullName(SDCDBCommandFullName);
            DbParameter = CreateFromFullName(SDCDBParameterFullName);
            DbReader = CreateFromFullName(SDCDBReaderFullName);

            #endregion


            #region WebTypes

            Web_ComponentBase = CreateFromFullName("Aquila.Web.Razor.AqComponentBase");
            Web_RenderTreeBuilder = CreateFromFullName("Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder");
            Web_Route = CreateFromFullName("Microsoft.AspNetCore.Components.RouteAttribute");
            Web_ParameterView = CreateFromFullName("Microsoft.AspNetCore.Component.ParameterView");
            #endregion

            ;
        }

        #region Table of types

        readonly Dictionary<string, CoreType> _table;

        readonly Dictionary<TypeSymbol, CoreType> _typetable = new Dictionary<TypeSymbol, CoreType>();


        CoreType CreateFromRuntimeName(string name) => CreateFromFullName(AquilaRuntimeNamespace + "." + name);

        CoreType Create(SpecialType type) => CreateFromFullName(type.GetMetadataName());

        CoreType CreateFromFullName(string fullName)
        {
            var type = new CoreType(fullName, this);

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

                    }
                }
            }
        }

        #endregion
    }
}