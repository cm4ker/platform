using System;
using System.Text;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.QueryBuilder.Contracts;

namespace ZenPlatform.QueryBuilder.Model
{
    public abstract class QuerySyntaxNode
    {
        public abstract T Accept<T>(QueryVisitorBase<T> visitor);
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TableNode : QuerySyntaxNode
    {
        public TableNode( string  name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }

        public SchemaNode Schema
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTableNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnNode : QuerySyntaxNode
    {
        public ColumnNode( string  name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SchemaNode : QuerySyntaxNode
    {
        public SchemaNode( string  name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSchemaNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class AlterTableNode : QuerySyntaxNode
    {
        public AlterTableNode()
        {
            AddColumns = new List<ColumnDefinitionNode>();
            AlterColumns = new List<ColumnDefinitionNode>();
        }

        public TableNode Table
        {
            get;
            set;
        }

        public List<ColumnDefinitionNode> AddColumns
        {
            get;
            set;
        }

        public List<ColumnDefinitionNode> AlterColumns
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitAlterTableNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TypeDefinitionNode : QuerySyntaxNode
    {
        public TypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class IntTypeDefinitionNode : TypeDefinitionNode
    {
        public IntTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitIntTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class BigintTypeDefinitionNode : TypeDefinitionNode
    {
        public BigintTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitBigintTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class BinaryTypeDefinitionNode : TypeDefinitionNode
    {
        public BinaryTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitBinaryTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DoubleTypeDefinitionNode : TypeDefinitionNode
    {
        public DoubleTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDoubleTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class BooleanTypeDefinitionNode : TypeDefinitionNode
    {
        public BooleanTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitBooleanTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class GuidTypeDefinitionNode : TypeDefinitionNode
    {
        public GuidTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitGuidTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class DateTimeTypeDefinitionNode : TypeDefinitionNode
    {
        public DateTimeTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitDateTimeTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ByteTypeDefinitionNode : TypeDefinitionNode
    {
        public ByteTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitByteTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TextTypeDefinitionNode : TypeDefinitionNode
    {
        public TextTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTextTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class XmlTypeDefinitionNode : TypeDefinitionNode
    {
        public XmlTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitXmlTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class FloatTypeDefinitionNode : TypeDefinitionNode
    {
        public FloatTypeDefinitionNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitFloatTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class NumericTypeDefinitionNode : TypeDefinitionNode
    {
        public NumericTypeDefinitionNode( int  scale,  int  precision)
        {
            Scale = scale;
            Precision = precision;
        }

        public int Scale
        {
            get;
            set;
        }

        public int Precision
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitNumericTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class VarbinaryTypeDefinitionNode : TypeDefinitionNode
    {
        public VarbinaryTypeDefinitionNode( int  size)
        {
            Size = size;
        }

        public int Size
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitVarbinaryTypeDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnDefinitionNode : QuerySyntaxNode
    {
        public ColumnDefinitionNode()
        {
        }

        public ColumnNode Column
        {
            get;
            set;
        }

        public TypeDefinitionNode Type
        {
            get;
            set;
        }

        public bool Null
        {
            get;
            set;
        }

        public bool NotNull
        {
            get;
            set;
        }

        public bool Identity
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
            return visitor.VisitColumnDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TypeConstraintNode : QuerySyntaxNode
    {
        public TypeConstraintNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTypeConstraintNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class PrimaryKeyTypeConstraintNode : TypeConstraintNode
    {
        public PrimaryKeyTypeConstraintNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitPrimaryKeyTypeConstraintNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ForeingKeyTypeConstraintNode : TypeConstraintNode
    {
        public ForeingKeyTypeConstraintNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitForeingKeyTypeConstraintNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class UniqueTypeConstraintNode : TypeConstraintNode
    {
        public UniqueTypeConstraintNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitUniqueTypeConstraintNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ConstraintDefinitionNode : QuerySyntaxNode
    {
        public ConstraintDefinitionNode( string  name)
        {
            Name = name;
            Columns = new List<ColumnNode>();
        }

        public TypeConstraintNode Type
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public TableNode Table
        {
            get;
            set;
        }

        public List<ColumnNode> Columns
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitConstraintDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class IndexDefinitionNode : QuerySyntaxNode
    {
        public IndexDefinitionNode( bool  unique,  bool  clustered)
        {
            Unique = unique;
            Clustered = clustered;
        }

        public IdentifierNode Name
        {
            get;
            set;
        }

        public TableNode Table
        {
            get;
            set;
        }

        public bool Unique
        {
            get;
            set;
        }

        public bool Clustered
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitIndexDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class TableDefinitionNode : QuerySyntaxNode
    {
        public TableDefinitionNode()
        {
            Columns = new List<ColumnDefinitionNode>();
            Constraints = new List<ConstraintDefinitionNode>();
        }

        public TableNode Table
        {
            get;
            set;
        }

        public List<ColumnDefinitionNode> Columns
        {
            get;
            set;
        }

        public List<ConstraintDefinitionNode> Constraints
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitTableDefinitionNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class IdentifierNode : QuerySyntaxNode
    {
        public IdentifierNode( string  name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitIdentifierNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ParameterNode : QuerySyntaxNode
    {
        public ParameterNode()
        {
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitParameterNode(this);
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

        public virtual T VisitTableNode(TableNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnNode(ColumnNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSchemaNode(SchemaNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAlterTableNode(AlterTableNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTypeDefinitionNode(TypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIntTypeDefinitionNode(IntTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitBigintTypeDefinitionNode(BigintTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitBinaryTypeDefinitionNode(BinaryTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDoubleTypeDefinitionNode(DoubleTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitBooleanTypeDefinitionNode(BooleanTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitGuidTypeDefinitionNode(GuidTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDateTimeTypeDefinitionNode(DateTimeTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitByteTypeDefinitionNode(ByteTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTextTypeDefinitionNode(TextTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitXmlTypeDefinitionNode(XmlTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFloatTypeDefinitionNode(FloatTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitNumericTypeDefinitionNode(NumericTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitVarbinaryTypeDefinitionNode(VarbinaryTypeDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnDefinitionNode(ColumnDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTypeConstraintNode(TypeConstraintNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPrimaryKeyTypeConstraintNode(PrimaryKeyTypeConstraintNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitForeingKeyTypeConstraintNode(ForeingKeyTypeConstraintNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitUniqueTypeConstraintNode(UniqueTypeConstraintNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConstraintDefinitionNode(ConstraintDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIndexDefinitionNode(IndexDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTableDefinitionNode(TableDefinitionNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIdentifierNode(IdentifierNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitParameterNode(ParameterNode node)
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
