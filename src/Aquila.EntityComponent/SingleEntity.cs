using System.ComponentModel;
using System.Runtime.CompilerServices;
using Aquila.Core;
using Aquila.Core.Sessions;
using Aquila.DataComponent.Entity;

namespace Aquila.EntityComponent
{
    public abstract class SingleEntity : EntityBase
    {
        protected SingleEntity(UserSession session) : base(session)
        {
        }

        /// <summary>
        /// Версия объекта, присваивается менеджером при загрузке объекта
        /// При этом, при сохранении, если версия отличается, в таком случае объект становится не пригодным для записи
        /// </summary>
        public object Version { get; set; }
    }
}