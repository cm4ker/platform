using System;
using System.Collections.Generic;
using System.Net.Mail;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Core;
using ZenPlatform.Core.Sessions;
using ZenPlatform.DataComponent.Entity;
using ZenPlatform.DataComponent.Helpers;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.QueryBuilder.DML.Select;

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

        public SingleEntity Create(UserSession session, Type entityType)
        {
            var def = session.GetMetadata(entityType);

            if (!CheckType(def.EntityType)) throw new Exception($"Wrong manager for entity type: {def.EntityType}");

            var dto = Activator.CreateInstance(def.DtoType);
            var document = CreateEntityFromDto(session, def.EntityType, dto);
            // Activator.CreateInstance(def.EntityType, session, dto) as DocumentEntity;

            return document;
        }

        /// <summary>
        /// Сохранить объект
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        public void Save(UserSession session, SingleEntity entity)
        {
            var def = session.GetMetadata(entity.GetType());
            //TODO: неоходимо вытащить dto из объекта, затем сохранить его
        }

        private SingleEntity CreateEntityFromDto(UserSession session, Type entityType, object dto)
        {
            var document = Activator.CreateInstance(entityType, session, dto) as SingleEntity;
            return document;
        }
//
//        public SingleEntity Load(UserSession session, int typeId, object key)
//        {
//        }

        public SingleEntity Load(UserSession session, Type entityType, Guid key)
        {
            var def = session.GetMetadata(entityType);

            var dto = LoadDtoObject(session, def.DtoType, def.EntityConfig.Id, key);

            return CreateEntityFromDto(session, entityType, dto);
        }

        /// <summary>
        /// Загрузить DTO сущность объекта
        /// </summary>
        /// <param name="session">Сессия в которой загружаем</param>
        /// <param name="dtoType">Тип Entity от DTO, которого хотим загрузить</param>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public object LoadDtoObject(UserSession session, Type dtoType, uint typeId, Guid key)
        {
            //Проверим кэш

            var dto = session.CacheService.Get(dtoType, 1, (int) typeId, key);

            if (dto != null) return dto;

            // Получить контекс данных 
            var context = session.DataContext;

            var def = session.GetMetadata(dtoType);

            var conf = def.EntityConfig as XCSingleEntity;

            var q = new SelectQueryNode();

            q.From(conf.RelTableName);

            foreach (var property in conf.Properties)
            {
                var cols = ColumnsHelper.GetColumnsFromProperty(property);
                foreach (var col in cols)
                {
                    q.Select(col.DatabaseColumnName);
                }
            }

            q.Where(f => f.Field("Id"), "=", f => f.Parameter("Id"));

            //TODO: Сделать RLS в предложении WHERE
            //TODO: На основании пользовательского контекста необходимо получить ограничение

            //есть несколько путей решения этой задачи

            var cmd = context.CreateCommand(q);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var mappedDto = (ICanMapSelfFromDataReader) Activator.CreateInstance(def.DtoType);

                //Вместо рефлексии нужно использовать статический маппер
                mappedDto.Map(reader);
                return dto;
            }
            else
            {
                return null;
            }
        }

        public void Delete(UserSession session, SingleEntity entity)
        {
        }

        public object GetKey(UserSession session, SingleEntity entity)
        {
            throw new NotImplementedException();
        }
    }


    public class DocumentEntityManager<T> where T : SingleEntity
    {
        private SingleEntityManager _singleEntityManager;
        private UserSession _session;

        public DocumentEntityManager(UserSession session)
        {
            _singleEntityManager = session.GetManager(typeof(T)) as SingleEntityManager;
            _session = session;
        }

        public T Load(Guid key)
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