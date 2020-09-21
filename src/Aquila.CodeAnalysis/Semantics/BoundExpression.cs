using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.Compiler.Utilities;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;
using Expression = Aquila.Syntax.Ast.Expression;

namespace Aquila.CodeAnalysis.Semantics
{
    #region BoundAccess

    /// <summary>
    /// Expression access information.
    /// Describes the context in which an expression is used.
    /// </summary>
    [DebuggerDisplay("[{_flags}]")]
    public struct BoundAccess
    {
        /// <summary>
        /// The expression access kind - read, write, ensured.
        /// </summary>
        AccessMask _flags;

        /// <summary>
        /// Optional. Type the expression will be converted to.
        /// </summary>
        TypeSymbol _targetType;

        /// <summary>
        /// Type information for the write access (right value of the assignment).
        /// In case of <see cref="EnsureArray"/>, the type represents the written element type.
        /// </summary>
        TypeRefMask _writeTypeMask;

        #region Properties

        /// <summary>
        /// In case the expression's value will be read.
        /// </summary>
        public bool IsRead => (_flags & AccessMask.Read) != 0;

        /// <summary>
        /// In case a value will be written to the variable.
        /// </summary>
        public bool IsWrite => (_flags & AccessMask.Write) != 0;

        /// <summary>
        /// In case a variable will be unset.
        /// </summary>
        public bool IsUnset => (_flags & AccessMask.Unset) != 0;

        /// <summary>
        /// In case the expression is read within <c>isset</c> operation.
        /// </summary>
        public bool IsIsSet => (_flags & AccessMask.Isset) == AccessMask.Isset;

        /// <summary>
        /// A flag denotating a value that is not aliased.
        /// In case of read access, it denotates the source value.
        /// In case of write access, it denotates the assignment target.
        /// </summary>
        public bool IsNotRef => _flags.IsNotRef();

        /// <summary>
        /// Gets type of value to be written.
        /// </summary>
        public TypeRefMask WriteMask => _writeTypeMask;

        /// <summary>
        /// Optional.
        /// Type the expression will be implicitly converted to.
        /// </summary>
        internal TypeSymbol TargetType => _targetType;

        /// <summary>
        /// Gets inyternal access flags.
        /// </summary>
        public AccessMask Flags => _flags;

        /// <summary>
        /// The variable will be aliased and read.
        /// </summary>
        public bool IsReadRef => (_flags & AccessMask.ReadRef) == AccessMask.ReadRef;

        /// <summary>
        /// A reference will be written.
        /// </summary>
        public bool IsWriteRef => (_flags & AccessMask.WriteRef) == AccessMask.WriteRef;

        /// <summary>
        /// The expression won't be read or written to.
        /// </summary>
        public bool IsNone => (_flags == 0);

        /// <summary>
        /// The read is for check purposes only and won't result in a warning in case the variable does not exist.
        /// </summary>
        public bool IsQuiet => (_flags & AccessMask.ReadQuiet) != 0;

        /// <summary>
        /// In case we might change the variable content to array, object or an alias (we may need write access).
        /// </summary>
        public bool IsEnsure => (_flags & ~AccessMask.Read &
                                 (AccessMask.ReadRef | AccessMask.EnsureObject | AccessMask.EnsureArray)) != 0;

        /// <summary>
        /// Gets value indicating the variable might be changed in context of the access.
        /// </summary>
        public bool MightChange => IsWrite || IsUnset || IsEnsure || (IsQuiet && !IsIsSet);

        /// <summary>
        /// In case an alias will be written to the variable.
        /// </summary>
        public bool WriteRef => (_flags & AccessMask.WriteRef) == AccessMask.WriteRef;

        /// <summary>
        /// In case the expression has to read as an object to allow writing its fields.
        /// In case of a variable, created object has to be written back.
        /// </summary>
        public bool EnsureObject => (_flags & AccessMask.EnsureObject) == AccessMask.EnsureObject;

        /// <summary>
        /// In case variable will be accessed as array in manner of setting its entries.
        /// <code>VARIABLE[] = ...</code>
        /// </summary>
        public bool EnsureArray => (_flags & AccessMask.EnsureArray) == AccessMask.EnsureArray;

        #endregion

        /// <summary>
        /// Gets human readable access flags.
        /// </summary>
        public override string ToString() => _flags.ToString();

        #region Construction

        private BoundAccess(AccessMask flags, TypeSymbol targetType, TypeRefMask writeTypeMask)
        {
            _flags = flags;
            _writeTypeMask = writeTypeMask;
            _targetType = targetType;

            Debug.Assert(EnsureArray ^ EnsureObject ^ IsReadRef || !IsEnsure); // only single ensure is possible
        }

        public BoundAccess WithRead()
        {
            return new BoundAccess(_flags | AccessMask.Read, _targetType, _writeTypeMask);
        }

        public BoundAccess WithWrite(TypeRefMask writeTypeMask)
        {
            return new BoundAccess(_flags | AccessMask.Write, _targetType, _writeTypeMask | writeTypeMask);
        }

        public BoundAccess WithWriteRef(TypeRefMask writeTypeMask)
        {
            return new BoundAccess(_flags | AccessMask.WriteRef, _targetType, _writeTypeMask | writeTypeMask);
        }

        public BoundAccess WithReadRef()
        {
            return new BoundAccess(_flags | AccessMask.ReadRef, _targetType, _writeTypeMask);
        }

        internal BoundAccess WithRead(TypeSymbol target)
        {
            Contract.ThrowIfNull(target);
            return new BoundAccess(_flags | AccessMask.Read, target, _writeTypeMask);
        }

        public BoundAccess WithQuiet()
        {
            return new BoundAccess(_flags | AccessMask.ReadQuiet, _targetType, _writeTypeMask);
        }

        public BoundAccess WithEnsureObject()
        {
            return new BoundAccess(_flags | AccessMask.EnsureObject, _targetType, _writeTypeMask);
        }

        public BoundAccess WithEnsureArray()
        {
            return new BoundAccess(_flags | AccessMask.EnsureArray, _targetType, _writeTypeMask);
        }

        /// <summary>
        /// Creates <see cref="BoundAccess"/> value with specified <see cref="IsNotRef"/> flag.
        /// </summary>
        /// <param name="mightBeRef">Whether the value might be a reference (aliased) value.</param>
        /// <returns>New access.</returns>
        public BoundAccess WithRefFlag(bool mightBeRef)
        {
            var newflags = mightBeRef ? (_flags & ~AccessMask.IsNotRef) : (_flags | AccessMask.IsNotRef);

            return new BoundAccess(newflags, _targetType, _writeTypeMask);
        }

        /// <summary>
        /// Simple read access.
        /// </summary>
        public static BoundAccess Read => new BoundAccess(AccessMask.Read, null, 0);

        /// <summary>
        /// Read as a reference access.
        /// </summary>
        public static BoundAccess ReadRef => new BoundAccess(AccessMask.ReadRef, null, 0);

        /// <summary>
        /// Simple write access without bound write type mask.
        /// </summary>
        public static BoundAccess Write => new BoundAccess(AccessMask.Write, null, 0);

