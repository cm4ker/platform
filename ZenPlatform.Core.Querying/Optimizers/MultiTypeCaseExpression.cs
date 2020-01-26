using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying.Optimizers
{
    public class MultiTypeCaseExpression : MultiTypedExpr
    {
        private readonly QCase _c;

        public MultiTypeCaseExpression(QCase c, RealWalker rw, QueryMachine qm) : base(rw, qm)
        {
            _c = c;
        }

        private void EmitTypeExpr(QExpression exp)
        {
            var expType = exp.GetExpressionType().ToList();
            if (expType.Count > 1)
            {
                TypedExprFactory.CreateMultiTypedExpr(exp, Qm, Rw).EmitTypeColumn();
            }
            else
            {
                Qm.ld_const(expType[1].Id);
            }
        }

        private void EmitValueExpr(QExpression exp, IType type)
        {
            var texpr = exp.GetExpressionType().ToList();

            if (texpr.Any(x => x.IsAssignableFrom(type)))
                if (texpr.Count > 1)
                {
                    TypedExprFactory.CreateMultiTypedExpr(exp, Qm, Rw).EmitValueColumn(type);
                }
                else
                {
                    Rw.Visit(exp);
                }
            else
            {
                Qm.ld_null();
            }
        }

        private void EmitRefExpr(QExpression exp)
        {
            var texpr = exp.GetExpressionType().ToList();

            if (texpr.Any(x => x.IsObject))
                if (texpr.Count > 1)
                {
                    TypedExprFactory.CreateMultiTypedExpr(exp, Qm, Rw).EmitRefColumn();
                }
                else
                {
                    Rw.Visit(exp);
                }
            else
            {
                Qm.ld_null();
            }
        }

        public override void EmitTypeColumn()
        {
            foreach (var w in _c.Whens)
            {
                Rw.Visit(w.When);
                EmitTypeExpr(w.Then);
                Qm.when();
            }

            if (_c.Else != null)
                EmitTypeExpr(_c.Else);

            Qm.@case();
        }

        public override void EmitValueColumn(IType type)
        {
            foreach (var w in _c.Whens)
            {
                Rw.Visit(w.When);

                EmitValueExpr(w.Then, type);

                Qm.when();
            }

            if (_c.Else != null)
                EmitValueExpr(_c.Else, type);

            Qm.@case();
        }

        public override void EmitRefColumn()
        {
            foreach (var w in _c.Whens)
            {
                Rw.Visit(w.When);

                EmitRefExpr(w.Then);

                Qm.when();
            }

            if (_c.Else != null)
                EmitRefExpr(_c.Else);

            Qm.@case();
        }
    }
}