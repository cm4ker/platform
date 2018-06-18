using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration
{
    public class ComponentInformation
    {
        /// <summary>
        /// Словарь названий компонента на разных языках
        /// </summary>
        public virtual string ComponentName => "Unknown component";

        /// <summary>
        /// Описание компонента
        /// </summary>
        public virtual string ComponentDescription =>
            "Это неопределённый компонент, для корректного отображения описания и названия компонента, необходимо переопределить созать класс-описание унаследованный от класса ComponentInformation";


        /// <summary>
        /// Идентификатор компонента, обязателен к переопределению 
        /// </summary>
        public virtual Guid ComponentId => Guid.Empty;

        /// <summary>
        /// Пространство в Session для компонента.
        /// Если вы будите обращаться к компоненту из кода или же ваш компонент учавствует в кодогенерации,
        /// то это совойство обязательно к реализации
        /// </summary>
        public virtual string ComponentSpaceName => throw new NotImplementedException();
    }
}