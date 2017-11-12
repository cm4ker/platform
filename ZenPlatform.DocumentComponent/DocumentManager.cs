using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ZenPlatform.Configuration.Data;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.Core;
using ZenPlatform.Core.Entity;
using ZenPlatform.Data;
using ZenPlatform.DataComponent;

namespace ZenPlatform.DocumentComponent
{
    /*
     TODO: Необходимо реализовать генерацию кода для менеджера. Добавить хелперов

        Статичный класс ManagerHelper помогает из сессии получать менеджеры
        Session.Documents.Invoice.Create(Session);
    */

    public class DocumentManager : EntityManagerBase
    {
        public DocumentManager(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {

        }

        //TODO: Сделать async API task 86

        private bool CheckType(Type type)
        {
            return type.BaseType == typeof(DocumentEntity);
        }

        public DocumentEntity Create(Session session, Type entityType)
        {
            var def = session.Environment.GetDefinition(entityType);

            if (!CheckType(def.EntityType)) throw new Exception($"Wrong manager for entity type: {def.EntityType}");

            var dto = Activator.CreateInstance(def.DtoType);
            var document = CreateEntityFromDto(session, def.EntityType, dto);
            // Activator.CreateInstance(def.EntityType, session, dto) as DocumentEntity;

            return document;
        }


        public void Save(Session session, DocumentEntity entity)
        {

        }


        public async void SaveAsync(Session session, DocumentEntity entity)
        {

        }

        private DocumentEntity CreateEntityFromDto(Session session, Type entityType, object dto)
        {
            var document = Activator.CreateInstance(entityType, session, dto) as DocumentEntity;
            return document;
        }

        public DocumentEntity Load(Session session, Type entityType, object key)
        {
            var def = session.Environment.GetDefinition(entityType);

            var dto = LoadDtoObject(session, def.DtoType, key);

            return CreateEntityFromDto(session, entityType, dto);
        }

        public object LoadDtoObject(Session session, Type type, object key)
        {
            throw new NotImplementedException();
        }

        public void Delete(Session session, DocumentEntity entity)
        {
        }

        public object GetKey(Session session, DocumentEntity entity)
        {
            //Todo: Необходиом получить ключ, в не зависимости от отго, какого типа ключ, составной или не составной
            throw new NotImplementedException();
        }
    }


    public class DocumentEntityManager<T> where T : DocumentEntity
    {
        private DocumentManager _documentManager;
        private Session _session;

        public DocumentEntityManager(Session session)
        {
            _documentManager = session.Environment.GetManager(typeof(T)) as DocumentManager;
            _session = session;
        }

        public T Load(object key)
        {
            return _documentManager.Load(_session, typeof(T), key) as T;
        }

        public void Delete(T entity)
        {
            _documentManager.Delete(_session, entity);
        }

        public void Save(T entity)
        {
            _documentManager.Save(_session, entity);
        }

        public T Create()
        {
            return _documentManager.Create(_session, typeof(T)) as T;
        }
    }
}