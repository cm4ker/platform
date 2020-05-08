using System;
using System.Collections.Generic;
using Aquila.Configuration.Common;

namespace Aquila.EntityComponent.Configuration
{
    public class MDInterface
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public InterfaceType Type { get; set; }

        public string Markup { get; set; }

        public List<MDFormProperty> Properties { get; set; }
    }

    public class MDFormProperty
    {
        public MDFormProperty()
        {
            Guid = Guid.NewGuid();
            Types = new List<MDType>();
        }

        /// <summary>
        /// Уникальный идентификатор свойства
        /// </summary>
        public Guid Guid { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Типы, описывающие поле
        /// </summary>
        public List<MDType> Types { get; set; }
    }
}