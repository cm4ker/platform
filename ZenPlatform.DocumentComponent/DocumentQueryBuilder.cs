using System;
using ZenPlatform.Configuration.Data;
using ZenPlatform.ConfigurationDataComponent;
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

        //public override DBSelectQuery GetSelect()
        //{
        //    throw new NotImplementedException();
        //}

        //public override DBUpdateQuery GetUpdate()
        //{
        //    throw new NotImplementedException();
        //}

        //public override DBDeleteQuery GetDelete()
        //{
        //    throw new NotImplementedException();
        //}

        //public override DBInsertQuery GetInsert()
        //{
        //    throw new NotImplementedException();
        //}
    }
}