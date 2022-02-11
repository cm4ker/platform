using System.Linq;
using Aquila.Core.Querying.Model;
using Aquila.Core.Querying.Optimizers;
using Aquila.Runtime;

namespace Aquila.Core.Querying
{
    public class DeletionRealWalker : RealWalkerBase
    {
        private int parameterIndex = 0;
        private SelectionRealWalker srw;

        public DeletionRealWalker(DatabaseRuntimeContext drContext) : base(drContext)
        {
            srw = new SelectionRealWalker(drContext, Qm);
        }

        public override void VisitQDeleteQuery(QDeleteQuery arg)
        {
            //begin insertion query
            Qm.bg_query();

            VisitQFrom(arg.From);
            VisitQWhere(arg.Where);

            VisitQCriterionList(arg.Criteria);

            VisitQDelete(arg.Delete);


            //store query
            Qm.st_query();
        }

        public override void VisitQDelete(QDelete arg)
        {
            Qm.m_delete();
            Qm.ld_table(arg.Target.GetDbName());
        }

        public override void VisitQFrom(QFrom arg)
        {
            //Qm.m_from();
            srw.VisitQFrom(arg);
        }

        public override void VisitQWhere(QWhere arg)
        {
            Qm.m_where();
            srw.Visit(arg.Expression);
        }


        public override void VisitQCriterionList(QCriterionList arg)
        {
            if (!arg.Any())
            {
                return;
            }

            //then
            Qm.ld_const(0);

            bool needOr = false;

            foreach (var item in arg)
            {
                //condition
                VisitQCriterion(item);
                Qm.exists();

                if (needOr)
                {
                    Qm.or();
                }

                needOr = true;
            }

            Qm.when();
            //else
            Qm.ld_const(int.MaxValue);
            Qm.@case()
                .ld_const(int.MaxValue)
                .add()
                .ld_const(int.MaxValue)
                .eq();

            //we have some criteria. previous statement id = @id and we have to chain it
            Qm.and();
        }

        public override void VisitQCriterion(QCriterion arg)
        {
            Qm.bg_query();
            Qm.m_from();
            Qm.bg_query().m_select().ld_const(1).@as("_sec_fld").st_query().@as("_sec_dummy");

            srw.Visit(arg.From.Joins);
            srw.Visit(arg.Where);

            Qm.m_select();
            Qm.ld_const(1);

            Qm.st_query();
        }

        public override void VisitQParameter(QParameter arg)
        {
            Qm.ld_param(arg.GetDbName());
        }

        public override void VisitQObjectTable(QObjectTable arg)
        {
            Qm.ld_table(arg.GetDbName());
        }
    }
}