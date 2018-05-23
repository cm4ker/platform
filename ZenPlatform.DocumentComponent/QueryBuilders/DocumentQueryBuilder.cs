using System.Linq;
using ZenPlatform.DataComponent.QueryBuilders;
using ZenPlatform.DocumentComponent.Configuration;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Queries;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.DocumentComponent.QueryBuilders
{

    /// <summary>
    /// Компонент отвечат за то, чтобы сгенерировать инструкции для CRUD операций
    /// </summary>
    public class DocumentQueryBuilder : QueryBuilderComponent<PDocumentObjectType>
    {

        public DocumentQueryBuilder()
        {

        }

        public override DBSelectQuery SelectSingleObject(PDocumentObjectType pDocumentObjectType)
        {
            var result = new DBSelectQuery();
            DBTable table = new DBTable(pDocumentObjectType.RelTableName);

            result.From(table);

            foreach (var property in pDocumentObjectType.Properties)
            {
                result.Select(property.DatabaseColumnName);
            }

            var id = pDocumentObjectType.Properties.Single(x => x.Unique);

            result.Where(table.DeclareField(id.DatabaseColumnName), CompareType.Equals, DBClause.CreateParameter("Id", DBType.UniqueIdentifier));

            return result;
        }
    }
}
