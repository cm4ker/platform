using System.Collections.Generic;
using System.Xml.Serialization;
using MoreLinq.Extensions;
using ServiceStack;
using ZenPlatform.Core.Language.QueryLanguage.Model;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Model;
using Q = ZenPlatform.QueryBuilder.Builders.Query;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    public class LogicToReal
    {
        public LogicToReal()
        {
        }

        public QuerySyntaxNode Build(QQuery query)
        {
            var q = Q.New();
            GenerateQuery(q.Select(), query);
            return q.Expression;
        }

        private void GenerateQuery(SelectBuilder b, QQuery q)
        {
            GenerateFrom(b, q.From);
            GenerateSelect(b, q.Select);
        }

        private void GenerateSource(SelectBuilder sb, QObjectTable ot)
        {
            ot.ObjectType.Parent.ComponentImpl.QueryInjector.GetDataSourceFragment(sb, ot.ObjectType, null);
            //TODO: Передаем управление компаненту
        }

        private void GenerateFrom(SelectBuilder b, QFrom from)
        {
            GenerateDataSource(b, from.Source);

            //TODO: делаем join
        }

        private void GenerateDataSource(SelectBuilder sb, IQDataSource ds)
        {
            if (ds is QNestedQuery nq)
            {
                sb.From(x => GenerateQuery(x, nq.Nested));
            }
            else if (ds is QObjectTable ot)
            {
                GenerateSource(sb, ot);
            }
            else if (ds is QAliasedDataSource ads)
            {
                GenerateDataSource(sb, ads.Parent);
                sb.As(ads.Alias);
            }
        }

        private void GenerateSelect(SelectBuilder sb, QSelect s)
        {
            foreach (var field in s.Fields)
            {
                GenerateField(sb, field);
            }
        }

        private void GenerateExpression(SelectBuilder b, QExpression q)
        {
        }

        private void GenerateItem(SelectBuilder b, QItem item)
        {
            switch (item)
            {
                case QField f:
                    GenerateField(b, f);
                    break;
            }
        }

        private void GenerateField(SelectBuilder b, QField field)
        {
            if (field is QSourceFieldExpression sf) GenerateSourceField(b, sf);
            else if (field is QAliasedSelectExpression ase)
            {
                GenerateItem(b, ase.Child);
                b.As(ase.Alias);
            }
        }

        private void GenerateSourceField(SelectBuilder sb, QSourceFieldExpression sf)
        {
            if (sf.Parent is QSelect)
            {
                var schema = sf.Property.GetPropertySchemas();

                foreach (var def in schema)
                {
                    sb.Select((SelectFieldsBuilder x) => x.Field($"{def.Name}"));
                }
            }
        }
    }
}