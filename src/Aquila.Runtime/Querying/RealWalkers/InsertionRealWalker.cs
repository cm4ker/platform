using Aquila.Core.Querying.Model;
using Aquila.Runtime;

namespace Aquila.Core.Querying
{
    public class InsertionRealWalker : RealWalkerBase
    {
        public InsertionRealWalker(DatabaseRuntimeContext drContext) : base(drContext)
        {
        }

        public override void VisitQInsertQuery(QInsertQuery arg)
        {
            //begin insertion query
            Qm.bg_query();

            //load values section
            Qm.m_values();
            Visit(arg.Values);

            //load insert section
            Visit(arg.Insert);

            //store query
            Qm.st_query();
        }

        public override void VisitQExpression(QExpression arg)
        {
            Qm.ld_column("test");
        }

        public override void VisitQParameter(QParameter arg)
        {
            Qm.ld_param("p1");
        }

        public override void VisitQSourceFieldExpression(QSourceFieldExpression arg)
        {
            Qm.ld_column(arg.GetName());
        }

        public override void VisitQInsert(QInsert arg)
        {
            Qm.m_insert();

            Visit(arg.Target);
            Visit(arg.Fields);
        }

        public override void VisitQObjectTable(QObjectTable arg)
        {
            Qm.ld_table(arg.ObjectType.Name);
        }
    }
}