using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.SimpleRealization;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.DataComponent.Configuration;

namespace ZenPlatform.DocumentComponent
{
    public class PDocumentObjectType : PDataObjectType
    {
        public PDocumentObjectType(string name, Guid id, PComponent owner) : base(name, id, owner)
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

        /// <summary>
        /// Имя связанной таблицы
        /// 
        /// При миграции присваивается движком. В последствии хранится в конфигурации.
        /// </summary>
        public string RelTableName { get; set; }

    }

    public class PDocumentComplexObjectType : PDataComplexObjectType
    {
        public PDocumentComplexObjectType(string name, Guid guid, PObjectType objectType) : base(name, guid, objectType)
        {
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
    }
}
