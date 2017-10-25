using System;
using System.Collections.Generic;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.Data;
using ZenPlatform.DataComponent;

namespace ZenPlatform.DocumentComponent
{

    /// <summary>
    /// Менеджер - связываеющее звено, для SQL компонента, сущности объекта и базы данных 
    /// 
    /// </summary>
    public class DocumentManager : EntityManagerBase<DocumentEntity>
    {
        private readonly Entity2SqlBase _sqlProvider;

        public DocumentManager(Entity2SqlBase sqlProvider) : base(sqlProvider)
        {
            _sqlProvider = sqlProvider;
        }

        public override DocumentEntity Create()
        {
            throw new NotImplementedException();
        }

        public override void Save(DocumentEntity entity)
        {
            throw new NotImplementedException();
        }

        public override DocumentEntity Load()
        {
            throw new NotImplementedException();
        }

        public override void Delete(DocumentEntity entity)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DocumentEntity> GetList()
        {
            throw new NotImplementedException();
        }


    }
}