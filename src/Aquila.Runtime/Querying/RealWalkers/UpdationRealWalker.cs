using System.Collections.Immutable;
using System.Linq;
using Aquila.Core.Querying.Model;
using Aquila.Core.Querying.Optimizers;
using Aquila.Metadata;
using Aquila.Runtime;

namespace Aquila.Core.Querying
{
    public class UpdationRealWalker : RealWalkerBase
    {
        private int parameterIndex = 0;
        private SelectionRealWalker srw;

        public UpdationRealWalker(DatabaseRuntimeContext drContext) : base(drContext)
        {
            srw = new SelectionRealWalker(drContext, Qm);
        }

        public override void VisitQUpdateQuery(QUpdateQuery arg)
        {
            //begin insertion query
            Qm.bg_query();

            VisitQFrom(arg.From);
            VisitQWhere(arg.Where);
            VisitQCriterionList(arg.Criteria);

            VisitQSet(arg.Set);
            VisitQUpdate(arg.Update);

            //store query
            Qm.st_query();
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

            int criterionOnStack = 0;
            int andOnStack = 0;
            foreach (var item in arg)
            {
                //condition
                VisitQCriterion(item);
                Qm.exists();

                criterionOnStack++;

                if (criterionOnStack == 2)
                {
                    Qm.and();
                    criterionOnStack = 0;
                    andOnStack++;
                }

                if (andOnStack == 2)
                {
                    Qm.or();
                    andOnStack = 0;
                }
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


        public override void VisitQUpdate(QUpdate arg)
        {
            Qm.m_update();
            Qm.ld_table(arg.Target.GetDbName());
        }

        public override void VisitQSet(QSet arg)
        {
            Qm.m_set();

            foreach (var a in arg.Assigns)
            {
                //if complex type then we have to unwrap columns
                if (a.Target.IsComplexExprType)
                {
                    var leftExpr = a.Target;
                    var rightExpr = a.Value;

                    MultiTypedExpr left = TypedExprFactory.CreateMultiTypedExpr(leftExpr, Qm, this);
                    MultiTypedExpr right = TypedExprFactory.CreateMultiTypedExpr(rightExpr, Qm, this);

                    left.EmitTypeColumn();
                    right.EmitTypeColumn();
                    Qm.assign();

                    var needEmitRef = true;
                    foreach (var type in leftExpr.GetExpressionType())
                    {
                        if (type.IsReference && needEmitRef)
                        {
                            left.EmitRefColumn();
                            right.EmitRefColumn();
                            Qm.assign();

                            needEmitRef = false;
                            continue;
                        }

                        if (!type.IsReference)
                        {
                            left.EmitValueColumn(type);
                            right.EmitValueColumn(type);
                            Qm.assign();
                        }
                    }
                }
                else
                {
                    //Qm.ld_column(a.Target.GetDbName(), a.Target.GetSource().GetDbName());
                    srw.Visit(a.Target);
                    srw.Visit(a.Value);
                    //Qm.ld_column(a.Value.GetDbName(), );
                    Qm.assign();
                }
            }
        }

        public override void VisitQParameter(QParameter arg)
        {
            var paramCount = arg.GetProp<int?>(QLangExtensions.ParamCountComplexHidden) ?? 1;

            for (int i = 0; i < paramCount; i++)
            {
                var p = $"p{parameterIndex++}";
                Qm.ld_param(p);
            }
        }

        public override void VisitQObjectTable(QObjectTable arg)
        {
            Qm.ld_table(arg.GetDbName());
        }
    }
}