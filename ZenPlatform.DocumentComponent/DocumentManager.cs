using System;
using System.Collections.Generic;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.Core;
using ZenPlatform.Core.Entity;
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

        public DocumentManager(Entity2SqlBase sqlProvider, Session session) : base(sqlProvider, session)
        {
            _sqlProvider = sqlProvider;
        }

        public override DocumentEntity Create()
        {
            throw new NotImplementedException();
        }

        public override void Save(DocumentEntity entity)
        {
            var context = Session.DataContextManger.GetContext();
            try
            {
                context.BeginTransaction();
                //TODO: Необходимо написать механизм определения инструкции. Insert\Update. Для этого необходимо сделать следующие задачи: Механизм кэширования(чтобы смотреть, елси объект в кэше не нужно дёргать его из БД)
                context.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.RollbackTransaction();
            }
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