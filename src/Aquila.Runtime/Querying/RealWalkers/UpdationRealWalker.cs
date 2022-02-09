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
            //VisitQWhere(arg.Where);
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