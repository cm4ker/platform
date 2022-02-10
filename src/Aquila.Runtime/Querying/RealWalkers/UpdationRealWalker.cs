using System.Collections.Immutable;
using System.Linq;
using Aquila.Core.Querying.Model;
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
            var needOr = false;
            foreach (var item in arg)
            {
                //condition
                VisitQCriterion(item);
                Qm.exists();

                if (needOr)
                    Qm.or();

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
                    //NOTE: the underneath must be QSourceFieldExpression     
                    var f = a.Target.Find<QSourceFieldExpression>().FirstOrDefault();
                    var schema = f.Property.GetSchema(DrContext);

                    foreach (var type in schema)
                    {
                        Qm.ld_column(type.FullName);
                        Qm.ld_param($"p{parameterIndex++}");
                        Qm.assign();
                    }
                }
                else
                {
                    Qm.ld_column(a.Target.GetDbName());
                    Qm.ld_param($"p{parameterIndex++}");
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