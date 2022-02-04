using Aquila.Core.Querying.Model;
using Aquila.Runtime;
using MoreLinq;

namespace Aquila.Core.Querying
{
    public class InsertionRealWalker : RealWalkerBase
    {
        private int parameterIndex = 0;

        public InsertionRealWalker(DatabaseRuntimeContext drContext) : base(drContext)
        {
        }

        public override void VisitQInsertQuery(QInsertQuery arg)
        {
            //begin insertion query
            Qm.bg_query();

            //load values section
            TransformValuesIntoSelect(arg.Values, arg.Criteria);

            //load insert section
            Visit(arg.Insert);

            //store query
            Qm.st_query();
        }

        public void TransformValuesIntoSelect(QExpressionSet set, QCriterionList criteria)
        {
            if (criteria.Any())
            {
                //transform criteria here
                Qm.m_where()
                    .ld_const(1)
                    .ld_const(1)
                    .eq();
            }

            //extract to the select statement
            Qm.m_select();
            var list = set[0];

            foreach (var qExpression in list)
            {
                Visit(qExpression);
            }
        }

        public override void VisitQParameter(QParameter arg)
        {
            var p = $"p{parameterIndex++}";
            Qm.ld_param(p);
        }

        public override void VisitQSourceFieldExpression(QSourceFieldExpression arg)
        {
            Qm.ld_column(arg.GetDbName());
        }

        public override void VisitQInsert(QInsert arg)
        {
            Qm.m_insert();

            Visit(arg.Target);
            Visit(arg.Fields);
        }

        public override void VisitQObjectTable(QObjectTable arg)
        {
            Qm.ld_table(arg.GetDbName());
        }
    }
}