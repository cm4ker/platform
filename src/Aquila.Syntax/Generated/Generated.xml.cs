using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.Syntax.Text;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Ast.Functions;

namespace Aquila.Syntax.Ast
{
    public record ComponentList : LangCollection<ComponentDecl>
    {
        public static ComponentList Empty => new ComponentList(ImmutableArray<ComponentDecl>.Empty);
        public ComponentList(ImmutableArray<ComponentDecl> elements): base(Span.Empty, elements)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitComponentList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitComponentList(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public record ExtendList : LangCollection<ExtendDecl>
    {
        public static ExtendList Empty => new ExtendList(ImmutableArray<ExtendDecl>.Empty);
        public ExtendList(ImmutableArray<ExtendDecl> elements): base(Span.Empty, elements)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitExtendList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitExtendList(this);
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public record MethodList : LangCollection<MethodDecl>
    {
        public static MethodList Empty => new MethodList(ImmutableArray<MethodDecl>.Empty);
        public MethodList(ImmutableArray<MethodDecl> elements): base(Span.Empty, elements)
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
    public record FieldList : LangCollection<FieldDecl>
    {
        public static FieldList Empty => new FieldList(ImmutableArray<FieldDecl>.Empty);
        public FieldList(ImmutableArray<FieldDecl> elements): base(Span.Empty, elements)
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
    public record TypeList : LangCollection<TypeRef>
    {
        public static TypeList Empty => new TypeList(ImmutableArray<TypeRef>.Empty);
        public TypeList(ImmutableArray<TypeRef> elements): base(Span.Empty, elements)
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
    public record StatementList : LangCollection<Statement>
    {
        public static StatementList Empty => new StatementList(ImmutableArray<Statement>.Empty);
        public StatementList(ImmutableArray<Statement> elements): base(Span.Empty, elements)
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
    public record ParameterList : LangCollection<Parameter>
    {
        public static ParameterList Empty => new ParameterList(ImmutableArray<Parameter>.Empty);
        public ParameterList(ImmutableArray<Parameter> elements): base(Span.Empty, elements)
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
    public record AnnotationList : LangCollection<Annotation>
    {
        public static AnnotationList Empty => new AnnotationList(ImmutableArray<Annotation>.Empty);
        public AnnotationList(ImmutableArray<Annotation> elements): base(Span.Empty, elements)
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
    public record ArgumentList : LangCollection<Argument>
    {
        public static ArgumentList Empty => new ArgumentList(ImmutableArray<Argument>.Empty);
        public ArgumentList(ImmutableArray<Argument> elements): base(Span.Empty, elements)
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
    public record DeclaratorList : LangCollection<VarDeclarator>
    {
        public static DeclaratorList Empty => new DeclaratorList(ImmutableArray<VarDeclarator>.Empty);
        public DeclaratorList(ImmutableArray<VarDeclarator> elements): base(Span.Empty, elements)
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
    public record UsingList : LangCollection<UsingBase>
    {
        public static UsingList Empty => new UsingList(ImmutableArray<UsingBase>.Empty);
        public UsingList(ImmutableArray<UsingBase> elements): base(Span.Empty, elements)
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
    public record CatchList : LangCollection<CatchItem>
    {
        public static CatchList Empty => new CatchList(ImmutableArray<CatchItem>.Empty);
        public CatchList(ImmutableArray<CatchItem> elements): base(Span.Empty, elements)
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
    public record SourceUnitList : LangCollection<SourceUnit>
    {
        public static SourceUnitList Empty => new SourceUnitList(ImmutableArray<SourceUnit>.Empty);
        public SourceUnitList(ImmutableArray<SourceUnit> elements): base(Span.Empty, elements)
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
    public partial record SourceUnit : LangElement
    {
        public SourceUnit(Span span, SyntaxKind syntaxKind,  string  filePath, UsingList usings, MethodList methods, ExtendList extends, ComponentList components): base(span, syntaxKind)
        {
            FilePath = filePath;
            this.usings = usings;
            this.methods = methods;
            this.extends = extends;
            this.components = components;
        }

        public string FilePath { get => this.filePath; init => this.filePath = value; }

        public UsingList Usings { get => this.usings; init => this.usings = value; }

        public MethodList Methods { get => this.methods; init => this.methods = value; }

        public ExtendList Extends { get => this.extends; init => this.extends = value; }

        public ComponentList Components { get => this.components; init => this.components = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitSourceUnit(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitSourceUnit(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.usings;
            yield return this.methods;
            yield return this.extends;
            yield return this.components;
            yield break;
        }

        private string filePath;
        private UsingList usings;
        private MethodList methods;
        private ExtendList extends;
        private ComponentList components;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record ExtendDecl : LangElement
    {
        public ExtendDecl(Span span, SyntaxKind syntaxKind, MethodList methods, IdentifierToken identifier): base(span, syntaxKind)
        {
            this.methods = methods;
            this.identifier = identifier;
        }

        public MethodList Methods { get => this.methods; init => this.methods = value; }

        public IdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitExtendDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitExtendDecl(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.methods;
            yield return this.identifier;
            yield break;
        }

        private MethodList methods;
        private IdentifierToken identifier;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record ComponentDecl : LangElement
    {
        public ComponentDecl(Span span, SyntaxKind syntaxKind, ExtendList extends, QualifiedIdentifierToken identifier): base(span, syntaxKind)
        {
            this.extends = extends;
            this.identifier = identifier;
        }

        public ExtendList Extends { get => this.extends; init => this.extends = value; }

        public QualifiedIdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitComponentDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitComponentDecl(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.extends;
            yield return this.identifier;
            yield break;
        }

        private ExtendList extends;
        private QualifiedIdentifierToken identifier;
    }
}

namespace Aquila.Syntax.Ast
{
    public abstract partial record UsingBase : LangElement
    {
        public UsingBase(Span span, SyntaxKind syntaxKind, String name): base(span, syntaxKind)
        {
            Name = name;
        }

        public String Name { get => this.name; init => this.name = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }

        private String name;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record UsingDecl : UsingBase
    {
        public UsingDecl(Span span, SyntaxKind syntaxKind, String name): base(span, syntaxKind, name)
        {
            Name = name;
        }

        public String Name { get => this.name; init => this.name = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingDecl(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }

        private String name;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record UsingAliasDecl : UsingBase
    {
        public UsingAliasDecl(Span span, SyntaxKind syntaxKind, String className, String alias): base(span, syntaxKind, className)
        {
            ClassName = className;
            Alias = alias;
        }

        public String ClassName { get => this.className; init => this.className = value; }

        public String Alias { get => this.alias; init => this.alias = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingAliasDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingAliasDecl(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }

        private String className;
        private String alias;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record Annotation : LangElement
    {
        public Annotation(Span span, SyntaxKind syntaxKind, ArgumentList arguments, IdentifierToken identifier): base(span, syntaxKind)
        {
            this.arguments = arguments;
            this.identifier = identifier;
        }

        public ArgumentList Arguments { get => this.arguments; init => this.arguments = value; }

        public IdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAnnotation(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAnnotation(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.arguments;
            yield return this.identifier;
            yield break;
        }

        private ArgumentList arguments;
        private IdentifierToken identifier;
    }
}

namespace Aquila.Syntax.Ast
{
    public abstract partial record Member : LangElement
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

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Syntax.Ast.Functions
{
    public partial record Parameter : LangElement
    {
        public Parameter(Span span, SyntaxKind syntaxKind, TypeRef type, IdentifierToken identifier, PassMethod passMethod = PassMethod.ByValue): base(span, syntaxKind)
        {
            this.type = type;
            this.identifier = identifier;
            PassMethod = passMethod;
        }

        public TypeRef Type { get => this.type; init => this.type = value; }

        public IdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public PassMethod PassMethod { get => this.passMethod; init => this.passMethod = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitParameter(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitParameter(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.type;
            yield return this.identifier;
            yield break;
        }

        private TypeRef type;
        private IdentifierToken identifier;
        private PassMethod passMethod;
    }
}

namespace Aquila.Syntax.Ast.Functions
{
    public partial record MethodDecl : Member
    {
        public MethodDecl(Span span, SyntaxKind syntaxKind, BlockStmt block, ParameterList parameters, AnnotationList annotations, IdentifierToken identifier, TypeRef returnType): base(span, syntaxKind)
        {
            this.block = block;
            this.parameters = parameters;
            this.annotations = annotations;
            this.identifier = identifier;
            this.returnType = returnType;
        }

        public BlockStmt Block { get => this.block; init => this.block = value; }

        public ParameterList Parameters { get => this.parameters; init => this.parameters = value; }

        public AnnotationList Annotations { get => this.annotations; init => this.annotations = value; }

        public IdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public TypeRef ReturnType { get => this.returnType; init => this.returnType = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMethodDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMethodDecl(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.block;
            yield return this.parameters;
            yield return this.annotations;
            yield return this.identifier;
            yield return this.returnType;
            yield break;
        }

        private BlockStmt block;
        private ParameterList parameters;
        private AnnotationList annotations;
        private IdentifierToken identifier;
        private TypeRef returnType;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record FieldDecl : Member
    {
        public FieldDecl(Span span, SyntaxKind syntaxKind, IdentifierToken identifier, TypeRef type): base(span, syntaxKind)
        {
            this.identifier = identifier;
            this.type = type;
        }

        public IdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public TypeRef Type { get => this.type; init => this.type = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFieldDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFieldDecl(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.identifier;
            yield return this.type;
            yield break;
        }

        private IdentifierToken identifier;
        private TypeRef type;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record IdentifierToken : SyntaxToken
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

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record QualifiedIdentifierToken : SyntaxToken
    {
        public QualifiedIdentifierToken(Span span, SyntaxKind syntaxKind, String text): base(span, syntaxKind, text)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitQualifiedIdentifierToken(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitQualifiedIdentifierToken(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public abstract partial record TypeRef : LangElement
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

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record NamedTypeRef : TypeRef
    {
        public NamedTypeRef(Span span, SyntaxKind syntaxKind,  string  value = "?"): base(span, syntaxKind)
        {
            Value = value;
        }

        public string Value { get => this.value; init => this.value = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNamedTypeRef(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNamedTypeRef(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }

        private string value;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record PredefinedTypeRef : TypeRef
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

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record ArrayType : TypeRef
    {
        public ArrayType(Span span, SyntaxKind syntaxKind, TypeRef type): base(span, syntaxKind)
        {
            this.type = type;
        }

        public TypeRef Type { get => this.type; init => this.type = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.type;
            yield break;
        }

        private TypeRef type;
    }
}

namespace Aquila.Syntax.Ast
{
    public abstract partial record Expression : LangElement
    {
        public Expression(Span span, SyntaxKind syntaxKind, Operations operation): base(span, syntaxKind)
        {
            Operation = operation;
        }

        public Operations Operation { get => this.operation; init => this.operation = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }

        private Operations operation;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record MissingEx : Expression
    {
        public MissingEx(Span span, SyntaxKind syntaxKind, Operations operation,  string  message): base(span, syntaxKind, operation)
        {
            Message = message;
        }

        public string Message { get => this.message; init => this.message = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMissingEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMissingEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }

        private string message;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record BinaryEx : Expression
    {
        public BinaryEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression right, Expression left): base(span, syntaxKind, operation)
        {
            this.right = right;
            this.left = left;
        }

        public Expression Right { get => this.right; init => this.right = value; }

        public Expression Left { get => this.left; init => this.left = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBinaryEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBinaryEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.right;
            yield return this.left;
            yield break;
        }

        private Expression right;
        private Expression left;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public abstract partial record UnaryExBase : Expression
    {
        public UnaryExBase(Span span, SyntaxKind syntaxKind, Operations operation, Expression expression): base(span, syntaxKind, operation)
        {
            this.expression = expression;
        }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.expression;
            yield break;
        }

        private Expression expression;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record CastEx : UnaryExBase
    {
        public CastEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression expression, TypeRef castType): base(span, syntaxKind, operation, expression)
        {
            this.expression = expression;
            this.castType = castType;
        }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public TypeRef CastType { get => this.castType; init => this.castType = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCastEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCastEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.expression;
            yield return this.castType;
            yield break;
        }

        private Expression expression;
        private TypeRef castType;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record IndexerEx : UnaryExBase
    {
        public IndexerEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression indexer, Expression expression): base(span, syntaxKind, operation, expression)
        {
            this.indexer = indexer;
            this.expression = expression;
        }

        public Expression Indexer { get => this.indexer; init => this.indexer = value; }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIndexerEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIndexerEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.indexer;
            yield return this.expression;
            yield break;
        }

        private Expression indexer;
        private Expression expression;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record UnaryEx : UnaryExBase
    {
        public UnaryEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression expression): base(span, syntaxKind, operation, expression)
        {
            this.expression = expression;
        }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUnaryEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUnaryEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.expression;
            yield break;
        }

        private Expression expression;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record AssignEx : Expression
    {
        public AssignEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression rValue, Expression lValue): base(span, syntaxKind, operation)
        {
            this.rValue = rValue;
            this.lValue = lValue;
        }

        public Expression RValue { get => this.rValue; init => this.rValue = value; }

        public Expression LValue { get => this.lValue; init => this.lValue = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAssignEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAssignEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.rValue;
            yield return this.lValue;
            yield break;
        }

        private Expression rValue;
        private Expression lValue;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record MemberAccessEx : Expression
    {
        public MemberAccessEx(Span span, SyntaxKind syntaxKind, Operations operation, IdentifierToken identifier, Expression expression): base(span, syntaxKind, operation)
        {
            this.identifier = identifier;
            this.expression = expression;
        }

        public IdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMemberAccessEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMemberAccessEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.identifier;
            yield return this.expression;
            yield break;
        }

        private IdentifierToken identifier;
        private Expression expression;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record CallEx : Expression
    {
        public CallEx(Span span, SyntaxKind syntaxKind, Operations operation, ArgumentList arguments, Expression expression): base(span, syntaxKind, operation)
        {
            this.arguments = arguments;
            this.expression = expression;
        }

        public ArgumentList Arguments { get => this.arguments; init => this.arguments = value; }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCallEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCallEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.arguments;
            yield return this.expression;
            yield break;
        }

        private ArgumentList arguments;
        private Expression expression;
    }
}

namespace Aquila.Syntax.Ast.Functions
{
    public partial record Argument : LangElement
    {
        public Argument(Span span, SyntaxKind syntaxKind, Expression expression, PassMethod passMethod = PassMethod.ByValue): base(span, syntaxKind)
        {
            this.expression = expression;
            PassMethod = passMethod;
        }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public PassMethod PassMethod { get => this.passMethod; init => this.passMethod = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArgument(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitArgument(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.expression;
            yield break;
        }

        private Expression expression;
        private PassMethod passMethod;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record New : Expression
    {
        public New(Span span, SyntaxKind syntaxKind, Operations operation,  string  @namespace, CallEx call): base(span, syntaxKind, operation)
        {
            Namespace = @namespace;
            this.call = call;
        }

        public string Namespace { get => this.@namespace; init => this.@namespace = value; }

        public CallEx Call { get => this.call; init => this.call = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNew(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNew(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.call;
            yield break;
        }

        private string @namespace;
        private CallEx call;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record LiteralEx : Expression
    {
        public LiteralEx(Span span, SyntaxKind syntaxKind, Operations operation, String value,  bool  isSqlLiteral): base(span, syntaxKind, operation)
        {
            Value = value;
            IsSqlLiteral = isSqlLiteral;
        }

        public String Value { get => this.value; init => this.value = value; }

        public bool IsSqlLiteral { get => this.isSqlLiteral; init => this.isSqlLiteral = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLiteralEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitLiteralEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }

        private String value;
        private bool isSqlLiteral;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record IncDecEx : Expression
    {
        public IncDecEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression operand,  bool  isIncrement,  bool  isPost): base(span, syntaxKind, operation)
        {
            this.operand = operand;
            IsIncrement = isIncrement;
            IsPost = isPost;
        }

        public Expression Operand { get => this.operand; init => this.operand = value; }

        public bool IsIncrement { get => this.isIncrement; init => this.isIncrement = value; }

        public bool IsPost { get => this.isPost; init => this.isPost = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIncDecEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIncDecEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.operand;
            yield break;
        }

        private Expression operand;
        private bool isIncrement;
        private bool isPost;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record ThrowEx : Expression
    {
        public ThrowEx(Span span, SyntaxKind syntaxKind, Operations operation, Expression expression): base(span, syntaxKind, operation)
        {
            this.expression = expression;
        }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitThrowEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitThrowEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.expression;
            yield break;
        }

        private Expression expression;
    }
}

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record NameEx : Expression
    {
        public NameEx(Span span, SyntaxKind syntaxKind, Operations operation, IdentifierToken identifier): base(span, syntaxKind, operation)
        {
            this.identifier = identifier;
        }

        public IdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNameEx(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNameEx(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.identifier;
            yield break;
        }

        private IdentifierToken identifier;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public abstract partial record Statement : LangElement
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

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record BlockStmt : Statement
    {
        public BlockStmt(Span span, SyntaxKind syntaxKind, StatementList statements): base(span, syntaxKind)
        {
            this.statements = statements;
        }

        public StatementList Statements { get => this.statements; init => this.statements = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBlockStmt(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.statements;
            yield break;
        }

        private StatementList statements;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record ReturnStmt : Statement
    {
        public ReturnStmt(Span span, SyntaxKind syntaxKind, Expression expression): base(span, syntaxKind)
        {
            this.expression = expression;
        }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitReturnStmt(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.expression;
            yield break;
        }

        private Expression expression;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record BreakStmt : Statement
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

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record ContinueStmt : Statement
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

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record VarDecl : Statement
    {
        public VarDecl(Span span, SyntaxKind syntaxKind, TypeRef variableType, DeclaratorList declarators): base(span, syntaxKind)
        {
            this.variableType = variableType;
            this.declarators = declarators;
        }

        public TypeRef VariableType { get => this.variableType; init => this.variableType = value; }

        public DeclaratorList Declarators { get => this.declarators; init => this.declarators = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitVarDecl(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitVarDecl(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.variableType;
            yield return this.declarators;
            yield break;
        }

        private TypeRef variableType;
        private DeclaratorList declarators;
    }
}

namespace Aquila.Syntax.Ast
{
    public partial record VarDeclarator : LangElement
    {
        public VarDeclarator(Span span, SyntaxKind syntaxKind, Expression initializer, IdentifierToken identifier): base(span, syntaxKind)
        {
            this.initializer = initializer;
            this.identifier = identifier;
        }

        public Expression Initializer { get => this.initializer; init => this.initializer = value; }

        public IdentifierToken Identifier { get => this.identifier; init => this.identifier = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitVarDeclarator(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitVarDeclarator(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.initializer;
            yield return this.identifier;
            yield break;
        }

        private Expression initializer;
        private IdentifierToken identifier;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record DoWhileStmt : Statement
    {
        public DoWhileStmt(Span span, SyntaxKind syntaxKind, Expression condition, BlockStmt block): base(span, syntaxKind)
        {
            this.condition = condition;
            this.block = block;
        }

        public Expression Condition { get => this.condition; init => this.condition = value; }

        public BlockStmt Block { get => this.block; init => this.block = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitDoWhileStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitDoWhileStmt(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.condition;
            yield return this.block;
            yield break;
        }

        private Expression condition;
        private BlockStmt block;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record TryStmt : Statement
    {
        public TryStmt(Span span, SyntaxKind syntaxKind, BlockStmt tryBlock, CatchList catches, FinallyItem finallyItem): base(span, syntaxKind)
        {
            this.tryBlock = tryBlock;
            this.catches = catches;
            this.finallyItem = finallyItem;
        }

        public BlockStmt TryBlock { get => this.tryBlock; init => this.tryBlock = value; }

        public CatchList Catches { get => this.catches; init => this.catches = value; }

        public FinallyItem FinallyItem { get => this.finallyItem; init => this.finallyItem = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTryStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitTryStmt(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.tryBlock;
            yield return this.catches;
            yield return this.finallyItem;
            yield break;
        }

        private BlockStmt tryBlock;
        private CatchList catches;
        private FinallyItem finallyItem;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record CatchItem : LangElement
    {
        public CatchItem(Span span, SyntaxKind syntaxKind, BlockStmt block): base(span, syntaxKind)
        {
            this.block = block;
        }

        public BlockStmt Block { get => this.block; init => this.block = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCatchItem(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCatchItem(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.block;
            yield break;
        }

        private BlockStmt block;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record FinallyItem : LangElement
    {
        public FinallyItem(Span span, SyntaxKind syntaxKind, BlockStmt block): base(span, syntaxKind)
        {
            this.block = block;
        }

        public BlockStmt Block { get => this.block; init => this.block = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFinallyItem(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFinallyItem(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.block;
            yield break;
        }

        private BlockStmt block;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record ExpressionStmt : Statement
    {
        public ExpressionStmt(Span span, SyntaxKind syntaxKind, Expression expression): base(span, syntaxKind)
        {
            this.expression = expression;
        }

        public Expression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitExpressionStmt(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.expression;
            yield break;
        }

        private Expression expression;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record ForStmt : Statement
    {
        public ForStmt(Span span, SyntaxKind syntaxKind, BlockStmt block, Expression counter, Expression condition, Expression initializer): base(span, syntaxKind)
        {
            this.block = block;
            this.counter = counter;
            this.condition = condition;
            this.initializer = initializer;
        }

        public BlockStmt Block { get => this.block; init => this.block = value; }

        public Expression Counter { get => this.counter; init => this.counter = value; }

        public Expression Condition { get => this.condition; init => this.condition = value; }

        public Expression Initializer { get => this.initializer; init => this.initializer = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitForStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitForStmt(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.block;
            yield return this.counter;
            yield return this.condition;
            yield return this.initializer;
            yield break;
        }

        private BlockStmt block;
        private Expression counter;
        private Expression condition;
        private Expression initializer;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record WhileStmt : Statement
    {
        public WhileStmt(Span span, SyntaxKind syntaxKind, BlockStmt block, Expression condition): base(span, syntaxKind)
        {
            this.block = block;
            this.condition = condition;
        }

        public BlockStmt Block { get => this.block; init => this.block = value; }

        public Expression Condition { get => this.condition; init => this.condition = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitWhileStmt(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.block;
            yield return this.condition;
            yield break;
        }

        private BlockStmt block;
        private Expression condition;
    }
}

namespace Aquila.Syntax.Ast.Statements
{
    public partial record IfStmt : Statement
    {
        public IfStmt(Span span, SyntaxKind syntaxKind, Statement elseBlock, Statement thenBlock, Expression condition): base(span, syntaxKind)
        {
            this.elseBlock = elseBlock;
            this.thenBlock = thenBlock;
            this.condition = condition;
        }

        public Statement ElseBlock { get => this.elseBlock; init => this.elseBlock = value; }

        public Statement ThenBlock { get => this.thenBlock; init => this.thenBlock = value; }

        public Expression Condition { get => this.condition; init => this.condition = value; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIfStmt(this);
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield return this.elseBlock;
            yield return this.thenBlock;
            yield return this.condition;
            yield break;
        }

        private Statement elseBlock;
        private Statement thenBlock;
        private Expression condition;
    }
}

namespace Aquila.Syntax
{
    public abstract partial class AstVisitorBase<T>
    {
        public virtual T VisitComponentList(ComponentList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitExtendList(ExtendList arg)
        {
            return DefaultVisit(arg);
        }

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

        public virtual T VisitExtendDecl(ExtendDecl arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitComponentDecl(ComponentDecl arg)
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

        public virtual T VisitQualifiedIdentifierToken(QualifiedIdentifierToken arg)
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

        public virtual T VisitMissingEx(MissingEx arg)
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

        public virtual T VisitMemberAccessEx(MemberAccessEx arg)
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
        public virtual void VisitComponentList(ComponentList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitExtendList(ExtendList arg)
        {
            DefaultVisit(arg);
        }

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

        public virtual void VisitExtendDecl(ExtendDecl arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitComponentDecl(ComponentDecl arg)
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

        public virtual void VisitQualifiedIdentifierToken(QualifiedIdentifierToken arg)
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

        public virtual void VisitMissingEx(MissingEx arg)
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

        public virtual void VisitMemberAccessEx(MemberAccessEx arg)
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
