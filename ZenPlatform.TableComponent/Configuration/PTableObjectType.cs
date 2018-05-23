﻿using System;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;


namespace ZenPlatform.DocumentComponent.Configuration
{
    public class PTableObjectProperty : PProperty
    {
        public PTableObjectProperty(PObjectType owner) : base(owner)
        {

        }
    }

    public class PTableObjectType : PObjectType, IComponentType
    {
        public PTableObjectType(string name, Guid id, PComponent owner) : base(name, id, owner)
        {
            Init();
        }

        private void Init()
        {
            var property = new PTableObjectProperty(this)
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
