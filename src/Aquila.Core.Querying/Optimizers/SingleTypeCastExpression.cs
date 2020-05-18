using System.Linq;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Core.Querying.Model;
using Aquila.QueryBuilder;

namespace Aquila.Core.Querying.Optimizers
{
    public class SingleTypeCastExpression : SingleTypeExpr
    {
        private readonly QCast _cast;

        public SingleTypeCastExpression(QCast cast, RealWalker rw, QueryMachine qm) : base(rw, qm)
        {
            _cast = cast;
        }

        public override void Emit()
        {
            var baseTypes = _cast.BaseExpression.GetExpressionType().ToList();
            if (baseTypes.Count == 1)
            {
                Rw.Visit(_cast.BaseExpression);
                Qm.ld_table(_cast.Type.ConvertToDbType());
                Qm.cast();

                return;
            }

            var mte = TypedExprFactory.CreateMultiTypedExpr(_cast.BaseExpression, Qm, Rw);

            var valueTypes = baseTypes.Where(x => x.IsPrimitive).ToList();
            var refTypes = baseTypes.Where(x => x.IsObject).ToList();

            foreach (var type in valueTypes)
            {
                mte.EmitValueColumn(type);

                Qm.ld_type(_cast.Type.ConvertToDbType());
                Qm.cast();

                mte.EmitTypeColumn();

                Qm.ld_const(type.GetSettings().SystemId);
                Qm.eq();

                Qm.when();
            }

            Qm.@case();

            //Ссылочные типы по умолчанию нельзя кастовать, там будет NULL
        }
    }
}