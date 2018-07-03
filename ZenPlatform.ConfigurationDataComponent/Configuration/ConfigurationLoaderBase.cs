using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Xml.Serialization;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.Structure;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.Contracts.Data;

namespace ZenPlatform.DataComponent.Configuration
{
    public abstract class ConfigurationLoaderBase<TObjectType> : IXCLoader
        where TObjectType : XCObjectTypeBase
    {
        /// <summary>
        /// Загрузить компонент возвращает загруженный компонент
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        protected virtual TObjectType LoadObjectAction(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var s = new XmlSerializer(typeof(TObjectType));

                var conf = s.Deserialize(sr) as TObjectType ?? throw new InvalidLoadConfigurationException(path);

                if (conf.Name is null) throw new NullReferenceException("Configuration broken fill the name");
                if (conf.Guid == Guid.Empty) throw new NullReferenceException("Configuration broken fill the id field");

                return conf;
            }
        }

        protected virtual XCDataRuleBase LoadRuleAction(XCDataRuleContent content)
        {
            throw new NotImplementedException();
        }

        public virtual IDataComponent GetComponentImpl(XCComponent component)
        {
            throw new NotImplementedException();
        }

        public XCObjectTypeBase LoadObject(string path)
        {
            return LoadObjectAction(path);
        }

        public XCDataRuleBase LoadRule(XCDataRuleContent content)
        {
            return LoadRuleAction(content);
        }
    }
}