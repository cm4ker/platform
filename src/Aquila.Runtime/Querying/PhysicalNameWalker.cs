using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.Migrations;
using Aquila.Runtime;

namespace Aquila.Core.Querying
{
    public class PhysicalNameWalker : QLangWalker
    {
        private readonly DatabaseRuntimeContext _context;
        public int _aliasCount;
        public int _fieldCount;
        public int _tableCount;

        public int _paramCount;
        private Dictionary<string, int> _params;


        public PhysicalNameWalker(DatabaseRuntimeContext context)
        {
            _context = context;
            _params = new Dictionary<string, int>();
        }

        public override void VisitQQuery(QQuery node)
        {
            VisitQFrom(node.From);
            VisitQWhere(node.Where);
            VisitQGroupBy(node.GroupBy);
            VisitQHaving(node.Having);
            VisitQSelect(node.Select);
            VisitQCriterionList(node.Criteria);
        }

        public override void VisitQFrom(QFrom arg)
        {
            Visit(arg.Source);
            Visit(arg.Joins);
        }

        public override void VisitQObjectTable(QObjectTable node)
        {
            node.SetDbNameIfEmpty($"{node.ObjectType.GetDescriptor(_context).DatabaseName}");
            base.VisitQObjectTable(node);
        }

        // public override void VisitQTable(QTable node)
        // {
        //     node.SetDbNameIfEmpty($"{node.Table.GetSettings().DatabaseName}");
        //     return base.VisitQTable(node);
        // }

        public override void VisitQAliasedDataSource(QAliasedDataSource node)
        {
            if (node.GetDbName() == null)
                node.SetDbNameIfEmpty($"T{_tableCount++}");

            base.VisitQAliasedDataSource(node);
        }

        public override void VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            node.SetDbNameIfEmpty($"{node.Property.GetDescriptor(_context).DatabaseName}");
            base.VisitQSourceFieldExpression(node);
        }

        public override void VisitQAliasedSelectExpression(QAliasedSelectExpression node)
        {
            node.SetDbNameIfEmpty($"A{_aliasCount++}");
            base.VisitQAliasedSelectExpression(node);
        }

        public override void VisitQNestedQueryField(QNestedQueryField node)
        {
            base.VisitQNestedQueryField(node);
            node.SetDbName(node.Field.GetDbName());
        }

        public override void VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
            base.VisitQIntermediateSourceField(node);
            node.SetDbName(node.Field.GetDbName());
        }

        public override void VisitQParameter(QParameter arg)
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
        }

        public override void VisitQSelectExpression(QSelectExpression node)
        {
            base.VisitQSelectExpression(node);
            node.SetDbName(node.Element.GetDbName());
        }
    }
}