using System;
using System.Linq;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.QueryBuilder.Contracts;

namespace ZenPlatform.QueryBuilder.Model
{
    public abstract partial class SSyntaxNode
    {
        public abstract T Accept<T>(QueryVisitorBase<T> visitor);
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SExpression : SSyntaxNode
    {
        public SExpression()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSExpression(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SDataSource : SSyntaxNode
    {
        public SDataSource()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSDataSource(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class STable : SSyntaxNode
    {
        public STable( string  name)
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
                return false; var  node  =  ( STable ) obj ;  return  ( ( this . Name == node . Name ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Name.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSTable(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class STop : SSyntaxNode
    {
        public STop( int  limit,  int  offset)
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
                return false; var  node  =  ( STop ) obj ;  return  ( ( this . Limit == node . Limit ) && ( this . Offset == node . Offset ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Limit.GetHashCode()) ^ (Offset.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSTop(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SDataSourceNestedQueryS : SSyntaxNode
    {
        public SDataSourceNestedQueryS(SSelect query)
        {
        }

        public SSelect Query
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SDataSourceNestedQueryS ) obj ;  return  ( Compare ( this . Query ,  node . Query ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Query == null ? 0 : Query.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSDataSourceNestedQueryS(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SExpressionNestedQueryNode : SSyntaxNode
    {
        public SExpressionNestedQueryNode(SSelect query)
        {
        }

        public SSelect Query
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SExpressionNestedQueryNode ) obj ;  return  ( Compare ( this . Query ,  node . Query ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Query == null ? 0 : Query.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSExpressionNestedQueryNode(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SSelectFieldExpression : SSyntaxNode
    {
        public SSelectFieldExpression(List<SExpression> exp)
        {
            Exp = new List<SExpression>();
        }

        public List<SExpression> Exp
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SSelectFieldExpression ) obj ;  return  ( SequenceEqual ( this . Exp ,  node . Exp ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Exp, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSSelectFieldExpression(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SSelect : SSyntaxNode
    {
        public SSelect(List<SSelectFieldExpression> fields, SFrom from, SWhere where, SGroupBy groupBy, SOrderBy orderBy, STop top)
        {
            Fields = new List<SSelectFieldExpression>();
        }

        public List<SSelectFieldExpression> Fields
        {
            get;
            set;
        }

        public SFrom From
        {
            get;
            set;
        }

        public SWhere Where
        {
            get;
            set;
        }

        public SGroupBy GroupBy
        {
            get;
            set;
        }

        public SOrderBy OrderBy
        {
            get;
            set;
        }

        public STop Top
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SSelect ) obj ;  return  ( SequenceEqual ( this . Fields ,  node . Fields ) && Compare ( this . From ,  node . From ) && Compare ( this . Where ,  node . Where ) && Compare ( this . GroupBy ,  node . GroupBy ) && Compare ( this . OrderBy ,  node . OrderBy ) && Compare ( this . Top ,  node . Top ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Fields, i => i.GetHashCode()) ^ (From == null ? 0 : From.GetHashCode()) ^ (Where == null ? 0 : Where.GetHashCode()) ^ (GroupBy == null ? 0 : GroupBy.GetHashCode()) ^ (OrderBy == null ? 0 : OrderBy.GetHashCode()) ^ (Top == null ? 0 : Top.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSSelect(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SOrderByExpression : SSyntaxNode
    {
        public SOrderByExpression(List<SExpression> exp)
        {
            Exp = new List<SExpression>();
        }

        public List<SExpression> Exp
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SOrderByExpression ) obj ;  return  ( SequenceEqual ( this . Exp ,  node . Exp ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Exp, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSOrderByExpression(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SOrderBy : SSyntaxNode
    {
        public SOrderBy(OrderDirection direction, List<SOrderByExpression> fields)
        {
            Fields = new List<SOrderByExpression>();
        }

        public OrderDirection Direction
        {
            get;
            set;
        }

        public List<SOrderByExpression> Fields
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SOrderBy ) obj ;  return  ( ( this . Direction == node . Direction ) && SequenceEqual ( this . Fields ,  node . Fields ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Direction.GetHashCode()) ^ Xor(Fields, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSOrderBy(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SGroupByExpression : SSyntaxNode
    {
        public SGroupByExpression(List<SExpression> exp)
        {
            Exp = new List<SExpression>();
        }

        public List<SExpression> Exp
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SGroupByExpression ) obj ;  return  ( SequenceEqual ( this . Exp ,  node . Exp ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Exp, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSGroupByExpression(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SGroupBy : SSyntaxNode
    {
        public SGroupBy(List<SGroupByExpression> fields)
        {
            Fields = new List<SGroupByExpression>();
        }

        public List<SGroupByExpression> Fields
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SGroupBy ) obj ;  return  ( SequenceEqual ( this . Fields ,  node . Fields ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Fields, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSGroupBy(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SCondition : SExpression
    {
        public SCondition()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSCondition(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SJoin : SSyntaxNode
    {
        public SJoin(SDataSource dataSource, SCondition condition, JoinType joinType)
        {
        }

        public SDataSource DataSource
        {
            get;
            set;
        }

        public SCondition Condition
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
                return false; var  node  =  ( SJoin ) obj ;  return  ( Compare ( this . DataSource ,  node . DataSource ) && Compare ( this . Condition ,  node . Condition ) && ( this . JoinType == node . JoinType ) ) ; 
        }

        public override int GetHashCode()
        {
            return (DataSource == null ? 0 : DataSource.GetHashCode()) ^ (Condition == null ? 0 : Condition.GetHashCode()) ^ (JoinType.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSJoin(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SWhere : SSyntaxNode
    {
        public SWhere(SCondition condition)
        {
        }

        public SCondition Condition
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SWhere ) obj ;  return  ( Compare ( this . Condition ,  node . Condition ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Condition == null ? 0 : Condition.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSWhere(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SFrom : SSyntaxNode
    {
        public SFrom(SDataSource dataSource, List<SJoin> join)
        {
            Join = new List<SJoin>();
        }

        public SDataSource DataSource
        {
            get;
            set;
        }

        public List<SJoin> Join
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SFrom ) obj ;  return  ( Compare ( this . DataSource ,  node . DataSource ) && SequenceEqual ( this . Join ,  node . Join ) ) ; 
        }

        public override int GetHashCode()
        {
            return (DataSource == null ? 0 : DataSource.GetHashCode()) ^ Xor(Join, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSFrom(this);
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

        public virtual T VisitSExpression(SExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSDataSource(SDataSource node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSTable(STable node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSTop(STop node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSDataSourceNestedQueryS(SDataSourceNestedQueryS node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSExpressionNestedQueryNode(SExpressionNestedQueryNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSSelectFieldExpression(SSelectFieldExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSSelect(SSelect node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSOrderByExpression(SOrderByExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSOrderBy(SOrderBy node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSGroupByExpression(SGroupByExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSGroupBy(SGroupBy node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSCondition(SCondition node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSJoin(SJoin node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSWhere(SWhere node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSFrom(SFrom node)
        {
            return DefaultVisit(node);
        }

        public virtual T DefaultVisit(SSyntaxNode node)
        {
            return default;
        }

        public virtual T Visit(SSyntaxNode visitable)
        {
            if (visitable is null)
                return default;
            return visitable.Accept(this);
        }
    }
}
