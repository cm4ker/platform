using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZenPlatform.Core;
using ZenPlatform.DataComponent.Entity;

namespace ZenPlatform.EntityComponent
{
    public abstract class SingleEntity : EntityBase
    {
        protected SingleEntity(Session session) : base(session)
        {
        }

        /// <summary>
        /// Версия объекта, присваивается менеджером при загрузке объекта
        /// При этом, при сохранении, если версия отличается, в таком случае объект становится не пригодным для записи
        /// </summary>
        public object Version { get; set; }
    }
}