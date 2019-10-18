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

        public virtual T VisitDropColumn(DropColumn node)
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