        /// <summary>
        /// Unset variable.
        /// </summary>
        public static BoundAccess Unset => new BoundAccess(AccessMask.Unset | AccessMask.ReadQuiet, null, 0);

        /// <summary>
        /// Check for isset.
        /// </summary>
        public static BoundAccess Isset => new BoundAccess(AccessMask.Isset, null, 0);

        /// <summary>
        /// Expression won't be read or written to.
        /// </summary>
        public static BoundAccess None => new BoundAccess(AccessMask.None, null, 0);

        /// <summary>
        /// Read and write without bound write type mask
        /// </summary>
        public static BoundAccess ReadAndWrite => new BoundAccess(AccessMask.Read | AccessMask.Write, null, 0);

        #endregion
    }

    #endregion

    #region BoundExpression

    public abstract partial class BoundExpression : BoundOperation, IAquilaExpression
    {
        /// <summary>
        /// The type analysis result.
        /// Gets possible combination of the value type after evaluation.
        /// </summary>
        public TypeRefMask TypeRefMask { get; set; } = default(TypeRefMask);

        /// <summary>
        /// Additional expression access,
        /// specifies how the expression is being accessed.
        /// </summary>
        public BoundAccess Access { get; internal set; }

        /// <summary>
        /// Lazily resolved conversion used to access the value.
        /// Emitted and the result always implicitly converted to <see cref="Type"/>.
        /// </summary>
        public CommonConversion BoundConversion { get; internal set; } // TODO: make it nullable

        /// <summary>
        /// Lazily resolved type of the expression,
        /// after applying the <see cref="Access"/>.
        /// </summary>
        internal TypeSymbol ResultType { get; set; }

        public LangElement AquilaSyntax { get; set; }

        /// <summary>
        /// Lazily resolved type of the expression result.
        /// </summary>
        public sealed override ITypeSymbol Type => ResultType;

        /// <summary>
        /// Whether the expression needs current <c>Pchp.Core.Context</c> to be evaluated.
        /// Otherwise, the expression can be evaluated in app context or in compile time.
        /// </summary>
        /// <remarks>
        /// E.g. If the expression is a literal, a resolved constant or immutable, it does not require the Context.
        /// </remarks>
        public virtual bool RequiresContext => !this.ConstantValue.HasValue;

        /// <summary>
        /// Decides whether an expression represented by this operation should be copied if it is passed by value (assignment, return).
        /// </summary>
        public virtual bool IsDeeplyCopied => !ConstantValue.HasValue;

        /// <summary>
        /// Resolved value of the expression.
        /// </summary>
        public Optional<object> ConstantValue { get; set; }

        protected sealed override Optional<object> ConstantValueHlp => ConstantValue;

        //public abstract TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor);
    }

    #endregion

    #region BoundFunctionCall, BoundArgument, BoundEcho, BoundConcatEx, BoundNewEx

    public partial class BoundArgument : BoundOperation, IArgumentOperation, IAquilaOperation
    {
        public CommonConversion InConversion => default(CommonConversion);

        /// <summary>
        /// Variable unpacking in PHP, the triple-dot syntax.
        /// </summary>
        public bool IsUnpacking => this.ArgumentKind == ArgumentKind.ParamArray;

        public override OperationKind Kind => OperationKind.Argument;

        public CommonConversion OutConversion => default(CommonConversion);

        public IParameterSymbol Parameter { get; set; }

        public SyntaxNode Syntax => null;

        public LangElement AquilaSyntax { get; set; }

        IOperation IArgumentOperation.Value => Value;

        public override ITypeSymbol Type => Value.ResultType;

        protected override Optional<object> ConstantValueHlp => Value.ConstantValue;

        /// <summary>
        /// Creates the argument.
        /// </summary>
        public static BoundArgument Create(BoundExpression value)
        {
            return new BoundArgument(value, ArgumentKind.Explicit);
        }

        /// <summary>
        /// Creates the argument that will be unpacked.
        /// The argument is an array which elements will be passed as actual arguments.
        /// </summary>
        public static BoundArgument CreateUnpacking(BoundExpression value)
        {
            Debug.Assert(!value.Access.IsReadRef);
            return new BoundArgument(value, ArgumentKind.ParamArray);
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitArgument(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            visitor.VisitArgument(this, argument);
        }

        TResult IAquilaOperation.Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
            visitor.VisitArgument(this);
    }

    /// <summary>
    /// Represents a function call.
    /// </summary>
    public abstract partial class BoundCallEx : IInvocationOperation
    {
        #region IInvocationOperation impl

        public IMethodSymbol TargetMethod => MethodSymbol;
        IOperation IInvocationOperation.Instance => Instance;
        public bool IsVirtual => MethodSymbol.IsVirtual;

        ImmutableArray<IArgumentOperation> IInvocationOperation.Arguments =>
            StaticCast<IArgumentOperation>.From(_arguments);

        #endregion

        public override bool IsDeeplyCopied => false;
        // routines deeply copy the return value if necessary within its `return` statement already

        public ImmutableArray<BoundArgument> ArgumentsInSourceOrder
        {
            get => _arguments;
            internal set => _arguments = value;
        }

        public IArgumentOperation ArgumentMatchingParameter(IParameterSymbol parameter)
        {
            foreach (var arg in _arguments)
            {
                if (arg.Parameter == parameter)
                    return arg;
            }

            return null;
        }

        /// <summary>
        /// Gets value indicating the arguments has to be unpacked in runtime before passed to the function.
        /// </summary>
        public bool HasArgumentsUnpacking
        {
            get
            {
                // the last argument must be unpacking,
                // otherwise unpacking is not even allowed
                var args = _arguments;
                return args.Length != 0 && args.Last().IsUnpacking;
            }
        }

        public BoundCallEx(ImmutableArray<BoundArgument> arguments, ImmutableArray<IBoundTypeRef> typeArgs) : this(null,
            null,
            arguments, typeArgs, null)
        {
        }

        internal void UpdateSymbol(MethodSymbol methodSymbol)
        {
            _methodSymbol = methodSymbol;
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitInvocation(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitInvocation(this, argument);
        }
    }

    public partial class BoundInstanceCallEx
    {
        public BoundInstanceCallEx Update(BoundRoutineName name, ImmutableArray<BoundArgument> arguments,
            ImmutableArray<IBoundTypeRef> typeArgs, BoundExpression instance)
        {
            if (Arguments == arguments && Instance == instance && TypeArguments == typeArgs && Name == name)
                return this;

            return new BoundInstanceCallEx(MethodSymbol, name, arguments, typeArgs, instance);
        }
    }

    public partial class BoundStaticCallEx
    {
        public BoundStaticCallEx Update(BoundRoutineName name, ImmutableArray<BoundArgument> arguments,
            ImmutableArray<IBoundTypeRef> typeArgs)
        {
            if (Arguments == arguments && TypeArguments == typeArgs && Name == name)
                return this;

            return new BoundStaticCallEx(MethodSymbol, name, arguments, typeArgs);
        }
    }

