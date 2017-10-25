using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Data;
using ZenPlatform.ConfigurationDataComponent;

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
        3) Поддерживаемые типы
        4) Реализация сущности по умолчанию
        5) Правила хранения в БД
        6) Виды UI для сущности
        7) Менеджер CRUD
        8) Транслятор запросов для сущности
             */


        public virtual string Name => "Unknown";
        public virtual string Version => this.GetType().Assembly.GetName().Version.ToString();

        public abstract IEnumerable<PPrimetiveType> GetSupportedEntityPrimitiveTypes();

        public abstract Type EntityBase { get; }
        public abstract EntityManagerBase Manager { get; }
        public abstract EntityGeneratorBase Generator { get; }

        //TODO: Реализовать UI описание конфигурации
        //TODO: Реализовать транслятор запросов для сущности
    }
}
