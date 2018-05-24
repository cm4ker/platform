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
        public virtual string ComponentDescription => "Это неопределённый компонент, для корректного отображения описания и названия компонента, необходимо переопределить созать класс-описание унаследованный от класса ComponentInformation";
    }
}
