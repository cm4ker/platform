using System;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.Core.Entity;
using ZenPlatform.DataComponent;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.DocumentComponent
{

    /// <summary>
    /// Компонент отвечат за то, чтобы сгенерировать инструкции для CRUD операций
    /// </summary>
    public class DocumentQueryBuilder : QueryBuilderComponent
    {
        public DocumentQueryBuilder(PObjectType objectType) : base(objectType)
        {

        }

        public override DBSelectQuery GetSelect()
        {
            return base.GetSelect();
        }
    }
}