using System;
using System.Linq;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.Migrations;
using Aquila.QueryBuilder;
using Aquila.Runtime;

namespace Aquila.Core.Querying.Optimizers
{
    public class MultiTypeQFieldExpression : MultiTypedExpr
    {
        private readonly QField _field;

        private (QField, string) HandleIntermediate(QField field)
        {
            QField result;

            if (field is QIntermediateSourceField isf1)
            {
                if (isf1.Field is QIntermediateSourceField isf)
                    result = isf.Field;
                else
                    result = isf1.Field;

                return (result, isf1.DataSource.GetDbName());
            }

            return (field, null);
        }

        public MultiTypeQFieldExpression(QField field, RealWalkerBase rw, QueryMachine qm) : base(rw, qm)
        {
            if (field.GetExpressionType().Count() == 1)
                throw new Exception($"Use {nameof(MultiTypedExpr)} for this field");

            _field = field;
        }

        private void EmitColumn(Func<ColumnSchemaDefinition, bool> criteria)
        {
            var res = HandleIntermediate(_field);

            var schemas =
                DRContextHelper.GetPropertySchemas(res.Item1.GetDbName(),
                    res.Item1.GetExpressionType().ToList());
            var schema = schemas.FirstOrDefault(criteria);


            //if (schema is null) throw new Exception($"Can't load column for field: {_field}");
            if (schema is null)
                Qm.ld_null();
            else
            {
                Qm.ld_str(res.Item2);
                Qm.ld_str($"{schema.Prefix}{res.Item1.GetDbName()}{schema.Postfix}");
                Qm.ld_column();
            }
        }

        public override void EmitTypeColumn()
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Type);
        }

        public override void EmitValueColumn(SMType ipType)
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Value && x.Type.IsAssignableFrom(ipType));
        }

        public override void EmitRefColumn()
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Ref);
        }
    }


    public class MultiTypeQParameterExpression : MultiTypedExpr
    {
        private readonly QParameter _parameter;

        public (QParameter, string) HandleIntermediate(QParameter field)
        {
            QParameter result;

            return (field, field.Name);
        }

        public MultiTypeQParameterExpression(QParameter parameter, RealWalkerBase rw, QueryMachine qm) : base(rw,
            qm)
        {
            if (parameter.GetExpressionType().Count() == 1)
                throw new Exception($"Use {nameof(MultiTypedExpr)} for this field");

            _parameter = parameter;
        }

        private void EmitColumn(Func<ColumnSchemaDefinition, bool> criteria)
        {
            var res = HandleIntermediate(_parameter);

            var schemas =
                DRContextHelper.GetPropertySchemas(res.Item1.GetDbName(),
                    res.Item1.GetExpressionType().ToList());
            var schema = schemas.FirstOrDefault(criteria);


            //if (schema is null) throw new Exception($"Can't load column for field: {_field}");
            if (schema is null)
                Qm.ld_null();
            else
            {
                // Qm.ld_str(res.Item2);
                Qm.ld_param($"{schema.Prefix}{res.Item1.GetDbName()}{schema.Postfix}");
                // Qm.ld_column();
            }
        }

        public override void EmitTypeColumn()
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Type);
        }

        public override void EmitValueColumn(SMType ipType)
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Value && x.Type.IsAssignableFrom(ipType));
        }

        public override void EmitRefColumn()
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Ref);
        }
    }


    public class MultiTypeQTypedParameterExpression : MultiTypedExpr
    {
        private readonly QTypedParameter _parameter;

        public (QTypedParameter, string) HandleIntermediate(QTypedParameter field)
        {
            QTypedParameter result;

            return (field, field.Name);
        }

        public MultiTypeQTypedParameterExpression(QTypedParameter parameter, RealWalkerBase rw, QueryMachine qm) :
            base(rw, qm)
        {
            if (parameter.GetExpressionType().Count() == 1)
                throw new Exception($"Use {nameof(MultiTypedExpr)} for this field");

            _parameter = parameter;
        }

        private void EmitColumn(Func<ColumnSchemaDefinition, bool> criteria)
        {
            var res = HandleIntermediate(_parameter);

            var schemas =
                DRContextHelper.GetPropertySchemas(res.Item1.GetDbName(),
                    res.Item1.GetExpressionType().ToList());
            var schema = schemas.FirstOrDefault(criteria);


            //if (schema is null) throw new Exception($"Can't load column for field: {_field}");
            if (schema is null)
                Qm.ld_null();
            else
            {
                Qm.ld_param($"{schema.Prefix}{res.Item1.GetDbName()}{schema.Postfix}");
            }
        }

        public override void EmitTypeColumn()
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Type);
        }

        public override void EmitValueColumn(SMType ipType)
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Value && x.Type.IsAssignableFrom(ipType));
        }

        public override void EmitRefColumn()
        {
            EmitColumn(x => x.SchemaType == ColumnSchemaType.Ref);
        }
    }
}