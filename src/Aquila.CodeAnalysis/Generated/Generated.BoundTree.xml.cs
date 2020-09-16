using System;
using System.Collections;
using System.Collections.Generic;
using Aquila.CodeAnalysis.FlowAnalysis;
using System.Collections.Immutable;
using Aquila.Syntax.Text;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Aquila.CodeAnalysis.Semantics
{
    abstract partial class BoundStatement : BoundOperation
    {
        public BoundStatement()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitStatement(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundEmptyStmt : BoundStatement
    {
        private Microsoft.CodeAnalysis.Text.TextSpan _span;
        public BoundEmptyStmt(Microsoft.CodeAnalysis.Text.TextSpan span)
        {
            _span = span;
            OnCreateImpl(span);
        }

        partial void OnCreateImpl(Microsoft.CodeAnalysis.Text.TextSpan span);
        public Microsoft.CodeAnalysis.Text.TextSpan Span
        {
            get
            {
                return _span;
            }
        }

        public override OperationKind Kind => OperationKind.Empty;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitEmptyStmt(this);
        }

        public BoundEmptyStmt Update(Microsoft.CodeAnalysis.Text.TextSpan span)
        {
            if (_span == span)
                return this;
            return new BoundEmptyStmt(span);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    partial class BoundBlock : BoundStatement
    {
        private List<BoundStatement> _statements;
        private Edge _nextEdge;
        public BoundBlock(List<BoundStatement> statements, Edge nextEdge = null)
        {
            _statements = statements;
            _nextEdge = nextEdge;
            OnCreateImpl(statements, nextEdge);
        }

        partial void OnCreateImpl(List<BoundStatement> statements, Edge nextEdge);
        public List<BoundStatement> Statements
        {
            get
            {
                return _statements;
            }
        }

        public Edge NextEdge
        {
            get
            {
                return _nextEdge;
            }
        }

        public override OperationKind Kind => OperationKind.Block;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitBlock(this);
        }

        public BoundBlock Update(List<BoundStatement> statements, Edge nextEdge)
        {
            if (_statements == statements && _nextEdge == nextEdge)
                return this;
            return new BoundBlock(statements, nextEdge);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundDeclareStmt : BoundStatement
    {
        public BoundDeclareStmt()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        public override OperationKind Kind => OperationKind.None;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitDeclareStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundExpressionStmt : BoundStatement
    {
        private BoundExpression _expression;
        public BoundExpressionStmt(BoundExpression expression)
        {
            _expression = expression;
            OnCreateImpl(expression);
        }

        partial void OnCreateImpl(BoundExpression expression);
        public BoundExpression Expression
        {
            get
            {
                return _expression;
            }
        }

        public override OperationKind Kind => OperationKind.ExpressionStatement;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }

        public BoundExpressionStmt Update(BoundExpression expression)
        {
            if (_expression == expression)
                return this;
            return new BoundExpressionStmt(expression);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundMethodDeclStmt : BoundStatement
    {
        private SourceFunctionSymbol _method;
        internal BoundMethodDeclStmt(SourceFunctionSymbol method)
        {
            _method = method;
            OnCreateImpl(method);
        }

        partial void OnCreateImpl(SourceFunctionSymbol method);
        internal SourceFunctionSymbol Method
        {
            get
            {
                return _method;
            }
        }

        public override OperationKind Kind => OperationKind.LocalFunction;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitMethodDeclStmt(this);
        }

        internal BoundMethodDeclStmt Update(SourceFunctionSymbol method)
        {
            if (_method == method)
                return this;
            return new BoundMethodDeclStmt(method);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundGlobalConstDeclStmt : BoundStatement
    {
        private QualifiedName _name;
        private BoundExpression _value;
        public BoundGlobalConstDeclStmt(QualifiedName name, BoundExpression value)
        {
            _name = name;
            _value = value;
            OnCreateImpl(name, value);
        }

        partial void OnCreateImpl(QualifiedName name, BoundExpression value);
        public QualifiedName Name
        {
            get
            {
                return _name;
            }
        }

        public BoundExpression Value
        {
            get
            {
                return _value;
            }
        }

        public override OperationKind Kind => OperationKind.VariableDeclaration;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitGlobalConstDeclStmt(this);
        }

        public BoundGlobalConstDeclStmt Update(QualifiedName name, BoundExpression value)
        {
            if (_name == name && _value == value)
                return this;
            return new BoundGlobalConstDeclStmt(name, value);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundReturnStmt : BoundStatement
    {
        private BoundExpression _returned;
        public BoundReturnStmt(BoundExpression returned)
        {
            _returned = returned;
            OnCreateImpl(returned);
        }

        partial void OnCreateImpl(BoundExpression returned);
        public BoundExpression Returned
        {
            get
            {
                return _returned;
            }
        }

        public override OperationKind Kind => OperationKind.Return;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }

        public BoundReturnStmt Update(BoundExpression returned)
        {
            if (_returned == returned)
                return this;
            return new BoundReturnStmt(returned);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundStaticVarStmt : BoundStatement
    {
        public BoundStaticVarStmt()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitStaticVarStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundYieldStmt : BoundStatement
    {
        public BoundYieldStmt()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        public override OperationKind Kind => OperationKind.YieldReturn;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitYieldStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    abstract partial class BoundExpression : BoundOperation
    {
        public BoundExpression()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitExpression(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundArrayEx : BoundExpression
    {
        private ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> _items;
        public BoundArrayEx(ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> items)
        {
            _items = items;
            OnCreateImpl(items);
        }

        partial void OnCreateImpl(ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> items);
        public ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> Items
        {
            get
            {
                return _items;
            }
        }

        public override OperationKind Kind => OperationKind.ArrayCreation;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitArrayEx(this);
        }

        public BoundArrayEx Update(ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> items)
        {
            if (_items == items)
                return this;
            return new BoundArrayEx(items);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundAssignEx : BoundExpression
    {
        private BoundReferenceEx _target;
        private BoundExpression _value;
        public BoundAssignEx(BoundReferenceEx target, BoundExpression value)
        {
            _target = target;
            _value = value;
            OnCreateImpl(target, value);
        }

        partial void OnCreateImpl(BoundReferenceEx target, BoundExpression value);
        public BoundReferenceEx Target
        {
            get
            {
                return _target;
            }
        }

        public BoundExpression Value
        {
            get
            {
                return _value;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitAssignEx(this);
        }

        public BoundAssignEx Update(BoundReferenceEx target, BoundExpression value)
        {
            if (_target == target && _value == value)
                return this;
            return new BoundAssignEx(target, value);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundCompoundAssignEx : BoundAssignEx
    {
        private BoundReferenceEx _target;
        private BoundExpression _value;
        private Operations _operation;
        public BoundCompoundAssignEx(BoundReferenceEx target, BoundExpression value, Operations operation): base(target, value)
        {
            _operation = operation;
            OnCreateImpl(target, value, operation);
        }

        partial void OnCreateImpl(BoundReferenceEx target, BoundExpression value, Operations operation);
        public Operations Operation
        {
            get
            {
                return _operation;
            }
        }

        public override OperationKind Kind => OperationKind.CompoundAssignment;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitCompoundAssignEx(this);
        }

        public BoundCompoundAssignEx Update(BoundReferenceEx target, BoundExpression value, Operations operation)
        {
            if (_target == target && _value == value && _operation == operation)
                return this;
            return new BoundCompoundAssignEx(target, value, operation);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundIncDecEx : BoundCompoundAssignEx
    {
        private BoundReferenceEx _target;
        private bool _isIncrement;
        private bool _isPostfix;
        public BoundIncDecEx(BoundReferenceEx target, bool isIncrement, bool isPostfix): base(target, new BoundLiteral(1L).WithAccess(BoundAccess.Read), Operations.IncDec)
        {
            _isIncrement = isIncrement;
            _isPostfix = isPostfix;
            OnCreateImpl(target, isIncrement, isPostfix);
        }

        partial void OnCreateImpl(BoundReferenceEx target, bool isIncrement, bool isPostfix);
        public bool IsIncrement
        {
            get
            {
                return _isIncrement;
            }
        }

        public bool IsPostfix
        {
            get
            {
                return _isPostfix;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitIncDecEx(this);
        }

        public BoundIncDecEx Update(BoundReferenceEx target, bool isIncrement, bool isPostfix)
        {
            if (_target == target && _isIncrement == isIncrement && _isPostfix == isPostfix)
                return this;
            return new BoundIncDecEx(target, isIncrement, isPostfix);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundUnaryEx : BoundExpression
    {
        private BoundExpression _operand;
        private Operations _operation;
        public BoundUnaryEx(BoundExpression operand, Operations operation)
        {
            _operand = operand;
            _operation = operation;
            OnCreateImpl(operand, operation);
        }

        partial void OnCreateImpl(BoundExpression operand, Operations operation);
        public BoundExpression Operand
        {
            get
            {
                return _operand;
            }
        }

        public Operations Operation
        {
            get
            {
                return _operation;
            }
        }

        public override OperationKind Kind => OperationKind.UnaryOperator;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitUnaryEx(this);
        }

        public BoundUnaryEx Update(BoundExpression operand, Operations operation)
        {
            if (_operand == operand && _operation == operation)
                return this;
            return new BoundUnaryEx(operand, operation);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundBinaryEx : BoundExpression
    {
        private BoundExpression _left;
        private BoundExpression _right;
        private Operations _operation;
        public BoundBinaryEx(BoundExpression left, BoundExpression right, Operations operation)
        {
            _left = left;
            _right = right;
            _operation = operation;
            OnCreateImpl(left, right, operation);
        }

        partial void OnCreateImpl(BoundExpression left, BoundExpression right, Operations operation);
        public BoundExpression Left
        {
            get
            {
                return _left;
            }
        }

        public BoundExpression Right
        {
            get
            {
                return _right;
            }
        }

        public Operations Operation
        {
            get
            {
                return _operation;
            }
        }

        public override OperationKind Kind => OperationKind.BinaryOperator;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitBinaryEx(this);
        }

        public BoundBinaryEx Update(BoundExpression left, BoundExpression right, Operations operation)
        {
            if (_left == left && _right == right && _operation == operation)
                return this;
            return new BoundBinaryEx(left, right, operation);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundConditionalEx : BoundExpression
    {
        private BoundExpression _condition;
        private BoundExpression _ifTrue;
        private BoundExpression _ifFalse;
        public BoundConditionalEx(BoundExpression condition, BoundExpression ifTrue, BoundExpression ifFalse)
        {
            _condition = condition;
            _ifTrue = ifTrue;
            _ifFalse = ifFalse;
            OnCreateImpl(condition, ifTrue, ifFalse);
        }

        partial void OnCreateImpl(BoundExpression condition, BoundExpression ifTrue, BoundExpression ifFalse);
        public BoundExpression Condition
        {
            get
            {
                return _condition;
            }
        }

        public BoundExpression IfTrue
        {
            get
            {
                return _ifTrue;
            }
        }

        public BoundExpression IfFalse
        {
            get
            {
                return _ifFalse;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitConditionalEx(this);
        }

        public BoundConditionalEx Update(BoundExpression condition, BoundExpression ifTrue, BoundExpression ifFalse)
        {
            if (_condition == condition && _ifTrue == ifTrue && _ifFalse == ifFalse)
                return this;
            return new BoundConditionalEx(condition, ifTrue, ifFalse);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundConversionEx : BoundExpression
    {
        private BoundExpression _operand;
        private BoundTypeRef _targetType;
        public BoundConversionEx(BoundExpression operand, BoundTypeRef targetType)
        {
            _operand = operand;
            _targetType = targetType;
            OnCreateImpl(operand, targetType);
        }

        partial void OnCreateImpl(BoundExpression operand, BoundTypeRef targetType);
        public BoundExpression Operand
        {
            get
            {
                return _operand;
            }
        }

        public BoundTypeRef TargetType
        {
            get
            {
                return _targetType;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitConversionEx(this);
        }

        public BoundConversionEx Update(BoundExpression operand, BoundTypeRef targetType)
        {
            if (_operand == operand && _targetType == targetType)
                return this;
            return new BoundConversionEx(operand, targetType);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundLiteral : BoundExpression
    {
        private object _value;
        public BoundLiteral(object value)
        {
            _value = value;
            OnCreateImpl(value);
        }

        partial void OnCreateImpl(object value);
        public object Value
        {
            get
            {
                return _value;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitLiteral(this);
        }

        public BoundLiteral Update(object value)
        {
            if (_value == value)
                return this;
            return new BoundLiteral(value);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundMethod : BoundExpression
    {
        public BoundMethod()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitMethod(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundCallEx : BoundExpression
    {
        public BoundCallEx()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        public override OperationKind Kind => OperationKind.None;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitCallEx(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundThrowEx : BoundExpression
    {
        private BoundExpression _thrown;
        public BoundThrowEx(BoundExpression thrown)
        {
            _thrown = thrown;
            OnCreateImpl(thrown);
        }

        partial void OnCreateImpl(BoundExpression thrown);
        public BoundExpression Thrown
        {
            get
            {
                return _thrown;
            }
        }

        public override OperationKind Kind => OperationKind.Throw;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitThrowEx(this);
        }

        public BoundThrowEx Update(BoundExpression thrown)
        {
            if (_thrown == thrown)
                return this;
            return new BoundThrowEx(thrown);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    abstract partial class BoundReferenceEx : BoundExpression
    {
        public BoundReferenceEx()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitReferenceEx(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundArrayItemEx : BoundReferenceEx
    {
        private AquilaCompilation _declaringCompilation;
        private BoundExpression _array;
        private BoundExpression _index;
        public BoundArrayItemEx(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index)
        {
            _declaringCompilation = declaringCompilation;
            _array = array;
            _index = index;
            OnCreateImpl(declaringCompilation, array, index);
        }

        partial void OnCreateImpl(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index);
        public AquilaCompilation DeclaringCompilation
        {
            get
            {
                return _declaringCompilation;
            }
        }

        public BoundExpression Array
        {
            get
            {
                return _array;
            }
        }

        public BoundExpression Index
        {
            get
            {
                return _index;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitArrayItemEx(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundArrayItemOrdEx : BoundArrayItemEx
    {
        private AquilaCompilation _declaringCompilation;
        private BoundExpression _array;
        private BoundExpression _index;
        public BoundArrayItemOrdEx(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index): base(declaringCompilation, array, index)
        {
            OnCreateImpl(declaringCompilation, array, index);
        }

        partial void OnCreateImpl(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index);
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitArrayItemOrdEx(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundFieldRef : BoundReferenceEx
    {
        private BoundExpression _instance;
        private IBoundTypeRef _containingType;
        private BoundVariableName _fieldName;
        private FieldType _fieldType;
        public BoundFieldRef(BoundExpression instance, IBoundTypeRef containingType, BoundVariableName fieldName, FieldType fieldType)
        {
            _instance = instance;
            _containingType = containingType;
            _fieldName = fieldName;
            _fieldType = fieldType;
            OnCreateImpl(instance, containingType, fieldName, fieldType);
        }

        partial void OnCreateImpl(BoundExpression instance, IBoundTypeRef containingType, BoundVariableName fieldName, FieldType fieldType);
        public IBoundTypeRef ContainingType
        {
            get
            {
                return _containingType;
            }
        }

        public BoundVariableName FieldName
        {
            get
            {
                return _fieldName;
            }
        }

        public FieldType fieldType
        {
            get
            {
                return _fieldType;
            }
        }

        public override OperationKind Kind => OperationKind.FieldReference;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitFieldRef(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundListEx : BoundReferenceEx
    {
        private ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> _items;
        public BoundListEx(ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> items)
        {
            _items = items;
            OnCreateImpl(items);
        }

        partial void OnCreateImpl(ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> items);
        public ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> Items
        {
            get
            {
                return _items;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitListEx(this);
        }

        public BoundListEx Update(ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> items)
        {
            if (_items == items)
                return this;
            return new BoundListEx(items);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundVariableRef : BoundReferenceEx
    {
        private BoundVariableName _name;
        public BoundVariableRef(BoundVariableName name)
        {
            _name = name;
            OnCreateImpl(name);
        }

        partial void OnCreateImpl(BoundVariableName name);
        public BoundVariableName Name
        {
            get
            {
                return _name;
            }
        }

        public override OperationKind Kind => OperationKind.LocalReference;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitVariableRef(this);
        }

        public BoundVariableRef Update(BoundVariableName name)
        {
            if (_name == name)
                return this;
            return new BoundVariableRef(name);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundTemporalVariableRef : BoundVariableRef
    {
        private string _name;
        public BoundTemporalVariableRef(string name): base(name)
        {
            OnCreateImpl(name);
        }

        partial void OnCreateImpl(string name);
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitTemporalVariableRef(this);
        }

        public BoundTemporalVariableRef Update(string name)
        {
            if (_name == name)
                return this;
            return new BoundTemporalVariableRef(name);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundArgument : BoundOperation
    {
        private BoundExpression _value;
        private ArgumentKind _argumentKind;
        public BoundArgument(BoundExpression value, ArgumentKind argumentKind)
        {
            _value = value;
            _argumentKind = argumentKind;
            OnCreateImpl(value, argumentKind);
        }

        partial void OnCreateImpl(BoundExpression value, ArgumentKind argumentKind);
        public BoundExpression Value
        {
            get
            {
                return _value;
            }
        }

        public ArgumentKind ArgumentKind
        {
            get
            {
                return _argumentKind;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitArgument(this);
        }

        public BoundArgument Update(BoundExpression value, ArgumentKind argumentKind)
        {
            if (_value == value && _argumentKind == argumentKind)
                return this;
            return new BoundArgument(value, argumentKind);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundRoutineName : BoundOperation
    {
        private QualifiedName _name;
        private BoundExpression _nameExpr;
        public BoundRoutineName(QualifiedName name, BoundExpression nameExpr)
        {
            _name = name;
            _nameExpr = nameExpr;
            OnCreateImpl(name, nameExpr);
        }

        partial void OnCreateImpl(QualifiedName name, BoundExpression nameExpr);
        public QualifiedName name
        {
            get
            {
                return _name;
            }
        }

        public BoundExpression nameExpr
        {
            get
            {
                return _nameExpr;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitRoutineName(this);
        }

        public BoundRoutineName Update(QualifiedName name, BoundExpression nameExpr)
        {
            if (_name == name && _nameExpr == nameExpr)
                return this;
            return new BoundRoutineName(name, nameExpr);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    abstract partial class BoundTypeRef : BoundOperation
    {
        public BoundTypeRef()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitTypeRef(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundArrayTypeRef : BoundTypeRef
    {
        private TypeRefMask _elementType;
        public BoundArrayTypeRef(TypeRefMask elementType)
        {
            _elementType = elementType;
            OnCreateImpl(elementType);
        }

        partial void OnCreateImpl(TypeRefMask elementType);
        public TypeRefMask ElementType
        {
            get
            {
                return _elementType;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitArrayTypeRef(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundClassTypeRef : BoundTypeRef
    {
        private QualifiedName _qName;
        private SourceRoutineSymbol _routine;
        private int _arity;
        internal BoundClassTypeRef(QualifiedName qName, SourceRoutineSymbol routine, int arity = -1)
        {
            _qName = qName;
            _routine = routine;
            _arity = arity;
            OnCreateImpl(qName, routine, arity);
        }

        partial void OnCreateImpl(QualifiedName qName, SourceRoutineSymbol routine, int arity);
        public QualifiedName QName
        {
            get
            {
                return _qName;
            }
        }

        internal SourceRoutineSymbol Routine
        {
            get
            {
                return _routine;
            }
        }

        public int Arity
        {
            get
            {
                return _arity;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitClassTypeRef(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundGenericClassTypeRef : BoundTypeRef
    {
        public BoundGenericClassTypeRef()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitGenericClassTypeRef(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundPrimitiveTypeRef : BoundTypeRef
    {
        private AquilaTypeCode _type;
        public BoundPrimitiveTypeRef(AquilaTypeCode type)
        {
            _type = type;
            OnCreateImpl(type);
        }

        partial void OnCreateImpl(AquilaTypeCode type);
        public AquilaTypeCode Type
        {
            get
            {
                return _type;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitPrimitiveTypeRef(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundTypeRefFromSymbol : BoundTypeRef
    {
        private ITypeSymbol _symbol;
        public BoundTypeRefFromSymbol(ITypeSymbol symbol)
        {
            _symbol = symbol;
            OnCreateImpl(symbol);
        }

        partial void OnCreateImpl(ITypeSymbol symbol);
        public ITypeSymbol Symbol
        {
            get
            {
                return _symbol;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitTypeRefFromSymbol(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    abstract partial class BoundVariable : BoundOperation
    {
        private VariableKind _variableKind;
        public BoundVariable(VariableKind variableKind)
        {
            _variableKind = variableKind;
            OnCreateImpl(variableKind);
        }

        partial void OnCreateImpl(VariableKind variableKind);
        public VariableKind VariableKind
        {
            get
            {
                return _variableKind;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitVariable(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundIndirectLocal : BoundVariable
    {
        private BoundExpression _nameExpr;
        public BoundIndirectLocal(BoundExpression nameExpr): base(VariableKind.LocalVariable)
        {
            _nameExpr = nameExpr;
            OnCreateImpl(nameExpr);
        }

        partial void OnCreateImpl(BoundExpression nameExpr);
        public BoundExpression NameExpr
        {
            get
            {
                return _nameExpr;
            }
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitIndirectLocal(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundLocal : BoundVariable
    {
        private SourceLocalSymbol _localSymbol;
        private VariableKind _variableKind;
        internal BoundLocal(SourceLocalSymbol localSymbol, VariableKind variableKind = Symbols.VariableKind.LocalVariable): base(variableKind)
        {
            _localSymbol = localSymbol;
            OnCreateImpl(localSymbol, variableKind);
        }

        partial void OnCreateImpl(SourceLocalSymbol localSymbol, VariableKind variableKind);
        internal SourceLocalSymbol LocalSymbol
        {
            get
            {
                return _localSymbol;
            }
        }

        public override OperationKind Kind => OperationKind.VariableDeclaration;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitLocal(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundParameter : BoundVariable
    {
        private ParameterSymbol _parameterSymbol;
        private BoundExpression _initializer;
        internal BoundParameter(ParameterSymbol parameterSymbol, BoundExpression initializer): base(VariableKind.Parameter)
        {
            _parameterSymbol = parameterSymbol;
            _initializer = initializer;
            OnCreateImpl(parameterSymbol, initializer);
        }

        partial void OnCreateImpl(ParameterSymbol parameterSymbol, BoundExpression initializer);
        internal ParameterSymbol ParameterSymbol
        {
            get
            {
                return _parameterSymbol;
            }
        }

        public BoundExpression Initializer
        {
            get
            {
                return _initializer;
            }
        }

        public override OperationKind Kind => OperationKind.ParameterInitializer;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitParameter(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundThisParameter : BoundVariable
    {
        private SourceRoutineSymbol _routine;
        internal BoundThisParameter(SourceRoutineSymbol routine): base(VariableKind.ThisParameter)
        {
            _routine = routine;
            OnCreateImpl(routine);
        }

        partial void OnCreateImpl(SourceRoutineSymbol routine);
        internal SourceRoutineSymbol Routine
        {
            get
            {
                return _routine;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitThisParameter(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundVariableName : BoundOperation
    {
        private VariableName _nameValue;
        private BoundExpression _nameExpression;
        public BoundVariableName(VariableName nameValue, BoundExpression nameExpression)
        {
            _nameValue = nameValue;
            _nameExpression = nameExpression;
            OnCreateImpl(nameValue, nameExpression);
        }

        partial void OnCreateImpl(VariableName nameValue, BoundExpression nameExpression);
        public VariableName NameValue
        {
            get
            {
                return _nameValue;
            }
        }

        public BoundExpression NameExpression
        {
            get
            {
                return _nameExpression;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return visitor.VisitVariableName(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    using Aquila.CodeAnalysis.Semantics.TypeRef;

    abstract partial class AquilaOperationVisitor<TResult>
    {
        public virtual TResult VisitStatement(BoundStatement x) => VisitDefault(x);
        public virtual TResult VisitEmptyStmt(BoundEmptyStmt x) => VisitDefault(x);
        public virtual TResult VisitBlock(BoundBlock x) => VisitDefault(x);
        public virtual TResult VisitDeclareStmt(BoundDeclareStmt x) => VisitDefault(x);
        public virtual TResult VisitExpressionStmt(BoundExpressionStmt x) => VisitDefault(x);
        public virtual TResult VisitMethodDeclStmt(BoundMethodDeclStmt x) => VisitDefault(x);
        public virtual TResult VisitGlobalConstDeclStmt(BoundGlobalConstDeclStmt x) => VisitDefault(x);
        public virtual TResult VisitReturnStmt(BoundReturnStmt x) => VisitDefault(x);
        public virtual TResult VisitStaticVarStmt(BoundStaticVarStmt x) => VisitDefault(x);
        public virtual TResult VisitYieldStmt(BoundYieldStmt x) => VisitDefault(x);
        public virtual TResult VisitExpression(BoundExpression x) => VisitDefault(x);
        public virtual TResult VisitArrayEx(BoundArrayEx x) => VisitDefault(x);
        public virtual TResult VisitAssignEx(BoundAssignEx x) => VisitDefault(x);
        public virtual TResult VisitCompoundAssignEx(BoundCompoundAssignEx x) => VisitDefault(x);
        public virtual TResult VisitIncDecEx(BoundIncDecEx x) => VisitDefault(x);
        public virtual TResult VisitUnaryEx(BoundUnaryEx x) => VisitDefault(x);
        public virtual TResult VisitBinaryEx(BoundBinaryEx x) => VisitDefault(x);
        public virtual TResult VisitConditionalEx(BoundConditionalEx x) => VisitDefault(x);
        public virtual TResult VisitConversionEx(BoundConversionEx x) => VisitDefault(x);
        public virtual TResult VisitLiteral(BoundLiteral x) => VisitDefault(x);
        public virtual TResult VisitMethod(BoundMethod x) => VisitDefault(x);
        public virtual TResult VisitCallEx(BoundCallEx x) => VisitDefault(x);
        public virtual TResult VisitThrowEx(BoundThrowEx x) => VisitDefault(x);
        public virtual TResult VisitReferenceEx(BoundReferenceEx x) => VisitDefault(x);
        public virtual TResult VisitArrayItemEx(BoundArrayItemEx x) => VisitDefault(x);
        public virtual TResult VisitArrayItemOrdEx(BoundArrayItemOrdEx x) => VisitDefault(x);
        public virtual TResult VisitFieldRef(BoundFieldRef x) => VisitDefault(x);
        public virtual TResult VisitListEx(BoundListEx x) => VisitDefault(x);
        public virtual TResult VisitVariableRef(BoundVariableRef x) => VisitDefault(x);
        public virtual TResult VisitTemporalVariableRef(BoundTemporalVariableRef x) => VisitDefault(x);
        public virtual TResult VisitArgument(BoundArgument x) => VisitDefault(x);
        public virtual TResult VisitRoutineName(BoundRoutineName x) => VisitDefault(x);
        public virtual TResult VisitTypeRef(BoundTypeRef x) => VisitDefault(x);
        public virtual TResult VisitArrayTypeRef(BoundArrayTypeRef x) => VisitDefault(x);
        public virtual TResult VisitClassTypeRef(BoundClassTypeRef x) => VisitDefault(x);
        public virtual TResult VisitGenericClassTypeRef(BoundGenericClassTypeRef x) => VisitDefault(x);
        public virtual TResult VisitPrimitiveTypeRef(BoundPrimitiveTypeRef x) => VisitDefault(x);
        public virtual TResult VisitTypeRefFromSymbol(BoundTypeRefFromSymbol x) => VisitDefault(x);
        public virtual TResult VisitVariable(BoundVariable x) => VisitDefault(x);
        public virtual TResult VisitIndirectLocal(BoundIndirectLocal x) => VisitDefault(x);
        public virtual TResult VisitLocal(BoundLocal x) => VisitDefault(x);
        public virtual TResult VisitParameter(BoundParameter x) => VisitDefault(x);
        public virtual TResult VisitThisParameter(BoundThisParameter x) => VisitDefault(x);
        public virtual TResult VisitVariableName(BoundVariableName x) => VisitDefault(x);
    }
}
