using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using MoreLinq.Extensions;
using ZenPlatform.Core.Language.QueryLanguage.Model;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Model;
using Q = ZenPlatform.QueryBuilder.Builders.Query;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    public class Logic2QueryTreeConverter
    {
        private string _context;

        public Logic2QueryTreeConverter()
        {
        }

        public QuerySyntaxNode Convert(QQuery query)
        {
            var q = Q.New();
            GenerateQuery(q.Select(), query);
            return q.Expression;
        }

        private void GenerateQuery(SelectBuilder b, QQuery q)
        {
            _context = "from";
            GenerateFrom(b, q.From);
            _context = "select";
            GenerateSelect(b, q.Select);
        }

        private void GenerateSource(SelectBuilder sb, QObjectTable ot)
        {
            
            /*
             FROM
                    Invoice i
                    LEFT JOIN Store s ON i.Store = s.Id AND i.TypeId = 2
                    LEFT JOIN Department d ON i.Store = d.Id AND i.TypeId = 3
                    LEFT JOIN Custom c ON i.Store = c.Id AND i.TypeId = 4
                WHERE
                    i.Id = @Id
                SELECT
                    COALESE(s.Name, d.Name, c.Name) as Name
                    
                    
                    =>
                    
                    
                SELECT
                    
                FROM
                    _Obje AS Expr1 --Invoice i
                    LEFT JOIN  (SELECT A,B,C  FROM _Obje123) AS Expr2 ON Expr1.A = Expr2                       --
                    
                    
                From(x=> x.TableName("A"))
                
                From(x=> x.FromRaw("Select * From Test"))
             */
            
            ot.ObjectType.Parent.ComponentImpl.QueryInjector.GetDataSourceFragment(sb, ot.ObjectType, null);
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
            if (field is QSourceFieldExpression sf)
            {
                GenerateSourceField(b, sf);
            }
            else if (field is QAliasedSelectExpression ase)
            {
                if (ase.Child is QSourceFieldExpression f)
                {
                    if (_context == "select")
                    {
                        var schema = f.Property.GetPropertySchemas();

                        foreach (var def in schema)
                        {
                            b.Select((SelectFieldsBuilder x) =>
                                x.Field($"{def.FullName}").As(def.Prefix + ase.Alias + def.Postfix));
                        }
                    }
                }
            }
        }

        private void GenerateSourceField(SelectBuilder sb, QSourceFieldExpression sf)
        {
            if (_context == "select")
            {
                var schema = sf.Property.GetPropertySchemas();

                foreach (var def in schema)
                {
                    sb.Select((SelectFieldsBuilder x) => x.Field($"{def.FullName}"));
                }
            }
        }
    }


    public class DataRequestGenerator
    {
        private SelectBuilder _q;

        private int _tableIndex;

        public DataRequestGenerator()
        {
            _q = Query.New().Select();
        }

        public void Gen(QDataRequest dr)
        {
            foreach (var field in dr.Source)
            {
                if (field is QSourceFieldExpression sfe)
                {
                    GenerateSourceFieldExp(sfe);
                }
                else if (field is QLookupField lf)
                {
                    // Промежуточное состояние. Уточняющий запрос
                    // мы должны пойти рекурсивно и джоинить таблицу каждый лукап
                }
            }
        }

        public void GenerateSourceFieldExp(QSourceFieldExpression sfe)
        {
            var ot = sfe.Object.ObjectType;
            // мы находимся на самом нижнем уровне
            //(SELECT A FROM TEST)
            _q.From(ot.Parent.ComponentImpl.QueryInjector.GetDataSourceFragment(_q, ot, null));

        }

        public void GenerateLookup(QLookupField lookup)
        {
            if (lookup.BaseExpression is QSourceFieldExpression sfe)
                GenerateSourceFieldExp(sfe);
            else if (lookup.BaseExpression is QLookupField lu)
                GenerateLookup(lu);

            /*
                Генерируем верхний уровень
                В качестве источника данных должно быть то, что снизу
            
                Document.Invoice.Store.Name    
                
                .Name - lookup (Finish lookup)
                .Store - Property (TYPES : Store, Department, Custom)
                .Inovice - RealObject
                
                FROM
                    Invoice i
                    LEFT JOIN Store s ON i.Store = s.Id AND i.TypeId = 2
                    LEFT JOIN Department d ON i.Store = d.Id AND i.TypeId = 3
                    LEFT JOIN Custom c ON i.Store = c.Id AND i.TypeId = 4
                WHERE
                    i.Id = @Id
                SELECT
                    COALESE(s.Name, d.Name, c.Name) as Name
             */
        }
    }
}