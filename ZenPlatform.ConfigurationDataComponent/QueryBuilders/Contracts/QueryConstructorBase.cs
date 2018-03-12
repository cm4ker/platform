using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Caching;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.DataComponent.QueryBuilders.Contracts
{
    public abstract class QueryConstructorBase
    {
        protected QueryConstructorBase(PDataObjectType objectType)
        {
            ObjectType = objectType;
            Cache = new MemoryCache("QueryCache");
        }

        protected MemoryCache Cache { get; }

        protected PDataObjectType ObjectType { get; }

        public virtual IQueryable Build()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Конструктор запроса. Необходимо ёвно реализовать его в компоненте.
    /// </summary>
    public abstract class SelectQueryConstructor : QueryConstructorBase
    {
        public override IQueryable Build()
        {
            //TODO: нужно обернуть PObjectType во что-то более конкретное. 
            //Скажем, PDataObjectType, который в свою очередь реализует доступ к таблице. Этот класс знает
            //как идентифицировать себя
            /*
             * Долго втыкал в фразу "PDataObjectType реализует доступ к таблице" -
             * Это абсолютная неправда, видимо я опечатолся. Этот объект отвечает только за ОПИСАНИИЕ.
             * Для примера, у компонента "Документы" есть предопределённые свойства, которых нет у других сущностей:
             * Статус, Дата, Номер и т.д.
             * Вот эти все совйства и определяет унаследованный от PObjectType класс.
             *
             */

            throw new NotImplementedException();
        }

        protected SelectQueryConstructor(PDataObjectType objectType) : base(objectType)
        {
        }
    }
}
