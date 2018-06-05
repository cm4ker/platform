using System;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;
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
    public class DocumentQueryBuilder
    {
        public DocumentQueryBuilder()
        {
        }

        public DBSelectQuery SelectSingleObject(XCObjectTypeBase obj, object key)
        {
            if (!(obj is Document doc)) throw new Exception();

            var result = new DBSelectQuery();
            DBTable table = new DBTable(doc.RelTableName);

            result.From(table);

            foreach (var property in doc.Properties)
            {
                result.Select(property.DatabaseColumnName);
            }

            var id = doc.Properties.Single(x => x.Unique);

            result.Where(table.DeclareField(id.DatabaseColumnName), CompareType.Equals, DBClause.CreateParameter("Id"));

            return result;
        }
    }
}