using System;
using System.Collections;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Core.Querying.Model
{
    public class QFieldList : QCollectionItem<QField>
    {
        public QFieldList()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQFieldList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQFieldList(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public class QJoinList : QCollectionItem<QFromItem>
    {
        public QJoinList()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQJoinList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQJoinList(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public class QExpressionList : QCollectionItem<QExpression>
    {
        public QExpressionList()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQExpressionList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQExpressionList(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public class QDataSourceList : QCollectionItem<QDataSource>
    {
        public QDataSourceList()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQDataSourceList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQDataSourceList(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public class QWhenList : QCollectionItem<QWhen>
    {
        public QWhenList()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQWhenList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQWhenList(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public class QQueryList : QCollectionItem<QQuery>
    {
        public QQueryList()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQQueryList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQQueryList(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QExpression : QItem
    {
        public QExpression(): base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QQuery : QItem
    {
        public QQuery(QOrderBy orderBy, QSelect select, QHaving having, QGroupBy groupBy, QWhere where, QFrom from): base()
        {
            this.Attach(0, (QItem)orderBy);
            this.Attach(1, (QItem)select);
            this.Attach(2, (QItem)having);
            this.Attach(3, (QItem)groupBy);
            this.Attach(4, (QItem)where);
            this.Attach(5, (QItem)from);
        }

        public QOrderBy OrderBy
        {
            get
            {
                return (QOrderBy)this.Children[0];
            }
        }

        public QSelect Select
        {
            get
            {
                return (QSelect)this.Children[1];
            }
        }

        public QHaving Having
        {
            get
            {
                return (QHaving)this.Children[2];
            }
        }

        public QGroupBy GroupBy
        {
            get
            {
                return (QGroupBy)this.Children[3];
            }
        }

        public QWhere Where
        {
            get
            {
                return (QWhere)this.Children[4];
            }
        }

        public QFrom From
        {
            get
            {
                return (QFrom)this.Children[5];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQQuery(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQQuery(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QSelect : QItem
    {
        public QSelect(QFieldList fields): base()
        {
            this.Attach(0, (QItem)fields);
        }

        public QFieldList Fields
        {
            get
            {
                return (QFieldList)this.Children[0];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSelect(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQSelect(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QFrom : QItem
    {
        public QFrom(QJoinList joins, QDataSource source): base()
        {
            this.Attach(0, (QItem)joins);
            this.Attach(1, (QItem)source);
        }

        public QJoinList Joins
        {
            get
            {
                return (QJoinList)this.Children[0];
            }
        }

        public QDataSource Source
        {
            get
            {
                return (QDataSource)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQFrom(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQFrom(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QGroupBy : QItem
    {
        public QGroupBy(QExpressionList expressions): base()
        {
            this.Attach(0, (QItem)expressions);
        }

        public QExpressionList Expressions
        {
            get
            {
                return (QExpressionList)this.Children[0];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGroupBy(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQGroupBy(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QOrderBy : QItem
    {
        public QOrderBy(): base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOrderBy(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQOrderBy(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QWhere : QItem
    {
        public QWhere(QExpression expression): base()
        {
            this.Attach(0, (QItem)expression);
        }

        public QExpression Expression
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQWhere(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQWhere(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QHaving : QItem
    {
        public QHaving(): base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQHaving(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQHaving(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QDataSource : QItem
    {
        public QDataSource(): base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQDataSource(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQDataSource(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAliasedDataSource : QDataSource
    {
        public QAliasedDataSource(QDataSource parentSource, String alias): base()
        {
            this.Attach(0, (QItem)parentSource);
            Alias = alias;
        }

        public QDataSource ParentSource
        {
            get
            {
                return (QDataSource)this.Children[0];
            }
        }

        public String Alias
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAliasedDataSource(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQAliasedDataSource(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCombinedDataSource : QDataSource
    {
        public QCombinedDataSource(QDataSourceList dataSources): base()
        {
            this.Attach(0, (QItem)dataSources);
        }

        public QDataSourceList DataSources
        {
            get
            {
                return (QDataSourceList)this.Children[0];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCombinedDataSource(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQCombinedDataSource(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QNestedQuery : QDataSource
    {
        public QNestedQuery(QQuery nested): base()
        {
            this.Attach(0, (QItem)nested);
        }

        public QQuery Nested
        {
            get
            {
                return (QQuery)this.Children[0];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQNestedQuery(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQNestedQuery(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public abstract partial class QPlatformDataSource : QDataSource
    {
        public QPlatformDataSource(): base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QObjectTable : QPlatformDataSource
    {
        public QObjectTable(): base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQObjectTable(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQObjectTable(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QTable : QPlatformDataSource
    {
        public QTable(QObjectTable objectTable, ITable table): base()
        {
            this.Attach(0, (QItem)objectTable);
            Table = table;
        }

        public QObjectTable ObjectTable
        {
            get
            {
                return (QObjectTable)this.Children[0];
            }
        }

        public ITable Table
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQTable(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQTable(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public abstract partial class QField : QExpression
    {
        public QField(QItem element): base()
        {
            this.Attach(0, (QItem)element);
        }

        public QItem Element
        {
            get
            {
                return (QItem)this.Children[0];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QIntermediateSourceField : QField
    {
        public QIntermediateSourceField(QField field, QDataSource dataSource): base(field)
        {
            Field = field;
            DataSource = dataSource;
        }

        public QField Field
        {
            get;
        }

        public QDataSource DataSource
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQIntermediateSourceField(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQIntermediateSourceField(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QNestedQueryField : QField
    {
        public QNestedQueryField(QField field, QDataSource dataSource): base(field)
        {
            Field = field;
            DataSource = dataSource;
        }

        public QField Field
        {
            get;
        }

        public QDataSource DataSource
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQNestedQueryField(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQNestedQueryField(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QLookupField : QField
    {
        public QLookupField(String propName, QExpression baseExpression): base(baseExpression)
        {
            PropName = propName;
            BaseExpression = baseExpression;
        }

        public String PropName
        {
            get;
        }

        public QExpression BaseExpression
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLookupField(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQLookupField(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QSourceFieldExpression : QField
    {
        public QSourceFieldExpression(QPlatformDataSource platformSource, IPProperty property): base(platformSource)
        {
            PlatformSource = platformSource;
            Property = property;
        }

        public QPlatformDataSource PlatformSource
        {
            get;
        }

        public IPProperty Property
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSourceFieldExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQSourceFieldExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QSelectExpression : QField
    {
        public QSelectExpression(QExpression expression): base(expression)
        {
            Expression = expression;
        }

        public QExpression Expression
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSelectExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQSelectExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAliasedSelectExpression : QSelectExpression
    {
        public QAliasedSelectExpression(QExpression aliasedExpression, String alias): base(aliasedExpression)
        {
            AliasedExpression = aliasedExpression;
            Alias = alias;
        }

        public QExpression AliasedExpression
        {
            get;
        }

        public String Alias
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAliasedSelectExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQAliasedSelectExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QFromItem : QItem
    {
        public QFromItem(QExpression condition, QDataSource joined, QJoinType joinType): base()
        {
            this.Attach(0, (QItem)condition);
            this.Attach(1, (QItem)joined);
            JoinType = joinType;
        }

        public QExpression Condition
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QDataSource Joined
        {
            get
            {
                return (QDataSource)this.Children[1];
            }
        }

        public QJoinType JoinType
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQFromItem(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQFromItem(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QConst : QExpression
    {
        public QConst(): base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQConst(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQConst(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QParameter : QExpression
    {
        public QParameter(String name): base()
        {
            Name = name;
        }

        public String Name
        {
            get;
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQParameter(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQParameter(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCase : QExpression
    {
        public QCase(QExpression @else, QWhenList whens): base()
        {
            this.Attach(0, (QItem)@else);
            this.Attach(1, (QItem)whens);
        }

        public QExpression Else
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QWhenList Whens
        {
            get
            {
                return (QWhenList)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCase(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQCase(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QWhen : QItem
    {
        public QWhen(QExpression then, QOperationExpression @when): base()
        {
            this.Attach(0, (QItem)then);
            this.Attach(1, (QItem)@when);
        }

        public QExpression Then
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QOperationExpression When
        {
            get
            {
                return (QOperationExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQWhen(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQWhen(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QOperationExpression : QExpression
    {
        public QOperationExpression(QExpression left, QExpression right): base()
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOperationExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQOperationExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAnd : QOperationExpression
    {
        public QAnd(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAnd(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQAnd(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAdd : QOperationExpression
    {
        public QAdd(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAdd(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQAdd(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QOr : QOperationExpression
    {
        public QOr(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOr(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQOr(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QEquals : QOperationExpression
    {
        public QEquals(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQEquals(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQEquals(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QNotEquals : QOperationExpression
    {
        public QNotEquals(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQNotEquals(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQNotEquals(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QGreatThen : QOperationExpression
    {
        public QGreatThen(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGreatThen(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQGreatThen(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QLessThen : QOperationExpression
    {
        public QLessThen(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLessThen(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQLessThen(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QLessThenOrEquals : QOperationExpression
    {
        public QLessThenOrEquals(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLessThenOrEquals(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQLessThenOrEquals(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QGreatThenOrEquals : QOperationExpression
    {
        public QGreatThenOrEquals(QExpression left, QExpression right): base(left, right)
        {
            this.Attach(0, (QItem)left);
            this.Attach(1, (QItem)right);
        }

        public QExpression Left
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public QExpression Right
        {
            get
            {
                return (QExpression)this.Children[1];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGreatThenOrEquals(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQGreatThenOrEquals(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCast : QExpression
    {
        public QCast(IPType type, QExpression baseExpression): base()
        {
            Type = type;
            this.Attach(0, (QItem)baseExpression);
        }

        public IPType Type
        {
            get;
        }

        public QExpression BaseExpression
        {
            get
            {
                return (QExpression)this.Children[0];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCast(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQCast(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QDataRequest : QItem
    {
        public QDataRequest(QFieldList source): base()
        {
            this.Attach(0, (QItem)source);
        }

        public QFieldList Source
        {
            get
            {
                return (QFieldList)this.Children[0];
            }
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQDataRequest(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQDataRequest(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public abstract partial class QLangVisitorBase<T>
    {
        public virtual T VisitQFieldList(QFieldList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQJoinList(QJoinList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQExpressionList(QExpressionList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQDataSourceList(QDataSourceList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQWhenList(QWhenList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQQueryList(QQueryList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQExpression(QExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQQuery(QQuery arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQSelect(QSelect arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQFrom(QFrom arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQGroupBy(QGroupBy arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQOrderBy(QOrderBy arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQWhere(QWhere arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQHaving(QHaving arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQDataSource(QDataSource arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQAliasedDataSource(QAliasedDataSource arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQCombinedDataSource(QCombinedDataSource arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQNestedQuery(QNestedQuery arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQObjectTable(QObjectTable arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQTable(QTable arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQIntermediateSourceField(QIntermediateSourceField arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQNestedQueryField(QNestedQueryField arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQLookupField(QLookupField arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQSourceFieldExpression(QSourceFieldExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQSelectExpression(QSelectExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQAliasedSelectExpression(QAliasedSelectExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQFromItem(QFromItem arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQConst(QConst arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQParameter(QParameter arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQCase(QCase arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQWhen(QWhen arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQOperationExpression(QOperationExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQAnd(QAnd arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQAdd(QAdd arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQOr(QOr arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQEquals(QEquals arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQNotEquals(QNotEquals arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQGreatThen(QGreatThen arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQLessThen(QLessThen arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQLessThenOrEquals(QLessThenOrEquals arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQGreatThenOrEquals(QGreatThenOrEquals arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQCast(QCast arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQDataRequest(QDataRequest arg)
        {
            return DefaultVisit(arg);
        }
    }

    public abstract partial class QLangVisitorBase
    {
        public virtual void VisitQFieldList(QFieldList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQJoinList(QJoinList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQExpressionList(QExpressionList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQDataSourceList(QDataSourceList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQWhenList(QWhenList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQQueryList(QQueryList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQExpression(QExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQQuery(QQuery arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQSelect(QSelect arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQFrom(QFrom arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQGroupBy(QGroupBy arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQOrderBy(QOrderBy arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQWhere(QWhere arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQHaving(QHaving arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQDataSource(QDataSource arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQAliasedDataSource(QAliasedDataSource arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQCombinedDataSource(QCombinedDataSource arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQNestedQuery(QNestedQuery arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQObjectTable(QObjectTable arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQTable(QTable arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQIntermediateSourceField(QIntermediateSourceField arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQNestedQueryField(QNestedQueryField arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQLookupField(QLookupField arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQSourceFieldExpression(QSourceFieldExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQSelectExpression(QSelectExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQAliasedSelectExpression(QAliasedSelectExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQFromItem(QFromItem arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQConst(QConst arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQParameter(QParameter arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQCase(QCase arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQWhen(QWhen arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQOperationExpression(QOperationExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQAnd(QAnd arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQAdd(QAdd arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQOr(QOr arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQEquals(QEquals arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQNotEquals(QNotEquals arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQGreatThen(QGreatThen arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQLessThen(QLessThen arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQLessThenOrEquals(QLessThenOrEquals arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQGreatThenOrEquals(QGreatThenOrEquals arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQCast(QCast arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQDataRequest(QDataRequest arg)
        {
            DefaultVisit(arg);
        }
    }
}
