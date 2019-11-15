using System;
using System.Linq;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;

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
        public SExpression(): base()
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
        public SDataSource(): base()
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
    public partial class STable : SDataSource
    {
        public STable( string  name): base()
        {
            Name = name;
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
        public STop( int  limit,  int  offset): base()
        {
            Limit = limit;
            Offset = offset;
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
    public partial class SDataSourceNestedQuery : SDataSource
    {
        public SDataSourceNestedQuery(SSelect query): base()
        {
            Query = query;
        }

        public SSelect Query
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SDataSourceNestedQuery ) obj ;  return  ( Compare ( this . Query ,  node . Query ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Query == null ? 0 : Query.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSDataSourceNestedQuery(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SExpressionNestedQueryNode : SExpression
    {
        public SExpressionNestedQueryNode(SSelect query): base()
        {
            Query = query;
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
    public partial class SField : SExpression
    {
        public SField( string  name,  string  table): base()
        {
            Name = name;
            Table = table;
        }

        public string Name
        {
            get;
            set;
        }

        public string Table
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SField ) obj ;  return  ( ( this . Name == node . Name ) && ( this . Table == node . Table ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Name.GetHashCode()) ^ (Table.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSField(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SAliasedExpression : SExpression
    {
        public SAliasedExpression(SExpression expression,  string  name): base()
        {
            Expression = expression;
            Name = name;
        }

        public SExpression Expression
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SAliasedExpression ) obj ;  return  ( Compare ( this . Expression ,  node . Expression ) && ( this . Name == node . Name ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Expression == null ? 0 : Expression.GetHashCode()) ^ (Name.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSAliasedExpression(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SAliasedDataSource : SDataSource
    {
        public SAliasedDataSource(SDataSource dataSource,  string  name): base()
        {
            DataSource = dataSource;
            Name = name;
        }

        public SDataSource DataSource
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SAliasedDataSource ) obj ;  return  ( Compare ( this . DataSource ,  node . DataSource ) && ( this . Name == node . Name ) ) ; 
        }

        public override int GetHashCode()
        {
            return (DataSource == null ? 0 : DataSource.GetHashCode()) ^ (Name.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSAliasedDataSource(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SSelect : SDataSource
    {
        public SSelect(List<SExpression> fields, STop top, SHaving having, SOrderBy orderBy, SGroupBy groupBy, SWhere where, SFrom from): base()
        {
            Fields = fields;
            Top = top;
            Having = having;
            OrderBy = orderBy;
            GroupBy = groupBy;
            Where = where;
            From = from;
        }

        public List<SExpression> Fields
        {
            get;
            set;
        }

        public STop Top
        {
            get;
            set;
        }

        public SHaving Having
        {
            get;
            set;
        }

        public SOrderBy OrderBy
        {
            get;
            set;
        }

        public SGroupBy GroupBy
        {
            get;
            set;
        }

        public SWhere Where
        {
            get;
            set;
        }

        public SFrom From
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SSelect ) obj ;  return  ( SequenceEqual ( this . Fields ,  node . Fields ) && Compare ( this . Top ,  node . Top ) && Compare ( this . Having ,  node . Having ) && Compare ( this . OrderBy ,  node . OrderBy ) && Compare ( this . GroupBy ,  node . GroupBy ) && Compare ( this . Where ,  node . Where ) && Compare ( this . From ,  node . From ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Fields, i => i.GetHashCode()) ^ (Top == null ? 0 : Top.GetHashCode()) ^ (Having == null ? 0 : Having.GetHashCode()) ^ (OrderBy == null ? 0 : OrderBy.GetHashCode()) ^ (GroupBy == null ? 0 : GroupBy.GetHashCode()) ^ (Where == null ? 0 : Where.GetHashCode()) ^ (From == null ? 0 : From.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSSelect(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SConstant : SExpression
    {
        public SConstant( object  value): base()
        {
            Value = value;
        }

        public object Value
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SConstant ) obj ;  return  ( ( this . Value . Equals ( node . Value ) ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Value == null ? 0 : Value.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSConstant(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SMarker : SSyntaxNode
    {
        public SMarker(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSMarker(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SCoalese : SExpression
    {
        public SCoalese(List<SExpression> expressions): base()
        {
            Expressions = expressions;
        }

        public List<SExpression> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SCoalese ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSCoalese(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SOrderBy : SSyntaxNode
    {
        public SOrderBy(OrderDirection direction, List<SExpression> fields): base()
        {
            Direction = direction;
            Fields = fields;
        }

        public OrderDirection Direction
        {
            get;
            set;
        }

        public List<SExpression> Fields
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
    public partial class SGroupBy : SSyntaxNode
    {
        public SGroupBy(List<SExpression> fields): base()
        {
            Fields = fields;
        }

        public List<SExpression> Fields
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
    public partial class SHaving : SSyntaxNode
    {
        public SHaving(SCondition condition): base()
        {
            Condition = condition;
        }

        public SCondition Condition
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SHaving ) obj ;  return  ( Compare ( this . Condition ,  node . Condition ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Condition == null ? 0 : Condition.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSHaving(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SCondition : SExpression
    {
        public SCondition(): base()
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
    public partial class SGreatThen : SCondition
    {
        public SGreatThen(SExpression left, SExpression right): base()
        {
            Left = left;
            Right = right;
        }

        public SExpression Left
        {
            get;
            set;
        }

        public SExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SGreatThen ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSGreatThen(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SLessThen : SCondition
    {
        public SLessThen(SExpression left, SExpression right): base()
        {
            Left = left;
            Right = right;
        }

        public SExpression Left
        {
            get;
            set;
        }

        public SExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SLessThen ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSLessThen(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SGreatThenOrEquals : SCondition
    {
        public SGreatThenOrEquals(SExpression left, SExpression right): base()
        {
            Left = left;
            Right = right;
        }

        public SExpression Left
        {
            get;
            set;
        }

        public SExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SGreatThenOrEquals ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSGreatThenOrEquals(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SLessThenOrEquals : SCondition
    {
        public SLessThenOrEquals(SExpression left, SExpression right): base()
        {
            Left = left;
            Right = right;
        }

        public SExpression Left
        {
            get;
            set;
        }

        public SExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SLessThenOrEquals ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSLessThenOrEquals(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SNotEquals : SCondition
    {
        public SNotEquals(SExpression left, SExpression right): base()
        {
            Left = left;
            Right = right;
        }

        public SExpression Left
        {
            get;
            set;
        }

        public SExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SNotEquals ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSNotEquals(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SEquals : SCondition
    {
        public SEquals(SExpression left, SExpression right): base()
        {
            Left = left;
            Right = right;
        }

        public SExpression Left
        {
            get;
            set;
        }

        public SExpression Right
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SEquals ) obj ;  return  ( Compare ( this . Left ,  node . Left ) && Compare ( this . Right ,  node . Right ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Left == null ? 0 : Left.GetHashCode()) ^ (Right == null ? 0 : Right.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSEquals(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SAnd : SCondition
    {
        public SAnd(List<SExpression> expressions): base()
        {
            Expressions = expressions;
        }

        public List<SExpression> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SAnd ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSAnd(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SOr : SCondition
    {
        public SOr(List<SExpression> expressions): base()
        {
            Expressions = expressions;
        }

        public List<SExpression> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SOr ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSOr(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SAdd : SExpression
    {
        public SAdd(List<SExpression> expressions): base()
        {
            Expressions = expressions;
        }

        public List<SExpression> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SAdd ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSAdd(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SSub : SExpression
    {
        public SSub(List<SExpression> expressions): base()
        {
            Expressions = expressions;
        }

        public List<SExpression> Expressions
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SSub ) obj ;  return  ( SequenceEqual ( this . Expressions ,  node . Expressions ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Expressions, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSSub(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SSum : SExpression
    {
        public SSum(SExpression argument): base()
        {
            Argument = argument;
        }

        public SExpression Argument
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SSum ) obj ;  return  ( Compare ( this . Argument ,  node . Argument ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Argument == null ? 0 : Argument.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSSum(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SCount : SExpression
    {
        public SCount(SExpression argument): base()
        {
            Argument = argument;
        }

        public SExpression Argument
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SCount ) obj ;  return  ( Compare ( this . Argument ,  node . Argument ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Argument == null ? 0 : Argument.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSCount(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SAvg : SExpression
    {
        public SAvg(SExpression argument): base()
        {
            Argument = argument;
        }

        public SExpression Argument
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SAvg ) obj ;  return  ( Compare ( this . Argument ,  node . Argument ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Argument == null ? 0 : Argument.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSAvg(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SParameter : SExpression
    {
        public SParameter( string  name): base()
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SParameter ) obj ;  return  ( ( this . Name == node . Name ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Name.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSParameter(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SJoin : SSyntaxNode
    {
        public SJoin(SCondition condition, SDataSource dataSource, JoinType joinType): base()
        {
            Condition = condition;
            DataSource = dataSource;
            JoinType = joinType;
        }

        public SCondition Condition
        {
            get;
            set;
        }

        public SDataSource DataSource
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
                return false; var  node  =  ( SJoin ) obj ;  return  ( Compare ( this . Condition ,  node . Condition ) && Compare ( this . DataSource ,  node . DataSource ) && ( this . JoinType == node . JoinType ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Condition == null ? 0 : Condition.GetHashCode()) ^ (DataSource == null ? 0 : DataSource.GetHashCode()) ^ (JoinType.GetHashCode());
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
        public SWhere(SCondition condition): base()
        {
            Condition = condition;
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
        public SFrom(List<SJoin> join, SDataSource dataSource): base()
        {
            Join = join;
            DataSource = dataSource;
        }

        public List<SJoin> Join
        {
            get;
            set;
        }

        public SDataSource DataSource
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SFrom ) obj ;  return  ( SequenceEqual ( this . Join ,  node . Join ) && Compare ( this . DataSource ,  node . DataSource ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Join, i => i.GetHashCode()) ^ (DataSource == null ? 0 : DataSource.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSFrom(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SInsert : SSyntaxNode
    {
        public SInsert(List<SField> fields, STable into, SDataSource dataSource): base()
        {
            Fields = fields;
            Into = into;
            DataSource = dataSource;
        }

        public List<SField> Fields
        {
            get;
            set;
        }

        public STable Into
        {
            get;
            set;
        }

        public SDataSource DataSource
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SInsert ) obj ;  return  ( SequenceEqual ( this . Fields ,  node . Fields ) && Compare ( this . Into ,  node . Into ) && Compare ( this . DataSource ,  node . DataSource ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Fields, i => i.GetHashCode()) ^ (Into == null ? 0 : Into.GetHashCode()) ^ (DataSource == null ? 0 : DataSource.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSInsert(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SValuesSource : SDataSource
    {
        public SValuesSource(List<SExpression> values): base()
        {
            Values = values;
        }

        public List<SExpression> Values
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SValuesSource ) obj ;  return  ( SequenceEqual ( this . Values ,  node . Values ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Values, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSValuesSource(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SSetItem : SSyntaxNode
    {
        public SSetItem(SField field, SExpression value): base()
        {
            Field = field;
            Value = value;
        }

        public SField Field
        {
            get;
            set;
        }

        public SExpression Value
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SSetItem ) obj ;  return  ( Compare ( this . Field ,  node . Field ) && Compare ( this . Value ,  node . Value ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Field == null ? 0 : Field.GetHashCode()) ^ (Value == null ? 0 : Value.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSSetItem(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SSet : SSyntaxNode
    {
        public SSet(List<SSetItem> items): base()
        {
            Items = items;
        }

        public List<SSetItem> Items
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SSet ) obj ;  return  ( SequenceEqual ( this . Items ,  node . Items ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(Items, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSSet(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SUpdate : SSyntaxNode
    {
        public SUpdate(SDataSource update, SSet set, SWhere where, SFrom from): base()
        {
            Update = update;
            Set = set;
            Where = where;
            From = from;
        }

        public SDataSource Update
        {
            get;
            set;
        }

        public SSet Set
        {
            get;
            set;
        }

        public SWhere Where
        {
            get;
            set;
        }

        public SFrom From
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SUpdate ) obj ;  return  ( Compare ( this . Update ,  node . Update ) && Compare ( this . Set ,  node . Set ) && Compare ( this . Where ,  node . Where ) && Compare ( this . From ,  node . From ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Update == null ? 0 : Update.GetHashCode()) ^ (Set == null ? 0 : Set.GetHashCode()) ^ (Where == null ? 0 : Where.GetHashCode()) ^ (From == null ? 0 : From.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSUpdate(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SWhen : SSyntaxNode
    {
        public SWhen(SCondition condition, SExpression then): base()
        {
            Condition = condition;
            Then = then;
        }

        public SCondition Condition
        {
            get;
            set;
        }

        public SExpression Then
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SWhen ) obj ;  return  ( Compare ( this . Condition ,  node . Condition ) && Compare ( this . Then ,  node . Then ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Condition == null ? 0 : Condition.GetHashCode()) ^ (Then == null ? 0 : Then.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSWhen(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class SCase : SExpression
    {
        public SCase(SExpression @else, List<SWhen> whens): base()
        {
            Else = @else;
            Whens = whens;
        }

        public SExpression Else
        {
            get;
            set;
        }

        public List<SWhen> Whens
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SCase ) obj ;  return  ( Compare ( this . Else ,  node . Else ) && SequenceEqual ( this . Whens ,  node . Whens ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Else == null ? 0 : Else.GetHashCode()) ^ Xor(Whens, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSCase(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class Querys : SSyntaxNode
    {
        public Querys(): base()
        {
            QueryList = new List<SSyntaxNode>();
        }

        public List<SSyntaxNode> QueryList
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( Querys ) obj ;  return  ( SequenceEqual ( this . QueryList ,  node . QueryList ) ) ; 
        }

        public override int GetHashCode()
        {
            return Xor(QueryList, i => i.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitQuerys(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class StringValue : SSyntaxNode
    {
        public StringValue(): base()
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
        public Scheme(): base()
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
        public Database(): base()
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
        public Table(): base()
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
        public Column(): base()
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
        public Constraint(): base()
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
    public partial class SchemeOperation : SSyntaxNode
    {
        public SchemeOperation(): base()
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
        public DatabaseOperation(): base()
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
        public TableOperation(): base()
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
    public partial class ColumnType : SSyntaxNode
    {
        public ColumnType(): base()
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
    public partial class SizableType : ColumnType
    {
        public SizableType(): base()
        {
        }

        public int Size
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( SizableType ) obj ;  return  ( ( this . Size == node . Size ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Size.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitSizableType(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class PrecisionType : ColumnType
    {
        public PrecisionType(): base()
        {
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

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
                return false; var  node  =  ( PrecisionType ) obj ;  return  ( ( this . Scale == node . Scale ) && ( this . Precision == node . Precision ) ) ; 
        }

        public override int GetHashCode()
        {
            return (Scale.GetHashCode()) ^ (Precision.GetHashCode());
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitPrecisionType(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeChar : ColumnType
    {
        public ColumnTypeChar(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeChar(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeVarChar : SizableType
    {
        public ColumnTypeVarChar(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeVarChar(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeBlob : SizableType
    {
        public ColumnTypeBlob(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeBlob(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeBynary : SizableType
    {
        public ColumnTypeBynary(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeBynary(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeGuid : ColumnType
    {
        public ColumnTypeGuid(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeGuid(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeText : SizableType
    {
        public ColumnTypeText(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeText(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeInt : ColumnType
    {
        public ColumnTypeInt(): base()
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
    public partial class ColumnTypeSmallInt : ColumnType
    {
        public ColumnTypeSmallInt(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeSmallInt(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeBigInt : ColumnType
    {
        public ColumnTypeBigInt(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeBigInt(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeBool : ColumnType
    {
        public ColumnTypeBool(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeBool(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeFloat : SizableType
    {
        public ColumnTypeFloat(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeFloat(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeDecimal : PrecisionType
    {
        public ColumnTypeDecimal(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeDecimal(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeNumeric : PrecisionType
    {
        public ColumnTypeNumeric(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeNumeric(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnTypeDataTime : ColumnType
    {
        public ColumnTypeDataTime(): base()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(QueryVisitorBase<T> visitor)
        {
            return visitor.VisitColumnTypeDataTime(this);
        }
    }
}

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class ColumnDefinition : SSyntaxNode
    {
        public ColumnDefinition(): base()
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
    public partial class ConstraintDefinition : SSyntaxNode
    {
        public ConstraintDefinition(): base()
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
        public ConstraintDefinitionUnique(): base()
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
        public ConstraintDefinitionPrimaryKey(): base()
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
        public ConstraintDefinitionForeignKey(): base()
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
        public CreateTable(): base()
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
        public DropTable(): base()
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
        public DropColumn(): base()
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
        public CopyTable(): base()
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
        public AlterAddColumn(): base()
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
        public AddColumn(): base()
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
        public AlterColumn(): base()
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
        public AddConstraint(): base()
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
        public DropConstraint(): base()
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
    public partial class RenameTableNode : SSyntaxNode
    {
        public RenameTableNode(): base()
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

        public virtual T VisitSDataSourceNestedQuery(SDataSourceNestedQuery node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSExpressionNestedQueryNode(SExpressionNestedQueryNode node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSField(SField node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSAliasedExpression(SAliasedExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSAliasedDataSource(SAliasedDataSource node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSSelect(SSelect node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSConstant(SConstant node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSMarker(SMarker node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSCoalese(SCoalese node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSOrderBy(SOrderBy node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSGroupBy(SGroupBy node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSHaving(SHaving node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSCondition(SCondition node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSGreatThen(SGreatThen node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSLessThen(SLessThen node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSGreatThenOrEquals(SGreatThenOrEquals node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSLessThenOrEquals(SLessThenOrEquals node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSNotEquals(SNotEquals node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSEquals(SEquals node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSAnd(SAnd node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSOr(SOr node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSAdd(SAdd node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSSub(SSub node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSSum(SSum node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSCount(SCount node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSAvg(SAvg node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSParameter(SParameter node)
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

        public virtual T VisitSInsert(SInsert node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSValuesSource(SValuesSource node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSSetItem(SSetItem node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSSet(SSet node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSUpdate(SUpdate node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSWhen(SWhen node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSCase(SCase node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQuerys(Querys node)
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

        public virtual T VisitSizableType(SizableType node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPrecisionType(PrecisionType node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeChar(ColumnTypeChar node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeVarChar(ColumnTypeVarChar node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeBlob(ColumnTypeBlob node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeBynary(ColumnTypeBynary node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeGuid(ColumnTypeGuid node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeText(ColumnTypeText node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeInt(ColumnTypeInt node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeSmallInt(ColumnTypeSmallInt node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeBigInt(ColumnTypeBigInt node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeBool(ColumnTypeBool node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeFloat(ColumnTypeFloat node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeDecimal(ColumnTypeDecimal node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeNumeric(ColumnTypeNumeric node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitColumnTypeDataTime(ColumnTypeDataTime node)
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

        public virtual T VisitRenameTableNode(RenameTableNode node)
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
