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
                Long_ToString = ct.Long.Method("ToString");
            }

            public readonly CoreMethod
                IsNullOrEmpty_String,
                Concat_String_String,
                Concat_String_String_String,
                Concat_String_String_String_String,
                //Concat_Args,
                Long_ToString;

            // public readonly CoreProperty GetName_PhpTypeInfo, GetTypeHandle_PhpTypeInfo;
        }

        //
        //     public struct PhpValueHolder
        //     {
        //         public PhpValueHolder(CoreTypes ct)
        //         {
        //             // ToString_Context = ct.PhpValue.Method("ToString", ct.Context);
        //             // ToClass = ct.PhpValue.Method("ToClass");
        //             //
        //             // Eq_PhpValue_PhpValue = ct.PhpValue.Operator(WellKnownMemberNames.EqualityOperatorName, ct.PhpValue, ct.PhpValue);
        //             // Eq_PhpValue_String = ct.PhpValue.Operator(WellKnownMemberNames.EqualityOperatorName, ct.PhpValue, ct.String);
        //             // Eq_String_PhpValue = ct.PhpValue.Operator(WellKnownMemberNames.EqualityOperatorName, ct.String, ct.PhpValue);
        //             //
        //             // Ineq_PhpValue_PhpValue = ct.PhpValue.Operator(WellKnownMemberNames.InequalityOperatorName, ct.PhpValue, ct.PhpValue);
        //             // Ineq_PhpValue_String = ct.PhpValue.Operator(WellKnownMemberNames.InequalityOperatorName, ct.PhpValue, ct.String);
        //             // Ineq_String_PhpValue = ct.PhpValue.Operator(WellKnownMemberNames.InequalityOperatorName, ct.String, ct.PhpValue);
        //             //
        //             // DeepCopy = ct.PhpValue.Method("DeepCopy");
        //             // GetValue = ct.PhpValue.Method("GetValue");
        //             // ToArray = ct.PhpValue.Method("ToArray");
        //             // AsObject = ct.PhpValue.Method("AsObject");
        //             //
        //             // Long = ct.PhpValue.Property("Long");
        //             // Double = ct.PhpValue.Property("Double");
        //             // Boolean = ct.PhpValue.Property("Boolean");
        //             // String = ct.PhpValue.Property("String");
        //             // Object = ct.PhpValue.Property("Object");
        //             // Array = ct.PhpValue.Property("Array");
        //             //
        //             // Create_Boolean = ct.PhpValue.Method("Create", ct.Boolean);
        //             // Create_Long = ct.PhpValue.Method("Create", ct.Long);
        //             // Create_Int = ct.PhpValue.Method("Create", ct.Int32);
        //             // Create_Double = ct.PhpValue.Method("Create", ct.Double);
        //             // Create_String = ct.PhpValue.Method("Create", ct.String);
        //             // Create_PhpString = ct.PhpValue.Method("Create", ct.PhpString);
        //             // Create_PhpNumber = ct.PhpValue.Method("Create", ct.PhpNumber);
        //             // Create_PhpArray = ct.PhpValue.Method("Create", ct.PhpArray);
        //             // Create_PhpAlias = ct.PhpValue.Method("Create", ct.PhpAlias);
        //             // Create_IntStringKey = ct.PhpValue.Method("Create", ct.IntStringKey);
        //             //
        //             // FromClr_Object = ct.PhpValue.Method("FromClr", ct.Object);
        //             // FromClass_Object = ct.PhpValue.Method("FromClass", ct.Object);
        //             //
        //             // Void = ct.PhpValue.Field("Void");
        //             // Null = ct.PhpValue.Field("Null");
        //             // True = ct.PhpValue.Field("True");
        //             // False = ct.PhpValue.Field("False");
        //         }
        //
        //         public readonly CoreMethod
        //             ToString_Context, ToClass, ToArray,
        //             AsObject,
        //             DeepCopy, GetValue,
        //             Eq_PhpValue_PhpValue, Eq_PhpValue_String, Eq_String_PhpValue,
        //             Ineq_PhpValue_PhpValue, Ineq_PhpValue_String, Ineq_String_PhpValue,
        //             Create_Boolean, Create_Long, Create_Int, Create_Double, Create_String, Create_PhpString, Create_PhpNumber, Create_PhpAlias, Create_PhpArray, Create_IntStringKey,
        //             FromClr_Object, FromClass_Object;
        //
        //         public readonly CoreField
        //             Void, Null, True, False;
        //
        //         public readonly CoreProperty
        //             Object, Long, Double, Boolean, String, Array;
        //
        //     }
        //
        //     public struct PhpAliasHolder
        //     {
        //         public PhpAliasHolder(CoreTypes ct)
        //         {
        //             Value = ct.PhpAlias.Field("Value");
        //
        //             EnsureObject = ct.PhpAlias.Method("EnsureObject");
        //             EnsureArray = ct.PhpAlias.Method("EnsureArray");
        //             ReleaseRef = ct.PhpAlias.Method("ReleaseRef");
        //         }
        //
        //         public readonly CoreField
        //             Value;
        //
        //         public readonly CoreMethod
        //             ReleaseRef,
        //             EnsureObject, EnsureArray;
        //     }
        //
        //     public struct PhpNumberHolder
        //     {
        //         public PhpNumberHolder(CoreTypes ct)
        //         {
        //             ToLong = ct.PhpNumber.Method("ToLong");
        //             ToDouble = ct.PhpNumber.Method("ToDouble");
        //             ToString_Context = ct.PhpNumber.Method("ToString", ct.Context);
        //             ToClass = ct.PhpNumber.Method("ToClass");
        //
        //             CompareTo_number = ct.PhpNumber.Method("CompareTo", ct.PhpNumber);
        //             CompareTo_long = ct.PhpNumber.Method("CompareTo", ct.Long);
        //             CompareTo_double = ct.PhpNumber.Method("CompareTo", ct.Double);
        //
        //             Create_Long = ct.PhpNumber.Method("Create", ct.Long);
        //             Create_Double = ct.PhpNumber.Method("Create", ct.Double);
        //             Default = ct.PhpNumber.Field("Default");
        //
        //             get_Long = ct.PhpNumber.Method("get_Long");   // TODO: special name, property
        //             get_Double = ct.PhpNumber.Method("get_Double");   // TODO: special name, property
        //
        //             Eq_number_PhpValue = ct.PhpNumber.Operator(WellKnownMemberNames.EqualityOperatorName, ct.PhpNumber, ct.PhpValue);
        //             Ineq_number_PhpValue = ct.PhpNumber.Operator(WellKnownMemberNames.InequalityOperatorName, ct.PhpNumber, ct.PhpValue);
        //             Eq_number_number = ct.PhpNumber.Operator(WellKnownMemberNames.EqualityOperatorName, ct.PhpNumber, ct.PhpNumber);
        //             Ineq_number_number = ct.PhpNumber.Operator(WellKnownMemberNames.InequalityOperatorName, ct.PhpNumber, ct.PhpNumber);
        //             Eq_number_long = ct.PhpNumber.Operator(WellKnownMemberNames.EqualityOperatorName, ct.PhpNumber, ct.Long);
        //             Ineq_number_long = ct.PhpNumber.Operator(WellKnownMemberNames.InequalityOperatorName, ct.PhpNumber, ct.Long);
        //             Eq_number_double = ct.PhpNumber.Operator(WellKnownMemberNames.EqualityOperatorName, ct.PhpNumber, ct.Double);
        //             Ineq_number_double = ct.PhpNumber.Operator(WellKnownMemberNames.InequalityOperatorName, ct.PhpNumber, ct.Double);
        //             Eq_long_number = ct.PhpNumber.Operator(WellKnownMemberNames.EqualityOperatorName, ct.Long, ct.PhpNumber);
        //             Ineq_long_number = ct.PhpNumber.Operator(WellKnownMemberNames.InequalityOperatorName, ct.Long, ct.PhpNumber);
        //
        //             Add_number_number = ct.PhpNumber.Operator(WellKnownMemberNames.AdditionOperatorName, ct.PhpNumber, ct.PhpNumber);
        //             Add_number_long = ct.PhpNumber.Operator(WellKnownMemberNames.AdditionOperatorName, ct.PhpNumber, ct.Long);
        //             Add_long_number = ct.PhpNumber.Operator(WellKnownMemberNames.AdditionOperatorName, ct.Long, ct.PhpNumber);
        //             Add_double_number = ct.PhpNumber.Operator(WellKnownMemberNames.AdditionOperatorName, ct.Double, ct.PhpNumber);
        //             Add_number_double = ct.PhpNumber.Operator(WellKnownMemberNames.AdditionOperatorName, ct.PhpNumber, ct.Double);
        //             Add_number_value = ct.PhpNumber.Operator(WellKnownMemberNames.AdditionOperatorName, ct.PhpNumber, ct.PhpValue);
        //             Add_value_number = ct.PhpNumber.Operator(WellKnownMemberNames.AdditionOperatorName, ct.PhpValue, ct.PhpNumber);
        //             Add_long_long = ct.PhpNumber.Method("Add", ct.Long, ct.Long);
        //             Add_long_double = ct.PhpNumber.Method("Add", ct.Long, ct.Double);
        //             Add_value_long = ct.PhpNumber.Method("Add", ct.PhpValue, ct.Long);
        //             Add_value_double = ct.PhpNumber.Method("Add", ct.PhpValue, ct.Double);
        //             Add_long_value = ct.PhpNumber.Method("Add", ct.Long, ct.PhpValue);
        //             Add_double_value = ct.PhpNumber.Method("Add", ct.Double, ct.PhpValue);
        //             Add_value_value = ct.PhpNumber.Method("Add", ct.PhpValue, ct.PhpValue);
        //             Add_value_array = ct.PhpNumber.Method("Add", ct.PhpValue, ct.PhpArray);
        //             Add_array_value = ct.PhpNumber.Method("Add", ct.PhpArray, ct.PhpValue);
        //
        //             Subtract_number_number = ct.PhpNumber.Operator(WellKnownMemberNames.SubtractionOperatorName, ct.PhpNumber, ct.PhpNumber);
        //             Subtract_long_number = ct.PhpNumber.Operator(WellKnownMemberNames.SubtractionOperatorName, ct.Long, ct.PhpNumber);
        //             Subtract_number_long = ct.PhpNumber.Operator(WellKnownMemberNames.SubtractionOperatorName, ct.PhpNumber, ct.Long);
        //             Subtract_long_long = ct.PhpNumber.Method("Sub", ct.Long, ct.Long);
        //             Subtract_number_double = ct.PhpNumber.Method("Sub", ct.PhpNumber, ct.Double);
        //             Subtract_long_double = ct.PhpNumber.Method("Sub", ct.Long, ct.Double);
        //             Subtract_value_value = ct.PhpNumber.Method("Sub", ct.PhpValue, ct.PhpValue);
        //             Subtract_value_long = ct.PhpNumber.Method("Sub", ct.PhpValue, ct.Long);
        //             Subtract_value_double = ct.PhpNumber.Method("Sub", ct.PhpValue, ct.Double);
        //             Subtract_value_number = ct.PhpNumber.Method("Sub", ct.PhpValue, ct.PhpNumber);
        //             Subtract_number_value = ct.PhpNumber.Method("Sub", ct.PhpNumber, ct.PhpValue);
        //             Subtract_long_value = ct.PhpNumber.Method("Sub", ct.Long, ct.PhpValue);
        //             Subtract_double_value = ct.PhpNumber.Method("Sub", ct.Double, ct.PhpValue);
        //
        //             Negation = ct.PhpNumber.Operator(WellKnownMemberNames.UnaryNegationOperatorName, ct.PhpNumber);
        //             Negation_long = ct.PhpNumber.Method("Minus", ct.Long);
        //
        //             Division_number_value = ct.PhpNumber.Operator(WellKnownMemberNames.DivisionOperatorName, ct.PhpNumber, ct.PhpValue);
        //             Division_number_number = ct.PhpNumber.Operator(WellKnownMemberNames.DivisionOperatorName, ct.PhpNumber, ct.PhpNumber);
        //             Division_number_long = ct.PhpNumber.Operator(WellKnownMemberNames.DivisionOperatorName, ct.PhpNumber, ct.Long);
        //             Division_number_double = ct.PhpNumber.Operator(WellKnownMemberNames.DivisionOperatorName, ct.PhpNumber, ct.Double);
        //             Division_long_number = ct.PhpNumber.Operator(WellKnownMemberNames.DivisionOperatorName, ct.Long, ct.PhpNumber);
        //
        //             Mul_number_number = ct.PhpNumber.Operator(WellKnownMemberNames.MultiplyOperatorName, ct.PhpNumber, ct.PhpNumber);
        //             Mul_number_double = ct.PhpNumber.Operator(WellKnownMemberNames.MultiplyOperatorName, ct.PhpNumber, ct.Double);
        //             Mul_number_long = ct.PhpNumber.Operator(WellKnownMemberNames.MultiplyOperatorName, ct.PhpNumber, ct.Long);
        //             Mul_long_number = ct.PhpNumber.Operator(WellKnownMemberNames.MultiplyOperatorName, ct.Long, ct.PhpNumber);
        //             Mul_number_value = ct.PhpNumber.Operator(WellKnownMemberNames.MultiplyOperatorName, ct.PhpNumber, ct.PhpValue);
        //             Mul_value_number = ct.PhpNumber.Operator(WellKnownMemberNames.MultiplyOperatorName, ct.PhpValue, ct.PhpNumber);
        //             Mul_long_long = ct.PhpNumber.Method("Multiply", ct.Long, ct.Long);
        //             Mul_long_double = ct.PhpNumber.Method("Multiply", ct.Long, ct.Double);
        //             Mul_double_value = ct.PhpNumber.Method("Multiply", ct.Double, ct.PhpValue);
        //             Mul_long_value = ct.PhpNumber.Method("Multiply", ct.Long, ct.PhpValue);
        //             Mul_value_value = ct.PhpNumber.Method("Multiply", ct.PhpValue, ct.PhpValue);
        //             Mul_value_long = ct.PhpNumber.Method("Multiply", ct.PhpValue, ct.Long);
        //             Mul_value_double = ct.PhpNumber.Method("Multiply", ct.PhpValue, ct.Double);
        //
        //             Pow_value_value = ct.PhpNumber.Method("Pow", ct.PhpValue, ct.PhpValue);
        //             Pow_number_number = ct.PhpNumber.Method("Pow", ct.PhpNumber, ct.PhpNumber);
        //             Pow_number_double = ct.PhpNumber.Method("Pow", ct.PhpNumber, ct.Double);
        //             Pow_number_value = ct.PhpNumber.Method("Pow", ct.PhpNumber, ct.PhpValue);
        //             Pow_double_double = ct.PhpNumber.Method("Pow", ct.Double, ct.Double);
        //             Pow_double_value = ct.PhpNumber.Method("Pow", ct.Double, ct.PhpValue);
        //             Pow_long_long = ct.PhpNumber.Method("Pow", ct.Long, ct.Long);
        //             Pow_long_double = ct.PhpNumber.Method("Pow", ct.Long, ct.Double);
        //             Pow_long_number = ct.PhpNumber.Method("Pow", ct.Long, ct.PhpNumber);
        //             Pow_long_value = ct.PhpNumber.Method("Pow", ct.Long, ct.PhpValue);
        //
        //             Mod_value_value = ct.PhpNumber.Method("Mod", ct.PhpValue, ct.PhpValue);
        //             Mod_value_long = ct.PhpNumber.Method("Mod", ct.PhpValue, ct.Long);
        //             Mod_long_long = ct.PhpNumber.Method("Mod", ct.Long, ct.Long);
        //             Mod_long_value = ct.PhpNumber.Method("Mod", ct.Long, ct.PhpValue);
        //
        //             gt_number_number = ct.PhpNumber.Operator(WellKnownMemberNames.GreaterThanOperatorName, ct.PhpNumber, ct.PhpNumber);
        //             gt_number_long = ct.PhpNumber.Operator(WellKnownMemberNames.GreaterThanOperatorName, ct.PhpNumber, ct.Long);
        //             gt_number_double = ct.PhpNumber.Operator(WellKnownMemberNames.GreaterThanOperatorName, ct.PhpNumber, ct.Double);
        //             gt_long_number = ct.PhpNumber.Operator(WellKnownMemberNames.GreaterThanOperatorName, ct.Long, ct.PhpNumber);
        //             lt_number_number = ct.PhpNumber.Operator(WellKnownMemberNames.LessThanOperatorName, ct.PhpNumber, ct.PhpNumber);
        //             lt_number_long = ct.PhpNumber.Operator(WellKnownMemberNames.LessThanOperatorName, ct.PhpNumber, ct.Long);
        //             lt_number_double = ct.PhpNumber.Operator(WellKnownMemberNames.LessThanOperatorName, ct.PhpNumber, ct.Double);
        //             lt_long_number = ct.PhpNumber.Operator(WellKnownMemberNames.LessThanOperatorName, ct.Long, ct.PhpNumber);
        //         }
        //
        //         public readonly CoreMethod
        //             ToLong, ToDouble, ToString_Context, ToClass,
        //             CompareTo_number, CompareTo_long, CompareTo_double,
        //             Add_long_long, Add_long_double, Add_value_long, Add_value_double, Add_long_value, Add_double_value, Add_value_value, Add_value_array, Add_array_value,
        //             Subtract_long_long, Subtract_number_double, Subtract_long_double, Subtract_value_value, Subtract_value_long, Subtract_value_double, Subtract_value_number, Subtract_number_value, Subtract_long_value, Subtract_double_value,
        //             Negation_long,
        //             get_Long, get_Double,
        //             Mul_long_long, Mul_long_double, Mul_long_value, Mul_double_value, Mul_value_value, Mul_value_long, Mul_value_double,
        //             Pow_long_long, Pow_long_double, Pow_long_number, Pow_long_value, Pow_double_double, Pow_double_value, Pow_number_double, Pow_number_number, Pow_number_value, Pow_value_value,
        //             Mod_value_value, Mod_value_long, Mod_long_value, Mod_long_long,
        //             Create_Long, Create_Double;
        //
        //         public readonly CoreOperator
        //             Eq_number_PhpValue, Ineq_number_PhpValue,
        //             Eq_number_number, Ineq_number_number,
        //             Eq_number_long, Ineq_number_long,
        //             Eq_number_double, Ineq_number_double,
        //             Eq_long_number, Ineq_long_number,
        //             Add_number_number, Add_number_long, Add_long_number, Add_value_number, Add_number_double, Add_double_number, Add_number_value,
        //             Subtract_number_number, Subtract_long_number, Subtract_number_long,
        //             Division_number_value, Division_number_number, Division_number_long, Division_number_double, Division_long_number,
        //             Mul_number_number, Mul_number_double, Mul_number_long, Mul_long_number, Mul_number_value, Mul_value_number,
        //             gt_number_number, gt_number_long, gt_number_double, gt_long_number,
        //             lt_number_number, lt_number_long, lt_number_double, lt_long_number,
        //             Negation;
        //
        //         public readonly CoreField
        //             Default;
        //     }
        //
        //     public struct IPhpConvertibleHolder
        //     {
        //         public IPhpConvertibleHolder(CoreTypes ct)
        //         {
        //             var t = ct.IPhpConvertible;
        //
        //             ToString_Context = t.Method("ToString", ct.Context);
        //             ToNumber = t.Method("ToNumber");
        //             ToClass = t.Method("ToClass");
        //             ToArray = t.Method("ToArray");
        //         }
        //
        //         public readonly CoreMethod
        //             ToString_Context, ToNumber, ToClass, ToArray;
        //     }
        //
        //     public struct PhpStringHolder
        //     {
        //         public PhpStringHolder(CoreTypes ct)
        //         {
        //             ToString_Context = ct.PhpString.Method("ToString", ct.Context);
        //             ToNumber = ct.PhpString.Method("ToNumber");
        //             ToBytes_Context = ct.PhpString.Method("ToBytes", ct.Context);
        //
        //             EnsureWritable = ct.PhpString.Method("EnsureWritable");
        //             AsWritable_PhpString = ct.PhpString.Method("AsWritable", ct.PhpString);
        //             AsArray_PhpString = ct.PhpString.Method("AsArray", ct.PhpString);
        //
        //             IsNull_PhpString = ct.PhpString.Method("IsNull", ct.PhpString);
        //
        //             implicit_from_string = ct.String.CastImplicit(ct.PhpString);
        //         }
        //
        //         public readonly CoreMethod
        //             ToString_Context, ToNumber, ToBytes_Context,
        //             EnsureWritable, AsWritable_PhpString, AsArray_PhpString,
        //             IsNull_PhpString;
        //
        //         public readonly CoreCast
        //             implicit_from_string;
        //     }
        //
        //     public struct PhpStringBlobHolder
        //     {
        //         public PhpStringBlobHolder(CoreTypes ct)
        //         {
        //             Add_String = ct.PhpString_Blob.Method("Add", ct.String);
        //             Add_PhpString = ct.PhpString_Blob.Method("Add", ct.PhpString);
        //             Add_PhpValue_Context = ct.PhpString_Blob.Method("Add", ct.PhpValue, ct.Context);
        //         }
        //
        //         public readonly CoreMethod
        //             Add_String, Add_PhpString, Add_PhpValue_Context;
        //     }
        //
        //     public struct IPhpArrayHolder
        //     {
        //         public IPhpArrayHolder(CoreTypes ct)
        //         {
        //             var arr = ct.IPhpArray;
        //
        //             RemoveKey_IntStringKey = arr.Method("RemoveKey", ct.IntStringKey);
        //             RemoveKey_PhpValue = arr.Method("RemoveKey", ct.PhpValue);
        //
        //             GetItemValue_IntStringKey = arr.Method("GetItemValue", ct.IntStringKey);
        //             GetItemValue_PhpValue = arr.Method("GetItemValue", ct.PhpValue);
        //
        //             SetItemValue_IntStringKey_PhpValue = arr.Method("SetItemValue", ct.IntStringKey, ct.PhpValue);
        //             SetItemValue_PhpValue_PhpValue = arr.Method("SetItemValue", ct.PhpValue, ct.PhpValue);
        //
        //             SetItemAlias_IntStringKey_PhpAlias = arr.Method("SetItemAlias", ct.IntStringKey, ct.PhpAlias);
        //             SetItemAlias_PhpValue_PhpAlias = arr.Method("SetItemAlias", ct.PhpValue, ct.PhpAlias);
        //
        //             AddValue_PhpValue = arr.Method("AddValue", ct.PhpValue);
        //
        //             EnsureItemObject_IntStringKey = arr.Method("EnsureItemObject", ct.IntStringKey);
        //             EnsureItemArray_IntStringKey = arr.Method("EnsureItemArray", ct.IntStringKey);
        //             EnsureItemAlias_IntStringKey = arr.Method("EnsureItemAlias", ct.IntStringKey);
        //
        //             Count = arr.Property("Count");
        //         }
        //
        //         public readonly CoreProperty
        //             Count;
        //
        //         public readonly CoreMethod
        //             RemoveKey_IntStringKey, RemoveKey_PhpValue,
        //             GetItemValue_IntStringKey, GetItemValue_PhpValue,
        //             SetItemValue_IntStringKey_PhpValue, SetItemValue_PhpValue_PhpValue,
        //             SetItemAlias_IntStringKey_PhpAlias, SetItemAlias_PhpValue_PhpAlias,
        //             AddValue_PhpValue,
        //             EnsureItemObject_IntStringKey, EnsureItemArray_IntStringKey, EnsureItemAlias_IntStringKey;
        //     }
        //
        //     public struct PhpArrayHolder
        //     {
        //         public PhpArrayHolder(CoreTypes ct)
        //         {
        //             var t = ct.PhpArray;
        //
        //             //
        //             ToString_Context = t.Method("ToString", ct.Context);
        //             ToClass = t.Method("ToClass");
        //
        //             RemoveKey_IntStringKey = t.Method("RemoveKey", ct.IntStringKey);
        //
        //             GetItemValue_IntStringKey = t.Method("GetItemValue", ct.IntStringKey);
        //             GetItemRef_IntStringKey = t.Method("GetItemRef", ct.IntStringKey);
        //
        //             DeepCopy = t.Method("DeepCopy");
        //             GetForeachEnumerator_Boolean = t.Method("GetForeachEnumerator", ct.Boolean);
        //
        //             SetItemValue_IntStringKey_PhpValue = t.Method("SetItemValue", ct.IntStringKey, ct.PhpValue);
        //             SetItemAlias_IntStringKey_PhpAlias = t.Method("SetItemAlias", ct.IntStringKey, ct.PhpAlias);
        //
        //             EnsureItemObject_IntStringKey = t.Method("EnsureItemObject", ct.IntStringKey);
        //             EnsureItemArray_IntStringKey = t.Method("EnsureItemArray", ct.IntStringKey);
        //             EnsureItemAlias_IntStringKey = t.Method("EnsureItemAlias", ct.IntStringKey);
        //
        //             Add_PhpValue = ct.PhpHashtable.Method("Add", ct.PhpValue);
        //             Add_IntStringKey_PhpValue = ct.PhpHashtable.Method("Add", ct.IntStringKey, ct.PhpValue);
        //
        //             New_PhpValue = t.Method("New", ct.PhpValue);
        //             Union_PhpArray_PhpArray = t.Method("Union", ct.PhpArray, ct.PhpArray);
        //
        //             Empty = t.Field("Empty");
        //         }
        //
        //         public readonly CoreMethod
        //             ToClass, ToString_Context,
        //             RemoveKey_IntStringKey,
        //             GetItemValue_IntStringKey, GetItemRef_IntStringKey,
        //             SetItemValue_IntStringKey_PhpValue, SetItemAlias_IntStringKey_PhpAlias, Add_PhpValue,
        //             EnsureItemObject_IntStringKey, EnsureItemArray_IntStringKey, EnsureItemAlias_IntStringKey,
        //             DeepCopy, GetForeachEnumerator_Boolean,
        //             Add_IntStringKey_PhpValue,
        //             New_PhpValue, Union_PhpArray_PhpArray;
        //
        //         public readonly CoreField
        //             Empty;
        //     }
        //
        //     // public struct ConstructorsHolder
        //     // {
        //     //     public ConstructorsHolder(CoreTypes ct)
        //     //     {
        //     //         PhpAlias_PhpValue_int = ct.PhpAlias.Ctor(ct.PhpValue, ct.Int32);
        //     //         PhpString_Blob = ct.PhpString.Ctor(ct.PhpString_Blob);
        //     //         PhpString_string_string = ct.PhpString.Ctor(ct.String, ct.String);
        //     //         PhpString_PhpValue_Context = ct.PhpString.Ctor(ct.PhpValue, ct.Context);
        //     //         PhpString_PhpString = ct.PhpString.Ctor(ct.PhpString);
        //     //         Blob = ct.PhpString_Blob.Ctor();
        //     //         PhpArray = ct.PhpArray.Ctor();
        //     //         PhpArray_int = ct.PhpArray.Ctor(ct.Int32);
        //     //         IntStringKey_long = ct.IntStringKey.Ctor(ct.Long);
        //     //         IntStringKey_string = ct.IntStringKey.Ctor(ct.String);
        //     //         ScriptAttribute_string_long = ct.ScriptAttribute.Ctor(ct.String, ct.Long);
        //     //         PhpTraitAttribute = ct.PhpTraitAttribute.Ctor();
        //     //         PharAttribute_string = ct.PharAttribute.Ctor(ct.String);
        //     //         PhpTypeAttribute_string_string = ct.PhpTypeAttribute.Ctor(ct.String, ct.String);
        //     //         PhpTypeAttribute_string_string_byte = ct.PhpTypeAttribute.Ctor(ct.String, ct.String, ct.Byte);
        //     //         PhpFieldsOnlyCtorAttribute = ct.PhpFieldsOnlyCtorAttribute.Ctor();
        //     //         PhpHiddenAttribute = ct.PhpHiddenAttribute.Ctor();
        //     //         NotNullAttribute = ct.NotNullAttribute.Ctor();
        //     //         DefaultValueAttribute_string = ct.DefaultValueAttribute.Ctor(ct.String);
        //     //
        //     //         ScriptDiedException = ct.ScriptDiedException.Ctor();
        //     //         ScriptDiedException_Long = ct.ScriptDiedException.Ctor(ct.Long);
        //     //         ScriptDiedException_PhpValue = ct.ScriptDiedException.Ctor(ct.PhpValue);
        //     //
        //     //         IndirectLocal_PhpArray_IntStringKey = ct.IndirectLocal.Ctor(ct.PhpArray, ct.IntStringKey);
        //     //     }
        //
        //         // public readonly CoreConstructor
        //         //     PhpAlias_PhpValue_int,
        //         //     PhpArray, PhpArray_int,
        //         //     PhpString_Blob, PhpString_PhpString, PhpString_string_string, PhpString_PhpValue_Context,
        //         //     Blob,
        //         //     IntStringKey_long, IntStringKey_string,
        //         //     ScriptAttribute_string_long, PhpTraitAttribute, PharAttribute_string, PhpTypeAttribute_string_string, PhpTypeAttribute_string_string_byte, PhpFieldsOnlyCtorAttribute, PhpHiddenAttribute, NotNullAttribute,
        //         //     DefaultValueAttribute_string,
        //         //     ScriptDiedException, ScriptDiedException_Long, ScriptDiedException_PhpValue,
        //         //     IndirectLocal_PhpArray_IntStringKey;
        //     // }
        //
        //     // public struct ContextHolder
        //     // {
        //     //     public ContextHolder(CoreTypes ct)
        //     //     {
        //     //         Dispose = ct.Context.Method("Dispose");
        //     //         
        //     //         DeclareFunction_RoutineInfo = ct.Context.Method("DeclareFunction", ct.RoutineInfo);
        //     //         DeclareType_T = ct.Context.Method("DeclareType");
        //     //         DeclareType_PhpTypeInfo_String = ct.Context.Method("DeclareType", ct.PhpTypeInfo, ct.String);
        //     //         
        //     //         DisableErrorReporting = ct.Context.Method("DisableErrorReporting");
        //     //         EnableErrorReporting = ct.Context.Method("EnableErrorReporting");
        //     //         
        //     //         CheckIncludeOnce_TScript = ct.Context.Method("CheckIncludeOnce");
        //     //         OnInclude_TScript = ct.Context.Method("OnInclude");
        //     //         Include_string_string_PhpArray_object_RuntimeTypeHandle_bool_bool = ct.Context.Method("Include", ct.String, ct.String, ct.PhpArray, ct.Object, ct.RuntimeTypeHandle, ct.Boolean, ct.Boolean);
        //     //         
        //     //         ExpectTypeDeclared_T = ct.Context.Method("ExpectTypeDeclared");
        //     //         
        //     //         GetStatic_T = ct.Context.Method("GetStatic");
        //     //         GetDeclaredType_string_bool = ct.Context.Method("GetDeclaredType", ct.String, ct.Boolean);
        //     //         GetDeclaredTypeOrThrow_string_bool = ct.Context.Method("GetDeclaredTypeOrThrow", ct.String, ct.Boolean);
        //     //         
        //     //         Assert_bool_PhpValue = ct.Context.Method("Assert", ct.Boolean, ct.PhpValue);
        //     //         
        //     //         // properties
        //     //         RootPath = ct.Context.Property("RootPath");
        //     //         Globals = ct.Context.Property("Globals");
        //     //         Server = ct.Context.Property("Server");
        //     //         Request = ct.Context.Property("Request");
        //     //         Get = ct.Context.Property("Get");
        //     //         Post = ct.Context.Property("Post");
        //     //         Cookie = ct.Context.Property("Cookie");
        //     //         Env = ct.Context.Property("Env");
        //     //         Files = ct.Context.Property("Files");
        //     //         Session = ct.Context.Property("Session");
        //     //         HttpRawPostData = ct.Context.Property("HttpRawPostData");
        //     //     }
        //     //
        //     //     public readonly CoreMethod
        //     //         DeclareFunction_RoutineInfo, DeclareType_T, DeclareType_PhpTypeInfo_String,
        //     //         DisableErrorReporting, EnableErrorReporting,
        //     //         CheckIncludeOnce_TScript, OnInclude_TScript, Include_string_string_PhpArray_object_RuntimeTypeHandle_bool_bool,
        //     //         ExpectTypeDeclared_T,
        //     //         GetStatic_T,
        //     //         GetDeclaredType_string_bool, GetDeclaredTypeOrThrow_string_bool,
        //     //         Assert_bool_PhpValue,
        //     //         Dispose;
        //     //
        //     //     public readonly CoreProperty
        //     //         RootPath,
        //     //         Globals, Server, Request, Get, Post, Cookie, Env, Files, Session, HttpRawPostData;
        //     // }
        //     //
        //     // public struct DynamicHolder
        //     // {
        //     //     public DynamicHolder(CoreTypes ct)
        //     //     {
        //     //         BinderFactory_Function = ct.BinderFactory.Method("Function", ct.String, ct.String, ct.RuntimeTypeHandle);
        //     //         BinderFactory_InstanceFunction = ct.BinderFactory.Method("InstanceFunction", ct.String, ct.RuntimeTypeHandle, ct.RuntimeTypeHandle);
        //     //         BinderFactory_StaticFunction = ct.BinderFactory.Method("StaticFunction", ct.RuntimeTypeHandle, ct.String, ct.RuntimeTypeHandle, ct.RuntimeTypeHandle);
        //     //
        //     //         GetFieldBinder = ct.BinderFactory.Method("GetField", ct.String, ct.RuntimeTypeHandle, ct.RuntimeTypeHandle, ct.AccessMask);
        //     //         SetFieldBinder = ct.BinderFactory.Method("SetField", ct.String, ct.RuntimeTypeHandle, ct.AccessMask);
        //     //         GetClassConstBinder = ct.BinderFactory.Method("GetClassConst", ct.String, ct.RuntimeTypeHandle, ct.RuntimeTypeHandle, ct.AccessMask);
        //     //
        //     //         GetPhpTypeInfo_T = ct.PhpTypeInfoExtension.Method("GetPhpTypeInfo");
        //     //         GetPhpTypeInfo_Object = ct.PhpTypeInfoExtension.Method("GetPhpTypeInfo", ct.Object);
        //     //         GetPhpTypeInfo_RuntimeTypeHandle = ct.PhpTypeInfoExtension.Method("GetPhpTypeInfo", ct.RuntimeTypeHandle);
        //     //     }
        //     //
        //     //     public readonly CoreMethod
        //     //         BinderFactory_Function, BinderFactory_InstanceFunction, BinderFactory_StaticFunction,
        //     //         GetFieldBinder, SetFieldBinder, GetClassConstBinder,
        //     //         GetPhpTypeInfo_T, GetPhpTypeInfo_Object, GetPhpTypeInfo_RuntimeTypeHandle;
        //     // }
        //     //
        //     // public struct ReflectionHolder
        //     // {
        //     //     readonly CoreTypes _ct;
        //     //
        //     //     public ReflectionHolder(CoreTypes ct)
        //     //     {
        //     //         _ct = ct;
        //     //         _lazyCreateUserRoutine = null;
        //     //         _lazyCreateUserRoutine_String_MethodInfoArray = null;
        //     //     }
        //     //
        //     //     public MethodSymbol CreateUserRoutine_string_RuntimeMethodHandle_RuntimeMethodHandleArr
        //     //     {
        //     //         get
        //     //         {
        //     //             if (_lazyCreateUserRoutine == null)
        //     //             {
        //     //                 _lazyCreateUserRoutine = _ct.RoutineInfo.Symbol.GetMembers("CreateUserRoutine").OfType<MethodSymbol>().Single(m =>
        //     //                     m.ParameterCount == 3 &&
        //     //                     m.Parameters[0].Type.SpecialType == SpecialType.System_String &&
        //     //                     m.Parameters[1].Type.Name == "RuntimeMethodHandle" &&
        //     //                     m.Parameters[2].Type.IsSZArray());
        //     //             }
        //     //
        //     //             return _lazyCreateUserRoutine;
        //     //         }
        //     //     }
        //     //     MethodSymbol _lazyCreateUserRoutine;
        //     //
        //     //     public MethodSymbol CreateUserRoutine_String_MethodInfoArray
        //     //     {
        //     //         get
        //     //         {
        //     //             if (_lazyCreateUserRoutine_String_MethodInfoArray == null)
        //     //             {
        //     //                 _lazyCreateUserRoutine_String_MethodInfoArray = _ct.RoutineInfo.Symbol.GetMembers("CreateUserRoutine").OfType<MethodSymbol>().Single(m =>
        //     //                     m.ParameterCount == 2 &&
        //     //                     m.Parameters[0].Type.SpecialType == SpecialType.System_String &&
        //     //                     m.Parameters[1].Type.IsSZArray());
        //     //             }
        //     //
        //     //             return _lazyCreateUserRoutine_String_MethodInfoArray;
        //     //         }
        //     //     }
        //     //     MethodSymbol _lazyCreateUserRoutine_String_MethodInfoArray;
        //     // }
        // }
    }
}