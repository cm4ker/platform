using System;
using System.Linq;
using System.Collections.Generic;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.Core.Querying.Visitor;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Querying.Model
{
    public abstract partial class QItem
    {
        public abstract T Accept<T>(QLangVisitorBase<T> visitor);
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QExpression : QItem
    {
        public QExpression(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QQuery : QItem
    {
        public QQuery(QOrderBy orderBy, QSelect select, QHaving having, QGroupBy groupBy, QWhere where, QFrom from): base()
        {
            Childs.Add(orderBy);
            OrderBy = orderBy;
            Childs.Add(select);
            Select = select;
            Childs.Add(having);
            Having = having;
            Childs.Add(groupBy);
            GroupBy = groupBy;
            Childs.Add(where);
            Where = where;
            Childs.Add(from);
            From = from;
        }

        public QOrderBy OrderBy
        {
            get;
            set;
        }

        public QSelect Select
        {
            get;
            set;
        }

        public QHaving Having
        {
            get;
            set;
        }

        public QGroupBy GroupBy
        {
            get;
            set;
        }

        public QWhere Where
        {
            get;
            set;
        }

        public QFrom From
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QQuery ) obj ;  return  ( Compare ( this . OrderBy ,  node . OrderBy ) && Compare ( this . Select ,  node . Select ) && Compare ( this . Having ,  node . Having ) && Compare ( this . GroupBy ,  node . GroupBy ) && Compare ( this . Where ,  node . Where ) && Compare ( this . From ,  node . From ) ) ; 
        }

        public override int GetHashCode()
        {
            return (OrderBy == null ? 0 : OrderBy.GetHashCode()) ^ (Select == null ? 0 : Select.GetHashCode()) ^ (Having == null ? 0 : Having.GetHashCode()) ^ (GroupBy == null ? 0 : GroupBy.GetHashCode()) ^ (Where == null ? 0 : Where.GetHashCode()) ^ (From == null ? 0 : From.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQQuery(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QSelect : QItem
    {
        public QSelect(List<QField> fields): base()
        {
            foreach (var item in fields)
                Childs.Add(item);
            Fields = fields;
        }

        public List<QField> Fields
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QSelect ) obj ;  return  ( SequenceEqual ( this . Fields ,  node . Fields ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Fields, i => i.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSelect(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QFrom : QItem
    {
        public QFrom(IEnumerable<QFromItem> joins, QDataSource source): base()
        {
            foreach (var item in joins)
                Childs.Add(item);
            Joins = joins;
            Childs.Add(source);
            Source = source;
        }

        public IEnumerable<QFromItem> Joins
        {
            get;
            set;
        }

        public QDataSource Source
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QFrom ) obj ;  return  ( SequenceEqual ( this . Joins ,  node . Joins ) && Compare ( this . Source ,  node . Source ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Joins, i => i.GetHashCode()) ^ (Source == null ? 0 : Source.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQFrom(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QGroupBy : QItem
    {
        public QGroupBy(List<QExpression> expressions): base()
        {
            foreach (var item in expressions)
                Childs.Add(item);
            Expressions = expressions;
        }

        public List<QExpression> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QGroupBy ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGroupBy(this);
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOrderBy(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QWhere : QItem
    {
        public QWhere(QExpression expression): base()
        {
            Childs.Add(expression);
            Expression = expression;
        }

        public QExpression Expression
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QWhere ) obj ;  return  ( Compare ( this . Expression ,  node . Expression ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Expression == null ? 0 : Expression.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQWhere(this);
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQHaving(this);
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQDataSource(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAliasedDataSource : QDataSource
    {
        public QAliasedDataSource(QDataSource parent, String alias): base()
        {
            Childs.Add(parent);
            Parent = parent;
            Alias = alias;
        }

        public QDataSource Parent
        {
            get;
            set;
        }

        public String Alias
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QAliasedDataSource ) obj ;  return  ( Compare ( this . Parent ,  node . Parent ) && Compare ( this . Alias ,  node . Alias ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Parent == null ? 0 : Parent.GetHashCode()) ^ (Alias == null ? 0 : Alias.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAliasedDataSource(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCombinedDataSource : QDataSource
    {
        public QCombinedDataSource(List<QDataSource> dataSources): base()
        {
            foreach (var item in dataSources)
                Childs.Add(item);
            DataSources = dataSources;
        }

        public List<QDataSource> DataSources
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QCombinedDataSource ) obj ;  return  ( SequenceEqual ( this . DataSources ,  node . DataSources ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(DataSources, i => i.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCombinedDataSource(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QNestedQuery : QDataSource
    {
        public QNestedQuery(QQuery nested): base()
        {
            Childs.Add(nested);
            Nested = nested;
        }

        public QQuery Nested
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QNestedQuery ) obj ;  return  ( Compare ( this . Nested ,  node . Nested ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Nested == null ? 0 : Nested.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQNestedQuery(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QObjectTable : QDataSource
    {
        public QObjectTable(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQObjectTable(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public abstract partial class QField : QExpression
    {
        public QField(QItem element): base()
        {
            Childs.Add(element);
            Element = element;
        }

        public QItem Element
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QField ) obj ;  return  ( Compare ( this . Element ,  node . Element ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Element == null ? 0 : Element.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQField(this);
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
            set;
        }

        public QDataSource DataSource
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QIntermediateSourceField ) obj ;  return  ( Compare ( this . Field ,  node . Field ) && Compare ( this . DataSource ,  node . DataSource ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Field == null ? 0 : Field.GetHashCode()) ^ (DataSource == null ? 0 : DataSource.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQIntermediateSourceField(this);
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
            set;
        }

        public QExpression BaseExpression
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QLookupField ) obj ;  return  ( Compare ( this . PropName ,  node . PropName ) && Compare ( this . BaseExpression ,  node . BaseExpression ) ) ; 
        }

        public override int GetHashCode()
        {
            return (PropName == null ? 0 : PropName.GetHashCode()) ^ (BaseExpression == null ? 0 : BaseExpression.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLookupField(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QSourceFieldExpression : QField
    {
        public QSourceFieldExpression(QObjectTable objectTable, XCObjectPropertyBase property): base(objectTable)
        {
            ObjectTable = objectTable;
            Property = property;
        }

        public QObjectTable ObjectTable
        {
            get;
            set;
        }

        public XCObjectPropertyBase Property
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QSourceFieldExpression ) obj ;  return  ( Compare ( this . ObjectTable ,  node . ObjectTable ) && Compare ( this . Property ,  node . Property ) ) ; 
        }

        public override int GetHashCode()
        {
            return (ObjectTable == null ? 0 : ObjectTable.GetHashCode()) ^ (Property == null ? 0 : Property.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSourceFieldExpression(this);
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
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QSelectExpression ) obj ;  return  ( Compare ( this . Expression ,  node . Expression ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Expression == null ? 0 : Expression.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQSelectExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAliasedSelectExpression : QSelectExpression
    {
        public QAliasedSelectExpression(QExpression expression, String alias): base(expression)
        {
            Expression = expression;
            Alias = alias;
        }

        public QExpression Expression
        {
            get;
            set;
        }

        public String Alias
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QAliasedSelectExpression ) obj ;  return  ( Compare ( this . Expression ,  node . Expression ) && Compare ( this . Alias ,  node . Alias ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Expression == null ? 0 : Expression.GetHashCode()) ^ (Alias == null ? 0 : Alias.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAliasedSelectExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QFromItem : QItem
    {
        public QFromItem(QOn condition, QDataSource joined, QJoinType joinType): base()
        {
            Childs.Add(condition);
            Condition = condition;
            Childs.Add(joined);
            Joined = joined;
            JoinType = joinType;
        }

        public QOn Condition
        {
            get;
            set;
        }

        public QDataSource Joined
        {
            get;
            set;
        }

        public QJoinType JoinType
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QFromItem ) obj ;  return  ( Compare ( this . Condition ,  node . Condition ) && Compare ( this . Joined ,  node . Joined ) && Compare ( this . JoinType ,  node . JoinType ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Condition == null ? 0 : Condition.GetHashCode()) ^ (Joined == null ? 0 : Joined.GetHashCode()) ^ (JoinType == null ? 0 : JoinType.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQFromItem(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QOperationExpression : QExpression
    {
        public QOperationExpression(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOperationExpression(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QOn : QOperationExpression
    {
        public QOn(QExpression expression): base()
        {
            Childs.Add(expression);
            Expression = expression;
        }

        public QExpression Expression
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QOn ) obj ;  return  ( Compare ( this . Expression ,  node . Expression ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Expression == null ? 0 : Expression.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOn(this);
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQConst(this);
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
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QParameter ) obj ;  return  ( Compare ( this . Name ,  node . Name ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Name == null ? 0 : Name.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQParameter(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCase : QExpression
    {
        public QCase(QExpression @else, List<QWhen> whens): base()
        {
            Childs.Add(@else);
            Else = @else;
            foreach (var item in whens)
                Childs.Add(item);
            Whens = whens;
        }

        public QExpression Else
        {
            get;
            set;
        }

        public List<QWhen> Whens
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QCase ) obj ;  return  ( Compare ( this . Else ,  node . Else ) && SequenceEqual ( this . Whens ,  node . Whens ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Else == null ? 0 : Else.GetHashCode()) ^ Xor(Whens, i => i.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCase(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QWhen : QItem
    {
        public QWhen(QExpression then, QOperationExpression @when): base()
        {
            Childs.Add(then);
            Then = then;
            Childs.Add(@when);
            When = @when;
        }

        public QExpression Then
        {
            get;
            set;
        }

        public QOperationExpression When
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QWhen ) obj ;  return  ( Compare ( this . Then ,  node . Then ) && Compare ( this . When ,  node . When ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Then == null ? 0 : Then.GetHashCode()) ^ (When == null ? 0 : When.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQWhen(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAnd : QOperationExpression
    {
        public QAnd(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QAnd ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAnd(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAdd : QOperationExpression
    {
        public QAdd(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QAdd ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQAdd(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QOr : QOperationExpression
    {
        public QOr(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QOr ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQOr(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QEquals : QOperationExpression
    {
        public QEquals(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QEquals ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQEquals(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QNotEquals : QOperationExpression
    {
        public QNotEquals(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QNotEquals ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQNotEquals(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QGreatThen : QOperationExpression
    {
        public QGreatThen(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QGreatThen ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGreatThen(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QLessThen : QOperationExpression
    {
        public QLessThen(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QLessThen ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLessThen(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QLessThenOrEquals : QOperationExpression
    {
        public QLessThenOrEquals(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QLessThenOrEquals ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQLessThenOrEquals(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QGreatThenOrEquals : QOperationExpression
    {
        public QGreatThenOrEquals(QExpression left, QExpression right): base()
        {
            Childs.Add(left);
            Left = left;
            Childs.Add(right);
            Right = right;
        }

        public QExpression Left
        {
            get;
            set;
        }

        public QExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QGreatThenOrEquals ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQGreatThenOrEquals(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCast : QExpression
    {
        public QCast(XCTypeBase type, QExpression baseExpression): base()
        {
            Type = type;
            Childs.Add(baseExpression);
            BaseExpression = baseExpression;
        }

        public XCTypeBase Type
        {
            get;
            set;
        }

        public QExpression BaseExpression
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( QCast ) obj ;  return  ( Compare ( this . Type ,  node . Type ) && Compare ( this . BaseExpression ,  node . BaseExpression ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Type == null ? 0 : Type.GetHashCode()) ^ (BaseExpression == null ? 0 : BaseExpression.GetHashCode());
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            return visitor.VisitQCast(this);
        }
    }
}

namespace ZenPlatform.Core.Querying.Visitor
{
    public abstract partial class QLangVisitorBase<T>
    {
        public QLangVisitorBase()
        {
        }

        public virtual T VisitQExpression(QExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQQuery(QQuery node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQSelect(QSelect node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQFrom(QFrom node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQGroupBy(QGroupBy node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQOrderBy(QOrderBy node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQWhere(QWhere node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQHaving(QHaving node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQDataSource(QDataSource node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQAliasedDataSource(QAliasedDataSource node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQCombinedDataSource(QCombinedDataSource node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQNestedQuery(QNestedQuery node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQObjectTable(QObjectTable node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQField(QField node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQLookupField(QLookupField node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQSelectExpression(QSelectExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQAliasedSelectExpression(QAliasedSelectExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQFromItem(QFromItem node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQOperationExpression(QOperationExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQOn(QOn node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQConst(QConst node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQParameter(QParameter node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQCase(QCase node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQWhen(QWhen node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQAnd(QAnd node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQAdd(QAdd node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQOr(QOr node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQEquals(QEquals node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQNotEquals(QNotEquals node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQGreatThen(QGreatThen node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQLessThen(QLessThen node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQLessThenOrEquals(QLessThenOrEquals node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQGreatThenOrEquals(QGreatThenOrEquals node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQCast(QCast node)
        {
            return DefaultVisit(node);
        }

        public virtual T DefaultVisit(QItem node)
        {
            return default;
        }

        public virtual T Visit(QItem visitable)
        {
            if (visitable is null)
                return default;
            return visitable.Accept(this);
        }
    }
}