    /// <summary>
    /// Direct or indirect routine name.
    /// </summary>
    [DebuggerDisplay("{DebugView,nq}")]
    public partial class BoundRoutineName : IAquilaOperation
    {
        public QualifiedName NameValue => _nameValue;
        readonly QualifiedName _nameValue;

        public BoundExpression NameExpression => _nameExpression;
        readonly BoundExpression _nameExpression;

        string DebugView
        {
            get { return IsDirect ? _nameValue.ToString() : _nameExpression.ToString(); }
        }

        public bool IsDirect => _nameExpression == null;

        /// <summary>
        /// Gets <see cref="NameValue"/> as string if the name is known.
        /// Otherwise (when <see cref="NameExpression"/> is used instead), throws <see cref="InvalidOperationException"/> exception.
        /// </summary>
        public string ToStringOrThrow() =>
            NameExpression == null ? NameValue.ToString() : throw new InvalidOperationException();

        public override string ToString() => NameExpression != null ? $"{{{NameExpression}}}" : NameValue.ToString();

        public override OperationKind Kind => OperationKind.None;

        public LangElement AquilaSyntax { get; set; }

        public BoundRoutineName(QualifiedName name)
            : this(name, null)
        {
        }

        public BoundRoutineName(BoundExpression nameExpr)
            : this(default, nameExpr)
        {
        }


        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.DefaultVisit(this);
        }


        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.DefaultVisit(this, argument);
        }
    }

    // public partial class BoundGlobalFunctionCall : BoundCallEx
    // {
    //     public BoundRoutineName Name => _name;
    //     readonly BoundRoutineName _name;
    //
    //     public QualifiedName? NameOpt => _nameOpt;
    //     readonly QualifiedName? _nameOpt;
    //
    //     public BoundGlobalFunctionCall(BoundExpression nameExpression, ImmutableArray<BoundArgument> arguments)
    //         : this(new BoundRoutineName(nameExpression), null, arguments)
    //     {
    //     }
    //
    //     public BoundGlobalFunctionCall(QualifiedName name, QualifiedName? nameOpt,
    //         ImmutableArray<BoundArgument> arguments)
    //         : this(new BoundRoutineName(name), nameOpt, arguments)
    //     {
    //     }
    //
    //     private BoundGlobalFunctionCall(BoundRoutineName name, QualifiedName? nameOpt,
    //         ImmutableArray<BoundArgument> arguments)
    //         : base(arguments)
    //     {
    //         Debug.Assert(nameOpt == null || name.IsDirect);
    //         _name = name;
    //         _nameOpt = nameOpt;
    //     }
    //
    //     public BoundGlobalFunctionCall Update(BoundRoutineName name, QualifiedName? nameOpt,
    //         ImmutableArray<BoundArgument> arguments, ImmutableArray<IBoundTypeRef> typeArguments)
    //     {
    //         if (name == _name && nameOpt == _nameOpt && arguments == ArgumentsInSourceOrder &&
    //             typeArguments == _typeargs)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundGlobalFunctionCall(name, nameOpt, arguments)
    //                 {TypeArguments = typeArguments}.WithContext(this);
    //         }
    //     }
    //
    //     /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
    //         throw new NotImplementedException("Function call");
    // }

    // public partial class BoundInstanceFunctionCall : BoundCallEx
    // {
    //     public override BoundExpression Instance => _instance;
    //     private BoundExpression _instance;
    //
    //     public BoundRoutineName Name => _name;
    //     readonly BoundRoutineName _name;
    //
    //     public override bool IsVirtual => this.TargetMethod.IsErrorMethodOrNull() || this.TargetMethod.IsVirtual;
    //
    //     internal void SetInstance(BoundExpression instance) => _instance = instance;
    //
    //     public BoundInstanceFunctionCall(BoundExpression instance, QualifiedName name,
    //         ImmutableArray<BoundArgument> arguments)
    //         : this(instance, new BoundRoutineName(name), arguments)
    //     {
    //     }
    //
    //     public BoundInstanceFunctionCall(BoundExpression instance, BoundExpression nameExpr,
    //         ImmutableArray<BoundArgument> arguments)
    //         : this(instance, new BoundRoutineName(nameExpr), arguments)
    //     {
    //     }
    //
    //     public BoundInstanceFunctionCall(BoundExpression instance, BoundRoutineName name,
    //         ImmutableArray<BoundArgument> arguments)
    //         : base(arguments)
    //     {
    //         Debug.Assert(instance != null);
    //         Debug.Assert(name != null);
    //
    //         _instance = instance;
    //         _name = name;
    //     }
    //
    //     public BoundInstanceFunctionCall Update(BoundExpression instance, BoundRoutineName name,
    //         ImmutableArray<BoundArgument> arguments, ImmutableArray<IBoundTypeRef> typeArguments)
    //     {
    //         if (instance == _instance && name == _name && arguments == ArgumentsInSourceOrder &&
    //             typeArguments == _typeargs)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundInstanceFunctionCall(instance, name, arguments) {TypeArguments = typeArguments}
    //                 .WithContext(this);
    //         }
    //     }
    //
    //     // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
    //     //     visitor.VisitInstanceFunctionCall(this);
    // }

    // public partial class BoundCall : BoundCallEx
    // {
    //     public IBoundTypeRef TypeRef => _typeRef;
    //     readonly BoundTypeRef _typeRef;
    //
    //     public override BoundExpression Instance => _instance;
    //     readonly BoundExpression _instance;
    //
    //     public BoundRoutineName Name => _name;
    //     readonly BoundRoutineName _name;
    //
    //     public BoundCall(IBoundTypeRef typeRef, BoundRoutineName name, BoundExpression instance,
    //         ImmutableArray<BoundArgument> arguments)
    //         : base(arguments)
    //     {
    //         _typeRef = (BoundTypeRef) typeRef;
    //         _name = name;
    //         instance = instance;
    //     }
    //
    //     public BoundCall Update(IBoundTypeRef typeRef, BoundRoutineName name, BoundExpression instance,
    //         ImmutableArray<BoundArgument> arguments, ImmutableArray<IBoundTypeRef> typeArguments)
    //     {
    //         if (typeRef == _typeRef && name == _name && arguments == ArgumentsInSourceOrder &&
    //             typeArguments == _typeargs)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundCall(typeRef, name, instance, arguments)
    //                 {TypeArguments = typeArguments}.WithContext(this);
    //         }
    //     }
    //
    //     // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
    //     //     visitor.VisitStaticFunctionCall(this);
    // }

    // /// <summary>
    // /// Specialized <c>echo</c> function call.
    // /// To be replaced with <c>Context.Echo</c> once overload resolution is implemented.
    // /// </summary>
    // public sealed partial class BoundEcho : BoundCallEx
    // {
    //     public override BoundExpression Instance => null;
    //
    //     public BoundEcho(ImmutableArray<BoundArgument> arguments)
    //         : base(arguments)
    //     {
    //     }
    //
    //     public BoundEcho Update(ImmutableArray<BoundArgument> arguments)
    //     {
    //         if (arguments == ArgumentsInSourceOrder)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundEcho(arguments).WithContext(this);
    //         }
    //     }
    //
    //     // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitEcho(this);
    // }

