using System;
using System.Collections;
using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Root : SyntaxNode, IScoped
    {
        public Root(ILineInfo lineInfo, List<CompilationUnit> units): base(lineInfo)
        {
            var slot = 0;
            Units = units;
            if (Units != null)
                foreach (var item in Units)
                {
                    if (item != null)
                        Childs.Add(item);
                }
        }

        public List<CompilationUnit> Units
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitRoot(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class NamespaceDeclaration : SyntaxNode
    {
        public NamespaceDeclaration(ILineInfo lineInfo, String name, List<UsingBase> usings, List<TypeEntity> entityes, List<NamespaceDeclaration> namespaceDeclarations): base(lineInfo)
        {
            var slot = 0;
            Name = name;
            Usings = usings;
            if (Usings != null)
                foreach (var item in Usings)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Entityes = entityes;
            if (Entityes != null)
                foreach (var item in Entityes)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            NamespaceDeclarations = namespaceDeclarations;
            if (NamespaceDeclarations != null)
                foreach (var item in NamespaceDeclarations)
                {
                    if (item != null)
                        Childs.Add(item);
                }
        }

        public String Name
        {
            get;
        }

        public List<UsingBase> Usings
        {
            get;
        }

        public List<TypeEntity> Entityes
        {
            get;
        }

        public List<NamespaceDeclaration> NamespaceDeclarations
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNamespaceDeclaration(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(ILineInfo lineInfo, List<UsingBase> usings, List<TypeEntity> entityes, List<NamespaceDeclaration> namespaceDeclarations): base(lineInfo)
        {
            var slot = 0;
            Usings = usings;
            if (Usings != null)
                foreach (var item in Usings)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Entityes = entityes;
            if (Entityes != null)
                foreach (var item in Entityes)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            NamespaceDeclarations = namespaceDeclarations;
            if (NamespaceDeclarations != null)
                foreach (var item in NamespaceDeclarations)
                {
                    if (item != null)
                        Childs.Add(item);
                }
        }

        public List<UsingBase> Usings
        {
            get;
        }

        public List<TypeEntity> Entityes
        {
            get;
        }

        public List<NamespaceDeclaration> NamespaceDeclarations
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCompilationUnit(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public abstract partial class UsingBase : SyntaxNode
    {
        public UsingBase(ILineInfo lineInfo, String name): base(lineInfo)
        {
            var slot = 0;
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
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class UsingDeclaration : UsingBase
    {
        public UsingDeclaration(ILineInfo lineInfo, String name): base(lineInfo, name)
        {
            var slot = 0;
            Name = name;
        }

        public String Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingDeclaration(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class UsingAliasDeclaration : UsingBase
    {
        public UsingAliasDeclaration(ILineInfo lineInfo, String className, String alias): base(lineInfo, className)
        {
            var slot = 0;
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
            return visitor.VisitUsingAliasDeclaration(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class TypeBody : SyntaxNode, IScoped
    {
        public TypeBody(ILineInfo lineInfo, List<Function> functions, List<Field> fields, List<Property> properties, List<Constructor> constructors, List<UsingBase> usings): base(lineInfo)
        {
            var slot = 0;
            Functions = functions;
            if (Functions != null)
                foreach (var item in Functions)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Fields = fields;
            if (Fields != null)
                foreach (var item in Fields)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Properties = properties;
            if (Properties != null)
                foreach (var item in Properties)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Constructors = constructors;
            if (Constructors != null)
                foreach (var item in Constructors)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Usings = usings;
            if (Usings != null)
                foreach (var item in Usings)
                {
                    if (item != null)
                        Childs.Add(item);
                }
        }

        public List<Function> Functions
        {
            get;
        }

        public List<Field> Fields
        {
            get;
        }

        public List<Property> Properties
        {
            get;
        }

        public List<Constructor> Constructors
        {
            get;
        }

        public List<UsingBase> Usings
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeBody(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public abstract partial class TypeEntity : SyntaxNode
    {
        public TypeEntity(ILineInfo lineInfo, TypeBody typeBody, String name, TypeSyntax @base = null): base(lineInfo)
        {
            var slot = 0;
            TypeBody = typeBody;
            if (TypeBody != null)
                Childs.Add(TypeBody);
            Name = name;
            Base = @base;
            if (Base != null)
                Childs.Add(Base);
        }

        public TypeBody TypeBody
        {
            get;
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Base
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Class : TypeEntity, IAstSymbol
    {
        public Class(ILineInfo lineInfo, TypeBody typeBody, String name): base(lineInfo, typeBody, name)
        {
            var slot = 0;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitClass(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Module : TypeEntity, IAstSymbol
    {
        public Module(ILineInfo lineInfo, TypeBody typeBody, String name): base(lineInfo, typeBody, name)
        {
            var slot = 0;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitModule(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public abstract partial class Expression : SyntaxNode
    {
        public Expression(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public abstract partial class Statement : SyntaxNode
    {
        public Statement(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class BinaryExpression : Expression
    {
        public BinaryExpression(ILineInfo lineInfo, Expression right, Expression left, BinaryOperatorType binaryOperatorType): base(lineInfo)
        {
            var slot = 0;
            Right = right;
            if (Right != null)
                Childs.Add(Right);
            Left = left;
            if (Left != null)
                Childs.Add(Left);
            BinaryOperatorType = binaryOperatorType;
        }

        public Expression Right
        {
            get;
        }

        public Expression Left
        {
            get;
        }

        public BinaryOperatorType BinaryOperatorType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public abstract partial class UnaryExpression : Expression
    {
        public UnaryExpression(ILineInfo lineInfo, Expression expression, UnaryOperatorType operaotrType): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get;
        }

        public UnaryOperatorType OperaotrType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class CastExpression : UnaryExpression
    {
        public CastExpression(ILineInfo lineInfo, Expression expression, TypeSyntax castType, UnaryOperatorType operaotrType): base(lineInfo, expression, operaotrType)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
            CastType = castType;
            if (CastType != null)
                Childs.Add(CastType);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get;
        }

        public TypeSyntax CastType
        {
            get;
        }

        public UnaryOperatorType OperaotrType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCastExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class IndexerExpression : UnaryExpression
    {
        public IndexerExpression(ILineInfo lineInfo, Expression indexer, Expression expression, UnaryOperatorType operaotrType): base(lineInfo, expression, operaotrType)
        {
            var slot = 0;
            Indexer = indexer;
            if (Indexer != null)
                Childs.Add(Indexer);
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
            OperaotrType = operaotrType;
        }

        public Expression Indexer
        {
            get;
        }

        public Expression Expression
        {
            get;
        }

        public UnaryOperatorType OperaotrType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIndexerExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class LogicalOrArithmeticExpression : UnaryExpression
    {
        public LogicalOrArithmeticExpression(ILineInfo lineInfo, Expression expression, UnaryOperatorType operaotrType): base(lineInfo, expression, operaotrType)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get;
        }

        public UnaryOperatorType OperaotrType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLogicalOrArithmeticExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class Assignment : Expression
    {
        public Assignment(ILineInfo lineInfo, Expression value, Expression index, ICanBeAssigned assignable): base(lineInfo)
        {
            var slot = 0;
            Value = value;
            if (Value != null)
                Childs.Add(Value);
            Index = index;
            if (Index != null)
                Childs.Add(Index);
            Assignable = assignable;
            if (Assignable != null)
                Childs.Add(Assignable);
        }

        public Expression Value
        {
            get;
        }

        public Expression Index
        {
            get;
        }

        public ICanBeAssigned Assignable
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public abstract partial class TypeSyntax : SyntaxNode
    {
        public TypeSyntax(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class PrimitiveTypeSyntax : TypeSyntax
    {
        public PrimitiveTypeSyntax(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPrimitiveTypeSyntax(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class SingleTypeSyntax : TypeSyntax
    {
        public SingleTypeSyntax(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitSingleTypeSyntax(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class ArrayTypeSyntax : TypeSyntax
    {
        public ArrayTypeSyntax(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArrayTypeSyntax(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class UnionTypeSyntax : TypeSyntax
    {
        public UnionTypeSyntax(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUnionTypeSyntax(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class GenericTypeSyntax : TypeSyntax
    {
        public GenericTypeSyntax(ILineInfo lineInfo, List<TypeSyntax> args): base(lineInfo)
        {
            var slot = 0;
            Args = args;
            if (Args != null)
                foreach (var item in Args)
                {
                    if (item != null)
                        Childs.Add(item);
                }
        }

        public List<TypeSyntax> Args
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGenericTypeSyntax(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Block : SyntaxNode, IScoped
    {
        public Block(ILineInfo lineInfo, List<Statement> statements): base(lineInfo)
        {
            var slot = 0;
            Statements = statements;
            if (Statements != null)
                foreach (var item in Statements)
                {
                    if (item != null)
                        Childs.Add(item);
                }
        }

        public List<Statement> Statements
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBlock(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Functions
{
    public partial class Parameter : SyntaxNode, IAstSymbol
    {
        public Parameter(ILineInfo lineInfo, String name, PassMethod passMethod = PassMethod.ByValue): base(lineInfo)
        {
            var slot = 0;
            Name = name;
            PassMethod = passMethod;
        }

        public String Name
        {
            get;
        }

        public PassMethod PassMethod
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitParameter(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Attribute : SyntaxNode
    {
        public Attribute(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAttribute(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public abstract partial class Member : SyntaxNode
    {
        public Member(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Functions
{
    public partial class Function : Member, IScoped, IAstSymbol
    {
        public Function(ILineInfo lineInfo, Block block, List<Parameter> parameters, List<Attribute> attributes, String name, TypeSyntax type): base(lineInfo)
        {
            var slot = 0;
            Block = block;
            if (Block != null)
                Childs.Add(Block);
            Parameters = parameters;
            if (Parameters != null)
                foreach (var item in Parameters)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Attributes = attributes;
            if (Attributes != null)
                foreach (var item in Attributes)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Name = name;
            Type = type;
        }

        public Block Block
        {
            get;
        }

        public List<Parameter> Parameters
        {
            get;
        }

        public List<Attribute> Attributes
        {
            get;
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Type
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFunction(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Constructor : Member, IScoped, IAstSymbol
    {
        public Constructor(ILineInfo lineInfo, Block block, List<Parameter> parameters, List<Attribute> attributes, String name): base(lineInfo)
        {
            var slot = 0;
            Block = block;
            if (Block != null)
                Childs.Add(Block);
            Parameters = parameters;
            if (Parameters != null)
                foreach (var item in Parameters)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Attributes = attributes;
            if (Attributes != null)
                foreach (var item in Attributes)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Name = name;
        }

        public Block Block
        {
            get;
        }

        public List<Parameter> Parameters
        {
            get;
        }

        public List<Attribute> Attributes
        {
            get;
        }

        public String Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitConstructor(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Field : Member, IAstSymbol
    {
        public Field(ILineInfo lineInfo, String name): base(lineInfo)
        {
            var slot = 0;
            Name = name;
        }

        public String Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitField(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Property : Member, IAstSymbol
    {
        public Property(ILineInfo lineInfo, String name, TypeSyntax type, String mapTo = null): base(lineInfo)
        {
            var slot = 0;
            Name = name;
            Type = type;
            if (Type != null)
                Childs.Add(Type);
            MapTo = mapTo;
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Type
        {
            get;
        }

        public String MapTo
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitProperty(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Functions
{
    public partial class Argument : SyntaxNode
    {
        public Argument(ILineInfo lineInfo, Expression expression, PassMethod passMethod = PassMethod.ByValue): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
            PassMethod = passMethod;
        }

        public Expression Expression
        {
            get;
        }

        public PassMethod PassMethod
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArgument(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Call : Expression
    {
        public Call(ILineInfo lineInfo, IList<Argument> arguments, Name name, Expression expression): base(lineInfo)
        {
            var slot = 0;
            Arguments = arguments;
            if (Arguments != null)
                foreach (var item in Arguments)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Name = name;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
        }

        public IList<Argument> Arguments
        {
            get;
        }

        public Name Name
        {
            get;
        }

        public Expression Expression
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCall(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class Return : Statement
    {
        public Return(ILineInfo lineInfo, Expression expression): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
        }

        public Expression Expression
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitReturn(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Variable : Expression, IAstSymbol
    {
        public Variable(ILineInfo lineInfo, Expression value, String name, TypeSyntax type): base(lineInfo)
        {
            var slot = 0;
            Value = value;
            if (Value != null)
                Childs.Add(Value);
            Name = name;
            Type = type;
            if (Type != null)
                Childs.Add(Type);
        }

        public Expression Value
        {
            get;
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Type
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitVariable(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class ContextVariable : Expression, IAstSymbol
    {
        public ContextVariable(ILineInfo lineInfo, String name, TypeSyntax type): base(lineInfo)
        {
            var slot = 0;
            Name = name;
            Type = type;
            if (Type != null)
                Childs.Add(Type);
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Type
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitContextVariable(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Literal : Expression
    {
        public Literal(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLiteral(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Name : Expression
    {
        public Name(ILineInfo lineInfo, String value): base(lineInfo)
        {
            var slot = 0;
            Value = value;
        }

        public String Value
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitName(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class GetFieldExpression : Expression
    {
        public GetFieldExpression(ILineInfo lineInfo, Expression expression, String fieldName): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
            FieldName = fieldName;
        }

        public Expression Expression
        {
            get;
        }

        public String FieldName
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGetFieldExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public abstract partial class LookupExpression : Expression
    {
        public LookupExpression(ILineInfo lineInfo, Expression lookup, Expression current): base(lineInfo)
        {
            var slot = 0;
            Lookup = lookup;
            if (Lookup != null)
                Childs.Add(Lookup);
            Current = current;
            if (Current != null)
                Childs.Add(Current);
        }

        public Expression Lookup
        {
            get;
        }

        public Expression Current
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class PropertyLookupExpression : LookupExpression
    {
        public PropertyLookupExpression(ILineInfo lineInfo, Expression lookup, Expression current): base(lineInfo, lookup, current)
        {
            var slot = 0;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPropertyLookupExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class MethodLookupExpression : LookupExpression
    {
        public MethodLookupExpression(ILineInfo lineInfo, Expression lookup, Expression current): base(lineInfo, lookup, current)
        {
            var slot = 0;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMethodLookupExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class AssignFieldExpression : Expression
    {
        public AssignFieldExpression(ILineInfo lineInfo, Expression expression, String fieldName): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
            FieldName = fieldName;
        }

        public Expression Expression
        {
            get;
        }

        public String FieldName
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAssignFieldExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class DoWhile : Statement
    {
        public DoWhile(ILineInfo lineInfo, Expression condition, Block block): base(lineInfo)
        {
            var slot = 0;
            Condition = condition;
            if (Condition != null)
                Childs.Add(Condition);
            Block = block;
            if (Block != null)
                Childs.Add(Block);
        }

        public Expression Condition
        {
            get;
        }

        public Block Block
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitDoWhile(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class Try : Statement
    {
        public Try(ILineInfo lineInfo, Block tryBlock, Block catchBlock, Block finallyBlock): base(lineInfo)
        {
            var slot = 0;
            TryBlock = tryBlock;
            if (TryBlock != null)
                Childs.Add(TryBlock);
            CatchBlock = catchBlock;
            if (CatchBlock != null)
                Childs.Add(CatchBlock);
            FinallyBlock = finallyBlock;
            if (FinallyBlock != null)
                Childs.Add(FinallyBlock);
        }

        public Block TryBlock
        {
            get;
        }

        public Block CatchBlock
        {
            get;
        }

        public Block FinallyBlock
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTry(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class ExpressionStatement : Statement
    {
        public ExpressionStatement(ILineInfo lineInfo, Expression expression): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
        }

        public Expression Expression
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class For : Statement
    {
        public For(ILineInfo lineInfo, Block block, Expression counter, Expression condition, Expression initializer): base(lineInfo)
        {
            var slot = 0;
            Block = block;
            if (Block != null)
                Childs.Add(Block);
            Counter = counter;
            if (Counter != null)
                Childs.Add(Counter);
            Condition = condition;
            if (Condition != null)
                Childs.Add(Condition);
            Initializer = initializer;
            if (Initializer != null)
                Childs.Add(Initializer);
        }

        public Block Block
        {
            get;
        }

        public Expression Counter
        {
            get;
        }

        public Expression Condition
        {
            get;
        }

        public Expression Initializer
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFor(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class If : Statement
    {
        public If(ILineInfo lineInfo, Block elseBlock, Block ifBlock, Expression condition): base(lineInfo)
        {
            var slot = 0;
            ElseBlock = elseBlock;
            if (ElseBlock != null)
                Childs.Add(ElseBlock);
            IfBlock = ifBlock;
            if (IfBlock != null)
                Childs.Add(IfBlock);
            Condition = condition;
            if (Condition != null)
                Childs.Add(Condition);
        }

        public Block ElseBlock
        {
            get;
        }

        public Block IfBlock
        {
            get;
        }

        public Expression Condition
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIf(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public abstract partial class PostOperationExpression : Expression
    {
        public PostOperationExpression(ILineInfo lineInfo, Expression expression): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
        }

        public Expression Expression
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class PostIncrementExpression : PostOperationExpression
    {
        public PostIncrementExpression(ILineInfo lineInfo, Expression expression): base(lineInfo, expression)
        {
            var slot = 0;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostIncrementExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class PostDecrementExpression : PostOperationExpression
    {
        public PostDecrementExpression(ILineInfo lineInfo, Expression expression): base(lineInfo, expression)
        {
            var slot = 0;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostDecrementExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class Throw : Expression
    {
        public Throw(ILineInfo lineInfo, Expression exception): base(lineInfo)
        {
            var slot = 0;
            Exception = exception;
            if (Exception != null)
                Childs.Add(Exception);
        }

        public Expression Exception
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitThrow(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class MatchAtom : SyntaxNode
    {
        public MatchAtom(ILineInfo lineInfo, Block block, Expression expression, TypeSyntax type): base(lineInfo)
        {
            var slot = 0;
            Block = block;
            if (Block != null)
                Childs.Add(Block);
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
            Type = type;
            if (Type != null)
                Childs.Add(Type);
        }

        public Block Block
        {
            get;
        }

        public Expression Expression
        {
            get;
        }

        public TypeSyntax Type
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMatchAtom(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class Match : Statement
    {
        public Match(ILineInfo lineInfo, List<MatchAtom> matches, Expression expression): base(lineInfo)
        {
            var slot = 0;
            Matches = matches;
            if (Matches != null)
                foreach (var item in Matches)
                {
                    if (item != null)
                        Childs.Add(item);
                }

            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
        }

        public List<MatchAtom> Matches
        {
            get;
        }

        public Expression Expression
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMatch(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class GlobalVar : Expression
    {
        public GlobalVar(ILineInfo lineInfo, Expression expression): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
            if (Expression != null)
                Childs.Add(Expression);
        }

        public Expression Expression
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGlobalVar(this);
        }
    }
}
