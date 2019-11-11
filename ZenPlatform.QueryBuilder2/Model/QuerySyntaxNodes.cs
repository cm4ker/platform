using System;
using System.Linq;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.QueryBuilder.Contracts;

namespace ZenPlatform.QueryBuilder.Model
{
    public abstract partial class QuerySyntaxNode : ZenPlatform.QueryBuilder.Common.SqlNode
    {
        public abstract T Accept<T>(QueryVisitorBase<T> visitor);
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class Expression : QuerySyntaxNode
    {
        public Expression()
        {
            Nodes = new List<QuerySyntaxNode>();
        }

        public List<QuerySyntaxNode> Nodes
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( Expression ) obj ;  return  ( SequenceEqual ( this . Nodes ,  node . Nodes ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Nodes, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpression(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class StringValue : QuerySyntaxNode
    {
        public StringValue()
        {
        }

        public string Value
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( StringValue ) obj ;  return  ( ( this . Value == node . Value ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Value.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitStringValue(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class Scheme : StringValue
    {
        public Scheme()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitScheme(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class Database : StringValue
    {
        public Database()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDatabase(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class Table : StringValue
    {
        public Table()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTable(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class Column : StringValue
    {
        public Column()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumn(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class Constraint : StringValue
    {
        public Constraint()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConstraint(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SchemeOperation : QuerySyntaxNode
    {
        public SchemeOperation()
        {
        }

        public Scheme Scheme
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SchemeOperation ) obj ;  return  ( Compare ( this . Scheme ,  node . Scheme ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Scheme == null ? 0 : Scheme.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSchemeOperation(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DatabaseOperation : SchemeOperation
    {
        public DatabaseOperation()
        {
        }

        public Database Database
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( DatabaseOperation ) obj ;  return  ( Compare ( this . Database ,  node . Database ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Database == null ? 0 : Database.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDatabaseOperation(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TableOperation : DatabaseOperation
    {
        public TableOperation()
        {
        }

        public Table Table
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( TableOperation ) obj ;  return  ( Compare ( this . Table ,  node . Table ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Table == null ? 0 : Table.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTableOperation(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnType : QuerySyntaxNode
    {
        public ColumnType()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnType(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeInt : ColumnType
    {
        public ColumnTypeInt()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeInt(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnDefinition : QuerySyntaxNode
    {
        public ColumnDefinition()
        {
        }

        public Column Column
        {
            get;
            set;
        }

        public ColumnType Type
        {
            get;
            set;
        }

        public bool IsNotNull
        {
            get;
            set;
        }

        public object DefaultValue
        {
            get;
            set;
        }

        public SystemMethods DefaultMethod
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ColumnDefinition ) obj ;  return  ( Compare ( this . Column ,  node . Column ) && Compare ( this . Type ,  node . Type ) && ( this . IsNotNull == node . IsNotNull ) && ( this . DefaultValue . Equals ( node . DefaultValue ) ) && ( this . DefaultMethod == node . DefaultMethod ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Column == null ? 0 : Column.GetHashCode()) ^ (Type == null ? 0 : Type.GetHashCode()) ^ (IsNotNull.GetHashCode()) ^ (DefaultValue == null ? 0 : DefaultValue.GetHashCode()) ^ (DefaultMethod.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnDefinition(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConstraintDefinition : QuerySyntaxNode
    {
        public ConstraintDefinition()
        {
        }

        public string Name
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConstraintDefinition ) obj ;  return  ( ( this . Name == node . Name ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Name.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConstraintDefinition(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConstraintDefinitionUnique : ConstraintDefinition
    {
        public ConstraintDefinitionUnique()
        {
            Columns = new List<Column>();
        }

        public List<Column> Columns
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConstraintDefinitionUnique ) obj ;  return  ( SequenceEqual ( this . Columns ,  node . Columns ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Columns, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConstraintDefinitionUnique(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConstraintDefinitionPrimaryKey : ConstraintDefinition
    {
        public ConstraintDefinitionPrimaryKey()
        {
            Columns = new List<Column>();
        }

        public List<Column> Columns
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConstraintDefinitionPrimaryKey ) obj ;  return  ( SequenceEqual ( this . Columns ,  node . Columns ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Columns, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConstraintDefinitionPrimaryKey(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConstraintDefinitionForeignKey : ConstraintDefinition
    {
        public ConstraintDefinitionForeignKey()
        {
            Columns = new List<Column>();
            ForeignColumns = new List<Column>();
        }

        public List<Column> Columns
        {
            get;
            set;
        }

        public List<Column> ForeignColumns
        {
            get;
            set;
        }

        public Table ForeignTable
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConstraintDefinitionForeignKey ) obj ;  return  ( SequenceEqual ( this . Columns ,  node . Columns ) && SequenceEqual ( this . ForeignColumns ,  node . ForeignColumns ) && Compare ( this . ForeignTable ,  node . ForeignTable ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Columns, i => i.GetHashCode()) ^ Xor(ForeignColumns, i => i.GetHashCode()) ^ (ForeignTable == null ? 0 : ForeignTable.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConstraintDefinitionForeignKey(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class CreateTable : TableOperation
    {
        public CreateTable()
        {
            Columns = new List<ColumnDefinition>();
            Constraints = new List<ConstraintDefinition>();
        }

        public List<ColumnDefinition> Columns
        {
            get;
            set;
        }

        public List<ConstraintDefinition> Constraints
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( CreateTable ) obj ;  return  ( SequenceEqual ( this . Columns ,  node . Columns ) && SequenceEqual ( this . Constraints ,  node . Constraints ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Columns, i => i.GetHashCode()) ^ Xor(Constraints, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitCreateTable(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DropTable : TableOperation
    {
        public DropTable()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDropTable(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DropColumn : TableOperation
    {
        public DropColumn()
        {
        }

        public Column Column
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( DropColumn ) obj ;  return  ( Compare ( this . Column ,  node . Column ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Column == null ? 0 : Column.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDropColumn(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class CopyTable : TableOperation
    {
        public CopyTable()
        {
        }

        public Table DstTable
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( CopyTable ) obj ;  return  ( Compare ( this . DstTable ,  node . DstTable ) ) ; 
        }

        public override int GetHashCode()
        {
            return (DstTable == null ? 0 : DstTable.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitCopyTable(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AlterAddColumn : TableOperation
    {
        public AlterAddColumn()
        {
        }

        public ColumnDefinition Column
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( AlterAddColumn ) obj ;  return  ( Compare ( this . Column ,  node . Column ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Column == null ? 0 : Column.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAlterAddColumn(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AddColumn : AlterAddColumn
    {
        public AddColumn()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAddColumn(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AlterColumn : AlterAddColumn
    {
        public AlterColumn()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAlterColumn(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AddConstraint : TableOperation
    {
        public AddConstraint()
        {
        }

        public ConstraintDefinition Constraint
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( AddConstraint ) obj ;  return  ( Compare ( this . Constraint ,  node . Constraint ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Constraint == null ? 0 : Constraint.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAddConstraint(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DropConstraint : TableOperation
    {
        public DropConstraint()
        {
        }

        public Constraint Constraint
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( DropConstraint ) obj ;  return  ( Compare ( this . Constraint ,  node . Constraint ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Constraint == null ? 0 : Constraint.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDropConstraint(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DataSourceAliasedNode : DataSourceNode
    {
        public DataSourceAliasedNode()
        {
        }

        public string Alias
        {
            get;
            set;
        }

        public QuerySyntaxNode Node
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( DataSourceAliasedNode ) obj ;  return  ( ( this . Alias == node . Alias ) && Compare ( this . Node ,  node . Node ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Alias.GetHashCode()) ^ (Node == null ? 0 : Node.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDataSourceAliasedNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ExpressionAliasedNode : ExpressionNode
    {
        public ExpressionAliasedNode()
        {
        }

        public string Alias
        {
            get;
            set;
        }

        public ExpressionNode Node
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ExpressionAliasedNode ) obj ;  return  ( ( this . Alias == node . Alias ) && Compare ( this . Node ,  node . Node ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Alias.GetHashCode()) ^ (Node == null ? 0 : Node.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionAliasedNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DataSourceNode : QuerySyntaxNode
    {
        public DataSourceNode()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDataSourceNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TableSourceNode : DataSourceNode
    {
        public TableSourceNode()
        {
        }

        public Table Table
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( TableSourceNode ) obj ;  return  ( Compare ( this . Table ,  node . Table ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Table == null ? 0 : Table.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTableSourceNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class FromNode : QuerySyntaxNode
    {
        public FromNode()
        {
            Join = new List<JoinNode>();
        }

        public DataSourceNode DataSource
        {
            get;
            set;
        }

        public List<JoinNode> Join
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( FromNode ) obj ;  return  ( Compare ( this . DataSource ,  node . DataSource ) && SequenceEqual ( this . Join ,  node . Join ) ) ; 
        }

        public override int GetHashCode()
        {
            return (DataSource == null ? 0 : DataSource.GetHashCode()) ^ Xor(Join, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitFromNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ExpressionNode : QuerySyntaxNode
    {
        public ExpressionNode()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ExpressionSumNode : ExpressionNode
    {
        public ExpressionSumNode()
        {
            Expressions = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ExpressionSumNode ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionSumNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ExpressionDiffNode : ExpressionNode
    {
        public ExpressionDiffNode()
        {
            Expressions = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ExpressionDiffNode ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionDiffNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ExpressionMulNode : ExpressionNode
    {
        public ExpressionMulNode()
        {
            Expressions = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ExpressionMulNode ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionMulNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ExpressionDevNode : ExpressionNode
    {
        public ExpressionDevNode()
        {
            Expressions = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ExpressionDevNode ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionDevNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConditionNode : ExpressionNode
    {
        public ConditionNode()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConditionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ExistsNode : ConditionNode
    {
        public ExistsNode()
        {
        }

        public DataSourceNode DataSource
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ExistsNode ) obj ;  return  ( Compare ( this . DataSource ,  node . DataSource ) ) ; 
        }

        public override int GetHashCode()
        {
            return (DataSource == null ? 0 : DataSource.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExistsNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConditionAndNode : ConditionNode
    {
        public ConditionAndNode()
        {
            Nodes = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Nodes
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConditionAndNode ) obj ;  return  ( SequenceEqual ( this . Nodes ,  node . Nodes ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Nodes, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConditionAndNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConditionOrNode : ConditionNode
    {
        public ConditionOrNode()
        {
            Nodes = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Nodes
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConditionOrNode ) obj ;  return  ( SequenceEqual ( this . Nodes ,  node . Nodes ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Nodes, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConditionOrNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConditionEqualNode : ConditionNode
    {
        public ConditionEqualNode()
        {
        }

        public ExpressionNode Left
        {
            get;
            set;
        }

        public ExpressionNode Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConditionEqualNode ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConditionEqualNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConditionNotNode : ConditionNode
    {
        public ConditionNotNode()
        {
        }

        public ExpressionNode Node
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConditionNotNode ) obj ;  return  ( Compare ( this . Node ,  node . Node ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Node == null ? 0 : Node.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConditionNotNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class JoinNode : QuerySyntaxNode
    {
        public JoinNode()
        {
        }

        public DataSourceNode DataSource
        {
            get;
            set;
        }

        public ConditionNode Condition
        {
            get;
            set;
        }

        public JoinType JoinType
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( JoinNode ) obj ;  return  ( Compare ( this . DataSource ,  node . DataSource ) && Compare ( this . Condition ,  node . Condition ) && ( this . JoinType == node . JoinType ) ) ; 
        }

        public override int GetHashCode()
        {
            return (DataSource == null ? 0 : DataSource.GetHashCode()) ^ (Condition == null ? 0 : Condition.GetHashCode()) ^ (JoinType.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitJoinNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class WhereNode : QuerySyntaxNode
    {
        public WhereNode()
        {
        }

        public ConditionNode Condition
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( WhereNode ) obj ;  return  ( Compare ( this . Condition ,  node . Condition ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Condition == null ? 0 : Condition.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitWhereNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConstNode : ExpressionNode
    {
        public ConstNode()
        {
        }

        public object Value
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ConstNode ) obj ;  return  ( ( this . Value . Equals ( node . Value ) ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Value == null ? 0 : Value.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConstNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TableFieldNode : ExpressionNode
    {
        public TableFieldNode()
        {
        }

        public string Field
        {
            get;
            set;
        }

        public Table Table
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( TableFieldNode ) obj ;  return  ( ( this . Field == node . Field ) && Compare ( this . Table ,  node . Table ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Field.GetHashCode()) ^ (Table == null ? 0 : Table.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTableFieldNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TopNode : QuerySyntaxNode
    {
        public TopNode()
        {
        }

        public int Limit
        {
            get;
            set;
        }

        public int Offset
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( TopNode ) obj ;  return  ( ( this . Limit == node . Limit ) && ( this . Offset == node . Offset ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Limit.GetHashCode()) ^ (Offset.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTopNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SelectNode : DataSourceNode
    {
        public SelectNode()
        {
            Fields = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Fields
        {
            get;
            set;
        }

        public FromNode From
        {
            get;
            set;
        }

        public WhereNode Where
        {
            get;
            set;
        }

        public GroupByNode GroupBy
        {
            get;
            set;
        }

        public OrderByNode OrderBy
        {
            get;
            set;
        }

        public TopNode Top
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SelectNode ) obj ;  return  ( SequenceEqual ( this . Fields ,  node . Fields ) && Compare ( this . From ,  node . From ) && Compare ( this . Where ,  node . Where ) && Compare ( this . GroupBy ,  node . GroupBy ) && Compare ( this . OrderBy ,  node . OrderBy ) && Compare ( this . Top ,  node . Top ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Fields, i => i.GetHashCode()) ^ (From == null ? 0 : From.GetHashCode()) ^ (Where == null ? 0 : Where.GetHashCode()) ^ (GroupBy == null ? 0 : GroupBy.GetHashCode()) ^ (OrderBy == null ? 0 : OrderBy.GetHashCode()) ^ (Top == null ? 0 : Top.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSelectNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class OrderByNode : QuerySyntaxNode
    {
        public OrderByNode()
        {
            Fields = new List<ExpressionNode>();
        }

        public OrderDirection Direction
        {
            get;
            set;
        }

        public List<ExpressionNode> Fields
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( OrderByNode ) obj ;  return  ( ( this . Direction == node . Direction ) && SequenceEqual ( this . Fields ,  node . Fields ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Direction.GetHashCode()) ^ Xor(Fields, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitOrderByNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class GroupByNode : QuerySyntaxNode
    {
        public GroupByNode()
        {
            Fields = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Fields
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( GroupByNode ) obj ;  return  ( SequenceEqual ( this . Fields ,  node . Fields ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Fields, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitGroupByNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AggregateFunctionNode : ExpressionNode
    {
        public AggregateFunctionNode()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAggregateFunctionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AggregateSumNode : AggregateFunctionNode
    {
        public AggregateSumNode()
        {
        }

        public ExpressionNode Node
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( AggregateSumNode ) obj ;  return  ( Compare ( this . Node ,  node . Node ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Node == null ? 0 : Node.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAggregateSumNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AggregateCountNode : AggregateFunctionNode
    {
        public AggregateCountNode()
        {
        }

        public ExpressionNode Node
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( AggregateCountNode ) obj ;  return  ( Compare ( this . Node ,  node . Node ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Node == null ? 0 : Node.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAggregateCountNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AllFieldNode : ExpressionNode
    {
        public AllFieldNode()
        {
        }

        public Table Table
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( AllFieldNode ) obj ;  return  ( Compare ( this . Table ,  node . Table ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Table == null ? 0 : Table.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAllFieldNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class CastNode : ExpressionNode
    {
        public CastNode()
        {
        }

        public ExpressionNode Expression
        {
            get;
            set;
        }

        public ColumnType Type
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( CastNode ) obj ;  return  ( Compare ( this . Expression ,  node . Expression ) && Compare ( this . Type ,  node . Type ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Expression == null ? 0 : Expression.GetHashCode()) ^ (Type == null ? 0 : Type.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitCastNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class FieldList : DataSourceNode
    {
        public FieldList()
        {
            Values = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Values
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( FieldList ) obj ;  return  ( SequenceEqual ( this . Values ,  node . Values ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Values, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitFieldList(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ValuesSourceNode : DataSourceNode
    {
        public ValuesSourceNode()
        {
            Values = new List<ExpressionNode>();
        }

        public List<ExpressionNode> Values
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( ValuesSourceNode ) obj ;  return  ( SequenceEqual ( this . Values ,  node . Values ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Values, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitValuesSourceNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class InsertNode : QuerySyntaxNode
    {
        public InsertNode()
        {
        }

        public Table Into
        {
            get;
            set;
        }

        public DataSourceNode DataSource
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( InsertNode ) obj ;  return  ( Compare ( this . Into ,  node . Into ) && Compare ( this . DataSource ,  node . DataSource ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Into == null ? 0 : Into.GetHashCode()) ^ (DataSource == null ? 0 : DataSource.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitInsertNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SetNode : QuerySyntaxNode
    {
        public SetNode()
        {
        }

        public TableFieldNode Field
        {
            get;
            set;
        }

        public ExpressionNode Value
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SetNode ) obj ;  return  ( Compare ( this . Field ,  node . Field ) && Compare ( this . Value ,  node . Value ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Field == null ? 0 : Field.GetHashCode()) ^ (Value == null ? 0 : Value.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSetNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class UpdateNode : QuerySyntaxNode
    {
        public UpdateNode()
        {
            Set = new List<SetNode>();
        }

        public Table Update
        {
            get;
            set;
        }

        public List<SetNode> Set
        {
            get;
            set;
        }

        public FromNode From
        {
            get;
            set;
        }

        public WhereNode Where
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( UpdateNode ) obj ;  return  ( Compare ( this . Update ,  node . Update ) && SequenceEqual ( this . Set ,  node . Set ) && Compare ( this . From ,  node . From ) && Compare ( this . Where ,  node . Where ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Update == null ? 0 : Update.GetHashCode()) ^ Xor(Set, i => i.GetHashCode()) ^ (From == null ? 0 : From.GetHashCode()) ^ (Where == null ? 0 : Where.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitUpdateNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DeleteNode : QuerySyntaxNode
    {
        public DeleteNode()
        {
        }

        public Table From
        {
            get;
            set;
        }

        public WhereNode Where
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( DeleteNode ) obj ;  return  ( Compare ( this . From ,  node . From ) && Compare ( this . Where ,  node . Where ) ) ; 
        }

        public override int GetHashCode()
        {
            return (From == null ? 0 : From.GetHashCode()) ^ (Where == null ? 0 : Where.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDeleteNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class RenameTableNode : QuerySyntaxNode
    {
        public RenameTableNode()
        {
        }

        public string From
        {
            get;
            set;
        }

        public string To
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( RenameTableNode ) obj ;  return  ( ( this . From == node . From ) && ( this . To == node . To ) ) ; 
        }

        public override int GetHashCode()
        {
            return (From.GetHashCode()) ^ (To.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitRenameTableNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Visitor
{
    public abstract partial class QueryVisitorBase<T>
    {
        public QueryVisitorBase()
        {
        }

        public virtual T VisitExpression(Expression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitStringValue(StringValue node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitScheme(Scheme node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDatabase(Database node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTable(Table node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumn(Column node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConstraint(Constraint node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSchemeOperation(SchemeOperation node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDatabaseOperation(DatabaseOperation node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTableOperation(TableOperation node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnType(ColumnType node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeInt(ColumnTypeInt node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnDefinition(ColumnDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConstraintDefinition(ConstraintDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConstraintDefinitionUnique(ConstraintDefinitionUnique node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConstraintDefinitionPrimaryKey(ConstraintDefinitionPrimaryKey node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConstraintDefinitionForeignKey(ConstraintDefinitionForeignKey node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitCreateTable(CreateTable node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDropTable(DropTable node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDropColumn(DropColumn node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitCopyTable(CopyTable node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAlterAddColumn(AlterAddColumn node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAddColumn(AddColumn node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAlterColumn(AlterColumn node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAddConstraint(AddConstraint node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDropConstraint(DropConstraint node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDataSourceAliasedNode(DataSourceAliasedNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitExpressionAliasedNode(ExpressionAliasedNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDataSourceNode(DataSourceNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTableSourceNode(TableSourceNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFromNode(FromNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitExpressionNode(ExpressionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitExpressionSumNode(ExpressionSumNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitExpressionDiffNode(ExpressionDiffNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitExpressionMulNode(ExpressionMulNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitExpressionDevNode(ExpressionDevNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConditionNode(ConditionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitExistsNode(ExistsNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConditionAndNode(ConditionAndNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConditionOrNode(ConditionOrNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConditionEqualNode(ConditionEqualNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConditionNotNode(ConditionNotNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitJoinNode(JoinNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitWhereNode(WhereNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConstNode(ConstNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTableFieldNode(TableFieldNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTopNode(TopNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSelectNode(SelectNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitOrderByNode(OrderByNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitGroupByNode(GroupByNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAggregateFunctionNode(AggregateFunctionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAggregateSumNode(AggregateSumNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAggregateCountNode(AggregateCountNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAllFieldNode(AllFieldNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitCastNode(CastNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFieldList(FieldList node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitValuesSourceNode(ValuesSourceNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitInsertNode(InsertNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSetNode(SetNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitUpdateNode(UpdateNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDeleteNode(DeleteNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitRenameTableNode(RenameTableNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T DefaultVisit(QuerySyntaxNode node)
        {
            return default;
        }

        public virtual T Visit(QuerySyntaxNode visitable)
        {
            if (visitable is null)
                return default;
            return visitable.Accept(this);
        }
    }
}
