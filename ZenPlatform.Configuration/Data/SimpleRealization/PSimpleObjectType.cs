using System;
using Newtonsoft.Json;

namespace ZenPlatform.Configuration.Data.SimpleRealization
{
    /// <summary>
    /// Простой объект с полями. 
    /// </summary>
    public class PSimpleObjectType : PObjectType
    {
        public PSimpleObjectType(string name, Guid id, PComponent owner) : base(name, id, owner)
        {
            Init();
        }

        public PSimpleObjectType(string name, PComponent owner) : base(name, Guid.Empty, owner)
        {
            Init();
        }

        private void Init()
        {
            var property = new PSimpleProperty(this)
            {
                Unique = true,
                Name = "Id"
            };

            property.Types.Add(new PGuid());
            Properties.Add(property);
        }

        public override bool IsAbstractType => false;

        /// <summary>
        /// Имя таблицы в SQL для этого объекта
        /// </summary>
        public string TableName { get; set; }
    }
}