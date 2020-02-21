using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Core.Querying.Model;

namespace ZenPlatform.Core.Querying
{
    public class PhysicalNameWalker : QLangWalker
    {
        public int _aliasCount;
        public int _fieldCount;
        public int _tableCount;

        public override object VisitQQuery(QQuery node)
        {
            VisitQFrom(node.From);
//            VisitQWhere(node.Where);
//            VisitQGroupBy(node.GroupBy);
//            VisitQHaving(node.Having);
            VisitQSelect(node.Select);

            return null;
        }

        public override object VisitQObjectTable(QObjectTable node)
        {
            node.SetDbNameIfEmpty($"{node.ObjectType.GetSettings().DatabaseName}");
            return base.VisitQObjectTable(node);
        }

        public override object VisitQTable(QTable node)
        {
            node.SetDbNameIfEmpty($"{node.Table.GetSettings().DatabaseName}");
            return base.VisitQTable(node);
        }

        public override object VisitQAliasedDataSource(QAliasedDataSource node)
        {
            node.SetDbNameIfEmpty($"T{_tableCount++}");
            return base.VisitQAliasedDataSource(node);
        }

        public override object VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            node.SetDbNameIfEmpty($"{node.Property.GetSettings().DatabaseName}");
            return base.VisitQSourceFieldExpression(node);
        }

        public override object VisitQAliasedSelectExpression(QAliasedSelectExpression node)
        {
            node.SetDbNameIfEmpty($"A{_aliasCount++}");
            return base.VisitQAliasedSelectExpression(node);
        }

        public override object VisitQNestedQueryField(QNestedQueryField node)
        {
            base.VisitQNestedQueryField(node);
            node.SetDbName(node.Field.GetDbName());
            return null;
        }

        public override object VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
            base.VisitQIntermediateSourceField(node);
            node.SetDbName(node.Field.GetDbName());
            return null;
        }

        public override object VisitQSelectExpression(QSelectExpression node)
        {
            base.VisitQSelectExpression(node);
            node.SetDbName(node.Element.GetDbName());
            return null;
        }
    }
}