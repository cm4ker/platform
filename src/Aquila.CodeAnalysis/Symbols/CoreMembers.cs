using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis;
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

        protected abstract T ResolveSymbol(NamedTypeSymbol declaringType);
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
        protected override MethodSymbol ResolveSymbol(NamedTypeSymbol declaringType)
        {
            return declaringType.GetMembers(MemberName).OfType<MethodSymbol>().First(MatchesSignature);
        }

        protected bool MatchesSignature(MethodSymbol m)
        {
            var ps = m.Parameters;
            if (ps.Length != _ptypes.Length)
                return false;

            for (int i = 0; i < ps.Length; i++)
                if (_ptypes[i] != ps[i].Type)
                    return false;

            return true;
        }
    }

    sealed class CoreField : CoreMember<FieldSymbol>
    {
        public CoreField(CoreType declaringClass, string fldName)
            : base(declaringClass, fldName)
        {
        }

        protected override FieldSymbol ResolveSymbol(NamedTypeSymbol declaringType)
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

        protected override PropertySymbol ResolveSymbol(NamedTypeSymbol declaringType)
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

        protected override MethodSymbol ResolveSymbol(NamedTypeSymbol declaringType)
        {
            return declaringType.InstanceConstructors.FirstOrDefault(MatchesSignature);
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

        protected override MethodSymbol ResolveSymbol(NamedTypeSymbol declaringType)
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

        static MethodSymbol ResolveMethod(NamedTypeSymbol declaringType, TypeSymbol castfrom, TypeSymbol castTo,
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

        protected override MethodSymbol ResolveSymbol(NamedTypeSymbol declaringType)
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


        /// <summary>Property name of <c>ScriptAttribute.IsAutoloaded</c>.</summary>
        public static string ScriptAttribute_IsAutoloaded => "IsAutoloaded";

        public CoreMethods(CoreTypes types)
        {
            Contract.ThrowIfNull(types);

            Operators = new OperatorsHolder(types);
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
            }

            public readonly CoreMethod
                IsNullOrEmpty_String,
                Concat_String_String,
                Concat_String_String_String,
                Concat_String_String_String_String,
                //Concat_Args,
                Long_ToString;
        }
    }
}