using System;
using System.Runtime.Serialization;

namespace ZenPlatform.DataComponent
{
    public abstract class EntityBase
    {
        public object Key { get; set; }

        /// <summary>
        /// Версия объекта, присваивается менеджером при загрузке объекта
        /// При этом, при сохранении, если версия отличается, в таком случае объект становится не пригодным для записи
        /// </summary>
        public object Version { get; set; }
    }

    /// <summary>
    /// Игнорирование свойста/метода/поля платформой при манипулировани объекта
    /// </summary>
    public class PlatformMemberIgnoreAttribute : Attribute
    {

    }
}