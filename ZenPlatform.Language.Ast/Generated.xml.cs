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
            foreach (var item in Units)
            {
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
    public partial class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(ILineInfo lineInfo, List<string> namespaces, List<TypeEntity> entityes): base(lineInfo)
        {
            var slot = 0;
            Namespaces = namespaces;
            Entityes = entityes;
            foreach (var item in Entityes)
            {
                Childs.Add(item);
            }
        }

        public List<string> Namespaces
        {
            get;
        }

        public List<TypeEntity> Entityes
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
    public partial class TypeBody : SyntaxNode, IScoped
    {
        public TypeBody(ILineInfo lineInfo, List<Function> functions, List<Field> fields, List<Property> properties): base(lineInfo)
        {
            var slot = 0;
            Functions = functions;
            foreach (var item in Functions)
            {
                Childs.Add(item);
            }

            Fields = fields;
            foreach (var item in Fields)
            {
                Childs.Add(item);
            }

            Properties = properties;
            foreach (var item in Properties)
            {
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
        public TypeEntity(ILineInfo lineInfo, String name): base(lineInfo)
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
            Childs.Add(Right);
            Left = left;
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
        public UnaryExpression(ILineInfo lineInfo, Expression expression): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
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
    public partial class CastExpression : UnaryExpression
    {
        public CastExpression(ILineInfo lineInfo, Expression expression, TypeSyntax castType): base(lineInfo, expression)
        {
            var slot = 0;
            Expression = expression;
            Childs.Add(Expression);
            CastType = castType;
            Childs.Add(CastType);
        }

        public Expression Expression
        {
            get;
        }

        public TypeSyntax CastType
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
        public IndexerExpression(ILineInfo lineInfo, Expression indexer, Expression expression): base(lineInfo, expression)
        {
            var slot = 0;
            Indexer = indexer;
            Childs.Add(Indexer);
            Expression = expression;
            Childs.Add(Expression);
        }

        public Expression Indexer
        {
            get;
        }

        public Expression Expression
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
        public LogicalOrArithmeticExpression(ILineInfo lineInfo, Expression expression, UnaryOperatorType operaotrType): base(lineInfo, expression)
        {
            var slot = 0;
            Expression = expression;
            Childs.Add(Expression);
            OperaotrType = operaotrType;
            Childs.Add(OperaotrType);
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
        public Assignment(ILineInfo lineInfo, Expression value, Expression index, Name name): base(lineInfo)
        {
            var slot = 0;
            Value = value;
            Childs.Add(Value);
            Index = index;
            Childs.Add(Index);
            Name = name;
            Childs.Add(Name);
        }

        public Expression Value
        {
            get;
        }

        public Expression Index
        {
            get;
        }

        public Name Name
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
    public partial class Block : SyntaxNode, IScoped
    {
        public Block(ILineInfo lineInfo, List<Statement> statements): base(lineInfo)
        {
            var slot = 0;
            Statements = statements;
            foreach (var item in Statements)
            {
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
            Childs.Add(Block);
            Parameters = parameters;
            foreach (var item in Parameters)
            {
                Childs.Add(item);
            }

            Attributes = attributes;
            foreach (var item in Attributes)
            {
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
    public partial class Field : Member, IAstSymbol
    {
        public Field(ILineInfo lineInfo, String name, TypeSyntax type): base(lineInfo)
        {
            var slot = 0;
            Name = name;
            Type = type;
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
            return visitor.VisitField(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Property : Member, IAstSymbol
    {
        public Property(ILineInfo lineInfo, String name, TypeSyntax type): base(lineInfo)
        {
            var slot = 0;
            Name = name;
            Type = type;
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
        public Call(ILineInfo lineInfo, IList<Argument> arguments, String name): base(lineInfo)
        {
            var slot = 0;
            Arguments = arguments;
            foreach (var item in Arguments)
            {
                Childs.Add(item);
            }

            Name = name;
            Childs.Add(Name);
        }

        public IList<Argument> Arguments
        {
            get;
        }

        public String Name
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
    public partial class Class : TypeEntity, IAstSymbol
    {
        public Class(ILineInfo lineInfo, TypeBody typeBody, String name): base(lineInfo, name)
        {
            var slot = 0;
            TypeBody = typeBody;
            Name = name;
        }

        public TypeBody TypeBody
        {
            get;
        }

        public String Name
        {
            get;
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
        public Module(ILineInfo lineInfo, TypeBody typeBody, String name): base(lineInfo, name)
        {
            var slot = 0;
            TypeBody = typeBody;
            Childs.Add(TypeBody);
            Name = name;
        }

        public TypeBody TypeBody
        {
            get;
        }

        public String Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitModule(this);
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
            Childs.Add(Value);
            Name = name;
            Type = type;
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
    public partial class FieldExpression : Expression
    {
        public FieldExpression(ILineInfo lineInfo, Expression expression, String fieldName): base(lineInfo)
        {
            var slot = 0;
            Expression = expression;
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
            return visitor.VisitFieldExpression(this);
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
            Childs.Add(Condition);
            Block = block;
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
            Childs.Add(TryBlock);
            CatchBlock = catchBlock;
            Childs.Add(CatchBlock);
            FinallyBlock = finallyBlock;
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
            Childs.Add(Block);
            Counter = counter;
            Childs.Add(Counter);
            Condition = condition;
            Childs.Add(Condition);
            Initializer = initializer;
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
            Childs.Add(ElseBlock);
            IfBlock = ifBlock;
            Childs.Add(IfBlock);
            Condition = condition;
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
    public partial class PostIncrementExpression : Expression
    {
        public PostIncrementExpression(ILineInfo lineInfo, Name name): base(lineInfo)
        {
            var slot = 0;
            Name = name;
            Childs.Add(Name);
        }

        public Name Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostIncrementExpression(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class PostDecrementExpression : Expression
    {
        public PostDecrementExpression(ILineInfo lineInfo, Name name): base(lineInfo)
        {
            var slot = 0;
            Name = name;
            Childs.Add(Name);
        }

        public Name Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostDecrementExpression(this);
        }
    }
}