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
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.Configuration.Exceptions;

namespace ZenPlatform.DataComponent.Configuration
{
    public abstract class ConfigurationLoaderBase
        <TXmlConf, TPObjectType, TPObjectProperty>
        : IComponenConfigurationLoader
        where TXmlConf : XCObjectTypeBase
        where TPObjectType : PObjectType, IComponentType
        where TPObjectProperty : PProperty
    {
        protected TXmlConf GetConf(string pathToXml)
        {
            using (var sr = new StreamReader(pathToXml))
            {
                var s = new XmlSerializer(typeof(TXmlConf));

                var conf = s.Deserialize(sr) as TXmlConf ?? throw new InvalidLoadConfigurationException(pathToXml);

                if (conf.Name is null) throw new NullReferenceException("Configuration broken fill the name");
                if (conf.Id == Guid.Empty) throw new NullReferenceException("Configuration broken fill the id field");

                return conf;
            }
        }

        public XCObjectTypeBase LoadComponentType(string path)
        {
            return GetConf(path);
        }

        [Obsolete]
        public IComponentType LoadComponentType(string pathToXml, PComponent component)
        {
            var conf = GetConf(pathToXml);
            return CreateNewComponentType(component, conf);
        }

        [Obsolete]
        protected abstract IComponentType CreateNewComponentType(PComponent component, TXmlConf conf);

        [Obsolete]
        public virtual IComponentType LoadComponentTypeDependencies(string pathToXml,
            List<IComponentType> supportedObjects)
        {
            var conf = GetConf(pathToXml);
            var objectType = supportedObjects.Find(x => x.Id == conf.Id) as TPObjectType;

            if (objectType is null) throw new InvalidOperationException();

            RelsolveDependencies(objectType, conf, supportedObjects);

            return objectType;
        }

        [Obsolete]
        protected virtual void RelsolveDependencies(TPObjectType obj, TXmlConf conf, List<IComponentType> anotherDeps)
        {
        }

        [Obsolete]
        public virtual IRule LoadComponentRole(IComponentType obj, string xmlContent)
        {
            throw new NotImplementedException();
        }
    }
}