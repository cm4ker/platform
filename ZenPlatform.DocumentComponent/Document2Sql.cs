using ZenPlatform.ConfigurationDataComponent;
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
            throw new System.NotImplementedException();
        }

        public override DBUpdateQuery GetUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override DBDeleteQuery GetDelete()
        {
            throw new System.NotImplementedException();
        }

        public override DBInsertQuery GetInsert()
        {
            throw new System.NotImplementedException();
        }
    }
}