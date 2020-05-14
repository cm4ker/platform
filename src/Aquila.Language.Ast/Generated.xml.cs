using System;
using System.Collections;
using System.Collections.Generic;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Definitions
{
    public class FunctionList : SyntaxCollectionNode<Function>
    {
        public static FunctionList Empty => new FunctionList();
        public FunctionList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFunctionList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFunctionList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class FieldList : SyntaxCollectionNode<Field>
    {
        public static FieldList Empty => new FieldList();
        public FieldList(): base(null)
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
    public class PropertyList : SyntaxCollectionNode<Property>
    {
        public static PropertyList Empty => new PropertyList();
        public PropertyList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPropertyList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPropertyList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class ConstructorList : SyntaxCollectionNode<Constructor>
    {
        public static ConstructorList Empty => new ConstructorList();
        public ConstructorList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitConstructorList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitConstructorList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class TypeList : SyntaxCollectionNode<TypeSyntax>
    {
        public static TypeList Empty => new TypeList();
        public TypeList(): base(null)
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
        public StatementList(): base(null)
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
        public ParameterList(): base(null)
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
    public class GenericParameterList : SyntaxCollectionNode<GenericParameter>
    {
        public static GenericParameterList Empty => new GenericParameterList();
        public GenericParameterList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGenericParameterList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitGenericParameterList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class AttributeList : SyntaxCollectionNode<AttributeSyntax>
    {
        public static AttributeList Empty => new AttributeList();
        public AttributeList(): base(null)
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
        public ArgumentList(): base(null)
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
    public class MatchAtomList : SyntaxCollectionNode<MatchAtom>
    {
        public static MatchAtomList Empty => new MatchAtomList();
        public MatchAtomList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMatchAtomList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMatchAtomList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class UsingList : SyntaxCollectionNode<UsingBase>
    {
        public static UsingList Empty => new UsingList();
        public UsingList(): base(null)
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
    public class EntityList : SyntaxCollectionNode<TypeEntity>
    {
        public static EntityList Empty => new EntityList();
        public EntityList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitEntityList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitEntityList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class NamespaceDeclarationList : SyntaxCollectionNode<NamespaceDeclaration>
    {
        public static NamespaceDeclarationList Empty => new NamespaceDeclarationList();
        public NamespaceDeclarationList(): base(null)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNamespaceDeclarationList(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNamespaceDeclarationList(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public class CompilationUnitList : SyntaxCollectionNode<CompilationUnit>
    {
        public static CompilationUnitList Empty => new CompilationUnitList();
        public CompilationUnitList(): base(null)
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
                return (CompilationUnitList)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitRoot(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitRoot(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (UsingList)this.Children[0];
            }
        }

        public EntityList Entityes
        {
            get
            {
                return (EntityList)this.Children[1];
            }
        }

        public NamespaceDeclarationList NamespaceDeclarations
        {
            get
            {
                return (NamespaceDeclarationList)this.Children[2];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNamespaceDeclaration(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNamespaceDeclaration(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (UsingList)this.Children[0];
            }
        }

        public EntityList Entityes
        {
            get
            {
                return (EntityList)this.Children[1];
            }
        }

        public NamespaceDeclarationList NamespaceDeclarations
        {
            get
            {
                return (NamespaceDeclarationList)this.Children[2];
            }
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingAliasDeclaration(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (FunctionList)this.Children[0];
            }
        }

        public FieldList Fields
        {
            get
            {
                return (FieldList)this.Children[1];
            }
        }

        public PropertyList Properties
        {
            get
            {
                return (PropertyList)this.Children[2];
            }
        }

        public ConstructorList Constructors
        {
            get
            {
                return (ConstructorList)this.Children[3];
            }
        }

        public UsingList Usings
        {
            get
            {
                return (UsingList)this.Children[4];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeBody(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitTypeBody(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (TypeBody)this.Children[0];
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
                return (TypeSyntax)this.Children[1];
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

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitClass(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitModule(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
        public Statement(ILineInfo lineInfo): base(lineInfo)
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

        public BinaryOperatorType BinaryOperatorType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBinaryExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
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
                return (Expression)this.Children[0];
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

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
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
                return (Expression)this.Children[0];
            }
        }

        public TypeSyntax CastType
        {
            get
            {
                return (TypeSyntax)this.Children[1];
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCastExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
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

        public UnaryOperatorType OperaotrType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIndexerExpression(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIndexerExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
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
                return (Expression)this.Children[0];
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitLogicalOrArithmeticExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
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
                return (Expression)this.Children[0];
            }
        }

        public Expression Index
        {
            get
            {
                return (Expression)this.Children[1];
            }
        }

        public ICanBeAssigned Assignable
        {
            get
            {
                return (ICanBeAssigned)this.Children[2];
            }
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
    public abstract partial class TypeSyntax : SyntaxNode
    {
        public TypeSyntax(ILineInfo lineInfo): base(lineInfo)
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
    public partial class PrimitiveTypeSyntax : TypeSyntax
    {
        public PrimitiveTypeSyntax(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPrimitiveTypeSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPrimitiveTypeSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitSingleTypeSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitArrayTypeSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUnionTypeSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (TypeList)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGenericTypeSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitGenericTypeSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (StatementList)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBlock(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBlock(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
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
        public GenericParameter(ILineInfo lineInfo, String name): base(lineInfo)
        {
            Name = name;
        }

        public String Name
        {
            get;
        }

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
    public partial class AttributeSyntax : SyntaxNode
    {
        public AttributeSyntax(ILineInfo lineInfo): base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAttributeSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAttributeSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
{
    public partial class Function : Member, IScoped, IAstSymbol
    {
        public Function(ILineInfo lineInfo, Block block, ParameterList parameters, GenericParameterList genericParameters, AttributeList attributes, String name, TypeSyntax type): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)parameters);
            this.Attach(2, (SyntaxNode)genericParameters);
            this.Attach(3, (SyntaxNode)attributes);
            Name = name;
            this.Attach(4, (SyntaxNode)type);
        }

        public Block Block
        {
            get
            {
                return (Block)this.Children[0];
            }
        }

        public ParameterList Parameters
        {
            get
            {
                return (ParameterList)this.Children[1];
            }
        }

        public GenericParameterList GenericParameters
        {
            get
            {
                return (GenericParameterList)this.Children[2];
            }
        }

        public AttributeList Attributes
        {
            get
            {
                return (AttributeList)this.Children[3];
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
                return (TypeSyntax)this.Children[4];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFunction(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFunction(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (Block)this.Children[0];
            }
        }

        public ParameterList Parameters
        {
            get
            {
                return (ParameterList)this.Children[1];
            }
        }

        public AttributeList Attributes
        {
            get
            {
                return (AttributeList)this.Children[2];
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitConstructor(this);
        }

        public SymbolTable SymbolTable
        {
            get;
            set;
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitField(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (TypeSyntax)this.Children[0];
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitProperty(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
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

namespace Aquila.Language.Ast.Definitions
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
                return (ArgumentList)this.Children[0];
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
                return (Expression)this.Children[1];
            }
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
        public New(ILineInfo lineInfo,  string  @namespace, Call call): base(lineInfo)
        {
            Namespace = @namespace;
            this.Attach(0, (SyntaxNode)call);
        }

        public string Namespace
        {
            get;
        }

        public Call Call
        {
            get
            {
                return (Call)this.Children[0];
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

namespace Aquila.Language.Ast.Definitions.Statements
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
                return (Expression)this.Children[0];
            }
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

namespace Aquila.Language.Ast.Definitions
{
    public partial class Variable : Expression, IAstSymbol
    {
        public Variable(ILineInfo lineInfo, Expression value, String name, TypeSyntax variableType): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)value);
            Name = name;
            this.Attach(1, (SyntaxNode)variableType);
        }

        public Expression Value
        {
            get
            {
                return (Expression)this.Children[0];
            }
        }

        public String Name
        {
            get;
        }

        public TypeSyntax VariableType
        {
            get
            {
                return (TypeSyntax)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitVariable(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitVariable(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (TypeSyntax)this.Children[0];
            }
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
        public Literal(ILineInfo lineInfo, String value,  bool  isSqlLiteral): base(lineInfo)
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitName(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (Expression)this.Children[0];
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitGetFieldExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (Expression)this.Children[0];
            }
        }

        public Expression Current
        {
            get
            {
                return (Expression)this.Children[1];
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

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPropertyLookupExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMethodLookupExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
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
                return (Expression)this.Children[0];
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAssignFieldExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
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
                return (Expression)this.Children[0];
            }
        }

        public Block Block
        {
            get
            {
                return (Block)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitDoWhile(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitDoWhile(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
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
                return (Block)this.Children[0];
            }
        }

        public Block CatchBlock
        {
            get
            {
                return (Block)this.Children[1];
            }
        }

        public Block FinallyBlock
        {
            get
            {
                return (Block)this.Children[2];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTry(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitTry(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
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
                return (Expression)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitExpressionStatement(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
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
                return (Block)this.Children[0];
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
        public While(ILineInfo lineInfo, Block block, Expression condition): base(lineInfo)
        {
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)condition);
        }

        public Block Block
        {
            get
            {
                return (Block)this.Children[0];
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
                return (Block)this.Children[0];
            }
        }

        public Block IfBlock
        {
            get
            {
                return (Block)this.Children[1];
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

namespace Aquila.Language.Ast.Definitions.Expressions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPostIncrementExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
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

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPostDecrementExpression(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
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
                return (Expression)this.Children[0];
            }
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

namespace Aquila.Language.Ast.Definitions.Statements
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
                return (Block)this.Children[0];
            }
        }

        public Expression Expression
        {
            get
            {
                return (Expression)this.Children[1];
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return (TypeSyntax)this.Children[2];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMatchAtom(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMatchAtom(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
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
                return (MatchAtomList)this.Children[0];
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
            return visitor.VisitMatch(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMatch(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
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
                return (Expression)this.Children[0];
            }
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

namespace Aquila.Language.Ast
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

        public virtual T VisitGenericParameterList(GenericParameterList arg)
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

        public virtual T VisitGenericParameter(GenericParameter arg)
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

        public virtual T VisitNew(New arg)
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

        public virtual T VisitWhile(While arg)
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

    public abstract partial class AstVisitorBase
    {
        public virtual void VisitFunctionList(FunctionList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitFieldList(FieldList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPropertyList(PropertyList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitConstructorList(ConstructorList arg)
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

        public virtual void VisitGenericParameterList(GenericParameterList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAttributeList(AttributeList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArgumentList(ArgumentList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMatchAtomList(MatchAtomList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitUsingList(UsingList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitEntityList(EntityList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNamespaceDeclarationList(NamespaceDeclarationList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCompilationUnitList(CompilationUnitList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitRoot(Root arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNamespaceDeclaration(NamespaceDeclaration arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCompilationUnit(CompilationUnit arg)
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

        public virtual void VisitTypeBody(TypeBody arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitClass(Class arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitModule(Module arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBinaryExpression(BinaryExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCastExpression(CastExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitIndexerExpression(IndexerExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitLogicalOrArithmeticExpression(LogicalOrArithmeticExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAssignment(Assignment arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPrimitiveTypeSyntax(PrimitiveTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitSingleTypeSyntax(SingleTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArrayTypeSyntax(ArrayTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitUnionTypeSyntax(UnionTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitGenericTypeSyntax(GenericTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBlock(Block arg)
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

        public virtual void VisitAttributeSyntax(AttributeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitFunction(Function arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitConstructor(Constructor arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitField(Field arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitProperty(Property arg)
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

        public virtual void VisitVariable(Variable arg)
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

        public virtual void VisitName(Name arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitGetFieldExpression(GetFieldExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPropertyLookupExpression(PropertyLookupExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMethodLookupExpression(MethodLookupExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAssignFieldExpression(AssignFieldExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitDoWhile(DoWhile arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitTry(Try arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitExpressionStatement(ExpressionStatement arg)
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

        public virtual void VisitPostIncrementExpression(PostIncrementExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPostDecrementExpression(PostDecrementExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitThrow(Throw arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMatchAtom(MatchAtom arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMatch(Match arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitGlobalVar(GlobalVar arg)
        {
            DefaultVisit(arg);
        }
    }
}
