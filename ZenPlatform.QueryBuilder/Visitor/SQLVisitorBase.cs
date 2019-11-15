using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Visitor
{
    public class SQLVisitorBase : QueryVisitorBase<string>
    {
        public override string VisitSAdd(SAdd node)
        {
            return string.Join(" + ", node.Expressions.Select(n => n.Accept(this)));
        }

        public override string VisitSAliasedDataSource(SAliasedDataSource node)
        {
            return string.Format("{0} as {1}",
                node.DataSource.Accept(this),
                node.Name);
        }

        public override string VisitSAliasedExpression(SAliasedExpression node)
        {
            return string.Format("{0} as {1}",
                node.Expression.Accept(this),
                node.Name);
        }

        public override string VisitSAnd(SAnd node)
        {
            return string.Join(" and ", node.Expressions.Select(n => n.Accept(this)));
        }

        public override string VisitSAvg(SAvg node)
        {
            return string.Format("avg({0})", node.Argument.Accept(this));
        }

        public override string VisitSCoalese(SCoalese node)
        {
            return string.Format("coalease({0})", string.Join(", ", node.Expressions.Select(e => e.Accept(this))));
        }

        public override string VisitSConstant(SConstant node)
        {
            return node.Value.ToString();
        }

        public override string VisitSCount(SCount node)
        {
            return string.Format("count({0})", node.Argument.Accept(this));
        }

        public override string VisitSDataSourceNestedQuery(SDataSourceNestedQuery node)
        {
            return $"({node.Query.Accept(this)})";
        }

        public override string VisitSEquals(SEquals node)
        {
            return string.Format("{0} = {1}", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSField(SField node)
        {
            return string.Format("{0}{1}",
                string.IsNullOrEmpty(node.Table) ? "" : node.Table + ".",
                node.Name
            );
        }

        public override string VisitSFrom(SFrom node)
        {
            return string.Format("FROM\n{0}{1}{2}\n",
                node.DataSource.Accept(this),
                node.Join.Count > 0 ? "\n" : "",
                string.Join("\n", node.Join.Select(j => j.Accept(this))));
        }

        public override string VisitSGreatThen(SGreatThen node)
        {
            return string.Format("{0} > {1}", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSGreatThenOrEquals(SGreatThenOrEquals node)
        {
            return string.Format("{0} >= {1}", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSGroupBy(SGroupBy node)
        {
            return string.Format("GROUP BY\n{0}\n",
                string.Join(", ", node.Fields.Select(f => f.Accept(this))));
        }

        public override string VisitSHaving(SHaving node)
        {
            return string.Format("HAVING\n{0}\n",
                node.Condition.Accept(this));
        }

        public override string VisitSInsert(SInsert node)
        {
            return string.Format("INSERT INTO {0}\n{1}",
                node.Into.Accept(this),
                node.DataSource.Accept(this)
            );
        }

        public override string VisitSJoin(SJoin node)
        {
            return string.Format("JOIN {0} ON {1}",
                node.DataSource.Accept(this),
                node.Condition.Accept(this)
            );
        }

        public override string VisitSLessThen(SLessThen node)
        {
            return string.Format("({0} < {1})", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSLessThenOrEquals(SLessThenOrEquals node)
        {
            return string.Format("({0} <= {1})", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSNotEquals(SNotEquals node)
        {
            return string.Format("({0} <> {1})", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSOr(SOr node)
        {
            return string.Format("({0})",
                string.Join(" OR ", node.Expressions.Select(e => e.Accept(this)))
            );
        }

        public override string VisitSOrderBy(SOrderBy node)
        {
            return string.Format("ORDER BY {0}{1}\n",
                string.Join(", ", node.Fields.Select(f => f.Accept(this))),
                node.Direction == OrderDirection.DESC ? " DESC" : ""
            );
        }

        public override string VisitSParameter(SParameter node)
        {
            return string.Format("@{0}", node.Name);
        }

        public override string VisitSSelect(SSelect node)
        {
            return string.Format("SELECT {6}{0}\n{1}{2}{3}{4}{5}",
                string.Join(",\n", node.Fields.Select(f => f.Accept(this))),
                node.From == null ? "" : node.From.Accept(this),
                node.Where == null ? "" : node.Where.Accept(this),
                node.GroupBy == null ? "" : node.GroupBy.Accept(this),
                node.Having == null ? "" : node.Having.Accept(this),
                node.OrderBy == null ? "" : node.OrderBy.Accept(this),
                node.Top == null ? "" : node.OrderBy.Accept(this)
            );
        }

        public override string VisitSSetItem(SSetItem node)
        {
            return string.Format("{0} = {1}",
                node.Field.Accept(this),
                node.Value.Accept(this)
            );
        }

        public override string VisitSSub(SSub node)
        {
            return string.Format("({0})",
                string.Join(" - ", node.Expressions.Select(e => e.Accept(this))));
        }

        public override string VisitSSum(SSum node)
        {
            return string.Format("sum({0})", node.Argument.Accept(this));
        }

        public override string VisitSTable(STable node)
        {
            return string.Format("{0}", node.Name);
        }

        public override string VisitSUpdate(SUpdate node)
        {
            return string.Format("UPDATE {0}\nSET {1}\n{2}{3}",
                node.Update.Accept(this),
                string.Join(", ", node.Set.Items.Select(s => s.Accept(this))),
                node.From == null ? "" : node.From.Accept(this),
                node.Where == null ? "" : node.Where.Accept(this)
            );
        }

        public override string VisitSValuesSource(SValuesSource node)
        {
            return string.Format("VALUES\n({0})\n",
                string.Join(", ", node.Values.Select(s => s.Accept(this)))
            );
        }

        public override string VisitSWhere(SWhere node)
        {
            return string.Format("WHERE\n{0}\n",
                node.Condition.Accept(this)
            );
        }
    }
}