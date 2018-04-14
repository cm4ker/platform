using System;

namespace ZenPlatform.Configuration.Data
{
    public abstract class PTypeBase
    {
        protected PTypeBase()
        {

        }

        /// <summary>
        /// Идентификатор типа
        /// </summary>
        public abstract Guid Id { get; }
        
        /// <summary>
        /// Наименование типа
        /// </summary>
        public virtual string Name { get; }


        /// <summary>
        /// Описание типа
        /// </summary>
        public virtual string Description { get; set; }
    }
}