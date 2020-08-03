using System;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Syntax;
using Aquila.Syntax.Text;

namespace Aquila.Language.Ast.Definitions
{
    public class MethodList : SyntaxCollectionNode<MethodDecl>
    {
        public static MethodList Empty => new MethodList();

        public MethodList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMethodList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMethodList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class FieldList : SyntaxCollectionNode<FieldDecl>
    {
        public static FieldList Empty => new FieldList();

        public FieldList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFieldList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFieldList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class TypeList : SyntaxCollectionNode<TypeRef>
    {
        public static TypeList Empty => new TypeList();

        public TypeList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitTypeList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class StatementList : SyntaxCollectionNode<Statement>
    {
        public static StatementList Empty => new StatementList();

        public StatementList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitStatementList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitStatementList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class ParameterList : SyntaxCollectionNode<Parameter>
    {
        public static ParameterList Empty => new ParameterList();

        public ParameterList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitParameterList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitParameterList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class AnnotationList : SyntaxCollectionNode<Annotation>
    {
        public static AnnotationList Empty => new AnnotationList();

        public AnnotationList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAttributeList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAttributeList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class ArgumentList : SyntaxCollectionNode<Argument>
    {
        public static ArgumentList Empty => new ArgumentList();

        public ArgumentList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArgumentList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitArgumentList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class DeclaratorList : SyntaxCollectionNode<VarDeclarator>
    {
        public static DeclaratorList Empty => new DeclaratorList();

        public DeclaratorList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitDeclaratorList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitDeclaratorList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class UsingList : SyntaxCollectionNode<UsingBase>
    {
        public static UsingList Empty => new UsingList();

        public UsingList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class CompilationUnitList : SyntaxCollectionNode<CompilationUnit>
    {
        public static CompilationUnitList Empty => new CompilationUnitList();

        public CompilationUnitList() : base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCompilationUnitList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCompilationUnitList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(Span span, SyntaxKind syntaxKind, UsingList usings, MethodList methods, FieldList fields)
            : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) usings);
            this.Attach(1, (SyntaxNode) methods);
            this.Attach(2, (SyntaxNode) fields);
        }

        public UsingList Usings
        {
            get { return (UsingList) this.Children[0]; }
        }

        public MethodList Methods
        {
            get { return (MethodList) this.Children[1]; }
        }

        public FieldList Fields
        {
            get { return (FieldList) this.Children[2]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCompilationUnit(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCompilationUnit(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class IdentifierToken : SyntaxToken
    {
        public IdentifierToken(Span span, SyntaxKind syntaxKind, String text) : base(span, syntaxKind, text)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIdentifierToken(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIdentifierToken(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class IdentifierName : Expression
    {
        public IdentifierName(Span span, SyntaxKind syntaxKind, IdentifierToken identifier) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) identifier);
        }

        public IdentifierToken Identifier
        {
            get { return (IdentifierToken) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIdentifierName(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIdentifierName(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public abstract partial class UsingBase : SyntaxNode
    {
        public UsingBase(Span span, SyntaxKind syntaxKind, String name) : base(span, syntaxKind)
        {
            Name = name;
        }

        public String Name { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class UsingDeclaration : UsingBase
    {
        public UsingDeclaration(Span span, SyntaxKind syntaxKind, String name) : base(span, syntaxKind, name)
        {
            Name = name;
        }

        public String Name { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingDeclaration(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingDeclaration(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class UsingAliasDeclaration : UsingBase
    {
        public UsingAliasDeclaration(Span span, SyntaxKind syntaxKind, String className, String alias) : base(span,
            syntaxKind, className)
        {
            ClassName = className;
            Alias = alias;
        }

        public String ClassName { get; }

        public String Alias { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingAliasDeclaration(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingAliasDeclaration(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public abstract partial class Expression : SyntaxNode
    {
        public Expression(Span span, SyntaxKind syntaxKind) : base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public abstract partial class Statement : SyntaxNode
    {
        public Statement(Span span, SyntaxKind syntaxKind) : base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class BinaryEx : Expression
    {
        public BinaryEx(Span span, SyntaxKind syntaxKind, Expression right, Expression left,
            BinaryOperatorType binaryOperatorType) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) right);
            this.Attach(1, (SyntaxNode) left);
            BinaryOperatorType = binaryOperatorType;
        }

        public Expression Right
        {
            get { return (Expression) this.Children[0]; }
        }

        public Expression Left
        {
            get { return (Expression) this.Children[1]; }
        }

        public BinaryOperatorType BinaryOperatorType { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBinaryEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBinaryEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public abstract partial class UnaryEx : Expression
    {
        public UnaryEx(Span span, SyntaxKind syntaxKind, Expression expression, UnaryOperatorType operaotrType) : base(
            span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) expression);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public UnaryOperatorType OperaotrType { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class CastEx : UnaryEx
    {
        public CastEx(Span span, SyntaxKind syntaxKind, Expression expression, TypeRef castType,
            UnaryOperatorType operaotrType) : base(span, syntaxKind, expression, operaotrType)
        {
            this.Attach(0, (SyntaxNode) expression);
            this.Attach(1, (SyntaxNode) castType);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public TypeRef CastType
        {
            get { return (TypeRef) this.Children[1]; }
        }

        public UnaryOperatorType OperaotrType { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCastEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCastEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class IndexerEx : UnaryEx
    {
        public IndexerEx(Span span, SyntaxKind syntaxKind, Expression indexer, Expression expression,
            UnaryOperatorType operaotrType) : base(span, syntaxKind, expression, operaotrType)
        {
            this.Attach(0, (SyntaxNode) indexer);
            this.Attach(1, (SyntaxNode) expression);
            OperaotrType = operaotrType;
        }

        public Expression Indexer
        {
            get { return (Expression) this.Children[0]; }
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[1]; }
        }

        public UnaryOperatorType OperaotrType { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIndexerEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIndexerEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class LogicalOrArithmeticEx : UnaryEx
    {
        public LogicalOrArithmeticEx(Span span, SyntaxKind syntaxKind, Expression expression,
            UnaryOperatorType operaotrType) : base(span, syntaxKind, expression, operaotrType)
        {
            this.Attach(0, (SyntaxNode) expression);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public UnaryOperatorType OperaotrType { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLogicalOrArithmeticEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitLogicalOrArithmeticEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class Assignment : Expression
    {
        public Assignment(Span span, SyntaxKind syntaxKind, Expression value, Expression assignable) : base(span,
            syntaxKind)
        {
            this.Attach(0, (SyntaxNode) value);
            this.Attach(1, (SyntaxNode) assignable);
        }

        public Expression Value
        {
            get { return (Expression) this.Children[0]; }
        }

        public Expression Assignable
        {
            get { return (Expression) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAssignment(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public abstract partial class TypeRef : Expression
    {
        public TypeRef(Span span, SyntaxKind syntaxKind) : base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class NamedTypeRef : TypeRef
    {
        public NamedTypeRef(Span span, SyntaxKind syntaxKind, string value = "?") : base(span, syntaxKind)
        {
            Value = value;
        }

        public string Value { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNamedTypeRef(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNamedTypeRef(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class ArrayType : TypeRef
    {
        public ArrayType(Span span, SyntaxKind syntaxKind, TypeRef type) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) type);
        }

        public TypeRef Type
        {
            get { return (TypeRef) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitArrayType(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class BlockStmt : Statement
    {
        public BlockStmt(Span span, SyntaxKind syntaxKind, StatementList statements) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) statements);
        }

        public StatementList Statements
        {
            get { return (StatementList) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBlockStmt(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
{
    public partial class Parameter : SyntaxNode
    {
        public Parameter(Span span, SyntaxKind syntaxKind, TypeRef type, IdentifierToken identifier,
            PassMethod passMethod = PassMethod.ByValue) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) type);
            this.Attach(1, (SyntaxNode) identifier);
            PassMethod = passMethod;
        }

        public TypeRef Type
        {
            get { return (TypeRef) this.Children[0]; }
        }

        public IdentifierToken Identifier
        {
            get { return (IdentifierToken) this.Children[1]; }
        }

        public PassMethod PassMethod { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitParameter(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitParameter(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
{
    public partial class GenericParameter : SyntaxNode
    {
        public GenericParameter(Span span, SyntaxKind syntaxKind, String name) : base(span, syntaxKind)
        {
            Name = name;
        }

        public String Name { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGenericParameter(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitGenericParameter(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class Annotation : SyntaxNode
    {
        public Annotation(Span span, SyntaxKind syntaxKind, ArgumentList arguments, IdentifierToken identifier) : base(
            span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) arguments);
            this.Attach(1, (SyntaxNode) identifier);
        }

        public ArgumentList Arguments
        {
            get { return (ArgumentList) this.Children[0]; }
        }

        public IdentifierToken Identifier
        {
            get { return (IdentifierToken) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAttribute(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAttribute(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public abstract partial class Member : SyntaxNode
    {
        public Member(Span span, SyntaxKind syntaxKind) : base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
{
    public partial class MethodDecl : Member
    {
        public MethodDecl(Span span, SyntaxKind syntaxKind, BlockStmt block, ParameterList parameters,
            AnnotationList annotations, IdentifierToken identifier, TypeRef type) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) block);
            this.Attach(1, (SyntaxNode) parameters);
            this.Attach(2, (SyntaxNode) annotations);
            this.Attach(3, (SyntaxNode) identifier);
            this.Attach(4, (SyntaxNode) type);
        }

        public BlockStmt Block
        {
            get { return (BlockStmt) this.Children[0]; }
        }

        public ParameterList Parameters
        {
            get { return (ParameterList) this.Children[1]; }
        }

        public AnnotationList Annotations
        {
            get { return (AnnotationList) this.Children[2]; }
        }

        public IdentifierToken Identifier
        {
            get { return (IdentifierToken) this.Children[3]; }
        }

        public TypeRef Type
        {
            get { return (TypeRef) this.Children[4]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMethodDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMethodDecl(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class FieldDecl : Member
    {
        public FieldDecl(Span span, SyntaxKind syntaxKind, IdentifierToken identifier, TypeRef type) : base(span,
            syntaxKind)
        {
            this.Attach(0, (SyntaxNode) identifier);
            this.Attach(1, (SyntaxNode) type);
        }

        public IdentifierToken Identifier
        {
            get { return (IdentifierToken) this.Children[0]; }
        }

        public TypeRef Type
        {
            get { return (TypeRef) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFieldDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFieldDecl(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
{
    public partial class Argument : SyntaxNode
    {
        public Argument(Span span, SyntaxKind syntaxKind, Expression expression,
            PassMethod passMethod = PassMethod.ByValue) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) expression);
            PassMethod = passMethod;
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public PassMethod PassMethod { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArgument(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitArgument(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class Call : Expression
    {
        public Call(Span span, SyntaxKind syntaxKind, ArgumentList arguments, Expression expression) : base(span,
            syntaxKind)
        {
            this.Attach(0, (SyntaxNode) arguments);
            this.Attach(1, (SyntaxNode) expression);
        }

        public ArgumentList Arguments
        {
            get { return (ArgumentList) this.Children[0]; }
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCall(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCall(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class New : Expression
    {
        public New(Span span, SyntaxKind syntaxKind, string @namespace, Call call) : base(span, syntaxKind)
        {
            Namespace = @namespace;
            this.Attach(0, (SyntaxNode) call);
        }

        public string Namespace { get; }

        public Call Call
        {
            get { return (Call) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNew(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNew(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class Return : Statement
    {
        public Return(Span span, SyntaxKind syntaxKind, Expression expression) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) expression);
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitReturn(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitReturn(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class Break : Statement
    {
        public Break(Span span, SyntaxKind syntaxKind) : base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBreak(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBreak(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class Continue : Statement
    {
        public Continue(Span span, SyntaxKind syntaxKind) : base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitContinue(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitContinue(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class VarDeclaration : Statement
    {
        public VarDeclaration(Span span, SyntaxKind syntaxKind, TypeRef variableType, DeclaratorList declarators) :
            base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) variableType);
            this.Attach(1, (SyntaxNode) declarators);
        }

        public TypeRef VariableType
        {
            get { return (TypeRef) this.Children[0]; }
        }

        public DeclaratorList Declarators
        {
            get { return (DeclaratorList) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitVarDeclaration(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitVarDeclaration(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class VarDeclarator : Statement
    {
        public VarDeclarator(Span span, SyntaxKind syntaxKind, Expression initializer, IdentifierToken identifier) :
            base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) initializer);
            this.Attach(1, (SyntaxNode) identifier);
        }

        public Expression Initializer
        {
            get { return (Expression) this.Children[0]; }
        }

        public IdentifierToken Identifier
        {
            get { return (IdentifierToken) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitVarDeclarator(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitVarDeclarator(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class ContextVariable : Expression
    {
        public ContextVariable(Span span, SyntaxKind syntaxKind, String name, TypeRef type) : base(span, syntaxKind)
        {
            Name = name;
            this.Attach(0, (SyntaxNode) type);
        }

        public String Name { get; }

        public TypeRef Type
        {
            get { return (TypeRef) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitContextVariable(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitContextVariable(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class Literal : Expression
    {
        public Literal(Span span, SyntaxKind syntaxKind, String value, bool isSqlLiteral) : base(span, syntaxKind)
        {
            Value = value;
            IsSqlLiteral = isSqlLiteral;
        }

        public String Value { get; }

        public bool IsSqlLiteral { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLiteral(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitLiteral(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class GetFieldEx : Expression
    {
        public GetFieldEx(Span span, SyntaxKind syntaxKind, Expression expression, String fieldName) : base(span,
            syntaxKind)
        {
            this.Attach(0, (SyntaxNode) expression);
            FieldName = fieldName;
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public String FieldName { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGetFieldEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitGetFieldEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public abstract partial class LookupEx : Expression
    {
        public LookupEx(Span span, SyntaxKind syntaxKind, Expression lookup, Expression current) : base(span,
            syntaxKind)
        {
            this.Attach(0, (SyntaxNode) lookup);
            this.Attach(1, (SyntaxNode) current);
        }

        public Expression Lookup
        {
            get { return (Expression) this.Children[0]; }
        }

        public Expression Current
        {
            get { return (Expression) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class PropertyLookupEx : LookupEx
    {
        public PropertyLookupEx(Span span, SyntaxKind syntaxKind, Expression lookup, Expression current) : base(span,
            syntaxKind, lookup, current)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPropertyLookupEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPropertyLookupEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class MethodLookupEx : LookupEx
    {
        public MethodLookupEx(Span span, SyntaxKind syntaxKind, Expression lookup, Expression current) : base(span,
            syntaxKind, lookup, current)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMethodLookupEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMethodLookupEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class AssignFieldEx : Expression
    {
        public AssignFieldEx(Span span, SyntaxKind syntaxKind, Expression expression, String fieldName) : base(span,
            syntaxKind)
        {
            this.Attach(0, (SyntaxNode) expression);
            FieldName = fieldName;
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public String FieldName { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAssignFieldEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAssignFieldEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class DoWhileStmt : Statement
    {
        public DoWhileStmt(Span span, SyntaxKind syntaxKind, Expression condition, BlockStmt block) : base(span,
            syntaxKind)
        {
            this.Attach(0, (SyntaxNode) condition);
            this.Attach(1, (SyntaxNode) block);
        }

        public Expression Condition
        {
            get { return (Expression) this.Children[0]; }
        }

        public BlockStmt Block
        {
            get { return (BlockStmt) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitDoWhileStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitDoWhileStmt(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class TryStmt : Statement
    {
        public TryStmt(Span span, SyntaxKind syntaxKind, BlockStmt tryBlock, BlockStmt catchBlock,
            BlockStmt finallyBlock) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) tryBlock);
            this.Attach(1, (SyntaxNode) catchBlock);
            this.Attach(2, (SyntaxNode) finallyBlock);
        }

        public BlockStmt TryBlock
        {
            get { return (BlockStmt) this.Children[0]; }
        }

        public BlockStmt CatchBlock
        {
            get { return (BlockStmt) this.Children[1]; }
        }

        public BlockStmt FinallyBlock
        {
            get { return (BlockStmt) this.Children[2]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTryStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitTryStmt(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class ExpressionStmt : Statement
    {
        public ExpressionStmt(Span span, SyntaxKind syntaxKind, Expression expression) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) expression);
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitExpressionStmt(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class For : Statement
    {
        public For(Span span, SyntaxKind syntaxKind, BlockStmt block, Expression counter, Expression condition,
            Expression initializer) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) block);
            this.Attach(1, (SyntaxNode) counter);
            this.Attach(2, (SyntaxNode) condition);
            this.Attach(3, (SyntaxNode) initializer);
        }

        public BlockStmt Block
        {
            get { return (BlockStmt) this.Children[0]; }
        }

        public Expression Counter
        {
            get { return (Expression) this.Children[1]; }
        }

        public Expression Condition
        {
            get { return (Expression) this.Children[2]; }
        }

        public Expression Initializer
        {
            get { return (Expression) this.Children[3]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFor(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFor(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class While : Statement
    {
        public While(Span span, SyntaxKind syntaxKind, BlockStmt block, Expression condition) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) block);
            this.Attach(1, (SyntaxNode) condition);
        }

        public BlockStmt Block
        {
            get { return (BlockStmt) this.Children[0]; }
        }

        public Expression Condition
        {
            get { return (Expression) this.Children[1]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitWhile(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitWhile(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class If : Statement
    {
        public If(Span span, SyntaxKind syntaxKind, Statement elseBlock, Statement thenBlock, Expression condition) :
            base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) elseBlock);
            this.Attach(1, (SyntaxNode) thenBlock);
            this.Attach(2, (SyntaxNode) condition);
        }

        public Statement ElseBlock
        {
            get { return (Statement) this.Children[0]; }
        }

        public Statement ThenBlock
        {
            get { return (Statement) this.Children[1]; }
        }

        public Expression Condition
        {
            get { return (Expression) this.Children[2]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIf(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIf(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public abstract partial class PostOperationEx : Expression
    {
        public PostOperationEx(Span span, SyntaxKind syntaxKind, Expression expression) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) expression);
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class PostIncrementEx : PostOperationEx
    {
        public PostIncrementEx(Span span, SyntaxKind syntaxKind, Expression expression) : base(span, syntaxKind,
            expression)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostIncrementEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPostIncrementEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class PostDecrementEx : PostOperationEx
    {
        public PostDecrementEx(Span span, SyntaxKind syntaxKind, Expression expression) : base(span, syntaxKind,
            expression)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostDecrementEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPostDecrementEx(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class Throw : Expression
    {
        public Throw(Span span, SyntaxKind syntaxKind, Expression exception) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) exception);
        }

        public Expression Exception
        {
            get { return (Expression) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitThrow(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitThrow(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class GlobalVar : Expression
    {
        public GlobalVar(Span span, SyntaxKind syntaxKind, Expression expression) : base(span, syntaxKind)
        {
            this.Attach(0, (SyntaxNode) expression);
        }

        public Expression Expression
        {
            get { return (Expression) this.Children[0]; }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGlobalVar(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitGlobalVar(this);
        }
    }
}

namespace Aquila.Syntax
{
    public abstract partial class AstVisitorBase<T>
    {
        public virtual T VisitMethodList(MethodList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitFieldList(FieldList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitTypeList(TypeList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitStatementList(StatementList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitParameterList(ParameterList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAttributeList(AnnotationList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArgumentList(ArgumentList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitDeclaratorList(DeclaratorList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitUsingList(UsingList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCompilationUnitList(CompilationUnitList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCompilationUnit(CompilationUnit arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIdentifierToken(IdentifierToken arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIdentifierName(IdentifierName arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitUsingDeclaration(UsingDeclaration arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitUsingAliasDeclaration(UsingAliasDeclaration arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBinaryEx(BinaryEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCastEx(CastEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIndexerEx(IndexerEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitLogicalOrArithmeticEx(LogicalOrArithmeticEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAssignment(Assignment arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNamedTypeRef(NamedTypeRef arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArrayType(ArrayType arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBlockStmt(BlockStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitParameter(Parameter arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitGenericParameter(GenericParameter arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAttribute(Annotation arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMethodDecl(MethodDecl arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitFieldDecl(FieldDecl arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArgument(Argument arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCall(Call arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNew(New arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitReturn(Return arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBreak(Break arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitContinue(Continue arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitVarDeclaration(VarDeclaration arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitVarDeclarator(VarDeclarator arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitContextVariable(ContextVariable arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitLiteral(Literal arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitGetFieldEx(GetFieldEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPropertyLookupEx(PropertyLookupEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMethodLookupEx(MethodLookupEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAssignFieldEx(AssignFieldEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitDoWhileStmt(DoWhileStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitTryStmt(TryStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitExpressionStmt(ExpressionStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitFor(For arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitWhile(While arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIf(If arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPostIncrementEx(PostIncrementEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPostDecrementEx(PostDecrementEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitThrow(Throw arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitGlobalVar(GlobalVar arg)
        {
            return DefaultVisit(arg);
        }
    }

    public abstract partial class AstVisitorBase
    {
        public virtual void VisitMethodList(MethodList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitFieldList(FieldList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitTypeList(TypeList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitStatementList(StatementList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitParameterList(ParameterList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAttributeList(AnnotationList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArgumentList(ArgumentList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitDeclaratorList(DeclaratorList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitUsingList(UsingList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCompilationUnitList(CompilationUnitList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCompilationUnit(CompilationUnit arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitIdentifierToken(IdentifierToken arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitIdentifierName(IdentifierName arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitUsingDeclaration(UsingDeclaration arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitUsingAliasDeclaration(UsingAliasDeclaration arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBinaryEx(BinaryEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCastEx(CastEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitIndexerEx(IndexerEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitLogicalOrArithmeticEx(LogicalOrArithmeticEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAssignment(Assignment arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNamedTypeRef(NamedTypeRef arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArrayType(ArrayType arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBlockStmt(BlockStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitParameter(Parameter arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitGenericParameter(GenericParameter arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAttribute(Annotation arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMethodDecl(MethodDecl arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitFieldDecl(FieldDecl arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArgument(Argument arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCall(Call arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNew(New arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitReturn(Return arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBreak(Break arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitContinue(Continue arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitVarDeclaration(VarDeclaration arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitVarDeclarator(VarDeclarator arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitContextVariable(ContextVariable arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitLiteral(Literal arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitGetFieldEx(GetFieldEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPropertyLookupEx(PropertyLookupEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMethodLookupEx(MethodLookupEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAssignFieldEx(AssignFieldEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitDoWhileStmt(DoWhileStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitTryStmt(TryStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitExpressionStmt(ExpressionStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitFor(For arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitWhile(While arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitIf(If arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPostIncrementEx(PostIncrementEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPostDecrementEx(PostDecrementEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitThrow(Throw arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitGlobalVar(GlobalVar arg)
        {
            DefaultVisit(arg);
        }
    }
}