    // /// <summary>
    // /// Represents a string concatenation.
    // /// </summary>
    // public partial class BoundConcatEx : BoundCallEx
    // {
    //     public override BoundExpression Instance => null;
    //
    //     public override bool IsDeeplyCopied => false;
    //
    //     public BoundConcatEx(ImmutableArray<BoundArgument> arguments)
    //         : base(arguments)
    //     {
    //     }
    //
    //     public BoundConcatEx Update(ImmutableArray<BoundArgument> arguments)
    //     {
    //         if (arguments == ArgumentsInSourceOrder)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundConcatEx(arguments).WithContext(this);
    //         }
    //     }
    //
    //     // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitConcat(this);
    // }

    /// <summary>
    /// Direct new expression with a constructor call.
    /// </summary>
    public partial class BoundNewEx
    {
        public override bool IsDeeplyCopied => false;
    }

    // /// <summary>
    // /// A script inclusion.
    // /// </summary>
    // public partial class BoundIncludeEx : BoundCallEx
    // {
    //     public override BoundExpression Instance => null;
    //
    //     /// <summary>
    //     /// Gets value indicating the target is resolved at compile time,
    //     /// so it will be called statically.
    //     /// </summary>
    //     public bool IsResolved => !TargetMethod.IsErrorMethodOrNull();
    //
    //     /// <summary>
    //     /// In case the inclusion target is resolved, gets reference to the <c>Main</c> method of the included script.
    //     /// </summary>
    //     internal new MethodSymbol TargetMethod
    //     {
    //         get { return base.TargetMethod; }
    //         set { base.TargetMethod = value; }
    //     }
    //
    //     /// <summary>
    //     /// Type of inclusion, <c>include</c>, <c>require</c>, <c>include_once</c>, <c>require_once</c>.
    //     /// </summary>
    //     public InclusionTypes InclusionType { get; private set; }
    //
    //     public BoundIncludeEx(BoundExpression target, InclusionTypes type)
    //         : base(ImmutableArray.Create(BoundArgument.Create(target)))
    //     {
    //         Debug.Assert(target.Access.IsRead);
    //
    //         this.InclusionType = type;
    //     }
    //
    //     public BoundIncludeEx Update(BoundExpression target, InclusionTypes type)
    //     {
    //         if (target == ArgumentsInSourceOrder[0].Value && type == InclusionType)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundIncludeEx(target, type).WithContext(this);
    //         }
    //     }
    //
    //     // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitInclude(this);
    // }

    // /// <summary>
    // /// <c>exit</c> construct.
    // /// </summary>
    // public sealed partial class BoundExitEx : BoundCallEx
    // {
    //     public override BoundExpression Instance => null;
    //
    //     public BoundExitEx(BoundExpression value = null)
    //         : base(value != null
    //             ? ImmutableArray.Create(BoundArgument.Create(value))
    //             : ImmutableArray<BoundArgument>.Empty)
    //     {
    //         Debug.Assert(value == null || value.Access.IsRead);
    //     }
    //
    //     public BoundExitEx Update(ImmutableArray<BoundArgument> args)
    //     {
    //         if (args == ArgumentsInSourceOrder)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundExitEx(args.Length == 0 ? null : args[0].Value).WithContext(this);
    //         }
    //     }
    //
    //     // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitExit(this);
    // }
    //
    // public sealed partial class BoundAssertEx : BoundCallEx
    // {
    //     public override BoundExpression Instance => null;
    //
    //     public BoundAssertEx(ImmutableArray<BoundArgument> arguments)
    //         : base(arguments)
    //     {
    //     }
    //
    //     public BoundAssertEx Update(ImmutableArray<BoundArgument> arguments)
    //     {
    //         if (arguments == ArgumentsInSourceOrder)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundAssertEx(arguments).WithContext(this);
    //         }
    //     }
    //
    //     // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitAssert(this);
    // }

    #endregion

    #region BoundThrowStatement

