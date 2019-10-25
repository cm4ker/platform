using System;
using System.Text;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.QueryBuilder.Contracts;

namespace ZenPlatform.QueryBuilder.Model
{
    public abstract class QuerySyntaxNode : ZenPlatform.QueryBuilder.Common.SqlNode
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

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitCopyTable(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AddColumn : TableOperation
    {
        public AddColumn()
        {
        }

        public ColumnDefinition Column
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAddColumn(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AlterColumn : TableOperation
    {
        public AlterColumn()
        {
        }

        public ColumnDefinition Column
        {
            get;
            set;
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

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDropConstraint(this);
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

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ExpressionSumNode : QuerySyntaxNode
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

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitExpressionSumNode(this);
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

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConditionNode(this);
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

        public ExpressionNode Reight
        {
            get;
            set;
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

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTableFieldNode(this);
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

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSelectNode(this);
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

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDeleteNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Visitor
{
    public abstract class QueryVisitorBase<T>
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

        public virtual T VisitConditionNode(ConditionNode node)
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

        public virtual T VisitSelectNode(SelectNode node)
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
