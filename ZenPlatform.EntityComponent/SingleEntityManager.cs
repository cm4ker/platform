using System;
using ZenPlatform.Core;
using ZenPlatform.DataComponent.Entity;
using ZenPlatform.EntityComponent.Configuration;

namespace ZenPlatform.EntityComponent
{
    /*
     TODO: Необходимо реализовать генерацию кода для менеджера. Добавить хелперов

        Статичный класс ManagerHelper помогает из сессии получать менеджеры
        Session.Documents.Invoice.Create(Session);
    */

    public class SingleEntityManager : EntityManager
    {
        public SingleEntityManager()
        {
        }

        //TODO: Сделать async API task 86

        private bool CheckType(Type type)
        {
            return type.BaseType == typeof(SingleEntity);
        }

        public SingleEntity Create(Session session, Type entityType)
        {
            var def = session.Environment.GetMetadata(entityType);

            if (!CheckType(def.EntityType)) throw new Exception($"Wrong manager for entity type: {def.EntityType}");

            var dto = Activator.CreateInstance(def.DtoType);
            var document = CreateEntityFromDto(session, def.EntityType, dto);
            // Activator.CreateInstance(def.EntityType, session, dto) as DocumentEntity;

            return document;
        }


        public void Save(Session session, SingleEntity entity)
        {
            var def = session.Environment.GetMetadata(entity.GetType());
        }

        private SingleEntity CreateEntityFromDto(Session session, Type entityType, object dto)
        {
            var document = Activator.CreateInstance(entityType, session, dto) as SingleEntity;
            return document;
        }

        public SingleEntity Load(Session session, Type entityType, object key)
        {
            var def = session.Environment.GetMetadata(entityType);

            var dto = LoadDtoObject(session, def.DtoType, key);

            return CreateEntityFromDto(session, entityType, dto);
        }

        /// <summary>
        /// Загрузить DTO сущность объекта
        /// </summary>
        /// <param name="session">Сессия в которой загружаем</param>
        /// <param name="type">Тип Entity от DTO, которого хотим загрузить</param>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public object LoadDtoObject(Session session, Type type, object key)
        {
            // Получить контекс данных 
            var context = session.DataContextManger.GetContext();

            var def = session.Environment.GetMetadata(type);

            var conf = def.EntityConfig as Configuration.XCSingleEntity;

            var dto = def.EntityConfig.Parent.ComponentImpl.Caches[def.EntityConfig.Name].Get(key.ToString());

            if (dto != null)
                return dto;

            var q = new QueryBuilder.DML.Select.SelectQueryNode();

            q.From(conf.RelTableName);
            q.Select("*");

            var cmd = context.CreateCommand();


            //TODO: загрузить объект из базы данных

            return null;
        }

        public void Delete(Session session, SingleEntity entity)
        {
        }

        public object GetKey(Session session, SingleEntity entity)
        {
            throw new NotImplementedException();
        }
    }


    public class DocumentEntityManager<T> where T : SingleEntity
    {
        private SingleEntityManager _singleEntityManager;
        private Session _session;

        public DocumentEntityManager(Session session)
        {
            _singleEntityManager = session.Environment.GetManager(typeof(T)) as SingleEntityManager;
            _session = session;
        }

        public T Load(object key)
        {
            return _singleEntityManager.Load(_session, typeof(T), key) as T;
        }

        public void Delete(T entity)
        {
            _singleEntityManager.Delete(_session, entity);
        }

        public void Save(T entity)
        {
            _singleEntityManager.Save(_session, entity);
        }

        public T Create()
        {
            return _singleEntityManager.Create(_session, typeof(T)) as T;
        }

        public Guid GetKey(T entity)
        {
            return new Guid();
        }
    }
}