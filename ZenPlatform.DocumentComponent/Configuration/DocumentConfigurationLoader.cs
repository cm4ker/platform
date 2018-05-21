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
    public class DocumentConfigurationLoader : IComponenConfigurationtLoader
    {
        public IComponentType Load(string pathToXml, PComponent component)
        {
            XmlConfDocument conf;
            using (var sr = new StreamReader(pathToXml))
            {
                var s = new XmlSerializer(typeof(XmlConfDocument));
                conf = s.Deserialize(sr) as XmlConfDocument ?? throw new InvalidLoadConfigurationException(pathToXml);
            }



            if (conf.Name is null) throw new NullReferenceException("Configuration broken fill the name");
            if (conf.Id == Guid.Empty) throw new NullReferenceException("Configuration broken fill the id field");

            PDocumentObjectType doc = new PDocumentObjectType(conf.Name, conf.Id, component);

            return doc;
        }

        public IComponentType LoadDependencies(string pathToXml, List<IComponentType> supportedObjects)
        {
            throw new NotImplementedException();
        }
    }

}
