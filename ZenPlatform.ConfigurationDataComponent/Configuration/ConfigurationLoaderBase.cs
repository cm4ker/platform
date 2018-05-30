using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;
using ZenPlatform.Configuration.Exceptions;

namespace ZenPlatform.DataComponent.Configuration
{
    public abstract class ConfigurationLoaderBase<TObjectType> : IComponenConfigurationLoader
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