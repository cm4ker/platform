using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.DocumentComponent.Configuration.XmlConfiguration;

namespace ZenPlatform.DocumentComponent.Configuration
{
    public class TableConfigurationLoader : IComponenConfigurationtLoader
    {
        public IComponentType LoadComponentType(string pathToXml, PComponent component)
        {
            XmlConfTable conf;
            using (var sr = new StreamReader(pathToXml))
            {
                var s = new XmlSerializer(typeof(XmlConfTable));
                conf = s.Deserialize(sr) as XmlConfTable ?? throw new InvalidLoadConfigurationException(pathToXml);
            }



            if (conf.Name is null) throw new NullReferenceException("Configuration broken fill the name");
            if (conf.Id == Guid.Empty) throw new NullReferenceException("Configuration broken fill the id field");

            PTableObjectType doc = new PTableObjectType(conf.Name, conf.Id, component);

            return doc;
        }

        public IComponentType LoadComponentTypeDependencies(string pathToXml, List<IComponentType> supportedObjects)
        {
            throw new NotImplementedException();
        }

        //TODO: Оставить что-то одно
        IRule LoadComponentRole(string xmlContent)
        {
            throw new NotImplementedException();
        }

        IRule IComponenConfigurationtLoader.LoadComponentRole(string xmlContent)
        {
            throw new NotImplementedException();
        }
    }

}
