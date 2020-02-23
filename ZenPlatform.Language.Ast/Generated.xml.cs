using System;
using System.Collections;
using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions
{
    public class FunctionList : SyntaxCollectionNode<Function>
    {
        public FunctionList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFunctionList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class FieldList : SyntaxCollectionNode<Field>
    {
        public FieldList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFieldList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class PropertyList : SyntaxCollectionNode<Property>
    {
        public PropertyList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPropertyList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class ConstructorList : SyntaxCollectionNode<Constructor>
    {
        public ConstructorList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitConstructorList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class TypeList : SyntaxCollectionNode<TypeSyntax>
    {
        public TypeList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class StatementList : SyntaxCollectionNode<Statement>
    {
        public StatementList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitStatementList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class ParameterList : SyntaxCollectionNode<Parameter>
    {
        public ParameterList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitParameterList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class AttributeList : SyntaxCollectionNode<AttributeSyntax>
    {
        public AttributeList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAttributeList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class ArgumentList : SyntaxCollectionNode<Argument>
    {
        public ArgumentList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArgumentList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class MatchAtomList : SyntaxCollectionNode<MatchAtom>
    {
        public MatchAtomList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMatchAtomList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class UsingList : SyntaxCollectionNode<UsingBase>
    {
        public UsingList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class EntityList : SyntaxCollectionNode<TypeEntity>
    {
        public EntityList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitEntityList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class NamespaceDeclarationList : SyntaxCollectionNode<NamespaceDeclaration>
    {
        public NamespaceDeclarationList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNamespaceDeclarationList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public class CompilationUnitList : SyntaxCollectionNode<CompilationUnit>
    {
        public CompilationUnitList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCompilationUnitList(this);
        }
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Root : SyntaxNode, IScoped
    {
        public Root(ILineInfo lineInfo, CompilationUnitList units): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)units);
        }

        public CompilationUnitList Units
        {
            get
            {
                return (CompilationUnitList)this.Childs[0];
            }
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
    public partial class NamespaceDeclaration : SyntaxNode, IScoped
    {
        public NamespaceDeclaration(ILineInfo lineInfo, String name, UsingList usings, EntityList entityes, NamespaceDeclarationList namespaceDeclarations): base(lineInfo)
        {
            Name = name;
            this.Attach(0, (SyntaxNode)usings);
            this.Attach(1, (SyntaxNode)entityes);
            this.Attach(2, (SyntaxNode)namespaceDeclarations);
        }

        public String Name
        {
            get;
        }

        public UsingList Usings
        {
            get
            {
                return (UsingList)this.Childs[0];
            }
        }

        public EntityList Entityes
        {
            get
            {
                return (EntityList)this.Childs[1];
            }
        }

        public NamespaceDeclarationList NamespaceDeclarations
        {
            get
            {
                return (NamespaceDeclarationList)this.Childs[2];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNamespaceDeclaration(this);
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
        public CompilationUnit(ILineInfo lineInfo, UsingList usings, EntityList entityes, NamespaceDeclarationList namespaceDeclarations): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)usings);
            this.Attach(1, (SyntaxNode)entityes);
            this.Attach(2, (SyntaxNode)namespaceDeclarations);
        }

        public UsingList Usings
        {
            get
            {
                return (UsingList)this.Childs[0];
            }
        }

        public EntityList Entityes
        {
            get
            {
                return (EntityList)this.Childs[1];
            }
        }

        public NamespaceDeclarationList NamespaceDeclarations
        {
            get
            {
                return (NamespaceDeclarationList)this.Childs[2];
            }
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
        public TypeBody(ILineInfo lineInfo, FunctionList functions, FieldList fields, PropertyList properties, ConstructorList constructors, UsingList usings): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)functions);
            this.Attach(1, (SyntaxNode)fields);
            this.Attach(2, (SyntaxNode)properties);
            this.Attach(3, (SyntaxNode)constructors);
            this.Attach(4, (SyntaxNode)usings);
        }

        public FunctionList Functions
        {
            get
            {
                return (FunctionList)this.Childs[0];
            }
        }

        public FieldList Fields
        {
            get
            {
                return (FieldList)this.Childs[1];
            }
        }

        public PropertyList Properties
        {
            get
            {
                return (PropertyList)this.Childs[2];
            }
        }

        public ConstructorList Constructors
        {
            get
            {
                return (ConstructorList)this.Childs[3];
            }
        }

        public UsingList Usings
        {
            get
            {
                return (UsingList)this.Childs[4];
            }
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
            this.Attach(0, (SyntaxNode)typeBody);
            Name = name;
            this.Attach(1, (SyntaxNode)@base);
        }

        public TypeBody TypeBody
        {
            get
            {
                return (TypeBody)this.Childs[0];
            }
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Base
        {
            get
            {
                return (TypeSyntax)this.Childs[1];
            }
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
            this.Attach(0, (SyntaxNode)right);
            this.Attach(1, (SyntaxNode)left);
            BinaryOperatorType = binaryOperatorType;
        }

        public Expression Right
        {
            get
            {
                return (Expression)this.Childs[0];
            }
        }

        public Expression Left
        {
            get
            {
                return (Expression)this.Childs[1];
            }
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
            this.Attach(0, (SyntaxNode)expression);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)expression);
            this.Attach(1, (SyntaxNode)castType);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
        }

        public TypeSyntax CastType
        {
            get
            {
                return (TypeSyntax)this.Childs[1];
            }
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
            this.Attach(0, (SyntaxNode)indexer);
            this.Attach(1, (SyntaxNode)expression);
            OperaotrType = operaotrType;
        }

        public Expression Indexer
        {
            get
            {
                return (Expression)this.Childs[0];
            }
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[1];
            }
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
            this.Attach(0, (SyntaxNode)expression);
            OperaotrType = operaotrType;
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)value);
            this.Attach(1, (SyntaxNode)index);
            this.Attach(2, (SyntaxNode)assignable);
        }

        public Expression Value
        {
            get
            {
                return (Expression)this.Childs[0];
            }
        }

        public Expression Index
        {
            get
            {
                return (Expression)this.Childs[1];
            }
        }

        public ICanBeAssigned Assignable
        {
            get
            {
                return (ICanBeAssigned)this.Childs[2];
            }
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
        public GenericTypeSyntax(ILineInfo lineInfo, TypeList args): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)args);
        }

        public TypeList Args
        {
            get
            {
                return (TypeList)this.Childs[0];
            }
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
        public Block(ILineInfo lineInfo, StatementList statements): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)statements);
        }

        public StatementList Statements
        {
            get
            {
                return (StatementList)this.Childs[0];
            }
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
    public partial class AttributeSyntax : SyntaxNode
    {
        public AttributeSyntax(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAttributeSyntax(this);
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
        public Function(ILineInfo lineInfo, Block block, ParameterList parameters, AttributeList attributes, String name, TypeSyntax type): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)parameters);
            this.Attach(2, (SyntaxNode)attributes);
            Name = name;
            Type = type;
        }

        public Block Block
        {
            get
            {
                return (Block)this.Childs[0];
            }
        }

        public ParameterList Parameters
        {
            get
            {
                return (ParameterList)this.Childs[1];
            }
        }

        public AttributeList Attributes
        {
            get
            {
                return (AttributeList)this.Childs[2];
            }
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
        public Constructor(ILineInfo lineInfo, Block block, ParameterList parameters, AttributeList attributes, String name): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)parameters);
            this.Attach(2, (SyntaxNode)attributes);
            Name = name;
        }

        public Block Block
        {
            get
            {
                return (Block)this.Childs[0];
            }
        }

        public ParameterList Parameters
        {
            get
            {
                return (ParameterList)this.Childs[1];
            }
        }

        public AttributeList Attributes
        {
            get
            {
                return (AttributeList)this.Childs[2];
            }
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
            Name = name;
            this.Attach(0, (SyntaxNode)type);
            MapTo = mapTo;
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Type
        {
            get
            {
                return (TypeSyntax)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)expression);
            PassMethod = passMethod;
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
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
    }
}

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Call : Expression
    {
        public Call(ILineInfo lineInfo, ArgumentList arguments, Name name, Expression expression): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)arguments);
            Name = name;
            this.Attach(1, (SyntaxNode)expression);
        }

        public ArgumentList Arguments
        {
            get
            {
                return (ArgumentList)this.Childs[0];
            }
        }

        public Name Name
        {
            get;
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[1];
            }
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
            this.Attach(0, (SyntaxNode)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)value);
            Name = name;
            this.Attach(1, (SyntaxNode)type);
        }

        public Expression Value
        {
            get
            {
                return (Expression)this.Childs[0];
            }
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Type
        {
            get
            {
                return (TypeSyntax)this.Childs[1];
            }
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
            Name = name;
            this.Attach(0, (SyntaxNode)type);
        }

        public String Name
        {
            get;
        }

        public TypeSyntax Type
        {
            get
            {
                return (TypeSyntax)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)expression);
            FieldName = fieldName;
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)lookup);
            this.Attach(1, (SyntaxNode)current);
        }

        public Expression Lookup
        {
            get
            {
                return (Expression)this.Childs[0];
            }
        }

        public Expression Current
        {
            get
            {
                return (Expression)this.Childs[1];
            }
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
            this.Attach(0, (SyntaxNode)expression);
            FieldName = fieldName;
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)condition);
            this.Attach(1, (SyntaxNode)block);
        }

        public Expression Condition
        {
            get
            {
                return (Expression)this.Childs[0];
            }
        }

        public Block Block
        {
            get
            {
                return (Block)this.Childs[1];
            }
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
            this.Attach(0, (SyntaxNode)tryBlock);
            this.Attach(1, (SyntaxNode)catchBlock);
            this.Attach(2, (SyntaxNode)finallyBlock);
        }

        public Block TryBlock
        {
            get
            {
                return (Block)this.Childs[0];
            }
        }

        public Block CatchBlock
        {
            get
            {
                return (Block)this.Childs[1];
            }
        }

        public Block FinallyBlock
        {
            get
            {
                return (Block)this.Childs[2];
            }
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
            this.Attach(0, (SyntaxNode)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)counter);
            this.Attach(2, (SyntaxNode)condition);
            this.Attach(3, (SyntaxNode)initializer);
        }

        public Block Block
        {
            get
            {
                return (Block)this.Childs[0];
            }
        }

        public Expression Counter
        {
            get
            {
                return (Expression)this.Childs[1];
            }
        }

        public Expression Condition
        {
            get
            {
                return (Expression)this.Childs[2];
            }
        }

        public Expression Initializer
        {
            get
            {
                return (Expression)this.Childs[3];
            }
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
            this.Attach(0, (SyntaxNode)elseBlock);
            this.Attach(1, (SyntaxNode)ifBlock);
            this.Attach(2, (SyntaxNode)condition);
        }

        public Block ElseBlock
        {
            get
            {
                return (Block)this.Childs[0];
            }
        }

        public Block IfBlock
        {
            get
            {
                return (Block)this.Childs[1];
            }
        }

        public Expression Condition
        {
            get
            {
                return (Expression)this.Childs[2];
            }
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
            this.Attach(0, (SyntaxNode)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)exception);
        }

        public Expression Exception
        {
            get
            {
                return (Expression)this.Childs[0];
            }
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
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)expression);
            this.Attach(2, (SyntaxNode)type);
        }

        public Block Block
        {
            get
            {
                return (Block)this.Childs[0];
            }
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[1];
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return (TypeSyntax)this.Childs[2];
            }
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
        public Match(ILineInfo lineInfo, MatchAtomList matches, Expression expression): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)matches);
            this.Attach(1, (SyntaxNode)expression);
        }

        public MatchAtomList Matches
        {
            get
            {
                return (MatchAtomList)this.Childs[0];
            }
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[1];
            }
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
            this.Attach(0, (SyntaxNode)expression);
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Childs[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGlobalVar(this);
        }
    }
}

