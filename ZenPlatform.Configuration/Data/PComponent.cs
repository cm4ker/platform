using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration.Data
{
    public class PComponent
    {
        public PComponent()
        {
            Objects = new List<PObjectType>();
        }

        /// <summary>
        /// Имя компонента
        /// (Документы, Справочники, Регистры и так далее)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Объекты, которые пренадлежат компоненту
        /// </summary>
        public IEnumerable<PObjectType> Objects { get; }
    }
}
