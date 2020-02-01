using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZenPlatform.Configuration.Common;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Если ваш компонент поддерживает свойства, их необходимо реализовывать через этот компонент
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class MDProperty
    {
        public MDProperty()
        {
            Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Уникальный идентификатор свойства
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Псевдоним в системе
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Типы, описывающие поле
        /// </summary>
        public List<MDType> Types { get; set; }
    }
}