namespace ZenPlatform.Language.Ast
{
    public abstract partial class AstVisitorBase<T>
    {
        public virtual T VisitFunctionList(FunctionList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitFieldList(FieldList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPropertyList(PropertyList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitConstructorList(ConstructorList arg)
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

        public virtual T VisitAttributeList(AttributeList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArgumentList(ArgumentList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMatchAtomList(MatchAtomList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitUsingList(UsingList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitEntityList(EntityList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNamespaceDeclarationList(NamespaceDeclarationList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCompilationUnitList(CompilationUnitList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitRoot(Root arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNamespaceDeclaration(NamespaceDeclaration arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCompilationUnit(CompilationUnit arg)
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

        public virtual T VisitTypeBody(TypeBody arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitClass(Class arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitModule(Module arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBinaryExpression(BinaryExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCastExpression(CastExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIndexerExpression(IndexerExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitLogicalOrArithmeticExpression(LogicalOrArithmeticExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAssignment(Assignment arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPrimitiveTypeSyntax(PrimitiveTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitSingleTypeSyntax(SingleTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArrayTypeSyntax(ArrayTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitUnionTypeSyntax(UnionTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitGenericTypeSyntax(GenericTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBlock(Block arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitParameter(Parameter arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAttributeSyntax(AttributeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitFunction(Function arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitConstructor(Constructor arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitField(Field arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitProperty(Property arg)
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

        public virtual T VisitReturn(Return arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitVariable(Variable arg)
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

        public virtual T VisitName(Name arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitGetFieldExpression(GetFieldExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPropertyLookupExpression(PropertyLookupExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMethodLookupExpression(MethodLookupExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAssignFieldExpression(AssignFieldExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitDoWhile(DoWhile arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitTry(Try arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitExpressionStatement(ExpressionStatement arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitFor(For arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIf(If arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPostIncrementExpression(PostIncrementExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPostDecrementExpression(PostDecrementExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitThrow(Throw arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMatchAtom(MatchAtom arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMatch(Match arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitGlobalVar(GlobalVar arg)
        {
            return DefaultVisit(arg);
        }
    }
}
