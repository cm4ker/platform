using System;
using System.Collections;
using System.Collections.Generic;
using Aquila.CodeAnalysis.FlowAnalysis;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

public enum BoundKind
{
    EmptyStmt,
    Block,
    DeclareStmt,
    ExpressionStmt,
    GlobalConstDeclStmt,
    ReturnStmt,
    StaticVarStmt,
    YieldStmt,
    ForEachStmt,
    BadStmt,
    HtmlMarkupStmt,
    HtmlOpenElementStmt,
    HtmlCloseElementStmt,
    HtmlAddAttributeStmt,
    ArrayEx,
    AssignEx,
    CompoundAssignEx,
    IncDecEx,
    UnaryEx,
    BinaryEx,
    ConditionalEx,
    ConversionEx,
    Literal,
    WildcardEx,
    MatchEx,
    MatchArm,
    BadEx,
    FuncEx,
    CallEx,
    NewEx,
    ThrowEx,
    AllocEx,
    AllocExAssign,
    GroupedEx,
    ArrayItemEx,
    ArrayItemOrdEx,
    FieldRef,
    ListEx,
    VariableRef,
    PropertyRef,
    Argument,
    MethodName,
    ArrayTypeRef,
    ClassTypeRef,
    GenericClassTypeRef,
    PrimitiveTypeRef,
    TypeRefFromSymbol,
    IndirectLocal,
    Local,
    Parameter,
    ThisParameter,
    VariableName,
}

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
        public override BoundKind BoundKind => BoundKind.EmptyStmt;
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
            return new BoundEmptyStmt(span).WithSyntax(this.AquilaSyntax);
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
        public override BoundKind BoundKind => BoundKind.Block;
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
            return new BoundBlock(statements, nextEdge).WithSyntax(this.AquilaSyntax);
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
        public override BoundKind BoundKind => BoundKind.DeclareStmt;
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
        public override BoundKind BoundKind => BoundKind.ExpressionStmt;
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
            return new BoundExpressionStmt(expression).WithSyntax(this.AquilaSyntax);
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
        public override BoundKind BoundKind => BoundKind.GlobalConstDeclStmt;
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
            return new BoundGlobalConstDeclStmt(name, value).WithSyntax(this.AquilaSyntax);
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
        public override BoundKind BoundKind => BoundKind.ReturnStmt;
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
            return new BoundReturnStmt(returned).WithSyntax(this.AquilaSyntax);
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
        public override BoundKind BoundKind => BoundKind.StaticVarStmt;
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
        public override BoundKind BoundKind => BoundKind.YieldStmt;
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
    partial class BoundForEachStmt : BoundStatement
    {
        private BoundReferenceEx _item;
        private BoundExpression _collection;
        private ForeachBindInfo _boundInfo;
        public BoundForEachStmt(BoundReferenceEx item, BoundExpression collection, ForeachBindInfo boundInfo)
        {
            _item = item;
            _collection = collection;
            _boundInfo = boundInfo;
            OnCreateImpl(item, collection, boundInfo);
        }

        partial void OnCreateImpl(BoundReferenceEx item, BoundExpression collection, ForeachBindInfo boundInfo);
        public BoundReferenceEx Item
        {
            get
            {
                return _item;
            }
        }

        public BoundExpression Collection
        {
            get
            {
                return _collection;
            }
        }

        public ForeachBindInfo BoundInfo
        {
            get
            {
                return _boundInfo;
            }
        }

        public override OperationKind Kind => OperationKind.Loop;
        public override BoundKind BoundKind => BoundKind.ForEachStmt;
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
            return visitor.VisitForEachStmt(this);
        }

        public BoundForEachStmt Update(BoundReferenceEx item, BoundExpression collection)
        {
            if (_item == item && _collection == collection)
                return this;
            return new BoundForEachStmt(item, collection, this.BoundInfo).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundBadStmt : BoundStatement
    {
        public BoundBadStmt()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.BadStmt;
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
            return visitor.VisitBadStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundHtmlMarkupStmt : BoundStatement
    {
        private string _markup;
        private int _instructionIndex;
        public BoundHtmlMarkupStmt(string markup, int instructionIndex)
        {
            _markup = markup;
            _instructionIndex = instructionIndex;
            OnCreateImpl(markup, instructionIndex);
        }

        partial void OnCreateImpl(string markup, int instructionIndex);
        public string Markup
        {
            get
            {
                return _markup;
            }
        }

        public int InstructionIndex
        {
            get
            {
                return _instructionIndex;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.HtmlMarkupStmt;
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
            return visitor.VisitHtmlMarkupStmt(this);
        }

        public BoundHtmlMarkupStmt Update(string markup, int instructionIndex)
        {
            if (_markup == markup && _instructionIndex == instructionIndex)
                return this;
            return new BoundHtmlMarkupStmt(markup, instructionIndex).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundHtmlOpenElementStmt : BoundStatement
    {
        private string _elementName;
        private int _instructionIndex;
        public BoundHtmlOpenElementStmt(string elementName, int instructionIndex)
        {
            _elementName = elementName;
            _instructionIndex = instructionIndex;
            OnCreateImpl(elementName, instructionIndex);
        }

        partial void OnCreateImpl(string elementName, int instructionIndex);
        public string ElementName
        {
            get
            {
                return _elementName;
            }
        }

        public int InstructionIndex
        {
            get
            {
                return _instructionIndex;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.HtmlOpenElementStmt;
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
            return visitor.VisitHtmlOpenElementStmt(this);
        }

        public BoundHtmlOpenElementStmt Update(string elementName, int instructionIndex)
        {
            if (_elementName == elementName && _instructionIndex == instructionIndex)
                return this;
            return new BoundHtmlOpenElementStmt(elementName, instructionIndex).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundHtmlCloseElementStmt : BoundStatement
    {
        public BoundHtmlCloseElementStmt()
        {
            OnCreateImpl();
        }

        partial void OnCreateImpl();
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.HtmlCloseElementStmt;
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
            return visitor.VisitHtmlCloseElementStmt(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundHtmlAddAttributeStmt : BoundStatement
    {
        private string _attributeName;
        private BoundExpression _expression;
        private int _instructionIndex;
        public BoundHtmlAddAttributeStmt(string attributeName, BoundExpression expression, int instructionIndex)
        {
            _attributeName = attributeName;
            _expression = expression;
            _instructionIndex = instructionIndex;
            OnCreateImpl(attributeName, expression, instructionIndex);
        }

        partial void OnCreateImpl(string attributeName, BoundExpression expression, int instructionIndex);
        public string AttributeName
        {
            get
            {
                return _attributeName;
            }
        }

        public BoundExpression Expression
        {
            get
            {
                return _expression;
            }
        }

        public int InstructionIndex
        {
            get
            {
                return _instructionIndex;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.HtmlAddAttributeStmt;
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
            return visitor.VisitHtmlAddAttributeStmt(this);
        }

        public BoundHtmlAddAttributeStmt Update(string attributeName, BoundExpression expression, int instructionIndex)
        {
            if (_attributeName == attributeName && _expression == expression && _instructionIndex == instructionIndex)
                return this;
            return new BoundHtmlAddAttributeStmt(attributeName, expression, instructionIndex).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    abstract partial class BoundExpression : BoundOperation
    {
        private ITypeSymbol _resultType;
        internal BoundExpression(ITypeSymbol resultType)
        {
            _resultType = resultType;
            OnCreateImpl(resultType);
        }

        partial void OnCreateImpl(ITypeSymbol resultType);
        internal ITypeSymbol ResultType
        {
            get
            {
                return _resultType;
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
            return visitor.VisitExpression(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundArrayEx : BoundExpression
    {
        private ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> _items;
        internal BoundArrayEx(ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> items, ITypeSymbol resultType): base(resultType)
        {
            _items = items;
            OnCreateImpl(items, resultType);
        }

        partial void OnCreateImpl(ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> items, ITypeSymbol resultType);
        public ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> Items
        {
            get
            {
                return _items;
            }
        }

        public override OperationKind Kind => OperationKind.ArrayCreation;
        public override BoundKind BoundKind => BoundKind.ArrayEx;
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

        internal BoundArrayEx Update(ImmutableArray<KeyValuePair<BoundExpression, BoundExpression>> items, ITypeSymbol resultType)
        {
            if (_items == items && ResultType == resultType)
                return this;
            return new BoundArrayEx(items, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundAssignEx : BoundExpression
    {
        private BoundReferenceEx _target;
        private BoundExpression _value;
        internal BoundAssignEx(BoundReferenceEx target, BoundExpression value, ITypeSymbol resultType): base(resultType)
        {
            _target = target;
            _value = value;
            OnCreateImpl(target, value, resultType);
        }

        partial void OnCreateImpl(BoundReferenceEx target, BoundExpression value, ITypeSymbol resultType);
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

        public override BoundKind BoundKind => BoundKind.AssignEx;
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

        internal BoundAssignEx Update(BoundReferenceEx target, BoundExpression value, ITypeSymbol resultType)
        {
            if (_target == target && _value == value && ResultType == resultType)
                return this;
            return new BoundAssignEx(target, value, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundCompoundAssignEx : BoundAssignEx
    {
        private Operations _operation;
        internal BoundCompoundAssignEx(BoundReferenceEx target, BoundExpression value, Operations operation, ITypeSymbol resultType): base(target, value, resultType)
        {
            _operation = operation;
            OnCreateImpl(target, value, operation, resultType);
        }

        partial void OnCreateImpl(BoundReferenceEx target, BoundExpression value, Operations operation, ITypeSymbol resultType);
        public Operations Operation
        {
            get
            {
                return _operation;
            }
        }

        public override OperationKind Kind => OperationKind.CompoundAssignment;
        public override BoundKind BoundKind => BoundKind.CompoundAssignEx;
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

        internal BoundCompoundAssignEx Update(BoundReferenceEx target, BoundExpression value, Operations operation, ITypeSymbol resultType)
        {
            if (Target == target && Value == value && _operation == operation && ResultType == resultType)
                return this;
            return new BoundCompoundAssignEx(target, value, operation, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundIncDecEx : BoundCompoundAssignEx
    {
        private bool _isIncrement;
        private bool _isPostfix;
        internal BoundIncDecEx(BoundReferenceEx target, bool isIncrement, bool isPostfix, ITypeSymbol resultType): base(target, new BoundLiteral(1, resultType).WithAccess(BoundAccess.Read), Operations.IncDec, resultType)
        {
            _isIncrement = isIncrement;
            _isPostfix = isPostfix;
            OnCreateImpl(target, isIncrement, isPostfix, resultType);
        }

        partial void OnCreateImpl(BoundReferenceEx target, bool isIncrement, bool isPostfix, ITypeSymbol resultType);
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

        public override BoundKind BoundKind => BoundKind.IncDecEx;
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

        internal BoundIncDecEx Update(BoundReferenceEx target, bool isIncrement, bool isPostfix, ITypeSymbol resultType)
        {
            if (Target == target && _isIncrement == isIncrement && _isPostfix == isPostfix && ResultType == resultType)
                return this;
            return new BoundIncDecEx(target, isIncrement, isPostfix, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundUnaryEx : BoundExpression
    {
        private BoundExpression _operand;
        private Operations _operation;
        internal BoundUnaryEx(BoundExpression operand, Operations operation, ITypeSymbol resultType): base(resultType)
        {
            _operand = operand;
            _operation = operation;
            OnCreateImpl(operand, operation, resultType);
        }

        partial void OnCreateImpl(BoundExpression operand, Operations operation, ITypeSymbol resultType);
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
        public override BoundKind BoundKind => BoundKind.UnaryEx;
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

        internal BoundUnaryEx Update(BoundExpression operand, Operations operation, ITypeSymbol resultType)
        {
            if (_operand == operand && _operation == operation && ResultType == resultType)
                return this;
            return new BoundUnaryEx(operand, operation, resultType).WithSyntax(this.AquilaSyntax);
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
        internal BoundBinaryEx(BoundExpression left, BoundExpression right, Operations operation, ITypeSymbol resultType): base(resultType)
        {
            _left = left;
            _right = right;
            _operation = operation;
            OnCreateImpl(left, right, operation, resultType);
        }

        partial void OnCreateImpl(BoundExpression left, BoundExpression right, Operations operation, ITypeSymbol resultType);
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
        public override BoundKind BoundKind => BoundKind.BinaryEx;
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

        internal BoundBinaryEx Update(BoundExpression left, BoundExpression right, Operations operation, ITypeSymbol resultType)
        {
            if (_left == left && _right == right && _operation == operation && ResultType == resultType)
                return this;
            return new BoundBinaryEx(left, right, operation, resultType).WithSyntax(this.AquilaSyntax);
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
        internal BoundConditionalEx(BoundExpression condition, BoundExpression ifTrue, BoundExpression ifFalse, ITypeSymbol resultType): base(resultType)
        {
            _condition = condition;
            _ifTrue = ifTrue;
            _ifFalse = ifFalse;
            OnCreateImpl(condition, ifTrue, ifFalse, resultType);
        }

        partial void OnCreateImpl(BoundExpression condition, BoundExpression ifTrue, BoundExpression ifFalse, ITypeSymbol resultType);
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

        public override BoundKind BoundKind => BoundKind.ConditionalEx;
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

        internal BoundConditionalEx Update(BoundExpression condition, BoundExpression ifTrue, BoundExpression ifFalse, ITypeSymbol resultType)
        {
            if (_condition == condition && _ifTrue == ifTrue && _ifFalse == ifFalse && ResultType == resultType)
                return this;
            return new BoundConditionalEx(condition, ifTrue, ifFalse, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundConversionEx : BoundExpression
    {
        private BoundExpression _operand;
        private BoundTypeRef _targetType;
        internal BoundConversionEx(BoundExpression operand, BoundTypeRef targetType, ITypeSymbol resultType): base(resultType)
        {
            _operand = operand;
            _targetType = targetType;
            OnCreateImpl(operand, targetType, resultType);
        }

        partial void OnCreateImpl(BoundExpression operand, BoundTypeRef targetType, ITypeSymbol resultType);
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

        public override BoundKind BoundKind => BoundKind.ConversionEx;
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

        internal BoundConversionEx Update(BoundExpression operand, BoundTypeRef targetType, ITypeSymbol resultType)
        {
            if (_operand == operand && _targetType == targetType && ResultType == resultType)
                return this;
            return new BoundConversionEx(operand, targetType, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundLiteral : BoundExpression
    {
        private object _value;
        internal BoundLiteral(object value, ITypeSymbol resultType): base(resultType)
        {
            _value = value;
            OnCreateImpl(value, resultType);
        }

        partial void OnCreateImpl(object value, ITypeSymbol resultType);
        public object Value
        {
            get
            {
                return _value;
            }
        }

        public override BoundKind BoundKind => BoundKind.Literal;
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

        internal BoundLiteral Update(object value, ITypeSymbol resultType)
        {
            if (_value == value && ResultType == resultType)
                return this;
            return new BoundLiteral(value, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundWildcardEx : BoundExpression
    {
        internal BoundWildcardEx(ITypeSymbol resultType): base(resultType)
        {
            OnCreateImpl(resultType);
        }

        partial void OnCreateImpl(ITypeSymbol resultType);
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.WildcardEx;
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
            return visitor.VisitWildcardEx(this);
        }

        internal BoundWildcardEx Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundWildcardEx(resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundMatchEx : BoundExpression
    {
        private BoundExpression _expression;
        private List<BoundMatchArm> _arms;
        internal BoundMatchEx(BoundExpression expression, List<BoundMatchArm> arms, ITypeSymbol resultType): base(resultType)
        {
            _expression = expression;
            _arms = arms;
            OnCreateImpl(expression, arms, resultType);
        }

        partial void OnCreateImpl(BoundExpression expression, List<BoundMatchArm> arms, ITypeSymbol resultType);
        public BoundExpression Expression
        {
            get
            {
                return _expression;
            }
        }

        public List<BoundMatchArm> Arms
        {
            get
            {
                return _arms;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.MatchEx;
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
            return visitor.VisitMatchEx(this);
        }

        internal BoundMatchEx Update(BoundExpression expression, List<BoundMatchArm> arms, ITypeSymbol resultType)
        {
            if (_expression == expression && _arms == arms && ResultType == resultType)
                return this;
            return new BoundMatchEx(expression, arms, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundMatchArm : BoundExpression
    {
        private BoundExpression _pattern;
        private BoundExpression _whenGuard;
        private BoundExpression _matchResult;
        internal BoundMatchArm(BoundExpression pattern, BoundExpression whenGuard, BoundExpression matchResult, ITypeSymbol resultType): base(resultType)
        {
            _pattern = pattern;
            _whenGuard = whenGuard;
            _matchResult = matchResult;
            OnCreateImpl(pattern, whenGuard, matchResult, resultType);
        }

        partial void OnCreateImpl(BoundExpression pattern, BoundExpression whenGuard, BoundExpression matchResult, ITypeSymbol resultType);
        public BoundExpression Pattern
        {
            get
            {
                return _pattern;
            }
        }

        public BoundExpression WhenGuard
        {
            get
            {
                return _whenGuard;
            }
        }

        public BoundExpression MatchResult
        {
            get
            {
                return _matchResult;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.MatchArm;
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
            return visitor.VisitMatchArm(this);
        }

        internal BoundMatchArm Update(BoundExpression pattern, BoundExpression whenGuard, BoundExpression matchResult, ITypeSymbol resultType)
        {
            if (_pattern == pattern && _whenGuard == whenGuard && _matchResult == matchResult && ResultType == resultType)
                return this;
            return new BoundMatchArm(pattern, whenGuard, matchResult, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundBadEx : BoundExpression
    {
        internal BoundBadEx(ITypeSymbol resultType): base(resultType)
        {
            OnCreateImpl(resultType);
        }

        partial void OnCreateImpl(ITypeSymbol resultType);
        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.BadEx;
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
            return visitor.VisitBadEx(this);
        }

        internal BoundBadEx Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundBadEx(resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundFuncEx : BoundExpression
    {
        private SourceLambdaSymbol _lambdaSymbol;
        internal BoundFuncEx(SourceLambdaSymbol lambdaSymbol, ITypeSymbol resultType): base(resultType)
        {
            _lambdaSymbol = lambdaSymbol;
            OnCreateImpl(lambdaSymbol, resultType);
        }

        partial void OnCreateImpl(SourceLambdaSymbol lambdaSymbol, ITypeSymbol resultType);
        internal SourceLambdaSymbol LambdaSymbol
        {
            get
            {
                return _lambdaSymbol;
            }
        }

        public override OperationKind Kind => OperationKind.AnonymousFunction;
        public override BoundKind BoundKind => BoundKind.FuncEx;
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
            return visitor.VisitFuncEx(this);
        }

        internal BoundFuncEx Update(SourceLambdaSymbol lambdaSymbol, ITypeSymbol resultType)
        {
            if (_lambdaSymbol == lambdaSymbol && ResultType == resultType)
                return this;
            return new BoundFuncEx(lambdaSymbol, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundCallEx : BoundExpression
    {
        private MethodSymbol _methodSymbol;
        private ImmutableArray<BoundArgument> _arguments;
        private ImmutableArray<ITypeSymbol> _typeArguments;
        private BoundExpression _instance;
        internal BoundCallEx(MethodSymbol methodSymbol, ImmutableArray<BoundArgument> arguments, ImmutableArray<ITypeSymbol> typeArguments, BoundExpression instance, ITypeSymbol resultType): base(resultType)
        {
            _methodSymbol = methodSymbol;
            _arguments = arguments;
            _typeArguments = typeArguments;
            _instance = instance;
            OnCreateImpl(methodSymbol, arguments, typeArguments, instance, resultType);
        }

        partial void OnCreateImpl(MethodSymbol methodSymbol, ImmutableArray<BoundArgument> arguments, ImmutableArray<ITypeSymbol> typeArguments, BoundExpression instance, ITypeSymbol resultType);
        internal MethodSymbol MethodSymbol
        {
            get
            {
                return _methodSymbol;
            }
        }

        public ImmutableArray<BoundArgument> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public ImmutableArray<ITypeSymbol> TypeArguments
        {
            get
            {
                return _typeArguments;
            }
        }

        public BoundExpression Instance
        {
            get
            {
                return _instance;
            }
        }

        public override OperationKind Kind => OperationKind.Invocation;
        public override BoundKind BoundKind => BoundKind.CallEx;
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

        internal BoundCallEx Update(MethodSymbol methodSymbol, ImmutableArray<BoundArgument> arguments, ImmutableArray<ITypeSymbol> typeArguments, BoundExpression instance, ITypeSymbol resultType)
        {
            if (_methodSymbol == methodSymbol && _arguments == arguments && _typeArguments == typeArguments && _instance == instance && ResultType == resultType)
                return this;
            return new BoundCallEx(methodSymbol, arguments, typeArguments, instance, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundNewEx : BoundExpression
    {
        private MethodSymbol _methodSymbol;
        private ITypeSymbol _typeRef;
        private ImmutableArray<BoundArgument> _arguments;
        private ImmutableArray<ITypeSymbol> _typeArguments;
        internal BoundNewEx(MethodSymbol methodSymbol, ITypeSymbol typeRef, ImmutableArray<BoundArgument> arguments, ImmutableArray<ITypeSymbol> typeArguments, ITypeSymbol resultType): base(resultType)
        {
            _methodSymbol = methodSymbol;
            _typeRef = typeRef;
            _arguments = arguments;
            _typeArguments = typeArguments;
            OnCreateImpl(methodSymbol, typeRef, arguments, typeArguments, resultType);
        }

        partial void OnCreateImpl(MethodSymbol methodSymbol, ITypeSymbol typeRef, ImmutableArray<BoundArgument> arguments, ImmutableArray<ITypeSymbol> typeArguments, ITypeSymbol resultType);
        internal MethodSymbol MethodSymbol
        {
            get
            {
                return _methodSymbol;
            }
        }

        public ITypeSymbol TypeRef
        {
            get
            {
                return _typeRef;
            }
        }

        public ImmutableArray<BoundArgument> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public ImmutableArray<ITypeSymbol> TypeArguments
        {
            get
            {
                return _typeArguments;
            }
        }

        public override OperationKind Kind => OperationKind.ObjectCreation;
        public override BoundKind BoundKind => BoundKind.NewEx;
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
            return visitor.VisitNewEx(this);
        }

        internal BoundNewEx Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundNewEx(this.MethodSymbol, this.TypeRef, this.Arguments, this.TypeArguments, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundThrowEx : BoundExpression
    {
        private BoundExpression _thrown;
        internal BoundThrowEx(BoundExpression thrown, ITypeSymbol resultType): base(resultType)
        {
            _thrown = thrown;
            OnCreateImpl(thrown, resultType);
        }

        partial void OnCreateImpl(BoundExpression thrown, ITypeSymbol resultType);
        public BoundExpression Thrown
        {
            get
            {
                return _thrown;
            }
        }

        public override OperationKind Kind => OperationKind.Throw;
        public override BoundKind BoundKind => BoundKind.ThrowEx;
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

        internal BoundThrowEx Update(BoundExpression thrown, ITypeSymbol resultType)
        {
            if (_thrown == thrown && ResultType == resultType)
                return this;
            return new BoundThrowEx(thrown, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundAllocEx : BoundExpression
    {
        private ITypeSymbol _typeRef;
        private List<BoundAllocExAssign> _initializer;
        internal BoundAllocEx(ITypeSymbol typeRef, List<BoundAllocExAssign> initializer, ITypeSymbol resultType): base(resultType)
        {
            _typeRef = typeRef;
            _initializer = initializer;
            OnCreateImpl(typeRef, initializer, resultType);
        }

        partial void OnCreateImpl(ITypeSymbol typeRef, List<BoundAllocExAssign> initializer, ITypeSymbol resultType);
        public ITypeSymbol TypeRef
        {
            get
            {
                return _typeRef;
            }
        }

        public List<BoundAllocExAssign> Initializer
        {
            get
            {
                return _initializer;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.AllocEx;
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
            return visitor.VisitAllocEx(this);
        }

        internal BoundAllocEx Update(List<BoundAllocExAssign> initializer, ITypeSymbol resultType)
        {
            if (_initializer == initializer && ResultType == resultType)
                return this;
            return new BoundAllocEx(this.TypeRef, initializer, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundAllocExAssign : BoundExpression
    {
        private ISymbol _receiverSymbol;
        private BoundExpression _expression;
        internal BoundAllocExAssign(ISymbol receiverSymbol, BoundExpression expression): base(expression.Type)
        {
            _receiverSymbol = receiverSymbol;
            _expression = expression;
            OnCreateImpl(receiverSymbol, expression);
        }

        partial void OnCreateImpl(ISymbol receiverSymbol, BoundExpression expression);
        public ISymbol ReceiverSymbol
        {
            get
            {
                return _receiverSymbol;
            }
        }

        public BoundExpression Expression
        {
            get
            {
                return _expression;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.AllocExAssign;
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
            return visitor.VisitAllocExAssign(this);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundGroupedEx : BoundExpression
    {
        private List<BoundExpression> _expressions;
        internal BoundGroupedEx(List<BoundExpression> expressions, ITypeSymbol resultType): base(resultType)
        {
            _expressions = expressions;
            OnCreateImpl(expressions, resultType);
        }

        partial void OnCreateImpl(List<BoundExpression> expressions, ITypeSymbol resultType);
        public List<BoundExpression> Expressions
        {
            get
            {
                return _expressions;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.GroupedEx;
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
            return visitor.VisitGroupedEx(this);
        }

        internal BoundGroupedEx Update(List<BoundExpression> expressions, ITypeSymbol resultType)
        {
            if (_expressions == expressions && ResultType == resultType)
                return this;
            return new BoundGroupedEx(expressions, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    abstract partial class BoundReferenceEx : BoundExpression
    {
        internal BoundReferenceEx(ITypeSymbol resultType): base(resultType)
        {
            OnCreateImpl(resultType);
        }

        partial void OnCreateImpl(ITypeSymbol resultType);
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
        internal BoundArrayItemEx(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index, ITypeSymbol resultType): base(resultType)
        {
            _declaringCompilation = declaringCompilation;
            _array = array;
            _index = index;
            OnCreateImpl(declaringCompilation, array, index, resultType);
        }

        partial void OnCreateImpl(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index, ITypeSymbol resultType);
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

        public override BoundKind BoundKind => BoundKind.ArrayItemEx;
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

        internal BoundArrayItemEx Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundArrayItemEx(this.DeclaringCompilation, this.Array, this.Index, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundArrayItemOrdEx : BoundArrayItemEx
    {
        internal BoundArrayItemOrdEx(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index, ITypeSymbol resultType): base(declaringCompilation, array, index, resultType)
        {
            OnCreateImpl(declaringCompilation, array, index, resultType);
        }

        partial void OnCreateImpl(AquilaCompilation declaringCompilation, BoundExpression array, BoundExpression index, ITypeSymbol resultType);
        public override BoundKind BoundKind => BoundKind.ArrayItemOrdEx;
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

        internal BoundArrayItemOrdEx Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundArrayItemOrdEx(this.DeclaringCompilation, this.Array, this.Index, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundFieldRef : BoundReferenceEx
    {
        private IFieldSymbol _field;
        private BoundExpression _instance;
        internal BoundFieldRef(IFieldSymbol field, BoundExpression instance): base(field.Type)
        {
            _field = field;
            _instance = instance;
            OnCreateImpl(field, instance);
        }

        partial void OnCreateImpl(IFieldSymbol field, BoundExpression instance);
        public IFieldSymbol Field
        {
            get
            {
                return _field;
            }
        }

        public BoundExpression Instance
        {
            get
            {
                return _instance;
            }
        }

        public override OperationKind Kind => OperationKind.FieldReference;
        public override BoundKind BoundKind => BoundKind.FieldRef;
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
        internal BoundListEx(ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> items, ITypeSymbol resultType): base(resultType)
        {
            _items = items;
            OnCreateImpl(items, resultType);
        }

        partial void OnCreateImpl(ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> items, ITypeSymbol resultType);
        public ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> Items
        {
            get
            {
                return _items;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.ListEx;
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

        internal BoundListEx Update(ImmutableArray<KeyValuePair<BoundExpression, BoundReferenceEx>> items, ITypeSymbol resultType)
        {
            if (_items == items && ResultType == resultType)
                return this;
            return new BoundListEx(items, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundVariableRef : BoundReferenceEx
    {
        private BoundVariableName _name;
        internal BoundVariableRef(BoundVariableName name, ITypeSymbol resultType): base(resultType)
        {
            _name = name;
            OnCreateImpl(name, resultType);
        }

        partial void OnCreateImpl(BoundVariableName name, ITypeSymbol resultType);
        public BoundVariableName Name
        {
            get
            {
                return _name;
            }
        }

        public override OperationKind Kind => OperationKind.LocalReference;
        public override BoundKind BoundKind => BoundKind.VariableRef;
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

        internal BoundVariableRef Update(BoundVariableName name, ITypeSymbol resultType)
        {
            if (_name == name && ResultType == resultType)
                return this;
            return new BoundVariableRef(name, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundPropertyRef : BoundReferenceEx
    {
        private IPropertySymbol _property;
        private BoundExpression _instance;
        internal BoundPropertyRef(IPropertySymbol property, BoundExpression instance): base(property.Type)
        {
            _property = property;
            _instance = instance;
            OnCreateImpl(property, instance);
        }

        partial void OnCreateImpl(IPropertySymbol property, BoundExpression instance);
        public IPropertySymbol Property
        {
            get
            {
                return _property;
            }
        }

        public BoundExpression Instance
        {
            get
            {
                return _instance;
            }
        }

        public override OperationKind Kind => OperationKind.Invocation;
        public override BoundKind BoundKind => BoundKind.PropertyRef;
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
            return visitor.VisitPropertyRef(this);
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

        public override BoundKind BoundKind => BoundKind.Argument;
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
            return new BoundArgument(value, argumentKind).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundMethodName : BoundOperation
    {
        private QualifiedName _name;
        private BoundExpression _nameExpr;
        public BoundMethodName(QualifiedName name, BoundExpression nameExpr)
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

        public override BoundKind BoundKind => BoundKind.MethodName;
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
            return visitor.VisitMethodName(this);
        }

        public BoundMethodName Update(QualifiedName name, BoundExpression nameExpr)
        {
            if (_name == name && _nameExpr == nameExpr)
                return this;
            return new BoundMethodName(name, nameExpr).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics
{
    abstract partial class BoundTypeRef : BoundExpression
    {
        internal BoundTypeRef(ITypeSymbol resultType): base(resultType)
        {
            OnCreateImpl(resultType);
        }

        partial void OnCreateImpl(ITypeSymbol resultType);
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
        internal BoundArrayTypeRef(ITypeSymbol resultType): base(resultType)
        {
            OnCreateImpl(resultType);
        }

        partial void OnCreateImpl(ITypeSymbol resultType);
        public override BoundKind BoundKind => BoundKind.ArrayTypeRef;
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

        internal BoundArrayTypeRef Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundArrayTypeRef(resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundClassTypeRef : BoundTypeRef
    {
        private QualifiedName _qName;
        private SourceMethodSymbolBase _method;
        private int _arity;
        internal BoundClassTypeRef(QualifiedName qName, SourceMethodSymbolBase method, ITypeSymbol resultType, int arity = -1): base(resultType)
        {
            _qName = qName;
            _method = method;
            _arity = arity;
            OnCreateImpl(qName, method, resultType, arity);
        }

        partial void OnCreateImpl(QualifiedName qName, SourceMethodSymbolBase method, ITypeSymbol resultType, int arity);
        public QualifiedName QName
        {
            get
            {
                return _qName;
            }
        }

        internal SourceMethodSymbolBase Method
        {
            get
            {
                return _method;
            }
        }

        public int Arity
        {
            get
            {
                return _arity;
            }
        }

        public override BoundKind BoundKind => BoundKind.ClassTypeRef;
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

        internal BoundClassTypeRef Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundClassTypeRef(this.QName, this.Method, resultType, this.Arity).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundGenericClassTypeRef : BoundTypeRef
    {
        internal BoundGenericClassTypeRef(ITypeSymbol resultType): base(resultType)
        {
            OnCreateImpl(resultType);
        }

        partial void OnCreateImpl(ITypeSymbol resultType);
        public override BoundKind BoundKind => BoundKind.GenericClassTypeRef;
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

        internal BoundGenericClassTypeRef Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundGenericClassTypeRef(resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundPrimitiveTypeRef : BoundTypeRef
    {
        private AquilaTypeCode _type;
        internal BoundPrimitiveTypeRef(AquilaTypeCode type, ITypeSymbol resultType): base(resultType)
        {
            _type = type;
            OnCreateImpl(type, resultType);
        }

        partial void OnCreateImpl(AquilaTypeCode type, ITypeSymbol resultType);
        public AquilaTypeCode Type
        {
            get
            {
                return _type;
            }
        }

        public override BoundKind BoundKind => BoundKind.PrimitiveTypeRef;
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

        internal BoundPrimitiveTypeRef Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundPrimitiveTypeRef(this.Type, resultType).WithSyntax(this.AquilaSyntax);
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    partial class BoundTypeRefFromSymbol : BoundTypeRef
    {
        internal BoundTypeRefFromSymbol(ITypeSymbol resultType): base(resultType)
        {
            OnCreateImpl(resultType);
        }

        partial void OnCreateImpl(ITypeSymbol resultType);
        public override BoundKind BoundKind => BoundKind.TypeRefFromSymbol;
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

        internal BoundTypeRefFromSymbol Update(ITypeSymbol resultType)
        {
            if (ResultType == resultType)
                return this;
            return new BoundTypeRefFromSymbol(resultType).WithSyntax(this.AquilaSyntax);
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

        public override BoundKind BoundKind => BoundKind.IndirectLocal;
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
        public override BoundKind BoundKind => BoundKind.Local;
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
        public override BoundKind BoundKind => BoundKind.Parameter;
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
        private MethodSymbol _method;
        internal BoundThisParameter(MethodSymbol method): base(VariableKind.ThisParameter)
        {
            _method = method;
            OnCreateImpl(method);
        }

        partial void OnCreateImpl(MethodSymbol method);
        internal MethodSymbol Method
        {
            get
            {
                return _method;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.ThisParameter;
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
        private ITypeSymbol _resultType;
        internal BoundVariableName(VariableName nameValue, BoundExpression nameExpression, ITypeSymbol resultType)
        {
            _nameValue = nameValue;
            _nameExpression = nameExpression;
            _resultType = resultType;
            OnCreateImpl(nameValue, nameExpression, resultType);
        }

        partial void OnCreateImpl(VariableName nameValue, BoundExpression nameExpression, ITypeSymbol resultType);
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

        internal ITypeSymbol ResultType
        {
            get
            {
                return _resultType;
            }
        }

        public override OperationKind Kind => OperationKind.None;
        public override BoundKind BoundKind => BoundKind.VariableName;
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

        internal BoundVariableName Update(ITypeSymbol resultType)
        {
            if (_resultType == resultType)
                return this;
            return new BoundVariableName(this.NameValue, this.NameExpression, resultType).WithSyntax(this.AquilaSyntax);
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
        public virtual TResult VisitGlobalConstDeclStmt(BoundGlobalConstDeclStmt x) => VisitDefault(x);
        public virtual TResult VisitReturnStmt(BoundReturnStmt x) => VisitDefault(x);
        public virtual TResult VisitStaticVarStmt(BoundStaticVarStmt x) => VisitDefault(x);
        public virtual TResult VisitYieldStmt(BoundYieldStmt x) => VisitDefault(x);
        public virtual TResult VisitForEachStmt(BoundForEachStmt x) => VisitDefault(x);
        public virtual TResult VisitBadStmt(BoundBadStmt x) => VisitDefault(x);
        public virtual TResult VisitHtmlMarkupStmt(BoundHtmlMarkupStmt x) => VisitDefault(x);
        public virtual TResult VisitHtmlOpenElementStmt(BoundHtmlOpenElementStmt x) => VisitDefault(x);
        public virtual TResult VisitHtmlCloseElementStmt(BoundHtmlCloseElementStmt x) => VisitDefault(x);
        public virtual TResult VisitHtmlAddAttributeStmt(BoundHtmlAddAttributeStmt x) => VisitDefault(x);
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
        public virtual TResult VisitWildcardEx(BoundWildcardEx x) => VisitDefault(x);
        public virtual TResult VisitMatchEx(BoundMatchEx x) => VisitDefault(x);
        public virtual TResult VisitMatchArm(BoundMatchArm x) => VisitDefault(x);
        public virtual TResult VisitBadEx(BoundBadEx x) => VisitDefault(x);
        public virtual TResult VisitFuncEx(BoundFuncEx x) => VisitDefault(x);
        public virtual TResult VisitCallEx(BoundCallEx x) => VisitDefault(x);
        public virtual TResult VisitNewEx(BoundNewEx x) => VisitDefault(x);
        public virtual TResult VisitThrowEx(BoundThrowEx x) => VisitDefault(x);
        public virtual TResult VisitAllocEx(BoundAllocEx x) => VisitDefault(x);
        public virtual TResult VisitAllocExAssign(BoundAllocExAssign x) => VisitDefault(x);
        public virtual TResult VisitGroupedEx(BoundGroupedEx x) => VisitDefault(x);
        public virtual TResult VisitReferenceEx(BoundReferenceEx x) => VisitDefault(x);
        public virtual TResult VisitArrayItemEx(BoundArrayItemEx x) => VisitDefault(x);
        public virtual TResult VisitArrayItemOrdEx(BoundArrayItemOrdEx x) => VisitDefault(x);
        public virtual TResult VisitFieldRef(BoundFieldRef x) => VisitDefault(x);
        public virtual TResult VisitListEx(BoundListEx x) => VisitDefault(x);
        public virtual TResult VisitVariableRef(BoundVariableRef x) => VisitDefault(x);
        public virtual TResult VisitPropertyRef(BoundPropertyRef x) => VisitDefault(x);
        public virtual TResult VisitArgument(BoundArgument x) => VisitDefault(x);
        public virtual TResult VisitMethodName(BoundMethodName x) => VisitDefault(x);
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
