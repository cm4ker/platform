using System;
using Aquila.Compiler.Syntax;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Misc;

namespace Aquila.Language.Ast.Definitions
{
    public class MethodList : SyntaxCollectionNode<MethodDeclarationSyntax>
    {
        public static MethodList Empty => new MethodList();
        public MethodList(): base(null)
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
    public class FieldList : SyntaxCollectionNode<FieldDeclarationSyntax>
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
    public class StatementList : SyntaxCollectionNode<StatementSyntax>
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
    public class ParameterList : SyntaxCollectionNode<ParameterSyntax>
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
    public class GenericParameterList : SyntaxCollectionNode<GenericParameterSyntax>
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
    public class ArgumentList : SyntaxCollectionNode<ArgumentSyntax>
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
    public class MatchAtomList : SyntaxCollectionNode<MatchAtomSyntax>
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
    public class DeclaratorList : SyntaxCollectionNode<VariableDeclaratorSyntax>
    {
        public static DeclaratorList Empty => new DeclaratorList();
        public DeclaratorList(): base(null)
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
    public class CompilationUnitList : SyntaxCollectionNode<CompilationUnitSyntax>
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
    public partial class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, UsingList usings, MethodList methods, FieldList fields): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)usings);
            this.Attach(1, (SyntaxNode)methods);
            this.Attach(2, (SyntaxNode)fields);
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
            return visitor.VisitCompilationUnitSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCompilationUnitSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class IdentifierToken : SyntaxToken
    {
        public IdentifierToken(ILineInfo lineInfo, SyntaxKind syntaxKind, String text): base(lineInfo, syntaxKind, text)
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
    public partial class IdentifierName : ExpressionSyntax
    {
        public IdentifierName(ILineInfo lineInfo, SyntaxKind syntaxKind, IdentifierToken identifier): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)identifier);
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
        public UsingBase(ILineInfo lineInfo, SyntaxKind syntaxKind, String name): base(lineInfo, syntaxKind)
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
    public partial class UsingDeclarationSyntax : UsingBase
    {
        public UsingDeclarationSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, String name): base(lineInfo, syntaxKind, name)
        {
            Name = name;
        }

        public String Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitUsingDeclarationSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingDeclarationSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class UsingAliasDeclarationSyntax : UsingBase
    {
        public UsingAliasDeclarationSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, String className, String alias): base(lineInfo, syntaxKind, className)
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
            return visitor.VisitUsingAliasDeclarationSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitUsingAliasDeclarationSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public abstract partial class ExpressionSyntax : SyntaxNode
    {
        public ExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind): base(lineInfo, syntaxKind)
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
    public abstract partial class StatementSyntax : SyntaxNode
    {
        public StatementSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind): base(lineInfo, syntaxKind)
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
    public partial class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax right, ExpressionSyntax left, BinaryOperatorType binaryOperatorType): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)right);
            this.Attach(1, (SyntaxNode)left);
            BinaryOperatorType = binaryOperatorType;
        }

        public ExpressionSyntax Right
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public ExpressionSyntax Left
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
            }
        }

        public BinaryOperatorType BinaryOperatorType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBinaryExpressionSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBinaryExpressionSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public abstract partial class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression, UnaryOperatorType operaotrType): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)expression);
            OperaotrType = operaotrType;
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
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
    public partial class CastExpressionSyntax : UnaryExpressionSyntax
    {
        public CastExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression, TypeSyntax castType, UnaryOperatorType operaotrType): base(lineInfo, syntaxKind, expression, operaotrType)
        {
            this.Attach(0, (SyntaxNode)expression);
            this.Attach(1, (SyntaxNode)castType);
            OperaotrType = operaotrType;
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
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
            return visitor.VisitCastExpressionSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCastExpressionSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class IndexerExpressionSyntax : UnaryExpressionSyntax
    {
        public IndexerExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax indexer, ExpressionSyntax expression, UnaryOperatorType operaotrType): base(lineInfo, syntaxKind, expression, operaotrType)
        {
            this.Attach(0, (SyntaxNode)indexer);
            this.Attach(1, (SyntaxNode)expression);
            OperaotrType = operaotrType;
        }

        public ExpressionSyntax Indexer
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
            }
        }

        public UnaryOperatorType OperaotrType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIndexerExpressionSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitIndexerExpressionSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class LogicalOrArithmeticExpressionSyntax : UnaryExpressionSyntax
    {
        public LogicalOrArithmeticExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression, UnaryOperatorType operaotrType): base(lineInfo, syntaxKind, expression, operaotrType)
        {
            this.Attach(0, (SyntaxNode)expression);
            OperaotrType = operaotrType;
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public UnaryOperatorType OperaotrType
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLogicalOrArithmeticExpressionSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitLogicalOrArithmeticExpressionSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class AssignmentSyntax : ExpressionSyntax
    {
        public AssignmentSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax value, ExpressionSyntax assignable): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)value);
            this.Attach(1, (SyntaxNode)assignable);
        }

        public ExpressionSyntax Value
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public ExpressionSyntax Assignable
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAssignmentSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitAssignmentSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public abstract partial class TypeSyntax : ExpressionSyntax
    {
        public TypeSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind): base(lineInfo, syntaxKind)
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
    public partial class PredefinedTypeSyntax : TypeSyntax
    {
        public PredefinedTypeSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind): base(lineInfo, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPredefinedTypeSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPredefinedTypeSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class NamedTypeSyntax : TypeSyntax
    {
        public NamedTypeSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind,  string  value = "?"): base(lineInfo, syntaxKind)
        {
            Value = value;
        }

        public string Value
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNamedTypeSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNamedTypeSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class ArrayTypeSyntax : TypeSyntax
    {
        public ArrayTypeSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, TypeSyntax type): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)type);
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
    public partial class GenericTypeSyntax : TypeSyntax
    {
        public GenericTypeSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, TypeList args): base(lineInfo, syntaxKind)
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
    public partial class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, StatementList statements): base(lineInfo, syntaxKind)
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
            return visitor.VisitBlockStatementSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBlockStatementSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
{
    public partial class ParameterSyntax : SyntaxNode
    {
        public ParameterSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, TypeSyntax type, IdentifierToken identifier, PassMethod passMethod = PassMethod.ByValue): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)type);
            this.Attach(1, (SyntaxNode)identifier);
            PassMethod = passMethod;
        }

        public TypeSyntax Type
        {
            get
            {
                return (TypeSyntax)this.Children[0];
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
            return visitor.VisitParameterSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitParameterSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
{
    public partial class GenericParameterSyntax : SyntaxNode
    {
        public GenericParameterSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, String name): base(lineInfo, syntaxKind)
        {
            Name = name;
        }

        public String Name
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitGenericParameterSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitGenericParameterSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class AttributeSyntax : SyntaxNode
    {
        public AttributeSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ArgumentList arguments, IdentifierToken identifier): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)arguments);
            this.Attach(1, (SyntaxNode)identifier);
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
    public abstract partial class MemberSyntax : SyntaxNode
    {
        public MemberSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind): base(lineInfo, syntaxKind)
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
    public partial class MethodDeclarationSyntax : MemberSyntax
    {
        public MethodDeclarationSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, BlockStatementSyntax block, ParameterList parameters, GenericParameterList genericParameters, AttributeList attributes, IdentifierToken identifier, TypeSyntax type): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)parameters);
            this.Attach(2, (SyntaxNode)genericParameters);
            this.Attach(3, (SyntaxNode)attributes);
            this.Attach(4, (SyntaxNode)identifier);
            this.Attach(5, (SyntaxNode)type);
        }

        public BlockStatementSyntax Block
        {
            get
            {
                return (BlockStatementSyntax)this.Children[0];
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

        public IdentifierToken Identifier
        {
            get
            {
                return (IdentifierToken)this.Children[4];
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return (TypeSyntax)this.Children[5];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMethodDeclarationSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMethodDeclarationSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class FieldDeclarationSyntax : MemberSyntax
    {
        public FieldDeclarationSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, IdentifierToken identifier, TypeSyntax type): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)identifier);
            this.Attach(1, (SyntaxNode)type);
        }

        public IdentifierToken Identifier
        {
            get
            {
                return (IdentifierToken)this.Children[0];
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return (TypeSyntax)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFieldDeclarationSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitFieldDeclarationSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Functions
{
    public partial class ArgumentSyntax : SyntaxNode
    {
        public ArgumentSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression, PassMethod passMethod = PassMethod.ByValue): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)expression);
            PassMethod = passMethod;
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public PassMethod PassMethod
        {
            get;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArgumentSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitArgumentSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class CallSyntax : ExpressionSyntax
    {
        public CallSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ArgumentList arguments, ExpressionSyntax expression): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)arguments);
            this.Attach(1, (SyntaxNode)expression);
        }

        public ArgumentList Arguments
        {
            get
            {
                return (ArgumentList)this.Children[0];
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCallSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitCallSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class NewSyntax : ExpressionSyntax
    {
        public NewSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind,  string  @namespace, CallSyntax call): base(lineInfo, syntaxKind)
        {
            Namespace = @namespace;
            this.Attach(0, (SyntaxNode)call);
        }

        public string Namespace
        {
            get;
        }

        public CallSyntax Call
        {
            get
            {
                return (CallSyntax)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitNewSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitNewSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class ReturnSyntax : StatementSyntax
    {
        public ReturnSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)expression);
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitReturnSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitReturnSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class BreakSyntax : StatementSyntax
    {
        public BreakSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind): base(lineInfo, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBreakSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitBreakSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class ContinueSyntax : StatementSyntax
    {
        public ContinueSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind): base(lineInfo, syntaxKind)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitContinueSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitContinueSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, TypeSyntax variableType, DeclaratorList declarators): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)variableType);
            this.Attach(1, (SyntaxNode)declarators);
        }

        public TypeSyntax VariableType
        {
            get
            {
                return (TypeSyntax)this.Children[0];
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
            return visitor.VisitVariableDeclarationSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitVariableDeclarationSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class VariableDeclaratorSyntax : StatementSyntax
    {
        public VariableDeclaratorSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax initializer, IdentifierToken identifier): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)initializer);
            this.Attach(1, (SyntaxNode)identifier);
        }

        public ExpressionSyntax Initializer
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
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
            return visitor.VisitVariableDeclaratorSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitVariableDeclaratorSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class ContextVariable : ExpressionSyntax
    {
        public ContextVariable(ILineInfo lineInfo, SyntaxKind syntaxKind, String name, TypeSyntax type): base(lineInfo, syntaxKind)
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
    public partial class Literal : ExpressionSyntax
    {
        public Literal(ILineInfo lineInfo, SyntaxKind syntaxKind, String value,  bool  isSqlLiteral): base(lineInfo, syntaxKind)
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
    public partial class GetFieldExpression : ExpressionSyntax
    {
        public GetFieldExpression(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression, String fieldName): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)expression);
            FieldName = fieldName;
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
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
    public abstract partial class LookupExpressionSyntax : ExpressionSyntax
    {
        public LookupExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax lookup, ExpressionSyntax current): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)lookup);
            this.Attach(1, (SyntaxNode)current);
        }

        public ExpressionSyntax Lookup
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public ExpressionSyntax Current
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
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
    public partial class PropertyLookupExpressionSyntax : LookupExpressionSyntax
    {
        public PropertyLookupExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax lookup, ExpressionSyntax current): base(lineInfo, syntaxKind, lookup, current)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPropertyLookupExpressionSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPropertyLookupExpressionSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class MethodLookupExpressionSyntax : LookupExpressionSyntax
    {
        public MethodLookupExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax lookup, ExpressionSyntax current): base(lineInfo, syntaxKind, lookup, current)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitMethodLookupExpressionSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMethodLookupExpressionSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions
{
    public partial class AssignFieldExpression : ExpressionSyntax
    {
        public AssignFieldExpression(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression, String fieldName): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)expression);
            FieldName = fieldName;
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
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
    public partial class DoWhile : StatementSyntax
    {
        public DoWhile(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax condition, BlockStatementSyntax block): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)condition);
            this.Attach(1, (SyntaxNode)block);
        }

        public ExpressionSyntax Condition
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public BlockStatementSyntax Block
        {
            get
            {
                return (BlockStatementSyntax)this.Children[1];
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
    public partial class Try : StatementSyntax
    {
        public Try(ILineInfo lineInfo, SyntaxKind syntaxKind, BlockStatementSyntax tryBlock, BlockStatementSyntax catchBlock, BlockStatementSyntax finallyBlock): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)tryBlock);
            this.Attach(1, (SyntaxNode)catchBlock);
            this.Attach(2, (SyntaxNode)finallyBlock);
        }

        public BlockStatementSyntax TryBlock
        {
            get
            {
                return (BlockStatementSyntax)this.Children[0];
            }
        }

        public BlockStatementSyntax CatchBlock
        {
            get
            {
                return (BlockStatementSyntax)this.Children[1];
            }
        }

        public BlockStatementSyntax FinallyBlock
        {
            get
            {
                return (BlockStatementSyntax)this.Children[2];
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
    public partial class ExpressionStatement : StatementSyntax
    {
        public ExpressionStatement(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)expression);
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
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
    public partial class For : StatementSyntax
    {
        public For(ILineInfo lineInfo, SyntaxKind syntaxKind, BlockStatementSyntax block, ExpressionSyntax counter, ExpressionSyntax condition, ExpressionSyntax initializer): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)counter);
            this.Attach(2, (SyntaxNode)condition);
            this.Attach(3, (SyntaxNode)initializer);
        }

        public BlockStatementSyntax Block
        {
            get
            {
                return (BlockStatementSyntax)this.Children[0];
            }
        }

        public ExpressionSyntax Counter
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
            }
        }

        public ExpressionSyntax Condition
        {
            get
            {
                return (ExpressionSyntax)this.Children[2];
            }
        }

        public ExpressionSyntax Initializer
        {
            get
            {
                return (ExpressionSyntax)this.Children[3];
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
    public partial class While : StatementSyntax
    {
        public While(ILineInfo lineInfo, SyntaxKind syntaxKind, BlockStatementSyntax block, ExpressionSyntax condition): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)condition);
        }

        public BlockStatementSyntax Block
        {
            get
            {
                return (BlockStatementSyntax)this.Children[0];
            }
        }

        public ExpressionSyntax Condition
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
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
    public partial class If : StatementSyntax
    {
        public If(ILineInfo lineInfo, SyntaxKind syntaxKind, StatementSyntax elseBlock, StatementSyntax thenBlock, ExpressionSyntax condition): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)elseBlock);
            this.Attach(1, (SyntaxNode)thenBlock);
            this.Attach(2, (SyntaxNode)condition);
        }

        public StatementSyntax ElseBlock
        {
            get
            {
                return (StatementSyntax)this.Children[0];
            }
        }

        public StatementSyntax ThenBlock
        {
            get
            {
                return (StatementSyntax)this.Children[1];
            }
        }

        public ExpressionSyntax Condition
        {
            get
            {
                return (ExpressionSyntax)this.Children[2];
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
    public abstract partial class PostOperationExpressionSyntax : ExpressionSyntax
    {
        public PostOperationExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)expression);
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
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
    public partial class PostIncrementExpressionSyntax : PostOperationExpressionSyntax
    {
        public PostIncrementExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression): base(lineInfo, syntaxKind, expression)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostIncrementExpressionSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPostIncrementExpressionSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class PostDecrementExpressionSyntax : PostOperationExpressionSyntax
    {
        public PostDecrementExpressionSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression): base(lineInfo, syntaxKind, expression)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostDecrementExpressionSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitPostDecrementExpressionSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class ThrowSyntax : ExpressionSyntax
    {
        public ThrowSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax exception): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)exception);
        }

        public ExpressionSyntax Exception
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitThrowSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitThrowSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class MatchAtomSyntax : SyntaxNode
    {
        public MatchAtomSyntax(ILineInfo lineInfo, SyntaxKind syntaxKind, BlockStatementSyntax block, ExpressionSyntax expression, TypeSyntax type): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)block);
            this.Attach(1, (SyntaxNode)expression);
            this.Attach(2, (SyntaxNode)type);
        }

        public BlockStatementSyntax Block
        {
            get
            {
                return (BlockStatementSyntax)this.Children[0];
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
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
            return visitor.VisitMatchAtomSyntax(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitMatchAtomSyntax(this);
        }
    }
}

namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class Match : StatementSyntax
    {
        public Match(ILineInfo lineInfo, SyntaxKind syntaxKind, MatchAtomList matches, ExpressionSyntax expression): base(lineInfo, syntaxKind)
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

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[1];
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
    public partial class GlobalVar : ExpressionSyntax
    {
        public GlobalVar(ILineInfo lineInfo, SyntaxKind syntaxKind, ExpressionSyntax expression): base(lineInfo, syntaxKind)
        {
            this.Attach(0, (SyntaxNode)expression);
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return (ExpressionSyntax)this.Children[0];
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

        public virtual T VisitCompilationUnitSyntax(CompilationUnitSyntax arg)
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

        public virtual T VisitUsingDeclarationSyntax(UsingDeclarationSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitUsingAliasDeclarationSyntax(UsingAliasDeclarationSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBinaryExpressionSyntax(BinaryExpressionSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCastExpressionSyntax(CastExpressionSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitIndexerExpressionSyntax(IndexerExpressionSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitLogicalOrArithmeticExpressionSyntax(LogicalOrArithmeticExpressionSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAssignmentSyntax(AssignmentSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPredefinedTypeSyntax(PredefinedTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNamedTypeSyntax(NamedTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArrayTypeSyntax(ArrayTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitGenericTypeSyntax(GenericTypeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBlockStatementSyntax(BlockStatementSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitParameterSyntax(ParameterSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitGenericParameterSyntax(GenericParameterSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitAttributeSyntax(AttributeSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMethodDeclarationSyntax(MethodDeclarationSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitFieldDeclarationSyntax(FieldDeclarationSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitArgumentSyntax(ArgumentSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitCallSyntax(CallSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitNewSyntax(NewSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitReturnSyntax(ReturnSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitBreakSyntax(BreakSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitContinueSyntax(ContinueSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitVariableDeclarationSyntax(VariableDeclarationSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitVariableDeclaratorSyntax(VariableDeclaratorSyntax arg)
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

        public virtual T VisitGetFieldExpression(GetFieldExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPropertyLookupExpressionSyntax(PropertyLookupExpressionSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMethodLookupExpressionSyntax(MethodLookupExpressionSyntax arg)
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

        public virtual T VisitPostIncrementExpressionSyntax(PostIncrementExpressionSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitPostDecrementExpressionSyntax(PostDecrementExpressionSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitThrowSyntax(ThrowSyntax arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitMatchAtomSyntax(MatchAtomSyntax arg)
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

        public virtual void VisitCompilationUnitSyntax(CompilationUnitSyntax arg)
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

        public virtual void VisitUsingDeclarationSyntax(UsingDeclarationSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitUsingAliasDeclarationSyntax(UsingAliasDeclarationSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBinaryExpressionSyntax(BinaryExpressionSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCastExpressionSyntax(CastExpressionSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitIndexerExpressionSyntax(IndexerExpressionSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitLogicalOrArithmeticExpressionSyntax(LogicalOrArithmeticExpressionSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAssignmentSyntax(AssignmentSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPredefinedTypeSyntax(PredefinedTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNamedTypeSyntax(NamedTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArrayTypeSyntax(ArrayTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitGenericTypeSyntax(GenericTypeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBlockStatementSyntax(BlockStatementSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitParameterSyntax(ParameterSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitGenericParameterSyntax(GenericParameterSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitAttributeSyntax(AttributeSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMethodDeclarationSyntax(MethodDeclarationSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitFieldDeclarationSyntax(FieldDeclarationSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitArgumentSyntax(ArgumentSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitCallSyntax(CallSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitNewSyntax(NewSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitReturnSyntax(ReturnSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitBreakSyntax(BreakSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitContinueSyntax(ContinueSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitVariableDeclarationSyntax(VariableDeclarationSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitVariableDeclaratorSyntax(VariableDeclaratorSyntax arg)
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

        public virtual void VisitGetFieldExpression(GetFieldExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPropertyLookupExpressionSyntax(PropertyLookupExpressionSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMethodLookupExpressionSyntax(MethodLookupExpressionSyntax arg)
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

        public virtual void VisitPostIncrementExpressionSyntax(PostIncrementExpressionSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitPostDecrementExpressionSyntax(PostDecrementExpressionSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitThrowSyntax(ThrowSyntax arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitMatchAtomSyntax(MatchAtomSyntax arg)
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
