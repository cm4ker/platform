using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.Core.Entity;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.DataComponent.Migrations;

namespace ZenPlatform.DataComponent
{
    /// <summary>
    /// Базовый класс компонента, от которого необходимо наследоваться, чтобы объявить новый компонент в категории "Данные"
    /// </summary>
    /// <typeparam name="TMigrationComponent">Компонент миграции</typeparam>
    /// <typeparam name="TEntityGenerator"></typeparam>
    /// <typeparam name="TManager"></typeparam>
    public abstract class DataComponentBase<TEntityGenerator,
        TManager>
        where TEntityGenerator : EntityGeneratorBase
        where TManager : EntityManagerBase
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
        protected DataComponentBase(PComponent component)
        {
            Component = component;

            /*
             * Есть ли смысл проверять IConfigurationSettings?
             * это должен быть статический класс, потому что он никак не передаётся во внутрь,
             * но мы как-то должны попытаться вызвать его поля
             */

            SupportedTypesManager = new SupportedTypeManager();
        }

        protected PComponent Component { get; }


        /// <summary>
        /// Событие, вызываемое перед инициализацией компонента
        /// </summary>
        public virtual void OnInitializing()
        {
        }

        public virtual string Name => "Unknown";
        public virtual string Version => this.GetType().Assembly.GetName().Version.ToString();

        public EntityManagerBase Manager { get; protected set; }
        public EntityGeneratorBase Generator { get; protected set; }


        /// <summary>
        /// Список поддерживаемых типов
        /// </summary>
        protected SupportedTypeManager SupportedTypesManager { get; }

        #region Configuration

        public virtual DataComponentObject ConfigurationUnloadHandler(PObjectType pobject)
        {
            throw new NotImplementedException(
                "You can't load configuration for this component. Please detach component from configuration by changing ComponentPath property.");
        }

        public virtual PObjectType ConfigurationComponentObjectLoadHandler(PComponent component,
            DataComponentObject componentObject)
        {
            throw new NotImplementedException(
                "You can't load configuration for this component. Please detach component from configuration by changing ComponentPath property.");
        }

        public virtual void ConfigurationObjectPropertyLoadHandler(PObjectType pObject,
            DataComponentObjectProperty objectProperty, List<PTypeBase> registeredTypes)
        {
            throw new NotImplementedException(
                "You can't load configuration for this component. Please detach component from configuration by changing ComponentPath property.");
        }

        #endregion

        //TODO: Реализовать UI описание конфигурации
        //TODO: Реализовать транслятор запросов для сущности
    }
}