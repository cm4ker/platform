using System;
using Aquila.Configuration.ConfigurationLoader.Contracts;
using Aquila.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive;
using Aquila.Configuration.Data;
using Aquila.Configuration.Data.Types.Complex;

namespace Aquila.InformationRegisterComponent.Configuration
{
    public class PInformationRegisterObjectProperty : PProperty
    {
        public PInformationRegisterObjectProperty(PObjectType owner) : base(owner)
        {

        }
    }

    public class PInformationRegisterObjectType : PObjectType, IComponentType
    {
        public PInformationRegisterObjectType(string name, Guid id, PComponent owner) : base(name, id, owner)
        {
            Init();
        }

        private void Init()
        {
            var property = new PInformationRegisterObjectProperty(this)
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
