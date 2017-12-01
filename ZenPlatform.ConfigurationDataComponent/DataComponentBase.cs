using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.Core.Entity;

namespace ZenPlatform.DataComponent
{

    /// <summary>
    /// Базовый класс компонента, от которого необходимо наследоваться, чтобы объявить новый компонент
    /// </summary>
    public abstract class DataComponentBase
    {


        /*TODO: Продумать интерфейс базового компонента
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
        /// 
        /// </summary>
        /// <param name="component">Настройки компонента, сюда компонент может публиковать какие-то структуры для общения с другими компонентами</param>
        protected DataComponentBase(PComponent component)
        {
            Component = component;
        }

        public PComponent Component { get; }

        public virtual string Name => "Unknown";
        public virtual string Version => this.GetType().Assembly.GetName().Version.ToString();

        public abstract Type ObjectConfigurationType { get; }
        public abstract EntityManagerBase Manager { get; }
        public abstract EntityGeneratorBase Generator { get; }

        public virtual DataComponentObject ConfigurationUnLoadHandler(PObjectType pobject)
        {
            throw new NotImplementedException("You can't load configuration for this component. Please detach component from configuration by changing ComponentPath property.");
        }

        public virtual void ConfigurationComponentObjectLoadHandler(PComponent component, DataComponentObject componentObject)
        {
            throw new NotImplementedException("You can't load configuration for this component. Please detach component from configuration by changing ComponentPath property.");
        }

        public virtual void ConfigurationObjectPropertyLoadHandler(PObjectType pObject, DataComponentObjectProperty objectProperty, List<PTypeBase> registeredTypes)
        {
            throw new NotImplementedException("You can't load configuration for this component. Please detach component from configuration by changing ComponentPath property.");
        }

        //TODO: Реализовать UI описание конфигурации
        //TODO: Реализовать транслятор запросов для сущности
    }

    /// <summary>
    /// Определяет, как компонент взаимодействует с другими компонентами в плане отношения
    /// 
    /// Пример: 
    ///     1) Справочник и документ могут иметь связть только 1 - 0..1
    ///     2) Табличная часть и справочник могут иметь связь 1 - 0..*
    ///     3) Регистр и документ имеют отношение * - 1
    /// </summary>
    public enum RelationType
    {
        One,
        OneZero,
        Many
    }
}
