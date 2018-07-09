using System;
using System.Linq;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.QueryBuilders
{
    /// <summary>
    /// Компонент отвечат за то, чтобы сгенерировать инструкции для CRUD операций
    /// </summary>
    public class DocumentQueryBuilder
    {
        public DocumentQueryBuilder()
        {
        }

        //public DBSelectQuery SelectSingleObject(XCObjectTypeBase obj, object key)
        //{
        //    if (!(obj is Configuration.XCSingleEntity doc)) throw new Exception();

        //    var result = new DBSelectQuery();
        //    DBTable table = new DBTable(doc.RelTableName);

        //    result.From(table);

        //    foreach (var property in doc.Properties)
        //    {
        //        result.Select(property.DatabaseColumnName);
        //    }

        //    var id = doc.Properties.Single(x => x.Unique);

        //    result.Where(table.DeclareField(id.DatabaseColumnName), CompareType.Equals, DBClause.CreateParameter("Id"));

        //    return result;
        //}
    }
}