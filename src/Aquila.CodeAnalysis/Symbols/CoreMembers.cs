using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    #region CoreMember<T>

    /// <summary>
    /// Helper object caching a well-known symbol.
    /// </summary>
    /// <typeparam name="T">Type of the symbol.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}"), DebuggerNonUserCode]
    abstract class CoreMember<T> where T : Symbol
    {
        string DebuggerDisplay => $"{GetType().Name} '{DeclaringClass.FullName}.{MemberName}'";

        /// <summary>
        /// Declaring class. Cannot be <c>null</c>.
        /// </summary>
        public CoreType DeclaringClass { get; }

        /// <summary>
        /// Member name.
        /// </summary>
        public string MemberName { get; }

        T _lazySymbol = null;

        protected CoreMember(CoreType declaringClass, string memberName)
        {
            this.DeclaringClass = declaringClass ?? throw ExceptionUtilities.ArgumentNull(nameof(declaringClass));
            this.MemberName = memberName ?? throw ExceptionUtilities.ArgumentNull(nameof(memberName));
        }

        /// <summary>Implicit cast to the symbol.</summary>
        public static implicit operator T(CoreMember<T> m) => m.Symbol;

        public T Symbol
        {
            get
            {
                if (_lazySymbol == null)
                {
                    var type = DeclaringClass.Symbol;
                    Contract.ThrowIfNull(type, "Predefined type '{0}' is not defined or imported",
                        DeclaringClass.FullName);

                    var symbol = ResolveSymbol(type);
                    Contract.ThrowIfNull(symbol, "{0} {1} is not defined.", typeof(T).Name, MemberName);

                    Interlocked.CompareExchange(ref _lazySymbol, symbol, null);
                }

                return _lazySymbol;
            }
        }

        protected abstract T ResolveSymbol(TypeSymbol declaringType);
    }

    /// <summary>
    /// Descriptor of a well-known method.
    /// </summary>
    class CoreMethod : CoreMember<MethodSymbol>
    {
        /// <summary>
        /// Parametyer types.
        /// </summary>
        readonly CoreType[] _ptypes;

        public CoreMethod(CoreType declaringClass, string methodName, params CoreType[] ptypes)
            : base(declaringClass, methodName)
        {
            _ptypes = ptypes;
        }

        /// <summary>
        /// Resolves <see cref="MethodSymbol"/> of this descriptor.
        /// </summary>
        protected override MethodSymbol ResolveSymbol(TypeSymbol declaringType)
        {
            return declaringType.GetMembers(MemberName).OfType<MethodSymbol>().First(MatchesSignature);
        }

        protected bool MatchesSignature(MethodSymbol m)
        {
            var ps = m.Parameters;
            if (ps.Length != _ptypes.Length)
                return false;

            for (int i = 0; i < ps.Length; i++)
            {
                var leftSym = (TypeSymbol)_ptypes[i];
                var rightSym = ps[i].Type;

                if (leftSym is ConstructedNamedTypeSymbol l_cnts && rightSym is not ConstructedNamedTypeSymbol)
                    leftSym = l_cnts.ConstructedFrom;

                if (rightSym is ConstructedNamedTypeSymbol r_cnts && leftSym is not ConstructedNamedTypeSymbol)
                    rightSym = r_cnts.ConstructedFrom;

                if (leftSym != rightSym)
                {
                    return false;
                }
            }

            return true;
        }
    }

    sealed class CoreField : CoreMember<FieldSymbol>
    {
        public CoreField(CoreType declaringClass, string fldName)
            : base(declaringClass, fldName)
        {
        }

        protected override FieldSymbol ResolveSymbol(TypeSymbol declaringType)
        {
            return declaringType.GetMembers(MemberName).OfType<FieldSymbol>().FirstOrDefault();
        }
    }

    sealed class CoreProperty : CoreMember<PropertySymbol>
    {
        public CoreProperty(CoreType declaringClass, string propertyName)
            : base(declaringClass, propertyName)
        {
        }

        public MethodSymbol Getter => Symbol.GetMethod;

        public MethodSymbol Setter => Symbol.SetMethod;

        protected override PropertySymbol ResolveSymbol(TypeSymbol declaringType)
        {
            return declaringType.GetMembers(MemberName).OfType<PropertySymbol>().FirstOrDefault();
        }
    }

    /// <summary>
    /// Descriptor of a well-known constructor.
    /// </summary>
    sealed class CoreConstructor : CoreMethod
    {
        public CoreConstructor(CoreType declaringClass, params CoreType[] ptypes)
            : base(declaringClass, WellKnownMemberNames.InstanceConstructorName, ptypes)
        {
        }

        protected override MethodSymbol ResolveSymbol(TypeSymbol declaringType)
        {
            if (declaringType is NamedTypeSymbol nst)
                return nst.InstanceConstructors.FirstOrDefault(MatchesSignature);

            return null;
        }
    }

    /// <summary>
    /// Descriptor of a well-known operator method.
    /// </summary>
    sealed class CoreOperator : CoreMethod
    {
        /// <summary>
        /// Creates the descriptor.
        /// </summary>
        /// <param name="declaringClass">Containing class.</param>
        /// <param name="name">Operator name, without <c>op_</c> prefix.</param>
        /// <param name="ptypes">CLR parameters.</param>
        public CoreOperator(CoreType declaringClass, string name, params CoreType[] ptypes)
            : base(declaringClass, name, ptypes)
        {
            Debug.Assert(name.StartsWith("op_"));
        }

        protected override MethodSymbol ResolveSymbol(TypeSymbol declaringType)
        {
            return declaringType.GetMembers(MemberName)
                .OfType<MethodSymbol>()
                .Where(m => m.HasSpecialName)
                .FirstOrDefault(MatchesSignature);
        }
    }

    /// <summary>
    /// Descriptor of a well-known cast operator method.
    /// </summary>
    sealed class CoreCast : CoreMethod
    {
        readonly CoreType _castTo;

        public CoreCast(CoreType castFrom, CoreType castTo, bool explicit_cast)
            : base(castFrom,
                explicit_cast
                    ? WellKnownMemberNames.ExplicitConversionName
                    : WellKnownMemberNames.ImplicitConversionName, castFrom)
        {
            _castTo = castTo ?? throw new ArgumentNullException(nameof(castTo));
        }

        static MethodSymbol ResolveMethod(TypeSymbol declaringType, TypeSymbol castfrom, TypeSymbol castTo,
            string memberName)
        {
            var methods = declaringType.GetMembers(memberName);
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i] is MethodSymbol m &&
                    m.HasSpecialName &&
                    m.IsStatic &&
                    m.ParameterCount == 1 &&
                    m.Parameters[0].Type == castfrom &&
                    m.ReturnType == castTo)
                {
                    return m;
                }
            }

            return null;
        }

        protected override MethodSymbol ResolveSymbol(TypeSymbol declaringType)
        {
            // {castTo}.op_implicit({castFrom}) : {castTo}
            // {castFrom}.op_implicit({castFrom}) : {castTo}

            return
                ResolveMethod(declaringType, declaringType, _castTo, MemberName) ??
                ResolveMethod(_castTo, declaringType, _castTo, MemberName);
        }
    }

    #endregion

    /// <summary>
    /// Set of well-known methods declared in a core library.
    /// </summary>
    class CoreMethods
    {
        public readonly OperatorsHolder Operators;
        public readonly RuntimeHolder Runtime;
        public readonly RenderTreeBuilderHolder RenderTreeBuilder;

        public CoreMethods(CoreTypes types)
        {
            Contract.ThrowIfNull(types);

            Operators = new OperatorsHolder(types);
            Runtime = new RuntimeHolder(types);
            RenderTreeBuilder = new RenderTreeBuilderHolder(types);
        }

        public struct OperatorsHolder
        {
            public OperatorsHolder(CoreTypes ct)
            {
                IsNullOrEmpty_String = ct.String.Method("IsNullOrEmpty", ct.String);
                Concat_String_String = ct.String.Method("Concat", ct.String, ct.String);
                Concat_String_String_String = ct.String.Method("Concat", ct.String, ct.String, ct.String);
                Concat_String_String_String_String =
                    ct.String.Method("Concat", ct.String, ct.String, ct.String, ct.String);
                Long_ToString = ct.Int64.Method("ToString");

                op_Equality_Guid_Guid = ct.Guid.Method("op_Equality", ct.Guid, ct.Guid);
                NewGuid = ct.Guid.Method("NewGuid");

                //Comparators
                Ceq_long_double = ct.AqComparison.Method("Ceq", ct.Int64, ct.Double);
                Ceq_long_bool = ct.AqComparison.Method("Ceq", ct.Int64, ct.Boolean);
                Ceq_long_string = ct.AqComparison.Method("Ceq", ct.Int64, ct.String);

                Ceq_int_double = ct.AqComparison.Method("Ceq", ct.Int32, ct.Double);
                Ceq_int_bool = ct.AqComparison.Method("Ceq", ct.Int32, ct.Boolean);
                Ceq_int_string = ct.AqComparison.Method("Ceq", ct.Int32, ct.String);

                Ceq_double_string = ct.AqComparison.Method("Ceq", ct.Double, ct.String);
                Ceq_string_long = ct.AqComparison.Method("Ceq", ct.String, ct.Int64);
                Ceq_string_double = ct.AqComparison.Method("Ceq", ct.String, ct.Double);
                Ceq_string_bool = ct.AqComparison.Method("Ceq", ct.String, ct.Boolean);
                Ceq_string_string = ct.AqComparison.Method("Ceq", ct.String, ct.String);
                Clt_long_double = ct.AqComparison.Method("Clt", ct.Int64, ct.Double);
                Cgt_long_double = ct.AqComparison.Method("Cgt", ct.Int64, ct.Double);
                Clt_int_double = ct.AqComparison.Method("Clt", ct.Int64, ct.Double);
                Cgt_int_double = ct.AqComparison.Method("Cgt", ct.Int64, ct.Double);
                Compare_bool_bool = ct.AqComparison.Method("Compare", ct.Boolean, ct.Boolean);

                Select = ct.System_linq_enumerable.Method("Select", ct.IEnumerable_arg1, ct.Func_arg2);
                ToList = ct.System_linq_enumerable.Method("ToList", ct.IEnumerable_arg1);

                ComponentBase_OnInitialized = null;
                ComponentBase_BuildRenderTree = null;
            }

            public readonly CoreMethod

                //String
                IsNullOrEmpty_String,
                Concat_String_String,
                Concat_String_String_String,
                Concat_String_String_String_String,

                //Long
                Long_ToString,

                //Guid                    
                op_Equality_Guid_Guid,
                NewGuid,

                //List
                Select,
                ToList,

                //Equality
                //
                Ceq_long_double,
                Ceq_long_bool,
                Ceq_long_string,
                Ceq_double_string,
                Ceq_string_long,
                Ceq_string_double,
                Ceq_string_bool,
                Ceq_string_string,
                Clt_long_double,
                Cgt_long_double,
                Compare_bool_bool,
                Ceq_int_double,
                Ceq_int_bool,
                Ceq_int_string,
                Clt_int_double,
                Cgt_int_double,

                #region Web

                //ComponentBase
                ComponentBase_BuildRenderTree,
                ComponentBase_OnInitialized

                #endregion

                ;
        }

        public struct RuntimeHolder
        {
            public RuntimeHolder(CoreTypes ct)
            {
                DataContext = ct.AqContext.Property("DataContext");
                DataRuntimeContext = ct.AqContext.Property("DataRuntimeContext");
                CreateCommand = ct.AqContext.Method("CreateCommand");
                CreateParameterHelper = ct.AqHelper.Method("AddParameter", ct.DbCommand, ct.String, ct.Object);

                InvokeInsert = ct.AqHelper.Method("InvokeInsert", ct.AqContext, ct.String, ct.AqParamValue.AsSZArray());
                InvokeUpdate = ct.AqHelper.Method("InvokeUpdate", ct.AqContext, ct.String, ct.AqParamValue.AsSZArray());
                InvokeDelete = ct.AqHelper.Method("InvokeDelete", ct.AqContext, ct.String, ct.AqParamValue.AsSZArray());
                InvokeSelect = ct.AqHelper.Method("InvokeSelect", ct.AqContext, ct.String, ct.AqParamValue.AsSZArray(),
                    ct.AqReadDelegate);
            }

            public readonly CoreMethod

                //Aquila.Runtime methods
                CreateCommand,
                CreateParameterHelper,
                InvokeInsert,
                InvokeUpdate,
                InvokeDelete,
                InvokeSelect;

            public readonly CoreProperty
                DataContext,
                DataRuntimeContext;
        }
        
        public struct RenderTreeBuilderHolder
        {
            public RenderTreeBuilderHolder(CoreTypes ct)
            {
                OpenElement = ct.Web_RenderTreeBuilder.Method(nameof(OpenElement), ct.Int32, ct.String);
                CloseElement = ct.Web_RenderTreeBuilder.Method(nameof(CloseElement));
                AddAttribute = ct.Web_RenderTreeBuilder.Method(nameof(AddAttribute), ct.Int32, ct.String, ct.String);
                AddMarkupContent = ct.Web_RenderTreeBuilder.Method(nameof(AddMarkupContent), ct.Int32, ct.String);
            }

            public readonly CoreMethod
                OpenElement,
                CloseElement,
                AddAttribute,
                AddMarkupContent
                ;

        }
    }
}