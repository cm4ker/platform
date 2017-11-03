using System;

namespace ZenPlatform.Core.Entity
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
}