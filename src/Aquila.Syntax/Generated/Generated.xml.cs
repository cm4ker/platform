using System;
using System.Collections;
using System.Collections.Generic;
using Aquila.Syntax.Text;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Ast.Functions;

namespace Aquila.Syntax.Ast
{
    public class MethodList : SyntaxCollectionNode<MethodDecl>
    {
        public static MethodList Empty => new MethodList();
        public MethodList(): base(Span.Empty)
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

namespace Aquila.Syntax.Ast
{
    public class FieldList : SyntaxCollectionNode<FieldDecl>
    {
        public static FieldList Empty => new FieldList();
        public FieldList(): base(Span.Empty)
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

namespace Aquila.Syntax.Ast
{
    public class TypeList : SyntaxCollectionNode<TypeRef>
    {
        public static TypeList Empty => new TypeList();
        public TypeList(): base(Span.Empty)
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

namespace Aquila.Syntax.Ast
{
    public class StatementList : SyntaxCollectionNode<Statement>
    {
        public static StatementList Empty => new StatementList();
        public StatementList(): base(Span.Empty)
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

namespace Aquila.Syntax.Ast
{
    public class ParameterList : SyntaxCollectionNode<Parameter>
    {
        public static ParameterList Empty => new ParameterList();
        public ParameterList(): base(Span.Empty)
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

namespace Aquila.Syntax.Ast
{
    public class AnnotationList : SyntaxCollectionNode<Annotation>
    {
        public static AnnotationList Empty => new AnnotationList();
        public AnnotationList(): base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAnnotationList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAnnotationList(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public class ArgumentList : SyntaxCollectionNode<Argument>
    {
        public static ArgumentList Empty => new ArgumentList();
        public ArgumentList(): base(Span.Empty)
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

namespace Aquila.Syntax.Ast
{
    public class DeclaratorList : SyntaxCollectionNode<VarDeclarator>
    {
        public static DeclaratorList Empty => new DeclaratorList();
        public DeclaratorList(): base(Span.Empty)
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

namespace Aquila.Syntax.Ast
{
    public class UsingList : SyntaxCollectionNode<UsingBase>
    {
        public static UsingList Empty => new UsingList();
        public UsingList(): base(Span.Empty)
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

namespace Aquila.Syntax.Ast
{
    public class CatchList : SyntaxCollectionNode<CatchItem>
    {
        public static CatchList Empty => new CatchList();
        public CatchList(): base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCatchList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCatchList(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public class SourceUnitList : SyntaxCollectionNode<SourceUnit>
    {
        public static SourceUnitList Empty => new SourceUnitList();
        public SourceUnitList(): base(Span.Empty)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitSourceUnitList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitSourceUnitList(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial class SourceUnit : LangElement
    {
        public SourceUnit(Span span, SyntaxKind syntaxKind,  string  filePath, UsingList usings, MethodList methods, FieldList fields): base(span, syntaxKind)
        {
            FilePath = filePath;
            this.Attach(0, (LangElement)usings);
            this.Attach(1, (LangElement)methods);
            this.Attach(2, (LangElement)fields);
        }

        public string FilePath
        {
            get;
        }

        public UsingList Usings
        {
            get
            {
                return (UsingList)this.Children[0];
            }
        }

        public MethodList Methods
        {
            get
            {
                return (MethodList)this.Children[1];
            }
        }

        public FieldList Fields
        {
            get
            {
                return (FieldList)this.Children[2];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitSourceUnit(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitSourceUnit(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public abstract partial class UsingBase : LangElement
    {
        public UsingBase(Span span, SyntaxKind syntaxKind, String name): base(span, syntaxKind)
        {
            Name = name;
        }

        public String Name
        {
            get;
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

namespace Aquila.Syntax.Ast
{
    public partial class UsingDecl : UsingBase
    {
        public UsingDecl(Span span, SyntaxKind syntaxKind, String name): base(span, syntaxKind, name)
        {
            Name = name;
        }

        public String Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingDecl(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial class UsingAliasDecl : UsingBase
    {
        public UsingAliasDecl(Span span, SyntaxKind syntaxKind, String className, String alias): base(span, syntaxKind, className)
        {
            ClassName = className;
            Alias = alias;
        }

        public String ClassName
        {
            get;
        }

        public String Alias
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingAliasDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingAliasDecl(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial class Annotation : LangElement
    {
        public Annotation(Span span, SyntaxKind syntaxKind, ArgumentList arguments, IdentifierToken identifier): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)arguments);
            this.Attach(1, (LangElement)identifier);
        }

        public ArgumentList Arguments
        {
            get
            {
                return (ArgumentList)this.Children[0];
            }
        }

        public IdentifierToken Identifier
        {
            get
            {
                return (IdentifierToken)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAnnotation(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAnnotation(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public abstract partial class Member : LangElement
    {
        public Member(Span span, SyntaxKind syntaxKind): base(span, syntaxKind)
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

namespace Aquila.Syntax.Ast.Functions
{
    public partial class Parameter : LangElement
    {
        public Parameter(Span span, SyntaxKind syntaxKind, TypeRef type, IdentifierToken identifier, PassMethod passMethod = PassMethod.ByValue): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)type);
            this.Attach(1, (LangElement)identifier);
            PassMethod = passMethod;
        }

        public TypeRef Type
        {
            get
            {
                return (TypeRef)this.Children[0];
            }
        }

        public IdentifierToken Identifier
        {
            get
            {
                return (IdentifierToken)this.Children[1];
            }
        }

        public PassMethod PassMethod
        {
            get;
        }

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

namespace Aquila.Syntax.Ast.Functions
{
    public partial class MethodDecl : Member
    {
        public MethodDecl(Span span, SyntaxKind syntaxKind, BlockStmt block, ParameterList parameters, AnnotationList annotations, IdentifierToken identifier, TypeRef returnType): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)block);
            this.Attach(1, (LangElement)parameters);
            this.Attach(2, (LangElement)annotations);
            this.Attach(3, (LangElement)identifier);
            this.Attach(4, (LangElement)returnType);
        }

        public BlockStmt Block
        {
            get
            {
                return (BlockStmt)this.Children[0];
            }
        }

        public ParameterList Parameters
        {
            get
            {
                return (ParameterList)this.Children[1];
            }
        }

        public AnnotationList Annotations
        {
            get
            {
                return (AnnotationList)this.Children[2];
            }
        }

        public IdentifierToken Identifier
        {
            get
            {
                return (IdentifierToken)this.Children[3];
            }
        }

        public TypeRef ReturnType
        {
            get
            {
                return (TypeRef)this.Children[4];
            }
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

namespace Aquila.Syntax.Ast
{
    public partial class FieldDecl : Member
    {
        public FieldDecl(Span span, SyntaxKind syntaxKind, IdentifierToken identifier, TypeRef type): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)identifier);
            this.Attach(1, (LangElement)type);
        }

        public IdentifierToken Identifier
        {
            get
            {
                return (IdentifierToken)this.Children[0];
            }
        }

        public TypeRef Type
        {
            get
            {
                return (TypeRef)this.Children[1];
            }
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

namespace Aquila.Syntax.Ast
{
    public partial class IdentifierToken : SyntaxToken
    {
        public IdentifierToken(Span span, SyntaxKind syntaxKind, String text): base(span, syntaxKind, text)
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

namespace Aquila.Syntax.Ast
{
    public abstract partial class TypeRef : LangElement
    {
        public TypeRef(Span span, SyntaxKind syntaxKind): base(span, syntaxKind)
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

namespace Aquila.Syntax.Ast
{
    public partial class NamedTypeRef : TypeRef
    {
        public NamedTypeRef(Span span, SyntaxKind syntaxKind,  string  value = "?"): base(span, syntaxKind)
        {
            Value = value;
        }

        public string Value
        {
            get;
        }

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

namespace Aquila.Syntax.Ast
{
    public partial class PredefinedTypeRef : TypeRef
    {
        public PredefinedTypeRef(Span span, SyntaxKind syntaxKind): base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPredefinedTypeRef(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPredefinedTypeRef(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial class ArrayType : TypeRef
    {
        public ArrayType(Span span, SyntaxKind syntaxKind, TypeRef type): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)type);
        }

        public TypeRef Type
        {
            get
            {
                return (TypeRef)this.Children[0];
            }
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

namespace Aquila.Syntax.Ast
{
    public abstract partial class Expression : LangElement
    {
        public Expression(Span span, SyntaxKind syntaxKind, Operations operation): base(span, syntaxKind)
        {
            Operation = operation;
        }

        public Operations Operation
        {
            get;
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

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class BinaryEx : Expression
    {
        public BinaryEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression right, Expression left): base(span, syntaxKind, operation)
        {
            this.Attach(0, (LangElement)right);
            this.Attach(1, (LangElement)left);
        }

        public Expression Right
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public Expression Left
        {
            get
            {
                return (Expression)this.Children[1];
            }
        }

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

namespace Aquila.Syntax.Ast.Expressions
{
    public abstract partial class UnaryExBase : Expression
    {
        public UnaryExBase(Span span, SyntaxKind syntaxKind, Operations operation, Expression expression): base(span, syntaxKind, operation)
        {
            this.Attach(0, (LangElement)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[0];
            }
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

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class CastEx : UnaryExBase
    {
        public CastEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression expression, TypeRef castType): base(span, syntaxKind, operation, expression)
        {
            this.Attach(0, (LangElement)expression);
            this.Attach(1, (LangElement)castType);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public TypeRef CastType
        {
            get
            {
                return (TypeRef)this.Children[1];
            }
        }

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

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class IndexerEx : UnaryExBase
    {
        public IndexerEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression indexer, Expression expression): base(span, syntaxKind, operation, expression)
        {
            this.Attach(0, (LangElement)indexer);
            this.Attach(1, (LangElement)expression);
        }

        public Expression Indexer
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[1];
            }
        }

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

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class UnaryEx : UnaryExBase
    {
        public UnaryEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression expression): base(span, syntaxKind, operation, expression)
        {
            this.Attach(0, (LangElement)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUnaryEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUnaryEx(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class AssignEx : Expression
    {
        public AssignEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression rValue, Expression lValue): base(span, syntaxKind, operation)
        {
            this.Attach(0, (LangElement)rValue);
            this.Attach(1, (LangElement)lValue);
        }

        public Expression RValue
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public Expression LValue
        {
            get
            {
                return (Expression)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAssignEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAssignEx(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class CallEx : Expression
    {
        public CallEx(Span span, SyntaxKind syntaxKind, Operations operation, ArgumentList arguments, Expression expression): base(span, syntaxKind, operation)
        {
            this.Attach(0, (LangElement)arguments);
            this.Attach(1, (LangElement)expression);
        }

        public ArgumentList Arguments
        {
            get
            {
                return (ArgumentList)this.Children[0];
            }
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCallEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCallEx(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Functions
{
    public partial class Argument : LangElement
    {
        public Argument(Span span, SyntaxKind syntaxKind, Expression expression, PassMethod passMethod = PassMethod.ByValue): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)expression);
            PassMethod = passMethod;
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public PassMethod PassMethod
        {
            get;
        }

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

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class New : Expression
    {
        public New(Span span, SyntaxKind syntaxKind, Operations operation,  string  @namespace, CallEx call): base(span, syntaxKind, operation)
        {
            Namespace = @namespace;
            this.Attach(0, (LangElement)call);
        }

        public string Namespace
        {
            get;
        }

        public CallEx Call
        {
            get
            {
                return (CallEx)this.Children[0];
            }
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

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class LiteralEx : Expression
    {
        public LiteralEx(Span span, SyntaxKind syntaxKind, Operations operation, String value,  bool  isSqlLiteral): base(span, syntaxKind, operation)
        {
            Value = value;
            IsSqlLiteral = isSqlLiteral;
        }

        public String Value
        {
            get;
        }

        public bool IsSqlLiteral
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLiteralEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitLiteralEx(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class IncDecEx : Expression
    {
        public IncDecEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression operand,  bool  isIncrement,  bool  isPost): base(span, syntaxKind, operation)
        {
            this.Attach(0, (LangElement)operand);
            IsIncrement = isIncrement;
            IsPost = isPost;
        }

        public Expression Operand
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public bool IsIncrement
        {
            get;
        }

        public bool IsPost
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIncDecEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIncDecEx(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class ThrowEx : Expression
    {
        public ThrowEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression expression): base(span, syntaxKind, operation)
        {
            this.Attach(0, (LangElement)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitThrowEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitThrowEx(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial class NameEx : Expression
    {
        public NameEx(Span span, SyntaxKind syntaxKind, Operations operation, IdentifierToken identifier): base(span, syntaxKind, operation)
        {
            this.Attach(0, (LangElement)identifier);
        }

        public IdentifierToken Identifier
        {
            get
            {
                return (IdentifierToken)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNameEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNameEx(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public abstract partial class Statement : LangElement
    {
        public Statement(Span span, SyntaxKind syntaxKind): base(span, syntaxKind)
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

namespace Aquila.Syntax.Ast
{
    public partial class BlockStmt : Statement
    {
        public BlockStmt(Span span, SyntaxKind syntaxKind, StatementList statements): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)statements);
        }

        public StatementList Statements
        {
            get
            {
                return (StatementList)this.Children[0];
            }
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

namespace Aquila.Syntax.Ast.Statements
{
    public partial class ReturnStmt : Statement
    {
        public ReturnStmt(Span span, SyntaxKind syntaxKind, Expression expression): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitReturnStmt(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial class BreakStmt : Statement
    {
        public BreakStmt(Span span, SyntaxKind syntaxKind): base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBreakStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBreakStmt(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial class ContinueStmt : Statement
    {
        public ContinueStmt(Span span, SyntaxKind syntaxKind): base(span, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitContinueStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitContinueStmt(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial class VarDecl : Statement
    {
        public VarDecl(Span span, SyntaxKind syntaxKind, TypeRef variableType, DeclaratorList declarators): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)variableType);
            this.Attach(1, (LangElement)declarators);
        }

        public TypeRef VariableType
        {
            get
            {
                return (TypeRef)this.Children[0];
            }
        }

        public DeclaratorList Declarators
        {
            get
            {
                return (DeclaratorList)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitVarDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitVarDecl(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial class VarDeclarator : LangElement
    {
        public VarDeclarator(Span span, SyntaxKind syntaxKind, Expression initializer, IdentifierToken identifier): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)initializer);
            this.Attach(1, (LangElement)identifier);
        }

        public Expression Initializer
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public IdentifierToken Identifier
        {
            get
            {
                return (IdentifierToken)this.Children[1];
            }
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

namespace Aquila.Syntax.Ast.Statements
{
    public partial class DoWhileStmt : Statement
    {
        public DoWhileStmt(Span span, SyntaxKind syntaxKind, Expression condition, BlockStmt block): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)condition);
            this.Attach(1, (LangElement)block);
        }

        public Expression Condition
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public BlockStmt Block
        {
            get
            {
                return (BlockStmt)this.Children[1];
            }
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

namespace Aquila.Syntax.Ast.Statements
{
    public partial class TryStmt : Statement
    {
        public TryStmt(Span span, SyntaxKind syntaxKind, BlockStmt tryBlock, CatchList catches, FinallyItem finallyItem): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)tryBlock);
            this.Attach(1, (LangElement)catches);
            this.Attach(2, (LangElement)finallyItem);
        }

        public BlockStmt TryBlock
        {
            get
            {
                return (BlockStmt)this.Children[0];
            }
        }

        public CatchList Catches
        {
            get
            {
                return (CatchList)this.Children[1];
            }
        }

        public FinallyItem FinallyItem
        {
            get
            {
                return (FinallyItem)this.Children[2];
            }
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

namespace Aquila.Syntax.Ast.Statements
{
    public partial class CatchItem : LangElement
    {
        public CatchItem(Span span, SyntaxKind syntaxKind, BlockStmt block): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)block);
        }

        public BlockStmt Block
        {
            get
            {
                return (BlockStmt)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCatchItem(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCatchItem(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial class FinallyItem : LangElement
    {
        public FinallyItem(Span span, SyntaxKind syntaxKind, BlockStmt block): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)block);
        }

        public BlockStmt Block
        {
            get
            {
                return (BlockStmt)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFinallyItem(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFinallyItem(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial class ExpressionStmt : Statement
    {
        public ExpressionStmt(Span span, SyntaxKind syntaxKind, Expression expression): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[0];
            }
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

namespace Aquila.Syntax.Ast.Statements
{
    public partial class ForStmt : Statement
    {
        public ForStmt(Span span, SyntaxKind syntaxKind, BlockStmt block, Expression counter, Expression condition, Expression initializer): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)block);
            this.Attach(1, (LangElement)counter);
            this.Attach(2, (LangElement)condition);
            this.Attach(3, (LangElement)initializer);
        }

        public BlockStmt Block
        {
            get
            {
                return (BlockStmt)this.Children[0];
            }
        }

        public Expression Counter
        {
            get
            {
                return (Expression)this.Children[1];
            }
        }

        public Expression Condition
        {
            get
            {
                return (Expression)this.Children[2];
            }
        }

        public Expression Initializer
        {
            get
            {
                return (Expression)this.Children[3];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitForStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitForStmt(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial class WhileStmt : Statement
    {
        public WhileStmt(Span span, SyntaxKind syntaxKind, BlockStmt block, Expression condition): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)block);
            this.Attach(1, (LangElement)condition);
        }

        public BlockStmt Block
        {
            get
            {
                return (BlockStmt)this.Children[0];
            }
        }

        public Expression Condition
        {
            get
            {
                return (Expression)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitWhileStmt(this);
        }
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial class IfStmt : Statement
    {
        public IfStmt(Span span, SyntaxKind syntaxKind, Statement elseBlock, Statement thenBlock, Expression condition): base(span, syntaxKind)
        {
            this.Attach(0, (LangElement)elseBlock);
            this.Attach(1, (LangElement)thenBlock);
            this.Attach(2, (LangElement)condition);
        }

        public Statement ElseBlock
        {
            get
            {
                return (Statement)this.Children[0];
            }
        }

        public Statement ThenBlock
        {
            get
            {
                return (Statement)this.Children[1];
            }
        }

        public Expression Condition
        {
            get
            {
                return (Expression)this.Children[2];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIfStmt(this);
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

        public virtual T VisitAnnotationList(AnnotationList arg)
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

        public virtual T VisitCatchList(CatchList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitSourceUnitList(SourceUnitList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitSourceUnit(SourceUnit arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitUsingDecl(UsingDecl arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitUsingAliasDecl(UsingAliasDecl arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAnnotation(Annotation arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitParameter(Parameter arg)
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

        public virtual T VisitIdentifierToken(IdentifierToken arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNamedTypeRef(NamedTypeRef arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPredefinedTypeRef(PredefinedTypeRef arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArrayType(ArrayType arg)
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

        public virtual T VisitUnaryEx(UnaryEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAssignEx(AssignEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCallEx(CallEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArgument(Argument arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNew(New arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitLiteralEx(LiteralEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIncDecEx(IncDecEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitThrowEx(ThrowEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNameEx(NameEx arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBlockStmt(BlockStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitReturnStmt(ReturnStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBreakStmt(BreakStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitContinueStmt(ContinueStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitVarDecl(VarDecl arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitVarDeclarator(VarDeclarator arg)
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

        public virtual T VisitCatchItem(CatchItem arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitFinallyItem(FinallyItem arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitExpressionStmt(ExpressionStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitForStmt(ForStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitWhileStmt(WhileStmt arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIfStmt(IfStmt arg)
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

        public virtual void VisitAnnotationList(AnnotationList arg)
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

        public virtual void VisitCatchList(CatchList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitSourceUnitList(SourceUnitList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitSourceUnit(SourceUnit arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitUsingDecl(UsingDecl arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitUsingAliasDecl(UsingAliasDecl arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAnnotation(Annotation arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitParameter(Parameter arg)
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

        public virtual void VisitIdentifierToken(IdentifierToken arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNamedTypeRef(NamedTypeRef arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPredefinedTypeRef(PredefinedTypeRef arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArrayType(ArrayType arg)
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

        public virtual void VisitUnaryEx(UnaryEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAssignEx(AssignEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCallEx(CallEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArgument(Argument arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNew(New arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitLiteralEx(LiteralEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitIncDecEx(IncDecEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitThrowEx(ThrowEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNameEx(NameEx arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBlockStmt(BlockStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitReturnStmt(ReturnStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBreakStmt(BreakStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitContinueStmt(ContinueStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitVarDecl(VarDecl arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitVarDeclarator(VarDeclarator arg)
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

        public virtual void VisitCatchItem(CatchItem arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitFinallyItem(FinallyItem arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitExpressionStmt(ExpressionStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitForStmt(ForStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitWhileStmt(WhileStmt arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitIfStmt(IfStmt arg)
        {
            DefaultVisit(arg);
        }
    }
}
