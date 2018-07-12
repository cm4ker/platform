using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Xml.Serialization;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.DataComponent.Configuration
{
    public abstract class ConfigurationLoaderBase<TObjectType> : IXComponentLoader
        where TObjectType : XCObjectTypeBase
    {
        /// <summary>
        /// Вызывается после загрузки объекта
        /// </summary>
        /// <param name="conf">Конфигурация объекта</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        protected virtual void AfterObjectLoad(TObjectType conf)
        {
        }

        /// <summary>
        /// Вызывается перед инициализацией
        /// </summary>
        /// <param name="conf">Конфигурация объекта</param>
        protected virtual void BeforeInitialize(TObjectType conf)
        {
        }

        /// <summary>
        /// Вызывается после инициализации
        /// </summary>
        /// <param name="conf">Конфигурация объекта</param>
        protected virtual void AfterInitialize(TObjectType conf)
        {
        }

        protected virtual XCDataRuleBase LoadRuleAction(XCDataRuleContent content)
        {
            throw new NotImplementedException();
        }

        public virtual IDataComponent GetComponentImpl(XCComponent component)
        {
            throw new NotImplementedException();
        }

        public XCObjectTypeBase LoadObject(XCComponent com, XCBlob blob)
        {
            var xml = com.Root.Storage.GetStringBlob(blob.Name, com.ComponentImpl.Name);

            using (var r = new StringReader(xml))
            {
                var s = new XmlSerializer(typeof(TObjectType));

                var conf = s.Deserialize(r) as TObjectType ?? throw new InvalidLoadConfigurationException(blob.Name);

                if (conf.Name is null) throw new NullReferenceException("Configuration broken fill the name");
                if (conf.Guid == Guid.Empty) throw new NullReferenceException("Configuration broken fill the id field");


                AfterObjectLoad(conf);

                //Сразу же указываем родителя
                ((IChildItem<XCComponent>) conf).Parent = com;

                com.Parent.PlatformTypes.Add(conf);

                BeforeInitialize(conf);
                conf.Initialize();
                AfterInitialize(conf);

                return conf;
            }
        }

        public XCDataRuleBase LoadRule(XCDataRuleContent content)
        {
            return LoadRuleAction(content);
        }
    }
}