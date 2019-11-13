using ZenPlatform.Core.Querying.Model;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying
{
    public class DataRequestGenerator
    {
        private int _tableIndex;
        private QueryMachine _qm;
        private int _lookupDepth = 0;

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

                    GenerateLookup(lf);

                    _qm.ld_column()
                }
            }
        }

        public void GenerateSourceFieldExp(QSourceFieldExpression sfe)
        {
            _qm.m_from();

            var ot = sfe.Object.ObjectType;
            ot.Parent.ComponentImpl.QueryInjector.InjectDataSource(_qm, ot, null);

            _qm.m_select();
            // мы находимся на самом нижнем уровне
            //(SELECT A FROM TEST)
        }

        public void GenerateLookup(QLookupField lookup)
        {
            _lookupDepth++;

            if (lookup.BaseExpression is QSourceFieldExpression sfe)
                GenerateSourceFieldExp(sfe);
            else if (lookup.BaseExpression is QLookupField lu)
            {
                GenerateLookup(lu);

                _qm.ld_table();
            }

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