    /// <summary>
    /// throw <c>Thrown</c>;
    /// </summary>
    public sealed partial class BoundThrowEx : BoundExpression, IThrowOperation
    {
        IOperation IThrowOperation.Exception => this.Thrown;

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitThrow(this, argument);
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitThrow(this);
        }
    }

    #endregion

    #region BoundLambda

    /// <summary>
    /// Anonymous function expression.
    /// </summary>
    public partial class BoundLambda : BoundExpression, IAnonymousFunctionOperation
    {
        /// <summary>
        /// Declared use variables.
        /// </summary>
        public ImmutableArray<BoundArgument> UseVars => _usevars;

        ImmutableArray<BoundArgument> _usevars;

        public IBlockOperation Body =>
            null; //(BoundLambdaMethod != null) ? BoundLambdaMethod.ControlFlowGraph.Start : null;

        public IMethodSymbol Signature => null; // BoundLambdaMethod;

        /// <summary>
        /// Reference to associated lambda method symbol.
        /// Bound during analysis.
        /// </summary>
        //internal SourceLambdaSymbol BoundLambdaMethod { get; set; }

        IMethodSymbol IAnonymousFunctionOperation.Symbol => null; // BoundLambdaMethod;

        public override bool IsDeeplyCopied => false;

        public BoundLambda(ImmutableArray<BoundArgument> usevars)
        {
            _usevars = usevars;
        }

        public BoundLambda Update(ImmutableArray<BoundArgument> usevars)
        {
            if (usevars == _usevars)
            {
                return this;
            }
            else
            {
                return new BoundLambda(usevars).WithContext(this);
            }
        }

        public override OperationKind Kind => OperationKind.AnonymousFunction;

        // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitLambda(this);

        public override void Accept(OperationVisitor visitor) => visitor.VisitAnonymousFunction(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument) => visitor.VisitAnonymousFunction(this, argument);
    }

    #endregion

    #region BoundEvalEx

    public partial class BoundEvalEx : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;

        public BoundExpression CodeExpression { get; internal set; }

        public BoundEvalEx(BoundExpression code)
        {
            Debug.Assert(code != null);
            this.CodeExpression = code;
        }

        public BoundEvalEx Update(BoundExpression code)
        {
            if (code == CodeExpression)
            {
                return this;
            }
            else
            {
                return new BoundEvalEx(code).WithContext(this);
            }
        }

        // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitEval(this);

        public override void Accept(OperationVisitor visitor) => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument) => visitor.DefaultVisit(this, argument);
    }

    #endregion

    #region BoundLiteral

    public partial class BoundLiteral : ILiteralOperation
    {
        public string Spelling => this.ConstantValue.Value?.ToString() ?? "NULL";

        public override OperationKind Kind => OperationKind.Literal;

        public override bool RequiresContext => false;

        public override bool IsDeeplyCopied => false;

        partial void OnCreateImpl(object value)
        {
            this.ConstantValue = value;
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitLiteral(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitLiteral(this, argument);
        }
    }

    #endregion

    #region BoundCopyValue

    /// <summary>
    /// Deeply copies the expression's dereferenced value.
    /// </summary>
    public partial class BoundCopyValue : BoundExpression
    {
        public BoundCopyValue(BoundExpression expression)
        {
            this.Expression = expression ?? throw ExceptionUtilities.ArgumentNull(nameof(expression));
        }

        public BoundExpression Expression { get; }

        public override bool RequiresContext => this.Expression.RequiresContext;

        public override bool IsDeeplyCopied => false; // already copied

        public override OperationKind Kind => OperationKind.None;

        public override void Accept(OperationVisitor visitor) => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument) => visitor.DefaultVisit(this, argument);

        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
        //     visitor.VisitCopyValue(this);

        internal BoundCopyValue Update(BoundExpression expression)
        {
            return expression == Expression ? this : new BoundCopyValue(expression).WithAccess(this.Access);
        }
    }

    #endregion

    #region BoundBinaryEx

    public sealed partial class BoundBinaryEx : BoundExpression, IBinaryOperation
    {
        public BinaryOperatorKind OperatorKind
        {
            get { throw new NotSupportedException(); }
        }

        public override bool RequiresContext => Left.RequiresContext || Right.RequiresContext;

        public override bool IsDeeplyCopied
        {
            get
            {
                switch (Operation)
                {
                    // respective operators returns immutable values:
                    case Operations.Xor:
                    case Operations.Or:
                    case Operations.And:
                    case Operations.BitOr:
                    case Operations.BitXor:
                    case Operations.BitAnd:
                    case Operations.Equal:
                    case Operations.NotEqual:
                    case Operations.Identical:
                    case Operations.NotIdentical:
                    case Operations.LessThan:
                    case Operations.GreaterThan:
                    case Operations.LessThanOrEqual:
                    case Operations.GreaterThanOrEqual:
                    case Operations.ShiftLeft:
                    case Operations.ShiftRight:
                    case Operations.Add:
                    case Operations.Sub:
                    case Operations.Mul:
                    case Operations.Pow:
                    case Operations.Div:
                    case Operations.Mod:
                    case Operations.Concat:
                        return false;

                    case Operations.Coalesce:
                        return Left.IsDeeplyCopied || Right.IsDeeplyCopied;

                    default:
                        return true;
                }
            }
        }

        public IMethodSymbol Operator { get; set; }

        IMethodSymbol IBinaryOperation.OperatorMethod => Operator;

        IOperation IBinaryOperation.LeftOperand => Left;

        IOperation IBinaryOperation.RightOperand => Right;

        bool IBinaryOperation.IsLifted => false;

        bool IBinaryOperation.IsChecked => false;

        bool IBinaryOperation.IsCompareText => false;

        public bool UsesOperatorMethod => this.Operator != null;


        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitBinaryOperator(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitBinaryOperator(this, argument);
        }
    }

    #endregion

    #region BoundUnaryEx

    public partial class BoundUnaryEx : BoundExpression, IUnaryOperation
    {
        public override bool RequiresContext => Operation == Operations.Print || Operand.RequiresContext;

        IOperation IUnaryOperation.Operand => Operand;

        public IMethodSymbol OperatorMethod => null;

        bool IUnaryOperation.IsLifted => false;

        bool IUnaryOperation.IsChecked => false;

        public bool UsesOperatorMethod => OperatorMethod != null;

        public UnaryOperatorKind OperatorKind
        {
            get { throw new NotSupportedException(); }
        }

        public override bool IsDeeplyCopied
        {
            get
            {
                if (!base.IsDeeplyCopied)
                {
                    return false;
                }

                switch (Operation)
                {
                    // respective operators returns immutable values:
                    case Operations.Plus:
                    case Operations.Minus:
                    case Operations.LogicNegation:
                    case Operations.BitNegation:

                    case Operations.Int8Cast:
                    case Operations.Int16Cast:
                    case Operations.Int32Cast:
                    case Operations.Int64Cast:
                    case Operations.UInt8Cast:
                    case Operations.UInt16Cast:
                    case Operations.UInt32Cast:
                    case Operations.UInt64Cast:
                    case Operations.DecimalCast:
                    case Operations.DoubleCast:
                    case Operations.FloatCast:
                    case Operations.StringCast:
                    case Operations.UnicodeCast:
                    case Operations.BoolCast:
                    case Operations.UnsetCast:

                    case Operations.Clone:
                    case Operations.Print:
                        return false;

                    case Operations.ObjectCast:
                        return false;

                    case Operations.ArrayCast:
                    case Operations.BinaryCast:
                        return true;

                    // the result depends on what follows @:
                    case Operations.AtSign:
                        return Operand.IsDeeplyCopied;

                    default:
                        return base.IsDeeplyCopied;
                }
            }
        }

        partial void OnCreateImpl(BoundExpression operand, Operations operation)
        {
            Contract.ThrowIfNull(operand);
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitUnaryOperator(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitUnaryOperator(this, argument);
        }
    }

    #endregion

    #region BoundConvertEx, BoundCallableConvert

    /// <summary>
    /// Explicit conversion operation (cast operation).
    /// </summary>
    public partial class BoundConversionEx : BoundExpression, IConversionOperation
    {
        public override OperationKind Kind => OperationKind.Conversion;

        IOperation IConversionOperation.Operand => Operand;

        IMethodSymbol IConversionOperation.OperatorMethod => Conversion.MethodSymbol;

        public override bool RequiresContext => Operand.RequiresContext ||
                                                (TargetType is TypeRef.BoundPrimitiveTypeRef pt &&
                                                 pt.TypeCode == AquilaTypeCode.String);

        public CommonConversion Conversion { get; set; }

        bool IConversionOperation.IsTryCast => false;

        public bool IsChecked { get; set; }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitConversion(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitConversion(this, argument);
        }
    }

    /// <summary>
    /// Conversion to <c>IPhpCallable</c> (callable).
    /// </summary>
    internal partial class BoundCallableConvert : BoundConversionEx
    {
        /// <summary>
        /// Resolved method to be converted to callable.
        /// </summary>
        public IMethodSymbol TargetCallable { get; internal set; }

        /// <summary>In case of an instance method, this is its receiver instance.</summary>
        internal BoundExpression Receiver { get; set; }

        internal BoundCallableConvert(BoundExpression operand, AquilaCompilation compilation)
            : base(operand, compilation.TypeRefFactory.Create((NamedTypeSymbol) null))
        {
        }
    }

    #endregion

    #region BoundIncDecEx

    public partial class BoundIncDecEx : BoundCompoundAssignEx, IIncrementOrDecrementOperation
    {
        public override OperationKind Kind => IsIncrement ? OperationKind.Increment : OperationKind.Decrement;

        IMethodSymbol IIncrementOrDecrementOperation.OperatorMethod => null;

        IOperation IIncrementOrDecrementOperation.Target => Target;


        bool IIncrementOrDecrementOperation.IsLifted => false;

        bool IIncrementOrDecrementOperation.IsChecked => false;

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitIncrementOrDecrement(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitIncrementOrDecrement(this, argument);
        }
    }

    #endregion

    #region BoundConditionalEx

    public partial class BoundConditionalEx : IConditionalOperation
    {
        IOperation IConditionalOperation.Condition => Condition;
        IOperation IConditionalOperation.WhenFalse => IfFalse;
        IOperation IConditionalOperation.WhenTrue => IfTrue;
        bool IConditionalOperation.IsRef => false;

        public override OperationKind Kind => OperationKind.Conditional;

        public override bool RequiresContext => Condition.RequiresContext ||
                                                (IfTrue != null && IfTrue.RequiresContext) || IfFalse.RequiresContext;

        public override bool IsDeeplyCopied => (IfTrue ?? Condition).IsDeeplyCopied || IfFalse.IsDeeplyCopied;


        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitConditional(this, argument);
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitConditional(this);
        }
    }

    #endregion

    #region BoundAssignEx, BoundCompoundAssignEx

    public partial class BoundAssignEx : BoundExpression, ISimpleAssignmentOperation
    {
        #region IAssignmentExpression

        IOperation IAssignmentOperation.Target => Target;

        IOperation IAssignmentOperation.Value => Value;

        bool ISimpleAssignmentOperation.IsRef => false;

        #endregion

        public override OperationKind Kind => OperationKind.SimpleAssignment;


        partial void AcceptImpl(OperationVisitor visitor) => visitor.VisitSimpleAssignment(this);

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitSimpleAssignment(this, argument);
        }
    }

    public partial class BoundCompoundAssignEx : BoundAssignEx, ICompoundAssignmentOperation
    {
        public BinaryOperatorKind OperatorKind
        {
            get { throw new NotSupportedException(); }
        }


        public IMethodSymbol OperatorMethod { get; set; }

        public bool UsesOperatorMethod => this.OperatorMethod != null;

        bool ICompoundAssignmentOperation.IsLifted => false;

        bool ICompoundAssignmentOperation.IsChecked => false;

        CommonConversion ICompoundAssignmentOperation.InConversion => throw new NotSupportedException();

        CommonConversion ICompoundAssignmentOperation.OutConversion => throw new NotSupportedException();

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitCompoundAssignment(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitCompoundAssignment(this, argument);
        }
    }

    #endregion

    #region BoundReferenceExpression

    public abstract partial class BoundReferenceEx
    {
        // internal IVariableReference Reference { get; } // TODO

        /// <summary>
        /// Gets or sets value indicating the variable is used while it was not initialized in all code paths.
        /// </summary>
        public bool MaybeUninitialized { get; set; }
    }

    #endregion

    #region BoundVariableRef

    /// <summary>
    /// Direct or indirect variable name.
    /// </summary>
    [DebuggerDisplay("{DebugView,nq}")]
    public partial class BoundVariableName : BoundOperation, IAquilaOperation
    {
        public static bool operator ==(BoundVariableName lname, BoundVariableName rname)
        {
            if (ReferenceEquals(lname, rname)) return true;
            if (ReferenceEquals(lname, null) || ReferenceEquals(rname, null)) return false;

            return lname.NameExpression == rname.NameExpression && lname.NameValue.Equals(rname.NameValue);
        }

        public static bool operator !=(BoundVariableName lname, BoundVariableName rname)
        {
            return !(lname == rname);
        }

        public override bool Equals(object obj) => obj is BoundVariableName bname && this == bname;

        public override int GetHashCode() =>
            NameValue.GetHashCode() ^ (NameExpression != null ? NameExpression.GetHashCode() : 0);

        string DebugView
        {
            get { return IsDirect ? NameValue.ToString() : "{indirect}"; }
        }

        public bool IsDirect => NameExpression == null;

        public LangElement AquilaSyntax { get; set; }

        public BoundVariableName(VariableName name)
            : this(name, null)
        {
        }

        public BoundVariableName(string name)
            : this(new VariableName(name))
        {
        }

        public BoundVariableName(BoundExpression nameExpr)
            : this(default, nameExpr)
        {
        }

        public BoundVariableName Update(VariableName name, BoundExpression nameExpr)
        {
            if (name.NameEquals(NameValue) && nameExpr == NameExpression)
            {
                return this;
            }
            else
            {
                return new BoundVariableName(name, nameExpr);
            }
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.DefaultVisit(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.DefaultVisit(this, argument);
        }
    }

    /// <summary>
    /// A variable reference that can be read or written to.
    /// </summary>
    public partial class BoundVariableRef : BoundReferenceEx, ILocalReferenceOperation
    {
        public BoundVariableRef(string name) : this(new BoundVariableName(new VariableName(name)))
        {
        }

        /// <summary>
        /// Resolved variable source.
        /// </summary>
        internal IVariableReference Variable { get; set; }

        /// <summary>
        /// The type of variable before it gets accessed by this expression.
        /// </summary>
        internal TypeRefMask BeforeTypeRef { get; set; }

        /// <summary>
        /// Local in case of the variable is resolved local variable.
        /// </summary>
        ILocalSymbol ILocalReferenceOperation.Local => this.Variable?.Symbol as ILocalSymbol;

        bool ILocalReferenceOperation.IsDeclaration => throw new NotSupportedException();

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitLocalReference(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitLocalReference(this, argument);
        }
    }

    /// <summary>
    /// A non-source synthesized variable reference that can be read or written to. 
    /// </summary>
    /// <remarks>
    /// Inheriting from <c>BoundVariableRef</c> is just a temporary measure. Do NOT take dependencies on anything but <c>IReferenceExpression</c>.
    /// </remarks>
    public partial class BoundTemporalVariableRef : BoundVariableRef
    {
        private string _name;

        partial void OnCreateImpl(string name)
        {
            _name = name;
        }

        public BoundTemporalVariableRef Update(string name)
        {
            if (_name == name)
                return this;
            return new BoundTemporalVariableRef(name);
        }
    }

    #endregion

    #region BoundListEx

    /// <summary>
    /// PHP <c>list</c> expression that can be written to.
    /// </summary>
    public partial class BoundListEx : BoundReferenceEx
    {
        public BoundListEx(IEnumerable<KeyValuePair<BoundExpression, BoundReferenceEx>> items)
        {
            Debug.Assert(items != null);

            _items = items
                .Select(pair =>
                    new KeyValuePair<BoundExpression, BoundReferenceEx>(pair.Key,
                        (BoundReferenceEx) pair.Value))
                .ToImmutableArray();
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.DefaultVisit(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.DefaultVisit(this, argument);
        }
    }

    #endregion

    #region BoundFieldRef

    public partial class BoundFieldRef : BoundReferenceEx, IFieldReferenceOperation
    {
        ISymbol IMemberReferenceOperation.Member => BoundReference?.Symbol;

        IFieldSymbol IFieldReferenceOperation.Field => BoundReference?.Symbol as IFieldSymbol;

        IOperation IMemberReferenceOperation.Instance => Instance;

        bool IFieldReferenceOperation.IsDeclaration => throw new NotSupportedException();

        public enum FieldType
        {
            InstanceField,
            StaticField,
            ClassConstant,
        }

        FieldType _type;

        public bool IsInstanceField => _type == FieldType.InstanceField;
        public bool IsStaticField => _type == FieldType.StaticField;
        public bool IsClassConstant => _type == FieldType.ClassConstant;

        /// <summary>
        /// In case of a non static field, gets its instance expression.
        /// </summary>
        public BoundExpression Instance
        {
            get => IsInstanceField ? _instance : null;
            set
            {
                if (IsInstanceField)
                    _instance = value;
                else
                    throw new InvalidOperationException();
            }
        }

        partial void OnCreateImpl(BoundExpression instance, IBoundTypeRef containingType, BoundVariableName fieldName,
            FieldType fieldType)
        {
            Debug.Assert((instance == null) != (containingType == null));
            Debug.Assert((fieldType == FieldType.InstanceField) == (instance != null));
        }

        public static BoundFieldRef CreateInstanceField(BoundExpression instance, BoundVariableName name) =>
            new BoundFieldRef(instance, null, name, FieldType.InstanceField);

        public static BoundFieldRef CreateStaticField(IBoundTypeRef type, BoundVariableName name) =>
            new BoundFieldRef(null, type, name, FieldType.StaticField);

        public static BoundFieldRef CreateClassConst(IBoundTypeRef type, BoundVariableName name) =>
            new BoundFieldRef(null, type, name, FieldType.ClassConstant);

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitFieldReference(this);
        }


        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitFieldReference(this, argument);
        }

        public BoundFieldRef Update(BoundExpression instance, IBoundTypeRef containingType, BoundVariableName fieldName)
        {
            if (_instance == instance && _containingType == containingType && _fieldName == fieldName)
                return this;
            return new BoundFieldRef(instance, containingType, fieldName, _type);
        }
    }

    #endregion

    #region BoundArrayEx

    public partial class BoundArrayEx : BoundExpression, IArrayCreationOperation
    {
        public class BoundArrayInitializer : BoundExpression, IArrayInitializerOperation
        {
            readonly BoundArrayEx _array;

            public override OperationKind Kind => OperationKind.ArrayInitializer;

            public override bool IsDeeplyCopied => false;

            ImmutableArray<IOperation> IArrayInitializerOperation.ElementValues =>
                _array._items.Select(x => x.Value).Cast<IOperation>().AsImmutable();

            public BoundArrayInitializer(BoundArrayEx array)
            {
                _array = array;
            }

            public BoundArrayInitializer Update(BoundArrayEx array)
            {
                if (array == _array)
                {
                    return this;
                }
                else
                {
                    return new BoundArrayInitializer(array).WithContext(this);
                }
            }

            public override void Accept(OperationVisitor visitor)
                => visitor.VisitArrayInitializer(this);

            public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
                TArgument argument)
                => visitor.VisitArrayInitializer(this, argument);

            /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
            /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
            /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
            public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsDeeplyCopied => false;

        public override bool RequiresContext =>
            _items.Any(x => (x.Key != null && x.Key.RequiresContext) || x.Value.RequiresContext);

        ImmutableArray<IOperation> IArrayCreationOperation.DimensionSizes =>
            ImmutableArray.Create<IOperation>(new BoundLiteral(_items.Length));

        IArrayInitializerOperation IArrayCreationOperation.Initializer => new BoundArrayInitializer(this);

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitArrayCreation(this);
        }


        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitArrayCreation(this, argument);
        }
    }

    #endregion

    #region BoundArrayItemEx

    /// <summary>
    /// Array item access.
    /// </summary>
    public partial class BoundArrayItemEx : BoundReferenceEx, IArrayElementReferenceOperation
    {
        public override OperationKind Kind => OperationKind.ArrayElementReference;

        IOperation IArrayElementReferenceOperation.ArrayReference => _array;

        ImmutableArray<IOperation> IArrayElementReferenceOperation.Indices
            => (_index != null) ? ImmutableArray.Create((IOperation) _index) : ImmutableArray<IOperation>.Empty;

        partial void OnCreateImpl(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index)
        {
            Contract.ThrowIfNull(array);

            if (declaringCompilation is null)
                throw ExceptionUtilities.ArgumentNull(nameof(declaringCompilation));
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitArrayElementReference(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitArrayElementReference(this, argument);
        }

        public BoundArrayItemEx Update(BoundExpression array, BoundExpression index)
        {
            if (_array == array && _index == index)
                return this;
            return new BoundArrayItemEx(_declaringCompilation, array, index);
        }
    }

    #region BoundArrayItemOrdEx

    public partial class BoundArrayItemOrdEx : BoundArrayItemEx
    {
        public new BoundArrayItemOrdEx Update(BoundExpression array, BoundExpression index)
        {
            if (array == Array && index == Index)
            {
                return this;
            }
            else
            {
                return new BoundArrayItemOrdEx(DeclaringCompilation, array, index).WithContext(this);
            }
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitArrayElementReference(this);
        }


        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitArrayElementReference(this, argument);
        }
    }

    #endregion

    #endregion

    #region BoundInstanceOfEx

    public partial class BoundInstanceOfEx : BoundExpression, IIsTypeOperation
    {
        #region IIsExpression

        IOperation IIsTypeOperation.ValueOperand => Operand;

        ITypeSymbol IIsTypeOperation.TypeOperand => AsType?.Type;

        bool IIsTypeOperation.IsNegated => false;

        #endregion

        /// <summary>
        /// The value to be checked.
        /// </summary>
        public BoundExpression Operand { get; internal set; }

        public override bool IsDeeplyCopied => false;

        /// <summary>
        /// The type.
        /// </summary>
        public IBoundTypeRef AsType { get; private set; }

        public BoundInstanceOfEx(BoundExpression operand, IBoundTypeRef tref)
        {
            Contract.ThrowIfNull(operand);

            this.Operand = operand;
            this.AsType = tref;
        }

        public BoundInstanceOfEx Update(BoundExpression operand, IBoundTypeRef tref)
        {
            if (operand == Operand && tref == AsType)
            {
                return this;
            }
            else
            {
                return new BoundInstanceOfEx(operand, tref).WithContext(this);
            }
        }

        public override OperationKind Kind => OperationKind.IsType;

        public override void Accept(OperationVisitor visitor)
            => visitor.VisitIsType(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.VisitIsType(this, argument);

        /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
        //     visitor.VisitInstanceOf(this);
    }

    #endregion

    #region BoundGlobalConst

    public partial class BoundGlobalConst : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;

        public override bool IsDeeplyCopied => false;

        /// <summary>
        /// Constant name.
        /// </summary>
        public QualifiedName Name { get; private set; }

        /// <summary>
        /// Alternative constant name if <see cref="Name"/> is not resolved.
        /// </summary>
        public QualifiedName? FallbackName { get; private set; }

        public BoundGlobalConst(QualifiedName name, QualifiedName? fallbackName)
        {
            this.Name = name;
            this.FallbackName = fallbackName;
        }

        public BoundGlobalConst Update(QualifiedName name, QualifiedName? fallbackName)
        {
            if (name == Name && fallbackName == FallbackName)
            {
                return this;
            }
            else
            {
                return new BoundGlobalConst(name, fallbackName).WithContext(this);
            }
        }

        /// <summary>
        /// In case the constant is resolved to a place.
        /// </summary>
        internal IVariableReference _boundExpressionOpt;

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);

        /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
        //     visitor.VisitGlobalConstUse(this);
    }

    #endregion

    #region BoundPseudoConst

    //
    // public partial class BoundPseudoConst : BoundExpression
    // {
    //     public override OperationKind Kind => OperationKind.None;
    //
    //     public Ast.PseudoConstUse.Types ConstType { get; private set; }
    //
    //     public override bool IsDeeplyCopied => false;
    //
    //     public BoundPseudoConst(Ast.PseudoConstUse.Types type)
    //     {
    //         this.ConstType = type;
    //     }
    //
    //     public BoundPseudoConst Update(PseudoConstUse.Types type)
    //     {
    //         if (type == ConstType)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundPseudoConst(type).WithContext(this);
    //         }
    //     }
    //
    //     public override void Accept(OperationVisitor visitor)
    //         => visitor.DefaultVisit(this);
    //
    //     public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
    //         TArgument argument)
    //         => visitor.DefaultVisit(this, argument);
    //
    //     /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     /// <param name="visitor">A reference to a <see cref="PhpOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     public override TResult Accept<TResult>(PhpOperationVisitor<TResult> visitor) =>
    //         visitor.VisitPseudoConstUse(this);
    // }

    #endregion

    #region BoundPseudoClassConst

    //
    // public partial class BoundPseudoClassConst : BoundExpression
    // {
    //     public Ast.PseudoClassConstUse.Types ConstType { get; private set; }
    //
    //     public override OperationKind Kind => OperationKind.None;
    //
    //     public override bool IsDeeplyCopied => false;
    //
    //     public IBoundTypeRef TargetType { get; private set; }
    //
    //     public BoundPseudoClassConst(IBoundTypeRef targetType, Ast.PseudoClassConstUse.Types type)
    //     {
    //         this.TargetType = targetType;
    //         this.ConstType = type;
    //     }
    //
    //     public BoundPseudoClassConst Update(IBoundTypeRef targetType, Ast.PseudoClassConstUse.Types type)
    //     {
    //         if (targetType == TargetType && type == ConstType)
    //         {
    //             return this;
    //         }
    //         else
    //         {
    //             return new BoundPseudoClassConst(targetType, type).WithContext(this);
    //         }
    //     }
    //
    //     /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
    //     /// <param name="visitor">A reference to a <see cref="PhpOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
    //     /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
    //     public override TResult Accept<TResult>(PhpOperationVisitor<TResult> visitor) =>
    //         visitor.VisitPseudoClassConstUse(this);
    //
    //     public override void Accept(OperationVisitor visitor) => visitor.DefaultVisit(this);
    //
    //     public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
    //         TArgument argument) => visitor.DefaultVisit(this, argument);
    // }
    //

    #endregion

    #region BoundIsSetEx, BoundOffsetExists, BoundIsEmptyEx, BoundTryGetItem

    public partial class BoundIsEmptyEx : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;

        /// <summary>
        /// Reference to be checked if it is set.
        /// </summary>
        public BoundExpression Operand { get; set; }

        public BoundIsEmptyEx(BoundExpression expression)
        {
            this.Operand = expression;
        }

        public BoundIsEmptyEx Update(BoundExpression expression)
        {
            if (expression == Operand)
            {
                return this;
            }
            else
            {
                return new BoundIsEmptyEx(expression).WithContext(this);
            }
        }

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);

        /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitIsEmpty(this);
    }

    public partial class BoundIsSetEx : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;

        public override bool IsDeeplyCopied => false;

        public override bool RequiresContext => VarReference.RequiresContext;

        /// <summary>
        /// Reference to be checked if it is set.
        /// </summary>
        public BoundReferenceEx VarReference { get; set; }

        public BoundIsSetEx(BoundReferenceEx varref)
        {
            this.VarReference = varref;
        }

        public BoundIsSetEx Update(BoundReferenceEx varref)
        {
            if (varref == VarReference)
            {
                return this;
            }
            else
            {
                return new BoundIsSetEx(varref).WithContext(this);
            }
        }

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);

        /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) => visitor.VisitIsSet(this);
    }

    public partial class BoundOffsetExists : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;

        public override bool IsDeeplyCopied => false;

        public override bool RequiresContext => Receiver.RequiresContext || Index.RequiresContext;

        /// <summary>
        /// The array.
        /// </summary>
        public BoundExpression Receiver { get; set; }

        /// <summary>
        /// The index.
        /// </summary>
        public BoundExpression Index { get; set; }

        public BoundOffsetExists(BoundExpression receiver, BoundExpression index)
        {
            this.Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            this.Index = index ?? throw new ArgumentNullException(nameof(index));
        }

        public BoundOffsetExists Update(BoundExpression receiver, BoundExpression index)
        {
            if (Receiver == receiver && Index == index)
            {
                return this;
            }
            else
            {
                return new BoundOffsetExists(receiver, index).WithContext(this);
            }
        }

        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        //     => visitor.VisitOffsetExists(this);

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);
    }

    /// <summary>
    /// Shortcut for <c>isset($Array[$Index]) ? $Array[$Index] : Fallback</c>.
    /// </summary>
    public partial class BoundTryGetItem : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;

        public BoundExpression Array { get; }
        public BoundExpression Index { get; }
        public BoundExpression Fallback { get; }

        public override bool RequiresContext =>
            Array.RequiresContext || Index.RequiresContext || Fallback.RequiresContext;

        public BoundTryGetItem(BoundExpression array, BoundExpression index, BoundExpression fallback)
        {
            Debug.Assert(array != null);
            Debug.Assert(index != null);
            Debug.Assert(fallback != null);

            Array = array;
            Index = index;
            Fallback = fallback;
        }

        public BoundTryGetItem Update(BoundExpression array, BoundExpression index, BoundExpression fallback)
        {
            if (Array == array && Index == index && Fallback == fallback)
            {
                return this;
            }
            else
            {
                return new BoundTryGetItem(array, index, fallback);
            }
        }

        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        //     => visitor.VisitTryGetItem(this);

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);
    }

    #endregion

    #region BoundYieldEx, BoundYieldFromEx

    /// <summary>
    /// Represents a reference to an item sent to the generator.
    /// </summary>
    public partial class BoundYieldEx : BoundExpression
    {
        public override OperationKind Kind => OperationKind.FieldReference;

        // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        //     => visitor.VisitYieldEx(this);

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);
    }

    /// <summary>
    /// Represents a return from `yield from` expression.
    /// That is the value returned from eventual `Generator` being yielded from.
    /// </summary>
    public partial class BoundYieldFromEx : BoundExpression
    {
        public override OperationKind Kind => OperationKind.FieldReference;

        public BoundExpression Operand { get; internal set; }

        public BoundYieldFromEx(BoundExpression expression)
        {
            Operand = expression;
        }

        public BoundYieldFromEx Update(BoundExpression expression)
        {
            if (expression == Operand)
            {
                return this;
            }
            else
            {
                return new BoundYieldFromEx(expression).WithContext(this);
            }
        }

        // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        //     => visitor.VisitYieldFromEx(this);

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);
    }

    #endregion
}