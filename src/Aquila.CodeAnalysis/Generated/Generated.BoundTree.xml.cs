using System;
using System.Collections;
using System.Collections.Generic;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.Syntax.Text;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal abstract partial class BoundStatement : BoundOperation
    {
        public BoundStatement()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitStatement(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundEmptyStmt : BoundStatement
    {
        private TextSpan _span;
        public BoundEmptyStmt(TextSpan span)
        {
            _span = span;
        }

        public TextSpan Span
        {
            get
            {
                return _span;
            }
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitEmptyStmt(this);
        }

        public BoundEmptyStmt Update(TextSpan span)
        {
            if (_span == span)
                return this;
            return new BoundEmptyStmt(span);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundBlock : BoundStatement
    {
        private List<BoundStatement> _statements;
        private Edge _nextEdge;
        public BoundBlock(List<BoundStatement> statements, Edge nextEdge)
        {
            _statements = statements;
            _nextEdge = nextEdge;
        }

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

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundDeclareStmt : BoundStatement
    {
        public BoundDeclareStmt()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitDeclareStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundExpressionStmt : BoundStatement
    {
        private BoundExpression _expression;
        public BoundExpressionStmt(BoundExpression expression)
        {
            _expression = expression;
        }

        public BoundExpression Expression
        {
            get
            {
                return _expression;
            }
        }

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundMethodDeclStmt : BoundStatement
    {
        private SourceFunctionSymbol _method;
        public BoundMethodDeclStmt(SourceFunctionSymbol method)
        {
            _method = method;
        }

        public SourceFunctionSymbol Method
        {
            get
            {
                return _method;
            }
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitMethodDeclStmt(this);
        }

        public BoundMethodDeclStmt Update(SourceFunctionSymbol method)
        {
            if (_method == method)
                return this;
            return new BoundMethodDeclStmt(method);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundGlobalConstDeclStmt : BoundStatement
    {
        public BoundGlobalConstDeclStmt()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitGlobalConstDeclStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundReturnStmt : BoundStatement
    {
        private BoundExpression _returned;
        public BoundReturnStmt(BoundExpression returned)
        {
            _returned = returned;
        }

        public BoundExpression Returned
        {
            get
            {
                return _returned;
            }
        }

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundStaticVarStmt : BoundStatement
    {
        public BoundStaticVarStmt()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitStaticVarStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundYieldStmt : BoundStatement
    {
        public BoundYieldStmt()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitYieldStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal abstract partial class BoundExpression : BoundOperation
    {
        public BoundExpression()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitExpression(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundArrayEx : BoundExpression
    {
        public BoundArrayEx()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitArrayEx(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundArrayInit : BoundExpression
    {
        public BoundArrayInit()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitArrayInit(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundAssignEx : BoundExpression
    {
        public BoundAssignEx()
        {
        }

        public override OperationKind Kind
        {
            get;
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
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundUnaryEx : BoundExpression
    {
        private BoundExpression _operand;
        private Operations _operation;
        public BoundUnaryEx(BoundExpression operand, Operations operation)
        {
            _operand = operand;
            _operation = operation;
        }

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

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundBinaryEx : BoundExpression
    {
        private BoundExpression _left;
        private BoundExpression _right;
        private Operations _operation;
        public BoundBinaryEx(BoundExpression left, BoundExpression right, Operations operation)
        {
            _left = left;
            _right = right;
            _operation = operation;
        }

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

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundConditionalEx : BoundExpression
    {
        public BoundConditionalEx()
        {
        }

        public override OperationKind Kind
        {
            get;
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
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundConversionEx : BoundExpression
    {
        private BoundExpression _operand;
        private BoundTypeRef _targetType;
        public BoundConversionEx(BoundExpression operand, BoundTypeRef targetType)
        {
            _operand = operand;
            _targetType = targetType;
        }

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

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundLiteral : BoundExpression
    {
        private object _value;
        public BoundLiteral(object value)
        {
            _value = value;
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundMethod : BoundExpression
    {
        public BoundMethod()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitMethod(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundCallEx : BoundExpression
    {
        public BoundCallEx()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitCallEx(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundThrowEx : BoundExpression
    {
        public BoundThrowEx()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitThrowEx(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal abstract partial class BoundArgument : BoundOperation
    {
        private BoundExpression _value;
        private ArgumentKind _argumentKind;
        public BoundArgument(BoundExpression value, ArgumentKind argumentKind)
        {
            _value = value;
            _argumentKind = argumentKind;
        }

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

        public override OperationKind Kind
        {
            get;
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
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal abstract partial class BoundRoutineName : BoundOperation
    {
        private QualifiedName _name;
        private BoundExpression _nameExpr;
        public BoundRoutineName(QualifiedName name, BoundExpression nameExpr)
        {
            _name = name;
            _nameExpr = nameExpr;
        }

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

        public override OperationKind Kind
        {
            get;
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
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal abstract partial class BoundTypeRef : BoundOperation
    {
        public BoundTypeRef()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitTypeRef(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundArrayTypeRef : BoundTypeRef
    {
        private TypeRefMask _elementType;
        public BoundArrayTypeRef(TypeRefMask elementType)
        {
            _elementType = elementType;
        }

        public TypeRefMask ElementType
        {
            get
            {
                return _elementType;
            }
        }

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundClassTypeRef : BoundTypeRef
    {
        private QualifiedName _qName;
        private SourceRoutineSymbol _routine;
        private int _arity;
        public BoundClassTypeRef(QualifiedName qName, SourceRoutineSymbol routine, int arity)
        {
            _qName = qName;
            _routine = routine;
            _arity = arity;
        }

        public QualifiedName QName
        {
            get
            {
                return _qName;
            }
        }

        public SourceRoutineSymbol Routine
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

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundGenericClassTypeRef : BoundTypeRef
    {
        public BoundGenericClassTypeRef()
        {
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitGenericClassTypeRef(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundPrimitiveTypeRef : BoundTypeRef
    {
        private AquilaTypeCode _type;
        public BoundPrimitiveTypeRef(AquilaTypeCode type)
        {
            _type = type;
        }

        public AquilaTypeCode Type
        {
            get
            {
                return _type;
            }
        }

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundTypeRefFromSymbol : BoundTypeRef
    {
        private ITypeSymbol _symbol;
        public BoundTypeRefFromSymbol(ITypeSymbol symbol)
        {
            _symbol = symbol;
        }

        public ITypeSymbol Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal abstract partial class BoundVariable : BoundOperation
    {
        private VariableKind _variableKind;
        public BoundVariable(VariableKind variableKind)
        {
            _variableKind = variableKind;
        }

        public VariableKind VariableKind
        {
            get
            {
                return _variableKind;
            }
        }

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundIndirectLocal : BoundVariable
    {
        private ParameterSymbol _symbol;
        public BoundIndirectLocal(ParameterSymbol symbol): base(VariableKind.LocalVariable)
        {
            _symbol = symbol;
        }

        public ParameterSymbol Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public override OperationKind Kind
        {
            get;
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

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundLocal : BoundVariable
    {
        private SourceLocalSymbol _symbol;
        private VariableKind _variableKind;
        public BoundLocal(SourceLocalSymbol symbol, VariableKind variableKind): base(variableKind)
        {
            _symbol = symbol;
        }

        public SourceLocalSymbol Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitLocal(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundParameter : BoundVariable
    {
        private ParameterSymbol _symbol;
        public BoundParameter(ParameterSymbol symbol): base(VariableKind.Parameter)
        {
            _symbol = symbol;
        }

        public ParameterSymbol Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitParameter(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal partial class BoundThisParameter : BoundVariable
    {
        private SourceRoutineSymbol _routine;
        public BoundThisParameter(SourceRoutineSymbol routine): base(VariableKind.ThisParameter)
        {
            _routine = routine;
        }

        public SourceRoutineSymbol Routine
        {
            get
            {
                return _routine;
            }
        }

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitThisParameter(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    internal abstract partial class BoundVariableName : BoundOperation
    {
        private VariableName _nameValue;
        private BoundExpression _nameExpression;
        public BoundVariableName(VariableName nameValue, BoundExpression nameExpression)
        {
            _nameValue = nameValue;
            _nameExpression = nameExpression;
        }

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

        public override OperationKind Kind
        {
            get;
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
            return visitor.VisitVariableName(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    public abstract partial class AquilaOperationVisitor<TResult>
    {
        internal virtual TResult VisitStatement(BoundStatement x) => VisitDefault(x);
        internal virtual TResult VisitEmptyStmt(BoundEmptyStmt x) => VisitDefault(x);
        internal virtual TResult VisitBlock(BoundBlock x) => VisitDefault(x);
        internal virtual TResult VisitDeclareStmt(BoundDeclareStmt x) => VisitDefault(x);
        internal virtual TResult VisitExpressionStmt(BoundExpressionStmt x) => VisitDefault(x);
        internal virtual TResult VisitMethodDeclStmt(BoundMethodDeclStmt x) => VisitDefault(x);
        internal virtual TResult VisitGlobalConstDeclStmt(BoundGlobalConstDeclStmt x) => VisitDefault(x);
        internal virtual TResult VisitReturnStmt(BoundReturnStmt x) => VisitDefault(x);
        internal virtual TResult VisitStaticVarStmt(BoundStaticVarStmt x) => VisitDefault(x);
        internal virtual TResult VisitYieldStmt(BoundYieldStmt x) => VisitDefault(x);
        internal virtual TResult VisitExpression(BoundExpression x) => VisitDefault(x);
        internal virtual TResult VisitArrayEx(BoundArrayEx x) => VisitDefault(x);
        internal virtual TResult VisitArrayInit(BoundArrayInit x) => VisitDefault(x);
        internal virtual TResult VisitAssignEx(BoundAssignEx x) => VisitDefault(x);
        internal virtual TResult VisitUnaryEx(BoundUnaryEx x) => VisitDefault(x);
        internal virtual TResult VisitBinaryEx(BoundBinaryEx x) => VisitDefault(x);
        internal virtual TResult VisitConditionalEx(BoundConditionalEx x) => VisitDefault(x);
        internal virtual TResult VisitConversionEx(BoundConversionEx x) => VisitDefault(x);
        internal virtual TResult VisitLiteral(BoundLiteral x) => VisitDefault(x);
        internal virtual TResult VisitMethod(BoundMethod x) => VisitDefault(x);
        internal virtual TResult VisitCallEx(BoundCallEx x) => VisitDefault(x);
        internal virtual TResult VisitThrowEx(BoundThrowEx x) => VisitDefault(x);
        internal virtual TResult VisitArgument(BoundArgument x) => VisitDefault(x);
        internal virtual TResult VisitRoutineName(BoundRoutineName x) => VisitDefault(x);
        internal virtual TResult VisitTypeRef(BoundTypeRef x) => VisitDefault(x);
        internal virtual TResult VisitArrayTypeRef(BoundArrayTypeRef x) => VisitDefault(x);
        internal virtual TResult VisitClassTypeRef(BoundClassTypeRef x) => VisitDefault(x);
        internal virtual TResult VisitGenericClassTypeRef(BoundGenericClassTypeRef x) => VisitDefault(x);
        internal virtual TResult VisitPrimitiveTypeRef(BoundPrimitiveTypeRef x) => VisitDefault(x);
        internal virtual TResult VisitTypeRefFromSymbol(BoundTypeRefFromSymbol x) => VisitDefault(x);
        internal virtual TResult VisitVariable(BoundVariable x) => VisitDefault(x);
        internal virtual TResult VisitIndirectLocal(BoundIndirectLocal x) => VisitDefault(x);
        internal virtual TResult VisitLocal(BoundLocal x) => VisitDefault(x);
        internal virtual TResult VisitParameter(BoundParameter x) => VisitDefault(x);
        internal virtual TResult VisitThisParameter(BoundThisParameter x) => VisitDefault(x);
        internal virtual TResult VisitVariableName(BoundVariableName x) => VisitDefault(x);
    }
}
