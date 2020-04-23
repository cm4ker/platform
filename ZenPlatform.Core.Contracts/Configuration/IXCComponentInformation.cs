using System;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCComponentInformation
    {
        /// <summary>
        /// Имя компонента, обязано быть одним словом и латинскими буквами
        /// </summary>
        string ComponentName { get; }

        /// <summary>
        /// Описание компонента
        /// </summary>
        string ComponentDescription { get; }

        /// <summary>
        /// Идентификатор компонента, обязателен к переопределению 
        /// </summary>
        Guid ComponentId { get; }

        /// <summary>
        /// Указывает на то, присоединяемый это компонент, или же живёт своей жизню
        /// False - компонент не присоединяется и будет отображаться в дереве конфигураций в корне раздела Data
        /// True - компонент присоединяется к другим компонентам и будет отображаться в дереве конфигураций в экземпляре объекта каждого компонента
        /// </summary>
        bool AttachedComponent { get; }

        /// <summary>
        /// Версия компонента
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Пространство имён в Session для компонента.
        /// Если вы будите обращаться к компоненту из кода или же ваш компонент учавствует в кодогенерации,
        /// то это совойство обязательно к реализации
        /// </summary>
        string ComponentSpaceName { get; }
    }
}