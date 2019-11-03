using System.Collections.Generic;
using System.Runtime.Caching;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.DataComponent.Entity;

namespace ZenPlatform.DataComponent
{
    /// <summary>
    /// Базовый класс компонента, от которого необходимо наследоваться, чтобы объявить новый компонент в категории "Данные"
    /// </summary>
    public abstract class DataComponentBase : IDataComponent
    {
        /*
        TODO: Продумать интерфейс базового компонента
        1) Наименование компанента
        2) Версия
        3) Поддерживаемые типы --- ????
        4) Реализация сущности по умолчанию
        5) Правила хранения в БД
        6) Виды UI для сущности
        7) Менеджер CRUD
        8) Транслятор запросов для сущности
        */

        /// <summary>
        /// Базовый компонент данных. Является основой для всех компонентов.
        /// </summary>
        /// <param name="component">Настройки компонента, сюда компонент может публиковать какие-то структуры для общения с другими компонентами</param>
        protected DataComponentBase(XCComponent component)
        {
            Component = component;

            /*
             * Есть ли смысл проверять IConfigurationSettings?
             * это должен быть статический класс, потому что он никак не передаётся во внутрь,
             * но мы как-то должны попытаться вызвать его поля
             */

            //SupportedTypesManager = new SupportedTypeManager();
        }

        protected XCComponent Component { get; }


        /// <summary>
        /// Событие, вызываемое перед инициализацией компонента
        /// </summary>
        public virtual void OnInitializing()
        {
        }

        public IEntityManager Manager { get; protected set; }
        public IXComponentManager ComponentManager { get; protected set; }

        public IPlatformGenerator Generator { get; protected set; }
        public IDatabaseObjectsGenerator DatabaseObjectsGenerator { get; protected set; }
        public IEntityMigrator Migrator { get; protected set; }
        public IQueryInjector QueryInjector { get; protected set; }


        /*
         * Кэширование выведено в отдельное приложение, которое связывается по протоколу общения
         */
        //public Dictionary<string, ObjectCache> Caches { get; protected set; }

        //Пометка: выпилено, компонент не хранит в себе эту инфомрацию для доступа извне,
        //но если нужно, тогда это будет скорее всего в  другом формате
        ///// <summary>
        ///// Список поддерживаемых типов
        ///// </summary>
        //protected SupportedTypeManager SupportedTypesManager { get; }

        #region Configuration

        //NOTE: Было выпилено в task-94, так как был полностью переписан механизм загрузки конфигурации.
        //Теперь всё реализовано через отдельный интерфейс, смотри IComponenConfigurationLoader

        #endregion

        //TODO: Реализовать UI описание конфигурации
        //TODO: Реализовать транслятор запросов для сущности
    }
}