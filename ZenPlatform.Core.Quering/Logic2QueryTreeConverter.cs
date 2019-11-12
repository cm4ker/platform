using ZenPlatform.Core.Quering.Model;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.Core.Quering
{
    public class Logic2QueryTreeConverter
    {
        private QueryMachine _qm;


        public Logic2QueryTreeConverter()
        {
            _qm = new QueryMachine();
        }

        public object Convert(QQuery query)
        {
            GenerateQuery(query);
            return _qm.pop();
        }

        private void GenerateQuery(QQuery q)
        {
            _qm.ct_query();

            _qm.m_from();
            GenerateFrom(q.From);

            _qm.m_select();
            GenerateSelect(q.Select);
        }

        private void GenerateSource(QObjectTable ot)
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

            ot.ObjectType.Parent.ComponentImpl.QueryInjector.GetDataSourceFragment(_qm, ot.ObjectType, null);
        }

        private void GenerateFrom(QFrom from)
        {
            GenerateDataSource(from.Source);

            //TODO: делаем join
        }

        private void GenerateDataSource(IQDataSource ds)
        {
            if (ds is QNestedQuery nq)
            {
                GenerateQuery(nq.Nested);
            }
            else if (ds is QObjectTable ot)
            {
                GenerateSource(ot);
            }
            else if (ds is QAliasedDataSource ads)
            {
                GenerateDataSource(ads.Parent);
                _qm.@as();
            }
        }

        private void GenerateSelect(QSelect s)
        {
            foreach (var field in s.Fields)
            {
                GenerateField(field);
            }
        }

        private void GenerateExpression(QExpression q)
        {
        }

        private void GenerateItem(QItem item)
        {
            switch (item)
            {
                case QField f:
                    GenerateField(f);
                    break;
            }
        }

        private void GenerateField(QField field)
        {
            if (field is QSourceFieldExpression sf)
            {
                GenerateSourceField(sf);
            }
            else if (field is QAliasedSelectExpression ase)
            {
                if (ase.Child is QSourceFieldExpression f)
                {
                    var schema = f.Property.GetPropertySchemas();

                    foreach (var def in schema)
                    {
                        _qm.ld_column(); //def.FullName
                        _qm.@as(); //def.Prefix + ase.Alias + def.Postfix
                    }
                }
            }
        }

        private void GenerateSourceField(QSourceFieldExpression sf)
        {
            var schema = sf.Property.GetPropertySchemas();

            foreach (var def in schema)
            {
                _qm.ld_column(); //def.FullName
            }
        }
    }


    public class DataRequestGenerator
    {
        private int _tableIndex;
        private QueryMachine _qm;


        public DataRequestGenerator()
        {
            _qm = new QueryMachine();
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
            ot.Parent.ComponentImpl.QueryInjector.GetDataSourceFragment(_qm, ot, null);
            // мы находимся на самом нижнем уровне
            //(SELECT A FROM TEST)
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