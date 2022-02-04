using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.Runtime;
using MoreLinq;

namespace Aquila.Core.Querying
{
    public class InsertionRealWalker : RealWalkerBase
    {
        private int parameterIndex = 0;
        private SelectionRealWalker srw;

        public InsertionRealWalker(DatabaseRuntimeContext drContext) : base(drContext)
        {
            srw = new SelectionRealWalker(drContext, Qm);
        }


        public override void VisitQCriterionList(QCriterionList arg)
        {
            if (!arg.Any())
            {
                return;
            }

            //we have some criteria. starting!
            Qm.m_where();

            //then
            Qm.ld_const(0);
            foreach (var item in arg)
            {
                //condition
                VisitQCriterion(item);
                Qm.exists();
            }

            Qm.when();
            //else
            Qm.ld_const(int.MaxValue);
            Qm.@case()
                .ld_const(int.MaxValue)
                .add()
                .ld_const(int.MaxValue)
                .eq();
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


        public override void VisitQInsertSelectQuery(QInsertSelectQuery arg)
        {
            //begin insertion query
            Qm.bg_query();

            //Emitting toplevel select
            var s = arg.Select;

            Qm.bg_query();
            srw.Visit(s.From);

            //Emit criteria here


            Visit(s.Criteria);

            //emitting select
            srw.Visit(s.Select);

            Qm.st_query();

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
            var paramCount = arg.GetProp<int?>(QLangExtensions.ParamCountComplexHidden) ?? 1;

            for (int i = 0; i < paramCount; i++)
            {
                var p = $"p{parameterIndex++}";
                Qm.ld_param(p);
            }
        }

        public override void VisitQSourceFieldExpression(QSourceFieldExpression arg)
        {
            //we heed get flatten ordered columns
            var types = arg.Property.GetOrderedFlattenTypes().ToImmutableList();
            var schemas = DRContextHelper.GetPropertySchemas(arg.GetDbName(), arg.GetExpressionType());

            foreach (var schema in schemas)
            {
                Qm.ld_column(schema.FullName);
            }
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