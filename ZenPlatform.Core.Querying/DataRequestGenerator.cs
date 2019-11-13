using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying
{
    public class DataRequestGenerator
    {
        private int _tableIndex;
        private QueryMachine _qm;
        private int _joinIndex = 0;

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
                }
            }
        }

        public void GenerateSourceFieldExp(QSourceFieldExpression sfe)
        {
            _qm.m_from();

            var ot = sfe.Object.ObjectType;
            ot.Parent.ComponentImpl.QueryInjector.InjectDataSource(_qm, ot, null);


            foreach (var propType in sfe.GetRexpressionType())
            {
            }

            _qm.m_select();
            // мы находимся на самом нижнем уровне
            //(SELECT A FROM TEST)
        }

        public void GenerateLookup(QLookupField lookup)
        {
            _joinIndex++;

            if (lookup.BaseExpression is QSourceFieldExpression sfe)
                GenerateSourceFieldExp(sfe);
            else if (lookup.BaseExpression is QLookupField lu)
            {
                GenerateLookup(lu);

                foreach (var type in lu.GetRexpressionType())
                {
                    if (type is XCObjectTypeBase ot)
                    {
                        ot.Parent.ComponentImpl.QueryInjector.InjectDataSource(_qm, ot, null);

                        //condition
                        _qm.ld_column("A1");
                        _qm.ld_column("A2");

                        _qm.eq();

                        _qm.ld_column("A1");
                        _qm.ld_const(ot.Id);

                        _qm.left_join();
                    }
                }
            }

            /*
                Генерируем верхний уровень
                В качестве источника данных должно быть то, что снизу
            
                Document.Invoice.Store.Employer.Name    
                
                .Name - lookup (Finish lookup)
                .Employer - Lookup (Intermediate Lookup)
                .Store - Property (TYPES : Store, Department, Custom)
                .Invoice - RealObject
                
                FROM
                    Invoice i
                    LEFT JOIN Store s ON i.Store = s.Id AND i.TypeId = 2
                    LEFT JOIN Department d ON i.Store = d.Id AND i.TypeId = 3
                    LEFT JOIN Custom c ON i.Store = c.Id AND i.TypeId = 4
                    LEFT JOIN Employer e ON e.Id = COALESCE(s.EmpId, d.EmpId, c.EmpId)                    
                WHERE
                    i.Id = @Id
                SELECT
                    e.Name
                    --COALESCE(s.Name, d.Name, c.Name) as Name
             */
        }
    }
}