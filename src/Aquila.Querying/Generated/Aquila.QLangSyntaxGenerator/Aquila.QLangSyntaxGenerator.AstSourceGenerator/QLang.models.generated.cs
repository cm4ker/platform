using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.Metadata;
using Aquila.Core.Querying.Model;

namespace Aquila.Core.Querying.Model
{
    public class QFieldList : QLangCollection<QField>
    {
        public static QFieldList Empty => new QFieldList(ImmutableArray<QField>.Empty);
        public QFieldList(ImmutableArray<QField> elements) : base(elements)
        {
        }

        public override QFieldList Add(QLangElement element)
        {
            var item = element as QField;
            if (item == null)
                throw new Exception("Element is not QField");
            return new QFieldList(Elements.Add(item));
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

namespace Aquila.Core.Querying.Model
{
    public class QJoinList : QLangCollection<QFromItem>
    {
        public static QJoinList Empty => new QJoinList(ImmutableArray<QFromItem>.Empty);
        public QJoinList(ImmutableArray<QFromItem> elements) : base(elements)
        {
        }

        public override QJoinList Add(QLangElement element)
        {
            var item = element as QFromItem;
            if (item == null)
                throw new Exception("Element is not QFromItem");
            return new QJoinList(Elements.Add(item));
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

namespace Aquila.Core.Querying.Model
{
    public class QExpressionList : QLangCollection<QExpression>
    {
        public static QExpressionList Empty => new QExpressionList(ImmutableArray<QExpression>.Empty);
        public QExpressionList(ImmutableArray<QExpression> elements) : base(elements)
        {
        }

        public override QExpressionList Add(QLangElement element)
        {
            var item = element as QExpression;
            if (item == null)
                throw new Exception("Element is not QExpression");
            return new QExpressionList(Elements.Add(item));
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

namespace Aquila.Core.Querying.Model
{
    public class QDataSourceList : QLangCollection<QDataSource>
    {
        public static QDataSourceList Empty => new QDataSourceList(ImmutableArray<QDataSource>.Empty);
        public QDataSourceList(ImmutableArray<QDataSource> elements) : base(elements)
        {
        }

        public override QDataSourceList Add(QLangElement element)
        {
            var item = element as QDataSource;
            if (item == null)
                throw new Exception("Element is not QDataSource");
            return new QDataSourceList(Elements.Add(item));
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

namespace Aquila.Core.Querying.Model
{
    public class QOrderList : QLangCollection<QOrderExpression>
    {
        public static QOrderList Empty => new QOrderList(ImmutableArray<QOrderExpression>.Empty);
        public QOrderList(ImmutableArray<QOrderExpression> elements) : base(elements)
        {
        }

        public override QOrderList Add(QLangElement element)
        {
            var item = element as QOrderExpression;
            if (item == null)
                throw new Exception("Element is not QOrderExpression");
            return new QOrderList(Elements.Add(item));
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOrderList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQOrderList(this);
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public class QGroupList : QLangCollection<QGroupExpression>
    {
        public static QGroupList Empty => new QGroupList(ImmutableArray<QGroupExpression>.Empty);
        public QGroupList(ImmutableArray<QGroupExpression> elements) : base(elements)
        {
        }

        public override QGroupList Add(QLangElement element)
        {
            var item = element as QGroupExpression;
            if (item == null)
                throw new Exception("Element is not QGroupExpression");
            return new QGroupList(Elements.Add(item));
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGroupList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQGroupList(this);
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public class QWhenList : QLangCollection<QWhen>
    {
        public static QWhenList Empty => new QWhenList(ImmutableArray<QWhen>.Empty);
        public QWhenList(ImmutableArray<QWhen> elements) : base(elements)
        {
        }

        public override QWhenList Add(QLangElement element)
        {
            var item = element as QWhen;
            if (item == null)
                throw new Exception("Element is not QWhen");
            return new QWhenList(Elements.Add(item));
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

namespace Aquila.Core.Querying.Model
{
    public class QQueryList : QLangCollection<QQueryBase>
    {
        public static QQueryList Empty => new QQueryList(ImmutableArray<QQueryBase>.Empty);
        public QQueryList(ImmutableArray<QQueryBase> elements) : base(elements)
        {
        }

        public override QQueryList Add(QLangElement element)
        {
            var item = element as QQueryBase;
            if (item == null)
                throw new Exception("Element is not QQueryBase");
            return new QQueryList(Elements.Add(item));
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

namespace Aquila.Core.Querying.Model
{
    public class QCriterionList : QLangCollection<QCriterion>
    {
        public static QCriterionList Empty => new QCriterionList(ImmutableArray<QCriterion>.Empty);
        public QCriterionList(ImmutableArray<QCriterion> elements) : base(elements)
        {
        }

        public override QCriterionList Add(QLangElement element)
        {
            var item = element as QCriterion;
            if (item == null)
                throw new Exception("Element is not QCriterion");
            return new QCriterionList(Elements.Add(item));
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCriterionList(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQCriterionList(this);
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QExpression : QLangElement
    {
        public QExpression() : base()
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

        public override IEnumerable<QLangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public abstract partial class QQueryBase : QLangElement
    {
        public QQueryBase() : base()
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

        public override IEnumerable<QLangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QInsertQuery : QQueryBase
    {
        public QInsertQuery() : base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQInsertQuery(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQInsertQuery(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QUpdateQuery : QQueryBase
    {
        public QUpdateQuery() : base()
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQUpdateQuery(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQUpdateQuery(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QSelectQuery : QQueryBase
    {
        public QSelectQuery(QOrderBy orderBy, QSelect select, QHaving having, QGroupBy groupBy, QWhere where, QFrom from, QCriterionList criteria) : base()
        {
            this.orderBy = orderBy;
            this.select = select;
            this.having = having;
            this.groupBy = groupBy;
            this.where = where;
            this.from = from;
            this.criteria = criteria;
        }

        public QOrderBy OrderBy { get => this.orderBy; init => this.orderBy = value; }

        public QSelect Select { get => this.select; init => this.select = value; }

        public QHaving Having { get => this.having; init => this.having = value; }

        public QGroupBy GroupBy { get => this.groupBy; init => this.groupBy = value; }

        public QWhere Where { get => this.where; init => this.where = value; }

        public QFrom From { get => this.from; init => this.from = value; }

        public QCriterionList Criteria { get => this.criteria; init => this.criteria = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSelectQuery(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQSelectQuery(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.orderBy != null)
                yield return this.orderBy;
            if (this.select != null)
                yield return this.select;
            if (this.having != null)
                yield return this.having;
            if (this.groupBy != null)
                yield return this.groupBy;
            if (this.where != null)
                yield return this.where;
            if (this.from != null)
                yield return this.from;
            if (this.criteria != null)
                yield return this.criteria;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QOrderBy orderBy;
        private QSelect select;
        private QHaving having;
        private QGroupBy groupBy;
        private QWhere where;
        private QFrom from;
        private QCriterionList criteria;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QCriterion : QLangElement
    {
        public QCriterion(QWhere where, QFrom from) : base()
        {
            this.where = where;
            this.from = from;
        }

        public QWhere Where { get => this.where; init => this.where = value; }

        public QFrom From { get => this.from; init => this.from = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCriterion(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQCriterion(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.where != null)
                yield return this.where;
            if (this.from != null)
                yield return this.from;
            yield break;
        }

        private QWhere where;
        private QFrom from;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QSelect : QLangElement
    {
        public QSelect(QFieldList fields) : base()
        {
            this.fields = fields;
        }

        public QFieldList Fields { get => this.fields; init => this.fields = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSelect(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQSelect(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.fields != null)
                yield return this.fields;
            yield break;
        }

        private QFieldList fields;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QFrom : QLangElement
    {
        public QFrom(QJoinList joins, QDataSource source) : base()
        {
            this.joins = joins;
            this.source = source;
        }

        public QJoinList Joins { get => this.joins; init => this.joins = value; }

        public QDataSource Source { get => this.source; init => this.source = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQFrom(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQFrom(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.joins != null)
                yield return this.joins;
            if (this.source != null)
                yield return this.source;
            yield break;
        }

        private QJoinList joins;
        private QDataSource source;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QGroupBy : QLangElement
    {
        public QGroupBy(QGroupList expressions) : base()
        {
            this.expressions = expressions;
        }

        public QGroupList Expressions { get => this.expressions; init => this.expressions = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGroupBy(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQGroupBy(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.expressions != null)
                yield return this.expressions;
            yield break;
        }

        private QGroupList expressions;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QOrderBy : QLangElement
    {
        public QOrderBy(QOrderList expressions) : base()
        {
            this.expressions = expressions;
        }

        public QOrderList Expressions { get => this.expressions; init => this.expressions = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOrderBy(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQOrderBy(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.expressions != null)
                yield return this.expressions;
            yield break;
        }

        private QOrderList expressions;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QWhere : QLangElement
    {
        public QWhere(QExpression expression) : base()
        {
            this.expression = expression;
        }

        public QExpression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQWhere(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQWhere(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.expression != null)
                yield return this.expression;
            yield break;
        }

        private QExpression expression;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QHaving : QLangElement
    {
        public QHaving(QExpression expression) : base()
        {
            this.expression = expression;
        }

        public QExpression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQHaving(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQHaving(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.expression != null)
                yield return this.expression;
            yield break;
        }

        private QExpression expression;
    }
}

namespace Aquila.Core.Querying.Model
{
    public abstract partial class QDataSource : QLangElement
    {
        public QDataSource() : base()
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

        public override IEnumerable<QLangElement> GetChildren()
        {
            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QAliasedDataSource : QDataSource
    {
        public QAliasedDataSource(QDataSource parentSource, String alias) : base()
        {
            this.parentSource = parentSource;
            Alias = alias;
        }

        public QDataSource ParentSource { get => this.parentSource; init => this.parentSource = value; }

        public String Alias { get => this.alias; init => this.alias = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAliasedDataSource(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQAliasedDataSource(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.parentSource != null)
                yield return this.parentSource;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QDataSource parentSource;
        private String alias;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QCombinedDataSource : QDataSource
    {
        public QCombinedDataSource(QDataSourceList dataSources) : base()
        {
            this.dataSources = dataSources;
        }

        public QDataSourceList DataSources { get => this.dataSources; init => this.dataSources = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCombinedDataSource(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQCombinedDataSource(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.dataSources != null)
                yield return this.dataSources;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QDataSourceList dataSources;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QNestedQuery : QDataSource
    {
        public QNestedQuery(QSelectQuery nested) : base()
        {
            this.nested = nested;
        }

        public QSelectQuery Nested { get => this.nested; init => this.nested = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQNestedQuery(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQNestedQuery(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.nested != null)
                yield return this.nested;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QSelectQuery nested;
    }
}

namespace Aquila.Core.Querying.Model
{
    public abstract partial class QPlatformDataSource : QDataSource
    {
        public QPlatformDataSource() : base()
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

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QObjectTable : QPlatformDataSource
    {
        public QObjectTable(SMEntity objectType) : base()
        {
            ObjectType = objectType;
        }

        public SMEntity ObjectType { get => this.objectType; init => this.objectType = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQObjectTable(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQObjectTable(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private SMEntity objectType;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QTable : QPlatformDataSource
    {
        public QTable(QObjectTable objectTable) : base()
        {
            this.objectTable = objectTable;
        }

        public QObjectTable ObjectTable { get => this.objectTable; init => this.objectTable = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQTable(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQTable(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.objectTable != null)
                yield return this.objectTable;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QObjectTable objectTable;
    }
}

namespace Aquila.Core.Querying.Model
{
    public abstract partial class QField : QExpression
    {
        public QField(QLangElement element) : base()
        {
            this.element = element;
        }

        public QLangElement Element { get => this.element; init => this.element = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.element != null)
                yield return this.element;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QLangElement element;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QIntermediateSourceField : QField
    {
        public QIntermediateSourceField(QField field, QDataSource dataSource) : base(field)
        {
            Field = field;
            DataSource = dataSource;
        }

        public QField Field { get => this.field; init => this.field = value; }

        public QDataSource DataSource { get => this.dataSource; init => this.dataSource = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQIntermediateSourceField(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQIntermediateSourceField(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QField field;
        private QDataSource dataSource;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QNestedQueryField : QField
    {
        public QNestedQueryField(QField field, QDataSource dataSource) : base(field)
        {
            Field = field;
            DataSource = dataSource;
        }

        public QField Field { get => this.field; init => this.field = value; }

        public QDataSource DataSource { get => this.dataSource; init => this.dataSource = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQNestedQueryField(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQNestedQueryField(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QField field;
        private QDataSource dataSource;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QLookupField : QField
    {
        public QLookupField(String propName, QExpression baseExpression) : base(baseExpression)
        {
            PropName = propName;
            BaseExpression = baseExpression;
        }

        public String PropName { get => this.propName; init => this.propName = value; }

        public QExpression BaseExpression { get => this.baseExpression; init => this.baseExpression = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLookupField(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQLookupField(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private String propName;
        private QExpression baseExpression;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QSourceFieldExpression : QField
    {
        public QSourceFieldExpression(QPlatformDataSource platformSource, SMProperty property) : base(platformSource)
        {
            PlatformSource = platformSource;
            Property = property;
        }

        public QPlatformDataSource PlatformSource { get => this.platformSource; init => this.platformSource = value; }

        public SMProperty Property { get => this.property; init => this.property = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSourceFieldExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQSourceFieldExpression(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QPlatformDataSource platformSource;
        private SMProperty property;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QOrderExpression : QExpression
    {
        public QOrderExpression(QSortDirection sortingDirection, QExpression expression) : base()
        {
            SortingDirection = sortingDirection;
            this.expression = expression;
        }

        public QSortDirection SortingDirection { get => this.sortingDirection; init => this.sortingDirection = value; }

        public QExpression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOrderExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQOrderExpression(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.expression != null)
                yield return this.expression;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QSortDirection sortingDirection;
        private QExpression expression;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QGroupExpression : QExpression
    {
        public QGroupExpression(QExpression expression) : base()
        {
            this.expression = expression;
        }

        public QExpression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGroupExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQGroupExpression(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.expression != null)
                yield return this.expression;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QExpression expression;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QSelectExpression : QField
    {
        public QSelectExpression(QExpression expression) : base(expression)
        {
            Expression = expression;
        }

        public QExpression Expression { get => this.expression; init => this.expression = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSelectExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQSelectExpression(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QExpression expression;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QAliasedSelectExpression : QSelectExpression
    {
        public QAliasedSelectExpression(QExpression aliasedExpression, String alias) : base(aliasedExpression)
        {
            AliasedExpression = aliasedExpression;
            Alias = alias;
        }

        public QExpression AliasedExpression { get => this.aliasedExpression; init => this.aliasedExpression = value; }

        public String Alias { get => this.alias; init => this.alias = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAliasedSelectExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQAliasedSelectExpression(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QExpression aliasedExpression;
        private String alias;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QFromItem : QLangElement
    {
        public QFromItem(QExpression condition, QDataSource joined, QJoinType joinType) : base()
        {
            this.condition = condition;
            this.joined = joined;
            JoinType = joinType;
        }

        public QExpression Condition { get => this.condition; init => this.condition = value; }

        public QDataSource Joined { get => this.joined; init => this.joined = value; }

        public QJoinType JoinType { get => this.joinType; init => this.joinType = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQFromItem(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQFromItem(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.condition != null)
                yield return this.condition;
            if (this.joined != null)
                yield return this.joined;
            yield break;
        }

        private QExpression condition;
        private QDataSource joined;
        private QJoinType joinType;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QConst : QExpression
    {
        public QConst() : base()
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

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QParameter : QExpression
    {
        public QParameter(String name) : base()
        {
            Name = name;
        }

        public String Name { get => this.name; init => this.name = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQParameter(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQParameter(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private String name;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QVar : QExpression
    {
        public QVar(String name) : base()
        {
            Name = name;
        }

        public String Name { get => this.name; init => this.name = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQVar(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQVar(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private String name;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QCase : QExpression
    {
        public QCase(QExpression @else, QWhenList whens) : base()
        {
            this.@else = @else;
            this.whens = whens;
        }

        public QExpression Else { get => this.@else; init => this.@else = value; }

        public QWhenList Whens { get => this.whens; init => this.whens = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCase(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQCase(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.@else != null)
                yield return this.@else;
            if (this.whens != null)
                yield return this.whens;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private QExpression @else;
        private QWhenList whens;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QWhen : QLangElement
    {
        public QWhen(QExpression then, QOperationExpression @when) : base()
        {
            this.then = then;
            this.@when = @when;
        }

        public QExpression Then { get => this.then; init => this.then = value; }

        public QOperationExpression When { get => this.@when; init => this.@when = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQWhen(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQWhen(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.then != null)
                yield return this.then;
            if (this.@when != null)
                yield return this.@when;
            yield break;
        }

        private QExpression then;
        private QOperationExpression @when;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QOperationExpression : QExpression
    {
        public QOperationExpression(QExpression left, QExpression right) : base()
        {
            this.left = left;
            this.right = right;
        }

        public QExpression Left { get => this.left; init => this.left = value; }

        public QExpression Right { get => this.right; init => this.right = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOperationExpression(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQOperationExpression(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.left != null)
                yield return this.left;
            if (this.right != null)
                yield return this.right;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        protected QExpression left;
        protected QExpression right;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QAnd : QOperationExpression
    {
        public QAnd(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAnd(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQAnd(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.left != null)
                yield return this.left;
            if (this.right != null)
                yield return this.right;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QAdd : QOperationExpression
    {
        public QAdd(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAdd(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQAdd(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.left != null)
                yield return this.left;
            if (this.right != null)
                yield return this.right;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QOr : QOperationExpression
    {
        public QOr(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOr(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQOr(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QEquals : QOperationExpression
    {
        public QEquals(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQEquals(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQEquals(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QNotEquals : QOperationExpression
    {
        public QNotEquals(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQNotEquals(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQNotEquals(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QGreatThen : QOperationExpression
    {
        public QGreatThen(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGreatThen(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQGreatThen(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QLessThen : QOperationExpression
    {
        public QLessThen(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLessThen(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQLessThen(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QLessThenOrEquals : QOperationExpression
    {
        public QLessThenOrEquals(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLessThenOrEquals(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQLessThenOrEquals(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QGreatThenOrEquals : QOperationExpression
    {
        public QGreatThenOrEquals(QExpression left, QExpression right) : base(left, right)
        {
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGreatThenOrEquals(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQGreatThenOrEquals(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QCast : QExpression
    {
        public QCast(SMType type, QExpression baseExpression) : base()
        {
            Type = type;
            this.baseExpression = baseExpression;
        }

        public SMType Type { get => this.type; init => this.type = value; }

        public QExpression BaseExpression { get => this.baseExpression; init => this.baseExpression = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCast(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQCast(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.baseExpression != null)
                yield return this.baseExpression;
            foreach (var item in base.GetChildren())
            {
                yield return item;
            }

            yield break;
        }

        private SMType type;
        private QExpression baseExpression;
    }
}

namespace Aquila.Core.Querying.Model
{
    public partial class QDataRequest : QLangElement
    {
        public QDataRequest(QFieldList source) : base()
        {
            this.source = source;
        }

        public QFieldList Source { get => this.source; init => this.source = value; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQDataRequest(this);
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            visitor.VisitQDataRequest(this);
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            if (this.source != null)
                yield return this.source;
            yield break;
        }

        private QFieldList source;
    }
}

namespace Aquila.Core.Querying
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

        public virtual T VisitQOrderList(QOrderList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQGroupList(QGroupList arg)
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

        public virtual T VisitQCriterionList(QCriterionList arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQExpression(QExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQInsertQuery(QInsertQuery arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQUpdateQuery(QUpdateQuery arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQSelectQuery(QSelectQuery arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQCriterion(QCriterion arg)
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

        public virtual T VisitQOrderExpression(QOrderExpression arg)
        {
            return DefaultVisit(arg);
        }

        public virtual T VisitQGroupExpression(QGroupExpression arg)
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

        public virtual T VisitQVar(QVar arg)
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

        public virtual void VisitQOrderList(QOrderList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQGroupList(QGroupList arg)
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

        public virtual void VisitQCriterionList(QCriterionList arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQExpression(QExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQInsertQuery(QInsertQuery arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQUpdateQuery(QUpdateQuery arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQSelectQuery(QSelectQuery arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQCriterion(QCriterion arg)
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

        public virtual void VisitQOrderExpression(QOrderExpression arg)
        {
            DefaultVisit(arg);
        }

        public virtual void VisitQGroupExpression(QGroupExpression arg)
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

        public virtual void VisitQVar(QVar arg)
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