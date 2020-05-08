using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Core.Querying.Model;

namespace Aquila.Core.Querying
{
    public class PhysicalNameWalker : QLangWalker
    {
        public int _aliasCount;
        public int _fieldCount;
        public int _tableCount;

        public int _paramCount;
        private Dictionary<string, int> _params;


        public PhysicalNameWalker()
        {
            _params = new Dictionary<string, int>();
        }

        public override object VisitQQuery(QQuery node)
        {
            VisitQFrom(node.From);
            VisitQWhere(node.Where);
            VisitQGroupBy(node.GroupBy);
            VisitQHaving(node.Having);
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

        public override object VisitQParameter(QParameter arg)
        {
            if (_params.TryGetValue(arg.Name, out var index))
            {
                arg.SetDbNameIfEmpty($"p{index}");
            }
            else
            {
                _params.Add(arg.Name, _paramCount);
                arg.SetDbNameIfEmpty($"p{_paramCount++}");
            }

            //Calculate type
            var binaryExpr = CurrentStackTree.FirstOrDefault(x => x is QOperationExpression) as QOperationExpression;

            if (binaryExpr != null)
            {
                QExpression expr;

                if (CurrentStackTree.Contains(binaryExpr.Left))
                    expr = binaryExpr.Right;
                else
                    expr = binaryExpr.Left;

                foreach (var atomType in expr.GetExpressionType())
                {
                    arg.AddType(atomType);
                }
            }

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