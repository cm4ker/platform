using System;
using Aquila.Configuration.ConfigurationLoader.Contracts;
using Aquila.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive;
using Aquila.Configuration.Data;
using Aquila.Configuration.Data.Types.Complex;

namespace Aquila.ReferenceComponent.Configuration
{
    public class PReferenceObjectProperty : PProperty
    {
        public PReferenceObjectProperty(PObjectType owner) : base(owner)
        {

        }
    }

    public class PReferenceObjectType : PObjectType, IComponentType
    {
        public PReferenceObjectType(string name, Guid id, PComponent owner) : base(name, id, owner)
        {
            Init();
        }

        private void Init()
        {
            var property = new PReferenceObjectProperty(this)
            {
                Unique = true,
                DatabaseColumnName = "Id"
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



}
