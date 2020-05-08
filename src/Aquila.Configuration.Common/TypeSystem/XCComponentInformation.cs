using System;
using Aquila.Configuration.Contracts;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class XCComponentInformation : IXCComponentInformation
    {
        /// <summary>
        /// Имя компонента, обязано быть одним словом и латинскими буквами
        /// </summary>
        public virtual string ComponentName => "Unknown component";

        /// <summary>
        /// Описание компонента
        /// </summary>
        public virtual string ComponentDescription =>
            "Это неопределённый компонент, для корректного отображения описания и названия компонента, необходимо созать класс-описание унаследованный от класса XCComponentInformation";


        /// <summary>
        /// Идентификатор компонента, обязателен к переопределению 
        /// </summary>
        public virtual Guid ComponentId => Guid.Empty;

        /// <summary>
        /// Указывает на то, присоединяемый это компонент, или же живёт своей жизню
        /// False - компонент не присоединяется и будет отображаться в дереве конфигураций в корне раздела Data
        /// True - компонент присоединяется к другим компонентам и будет отображаться в дереве конфигураций в экземпляре объекта каждого компонента
        /// </summary>
        public virtual bool AttachedComponent => false;

        /// <summary>
        /// Версия компонента
        /// </summary>
        public virtual string Version => this.GetType().Assembly.GetName().Version.ToString();

        /// <summary>
        /// Пространство имён в Session для компонента.
        /// Если вы будите обращаться к компоненту из кода или же ваш компонент учавствует в кодогенерации,
        /// то это совойство обязательно к реализации
        /// </summary>
        public virtual string ComponentSpaceName => throw new NotImplementedException();
    }
}