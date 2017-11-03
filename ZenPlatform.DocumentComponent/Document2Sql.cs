using System;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.Core.Entity;
using ZenPlatform.QueryCompiler.Queries;

namespace ZenPlatform.DocumentComponent
{

    /// <summary>
    /// Компонент отвечат за то, чтобы сгенерировать инструкции для CRUD операций
    /// </summary>
    public class Document2Sql : Entity2SqlBase
    {
        public override DBSelectQuery GetSelect()
        {
            throw new NotImplementedException();
        }

        public override DBUpdateQuery GetUpdate()
        {
            throw new NotImplementedException();
        }

        public override DBDeleteQuery GetDelete()
        {
            throw new NotImplementedException();
        }

        public override DBInsertQuery GetInsert()
        {
            throw new NotImplementedException();
        }
    }
}