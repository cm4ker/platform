﻿using Microsoft.CodeAnalysis.Operations;
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
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;
using Xunit;

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
        /// Is invoke
        /// </summary>
        public bool IsInvoke => (_flags & AccessMask.Invoke) == AccessMask.Invoke;


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

        private BoundAccess(AccessMask flags, TypeSymbol targetType)
        {
            _flags = flags;
            _targetType = targetType;

            Debug.Assert(EnsureArray ^ EnsureObject ^ IsReadRef || !IsEnsure); // only single ensure is possible
        }

        public BoundAccess WithRead()
        {
            return new BoundAccess(_flags | AccessMask.Read, _targetType);
        }

        public BoundAccess WithWrite()
        {
            return new BoundAccess(_flags | AccessMask.Write, _targetType);
        }

        public BoundAccess WithWriteRef()
        {
            return new BoundAccess(_flags | AccessMask.WriteRef, _targetType);
        }

        public BoundAccess WithReadRef()
        {
            return new BoundAccess(_flags | AccessMask.ReadRef, _targetType);
        }

        internal BoundAccess WithRead(TypeSymbol target)
        {
            Contract.ThrowIfNull(target);
            return new BoundAccess(_flags | AccessMask.Read, target);
        }

        internal BoundAccess WithInvoke(TypeSymbol target)
        {
            Contract.ThrowIfNull(target);
            return new BoundAccess(_flags | AccessMask.Invoke | AccessMask.Read, target);
        }


        public BoundAccess WithQuiet()
        {
            return new BoundAccess(_flags | AccessMask.ReadQuiet, _targetType);
        }

        public BoundAccess WithEnsureObject()
        {
            return new BoundAccess(_flags | AccessMask.EnsureObject, _targetType);
        }

        public BoundAccess WithEnsureArray()
        {
            return new BoundAccess(_flags | AccessMask.EnsureArray, _targetType);
        }

        /// <summary>
        /// Creates <see cref="BoundAccess"/> value with specified <see cref="IsNotRef"/> flag.
        /// </summary>
        /// <param name="mightBeRef">Whether the value might be a reference (aliased) value.</param>
        /// <returns>New access.</returns>
        public BoundAccess WithRefFlag(bool mightBeRef)
        {
            var newflags = mightBeRef ? (_flags & ~AccessMask.IsNotRef) : (_flags | AccessMask.IsNotRef);

            return new BoundAccess(newflags, _targetType);
        }

        /// <summary>
        /// Simple read access.
        /// </summary>
        public static BoundAccess Read => new BoundAccess(AccessMask.Read, null);

        /// <summary>
        /// Read as a reference access.
        /// </summary>
        public static BoundAccess ReadRef => new BoundAccess(AccessMask.ReadRef, null);

        /// <summary>
        /// Simple write access without bound write type mask.
        /// </summary>
        public static BoundAccess Write => new BoundAccess(AccessMask.Write, null);

        /// <summary>
        /// Unset variable.
        /// </summary>
        public static BoundAccess Unset => new BoundAccess(AccessMask.Unset | AccessMask.ReadQuiet, null);

        /// <summary>
        /// Check for isset.
        /// </summary>
        public static BoundAccess Isset => new BoundAccess(AccessMask.Isset, null);

        /// <summary>
        /// Expression won't be read or written to.
        /// </summary>
        public static BoundAccess None => new BoundAccess(AccessMask.None, null);

        /// <summary>
        /// Read and write without bound write type mask
        /// </summary>
        public static BoundAccess ReadAndWrite => new BoundAccess(AccessMask.Read | AccessMask.Write, null);

        //TODO: set another name
        public static BoundAccess Invoke => new BoundAccess(AccessMask.Invoke | AccessMask.Read, null);

        #endregion
    }

    #endregion

    #region BoundExpression

    public abstract partial class BoundExpression : BoundOperation, IAquilaExpression
    {
        private AquilaSyntaxNode _aquilaSyntax;

        private BoundAccess _acc;

        /// <summary>
        /// Additional expression access,
        /// specifies how the expression is being accessed.
        /// </summary>
        public BoundAccess Access
        {
            get => _acc;
            set => _acc = value;
        }

        /// <summary>
        /// Lazily resolved conversion used to access the value.
        /// Emitted and the result always implicitly converted to <see cref="Type"/>.
        /// </summary>
        public CommonConversion BoundConversion { get; internal set; } // TODO: make it nullable

        public AquilaSyntaxNode AquilaSyntax
        {
            get => _aquilaSyntax ?? SyntaxFactory.EmptyStmt();
            set => _aquilaSyntax = value;
        }

        /// <summary>
        /// Lazily resolved type of the expression result.
        /// </summary>
        public sealed override ITypeSymbol Type => ResultType;

        /// <summary>
        /// Whether the expression needs current <c>Aquila.Core.Context</c> to be evaluated.
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
    }

    #endregion

    #region BoundFunctionCall, BoundArgument, BoundEcho, BoundConcatEx, BoundNewEx

    public partial class BoundArgument : BoundOperation, IArgumentOperation, IAquilaOperation
    {
        public CommonConversion InConversion => default(CommonConversion);

        public bool IsUnpacking => this.ArgumentKind == ArgumentKind.ParamArray;

        public override OperationKind Kind => OperationKind.Argument;

        public CommonConversion OutConversion => default(CommonConversion);

        public IParameterSymbol Parameter { get; set; }

        public SyntaxNode Syntax => null;

        public AquilaSyntaxNode AquilaSyntax { get; set; }

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
    public partial class BoundCallEx : IInvocationOperation
    {
        #region IInvocationOperation impl

        public IMethodSymbol TargetMethod => MethodSymbol;
        IOperation IInvocationOperation.Instance => Instance;
        public bool IsVirtual => MethodSymbol.IsVirtual;

        ImmutableArray<IArgumentOperation> IInvocationOperation.Arguments =>
            StaticCast<IArgumentOperation>.From(_arguments);

        #endregion

        public override bool IsDeeplyCopied => false;
        // methods deeply copy the return value if necessary within its `return` statement already

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

    /// <summary>
    /// Direct or indirect method name.
    /// </summary>
    [DebuggerDisplay("{DebugView,nq}")]
    public partial class BoundMethodName : IAquilaOperation
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

        public AquilaSyntaxNode AquilaSyntax { get; set; }

        public BoundMethodName(QualifiedName name)
            : this(name, null)
        {
        }

        public BoundMethodName(BoundExpression nameExpr)
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

    /// <summary>
    /// Direct new expression with a constructor call.
    /// </summary>
    public partial class BoundNewEx
    {
        public override bool IsDeeplyCopied => false;
    }

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

        public IBlockOperation Body => null; 
        public IMethodSymbol Signature => null;
        IMethodSymbol IAnonymousFunctionOperation.Symbol => null;

        public override bool IsDeeplyCopied => false;

        public BoundLambda(ImmutableArray<BoundArgument> usevars) : base(null)
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
        public override BoundKind BoundKind { get; }
        public override void Accept(OperationVisitor visitor) => visitor.VisitAnonymousFunction(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument) => visitor.VisitAnonymousFunction(this, argument);
    }

    #endregion

    #region BoundEvalEx

    public partial class BoundEvalEx : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind { get; }

        public BoundExpression CodeExpression { get; internal set; }

        public BoundEvalEx(BoundExpression code) : base(null)
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

        partial void OnCreateImpl(object value, ITypeSymbol typeSymbol)
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
        public BoundCopyValue(BoundExpression expression) : base(null)
        {
            this.Expression = expression ?? throw ExceptionUtilities.ArgumentNull(nameof(expression));
        }

        public BoundExpression Expression { get; }

        public override bool RequiresContext => this.Expression.RequiresContext;

        public override bool IsDeeplyCopied => false; // already copied

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind { get; }

        public override void Accept(OperationVisitor visitor) => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument) => visitor.DefaultVisit(this, argument);
        
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

        partial void OnCreateImpl(BoundExpression operand, Operations operation, ITypeSymbol typeSymbol)
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

        public BoundAssignEx(BoundReferenceEx targer, BoundExpression value) : this(targer, value, targer.Type)
        {
        }


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
        private ITypeSymbol _type;

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

        public override ITypeSymbol Type => ResultType;


        public override bool Equals(object obj) => obj is BoundVariableName bname && this == bname;

        public override int GetHashCode() =>
            NameValue.GetHashCode() ^ (NameExpression != null ? NameExpression.GetHashCode() : 0);

        string DebugView
        {
            get { return IsDirect ? NameValue.ToString() : "{indirect}"; }
        }

        public bool IsDirect => NameExpression == null;

        public AquilaSyntaxNode AquilaSyntax { get; set; }

        public BoundVariableName(VariableName name, ITypeSymbol type)
            : this(name, null, type)
        {
        }

        public BoundVariableName(string name, ITypeSymbol type)
            : this(new VariableName(name), type)
        {
        }

        public BoundVariableName(BoundExpression nameExpr, ITypeSymbol type)
            : this(default, nameExpr, type)
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
                return new BoundVariableName(name, nameExpr, ResultType);
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
        public BoundVariableRef(string name, ITypeSymbol type) : this(
            new BoundVariableName(new VariableName(name), type), type)
        {
        }

        /// <summary>
        /// Resolved variable source.
        /// </summary>
        internal IVariableReference Variable { get; set; }

        /// <summary>
        /// Local in case of the variable is resolved local variable.
        /// </summary>
        ILocalSymbol ILocalReferenceOperation.Local => null;

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
    public partial class BoundTemporalVariableRef
    {
        private string _name;

        partial void OnCreateImpl(BoundVariableName name, ITypeSymbol typeSymbol)
        {
            
        }
    }

    #endregion

    #region BoundListEx

    /// <summary>
    /// Aquila <c>list</c> expression that can be written to.
    /// </summary>
    public partial class BoundListEx : BoundReferenceEx
    {
        public BoundListEx(IEnumerable<KeyValuePair<BoundExpression, BoundReferenceEx>> items) : base(null)
        {
            Debug.Assert(items != null);

            _items = items
                .Select(pair =>
                    new KeyValuePair<BoundExpression, BoundReferenceEx>(pair.Key,
                        (BoundReferenceEx)pair.Value))
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
        ISymbol IMemberReferenceOperation.Member => throw new NotImplementedException();

        IFieldSymbol IFieldReferenceOperation.Field => throw new NotImplementedException();

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

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitFieldReference(this);
        }


        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitFieldReference(this, argument);
        }

        public BoundFieldRef Update(BoundExpression instance)
        {
            if (_instance == instance)
                return this;
            return new BoundFieldRef(_field, instance);
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
            public override BoundKind BoundKind { get; }

            public override bool IsDeeplyCopied => false;

            ImmutableArray<IOperation> IArrayInitializerOperation.ElementValues =>
                _array._items.Select(x => x.Value).Cast<IOperation>().AsImmutable();

            public BoundArrayInitializer(BoundArrayEx array) : base(array.ResultType)
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
            ImmutableArray.Create<IOperation>(new BoundLiteral(_items.Length, null));

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
            => (_index != null) ? ImmutableArray.Create((IOperation)_index) : ImmutableArray<IOperation>.Empty;

        partial void OnCreateImpl(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index,
            ITypeSymbol typeSymbol)
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
            return new BoundArrayItemEx(_declaringCompilation, array, index, this.ResultType);
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
                return new BoundArrayItemOrdEx(DeclaringCompilation, array, index, this.ResultType).WithContext(this);
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

        public BoundInstanceOfEx(BoundExpression operand, IBoundTypeRef tref) : base(null)
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
        public override BoundKind BoundKind { get; }

        public override void Accept(OperationVisitor visitor)
            => visitor.VisitIsType(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.VisitIsType(this, argument);
    }

    #endregion

    #region BoundGlobalConst

    public partial class BoundGlobalConst : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind { get; }

        public override bool IsDeeplyCopied => false;

        /// <summary>
        /// Constant name.
        /// </summary>
        public QualifiedName Name { get; private set; }

        /// <summary>
        /// Alternative constant name if <see cref="Name"/> is not resolved.
        /// </summary>
        public QualifiedName? FallbackName { get; private set; }

        public BoundGlobalConst(QualifiedName name, QualifiedName? fallbackName) : base(null)
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

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);
    }

    #endregion

    #region BoundPseudoConst

    #endregion

    #region BoundPseudoClassConst

    #endregion

    #region BoundIsSetEx, BoundOffsetExists, BoundIsEmptyEx, BoundTryGetItem

    public partial class BoundIsEmptyEx : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind { get; }

        /// <summary>
        /// Reference to be checked if it is set.
        /// </summary>
        public BoundExpression Operand { get; set; }

        public BoundIsEmptyEx(BoundExpression expression) : base(null)
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
    }

    public partial class BoundIsSetEx : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind { get; }

        public override bool IsDeeplyCopied => false;

        public override bool RequiresContext => VarReference.RequiresContext;

        /// <summary>
        /// Reference to be checked if it is set.
        /// </summary>
        public BoundReferenceEx VarReference { get; set; }

        public BoundIsSetEx(BoundReferenceEx varref) : base(null)
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
    }

    public partial class BoundOffsetExists : BoundExpression
    {
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind { get; }

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

        public BoundOffsetExists(BoundExpression receiver, BoundExpression index) : base(null)
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
        public override BoundKind BoundKind { get; }

        public BoundExpression Array { get; }
        public BoundExpression Index { get; }
        public BoundExpression Fallback { get; }

        public override bool RequiresContext =>
            Array.RequiresContext || Index.RequiresContext || Fallback.RequiresContext;

        public BoundTryGetItem(BoundExpression array, BoundExpression index, BoundExpression fallback) : base(null)
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

        public override void Accept(OperationVisitor visitor)
            => visitor.DefaultVisit(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.DefaultVisit(this, argument);
    }

    #endregion


    public partial class BoundWildcardEx
    {
    }

    public partial class BoundMatchEx
    {
    }

    public partial class BoundMatchArm
    {
    }

    public partial class BoundAllocEx
    {
    }

    public partial class BoundAllocExAssign
    {
    }

    /// <summary>
    /// Grouped chain of expressions into one expression and return last expression as result
    /// </summary>
    public partial class BoundGroupedEx
    {
    }

    public partial class BoundBadEx
    {
    }

}