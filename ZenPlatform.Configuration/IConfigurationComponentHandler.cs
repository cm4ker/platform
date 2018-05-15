using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;

namespace ZenPlatform.Configuration
{
    /// <summary>
    /// Интерфейс менеджера конфигурации компонента используется для загрузки/выгрузки конфигурации
    /// </summary>
    public interface IConfigurationComponentHandler
    {
        /// <summary>
        /// Упаковать объект в ветку конфигурации
        /// </summary>
        /// <param name="pobject"></param>
        /// <returns></returns>
        DataComponentObject PackObject(PObjectType pobject);

        /// <summary>
        /// Распоковать объект из ветки конфигурации
        /// </summary>
        /// <param name="component"></param>
        /// <param name="componentObject"></param>
        /// <returns></returns>
        PObjectType UnpackObject(PComponent component, DataComponentObject componentObject);

        /// <summary>
        /// Распоковать свойство объекта из ветки конфигурации
        /// </summary>
        /// <param name="pObject">Объект</param>
        /// <param name="objectProperty">Запакованное свойство</param>
        /// <param name="registeredTypes">Зарегистрированные типы</param>
        void UnpackProperty(PObjectType pObject, DataComponentObjectProperty objectProperty,
            List<PTypeBase> registeredTypes);
    }
}