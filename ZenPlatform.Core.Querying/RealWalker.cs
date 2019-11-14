using System.IO;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying
{
    public class RealWalker : QLangWalker
    {
        private QueryMachine _qm;
        private StringWriter _l;
        public string Log => _l.ToString();

        public RealWalker()
        {
            _qm = new QueryMachine();
            _l = new StringWriter();
        }

        public QueryMachine QueryMachine => _qm;


        public override object VisitQQuery(QQuery node)
        {
            _qm.ct_query();
            _l.WriteLine("ct_query");

            var res = base.VisitQQuery(node);

            _qm.st_query();
            _l.WriteLine("st_query");

            return res;
        }

        public override object VisitQSelect(QSelect node)
        {
            _qm.m_select();
            _l.WriteLine("m_select");
            return base.VisitQSelect(node);
        }


        public override object VisitQFrom(QFrom node)
        {
            _qm.m_from();
            _l.WriteLine("m_from");

            Visit(node.Source);

            foreach (var nodeJoin in node.Joins)
            {
                VisitQFromItem(nodeJoin);
            }

            return null;
        }

        public override object VisitQSelectExpression(QSelectExpression node)
        {
            return base.VisitQSelectExpression(node);
        }

        public override object VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
            if (node.DataSource is QAliasedDataSource ads)
            {
                _qm.ld_str(ads.Alias);
                _l.WriteLine($"ld_str({ads.Alias})");
            }

            return null;
        }

        public override object VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            _qm.ld_str(node.Property.Name);
            _l.WriteLine($"ld_str({node.Property.Name})");
            _qm.ld_column();
            _l.WriteLine($"ld_column");
            
            return base.VisitQSourceFieldExpression(node);
        }

        public override object VisitQAliasedSelectExpression(QAliasedSelectExpression node)
        {
            var res = base.VisitQAliasedSelectExpression(node);
            _qm.@as(node.Alias);
            _l.WriteLine($"as({node.Alias})");
            return res;
        }
    }
}